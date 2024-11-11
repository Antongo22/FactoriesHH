namespace FactoriesHH;

/// <summary>
/// Класс, представляющий грузовик.
/// </summary>
public class Truck
{
    public int Capacity { get; }
    public double AverageLoad { get; private set; }
    public string LoadComposition { get; private set; }
    public Dictionary<string, int> TotalProductCounts { get; private set; }
    public int Trips { get; private set; }


    private int totalLoad;


    public Truck(int capacity)
    {
        Capacity = capacity;
        totalLoad = 0;
        Trips = 0;
        TotalProductCounts = new Dictionary<string, int>();
    }

    /// <summary>
    /// Запускает доставку продукции.
    /// </summary>
    /// <param name="warehouse">Склад, с которого забирается продукция.</param>
    public void StartDelivery(Warehouse warehouse)
    {
        while (true)
        {
            Thread.Sleep(100);

            if (warehouse.IsFinish) return;

            if (!warehouse.IsCanUpload) continue;

            var load = warehouse.LoadTruck(Capacity);
            totalLoad += load.Values.Sum();
            Trips++;

            AverageLoad = (double)totalLoad / Trips;
            LoadComposition = string.Join(", ", load.Select(p => $"{p.Key}: {p.Value}"));

            foreach (var product in load)
            {
                if (!TotalProductCounts.ContainsKey(product.Key))
                {
                    TotalProductCounts[product.Key] = 0;
                }
                TotalProductCounts[product.Key] += product.Value;
            }

            Console.WriteLine($"Грузовик вместимостью {Capacity} загружен: {LoadComposition}");
        }
    }
}