namespace FactoriesHH;

/// <summary>
/// Класс, представляющий склад.
/// </summary>
public class Warehouse
{
    public int capacity { get; private set; }
    private Dictionary<string, int> productCounts;
    public bool IsCanUpload { get; private set; } = false;
    public bool IsFinish { get; private set; } = false;


    private int _currentLoad;
    public int CurrentLoad
    {
        get { return _currentLoad; }
        private set
        {
            if (value > capacity)
            {
                throw new InvalidOperationException("Невозможно добавить товары на склад, превышена вместимость.");
            }
            _currentLoad = value;
        }
    }



    public Warehouse(int capacity, int days, int hours)
    {
        this.capacity = capacity;
        this.CurrentLoad = 0;
        this.productCounts = new Dictionary<string, int>();
    }

    /// <summary>
    /// Добавляет продукцию на склад.
    /// </summary>
    /// <param name="product">Продукт.</param>
    /// <param name="quantity">Количество продукции.</param>
    public void AddProduct(Product product, int quantity)
    {
        lock (this)
        {
            if (CurrentLoad + quantity > capacity * 0.95 && !IsCanUpload)
            {
                IsCanUpload = true;
                Console.WriteLine("Склад заполнен на 95%, начинаем вывоз продукции.\n\n\n\n");
            }

            if (!productCounts.ContainsKey(product.Name))
            {
                productCounts[product.Name] = 0;
            }

            productCounts[product.Name] += quantity;
            CurrentLoad += quantity;

            if (CurrentLoad < capacity * 0.5 && IsCanUpload)
            {
                IsCanUpload = false;
                Console.WriteLine("Склад опустошен менее чем на 50%, вывоз продукции прекращен.\n\n\n\n");
            }

        }
    }

    /// <summary>
    /// Загружает продукцию в грузовик.
    /// </summary>
    /// <param name="truckCapacity">Вместимость грузовика.</param>
    /// <returns>Словарь с продукцией и ее количеством.</returns>
    public Dictionary<string, int> LoadTruck(int truckCapacity)
    {
        lock (this)
        {
            var load = new Dictionary<string, int>();
            int remainingCapacity = truckCapacity;

            foreach (var product in productCounts.Keys.ToList())
            {
                if (remainingCapacity <= 0) break;

                int quantity = Math.Min(productCounts[product], remainingCapacity);
                load[product] = quantity;
                productCounts[product] -= quantity;
                CurrentLoad -= quantity;
                remainingCapacity -= quantity;
            }

            return load;
        }
    }

    public void Stop()
    {
        IsFinish = true;
    }
}