namespace FactoriesHH;

/// <summary>
/// Главный класс программы, отвечающий за управление производством, складированием и доставкой продукции.
/// </summary>
public class Program
{
    static void Main(string[] args)
    {
        int n = GetValidInput("Введите базовое количество продукции в час (n, не менее 50):", 50);
        int M = GetValidInput("Введите множитель для расчета вместимости склада (M, не менее 100):", 100);
        int numFactories = GetValidInput("Введите количество заводов (не менее 3):", 3);
        var trucks = GetTrucks(M, n, numFactories);

        int days = GetValidInput("Введите количество дней (не менее 1 и не более 365):", 1, 365);
        int hours = GetValidInput("Введите количество часов в день (не менее 1 и не более 24):", 1, 24);

        var warehouse = CreateWarehouse(n, M, numFactories, days, hours);
        var factories = CreateFactories(n, numFactories, warehouse);

        Console.WriteLine($"Вместительность склада {warehouse.capacity} единиц.\n\nНачался цикл производства.\n\n\n");

        var productionTask = Task.Run(() => StartProduction(factories, warehouse, days, hours));
        var deliveryTask = StartDelivery(trucks, warehouse);

        Task.WaitAll(productionTask, deliveryTask);

        Console.WriteLine("\n\nСтатистика по перевозкам:");
        PrintTruckStatistics(trucks);
    }

    /// <summary>
    /// Запрашивает у пользователя ввод данных и проверяет, является ли введенное значение целым числом и находится ли оно в допустимом диапазоне.
    /// </summary>
    /// <param name="prompt">Приглашение для ввода данных.</param>
    /// <param name="minValue">Минимально допустимое значение.</param>
    /// <param name="maxValue">Максимально допустимое значение.</param>
    /// <returns>Введенное значение, прошедшее проверку.</returns>
    static int GetValidInput(string prompt, int minValue, int maxValue = int.MaxValue)
    {
        int value;
        while (true)
        {
            Console.WriteLine(prompt);
            if (int.TryParse(Console.ReadLine(), out value) && value >= minValue && value <= maxValue)
            {
                return value;
            }
            Console.WriteLine($"Неверный ввод. Пожалуйста, введите число от {minValue} до {maxValue}.");
        }
    }

    /// <summary>
    /// Создает и возвращает список грузовиков с заданной вместимостью.
    /// </summary>
    /// <param name="M">Множитель для расчета вместимости склада.</param>
    /// <param name="n">Базовое количество продукции в час.</param>
    /// <param name="numFactories">Количество заводов.</param>
    /// <returns>Список грузовиков.</returns>
    static List<Truck> GetTrucks(int M, int n, int numFactories)
    {
        var trucks = new List<Truck>();
        int numTrucks = GetValidInput("Введите количество грузовиков (не менее 2):", 2);
        int warehouseCapacity = M * (n + (int)(1.1 * n) + (int)(1.2 * n));
        int minTruckCapacity = warehouseCapacity * 5 / 100 / numTrucks;

        for (int i = 0; i < numTrucks; i++)
        {
            int capacity = GetValidInput($"Введите вместимость грузовика {i + 1} (не менее {minTruckCapacity}):", minTruckCapacity);
            trucks.Add(new Truck(capacity));
        }
        return trucks;
    }

    /// <summary>
    /// Создает и возвращает склад с заданной вместимостью.
    /// </summary>
    /// <param name="n">Базовое количество продукции в час.</param>
    /// <param name="M">Множитель для расчета вместимости склада.</param>
    /// <param name="numFactories">Количество заводов.</param>
    /// <param name="days">Количество дней.</param>
    /// <param name="hours">Количество часов в день.</param>
    /// <returns>Склад с заданной вместимостью.</returns>
    static Warehouse CreateWarehouse(int n, int M, int numFactories, int days, int hours)
    {
        int totalProductionPerHour = 0;
        for (int i = 0; i < numFactories; i++)
        {
            totalProductionPerHour += (int)(n * (1 + 0.1 * i));
        }
        return new Warehouse(M * totalProductionPerHour, days, hours);
    }

    /// <summary>
    /// Создает и возвращает список заводов с заданной производительностью.
    /// </summary>
    /// <param name="n">Базовое количество продукции в час.</param>
    /// <param name="numFactories">Количество заводов.</param>
    /// <param name="warehouse">Склад, на который поступает продукция.</param>
    /// <returns>Список заводов.</returns>
    static List<Factory> CreateFactories(int n, int numFactories, Warehouse warehouse)
    {
        var factories = new List<Factory>();
        for (int i = 0; i < numFactories; i++)
        {
            string factoryName = ((char)('A' + i)).ToString();
            int productionRate = (int)(n * (1 + 0.1 * i));
            factories.Add(new Factory(factoryName, productionRate, warehouse));
        }
        return factories;
    }

    /// <summary>
    /// Запускает производство на заводах.
    /// </summary>
    /// <param name="factories">Список заводов.</param>
    /// <param name="warehouse">Склад, на который поступает продукция.</param>
    /// <param name="days">Количество дней.</param>
    /// <param name="hours">Количество часов в день.</param>
    /// <returns>Задача, представляющая собой параллельное выполнение производства.</returns>
    static Task StartProduction(List<Factory> factories, Warehouse warehouse, int days, int hours)
    {
        var warehouseLoadByDay = new Dictionary<int, int>();

        for (int day = 1; day <= days; day++)
        {
            warehouseLoadByDay[day] = 0;

            for (int hour = 1; hour <= hours; hour++)
            {
                var factoryTasks = factories.Select(f => Task.Run(() => f.StartProduction(day, hour))).ToArray();
                Task.WaitAll(factoryTasks);

                warehouseLoadByDay[day] = warehouse.CurrentLoad;
            }

            Console.WriteLine($"День {day}: Наполнение склада - {warehouseLoadByDay[day]} единиц продукции");
        }

        warehouse.Stop();
        Console.WriteLine("Цикл производства закончен.");

        return Task.CompletedTask;
    }

    /// <summary>
    /// Запускает доставку продукции грузовиками.
    /// </summary>
    /// <param name="trucks">Список грузовиков.</param>
    /// <param name="warehouse">Склад, с которого забирается продукция.</param>
    /// <returns>Задача, представляющая собой параллельное выполнение доставки.</returns>
    static Task StartDelivery(List<Truck> trucks, Warehouse warehouse)
    {
        var truckTasks = trucks.Select(t => Task.Run(() => t.StartDelivery(warehouse))).ToArray();
        return Task.WhenAll(truckTasks);
    }

    /// <summary>
    /// Выводит статистику по перевозкам грузовиков.
    /// </summary>
    /// <param name="trucks">Список грузовиков.</param>
    static void PrintTruckStatistics(List<Truck> trucks)
    {
        foreach (var truck in trucks)
        {
            Console.WriteLine($"Грузовик вместимостью {truck.Capacity} единиц:");

            int totalProductCount = 0;
            foreach (var product in truck.TotalProductCounts)
            {
                totalProductCount += product.Value;
                Console.WriteLine($"\tПродукт {product.Key}: перевезено {product.Value} единиц");
            }

            foreach (var product in truck.TotalProductCounts)
            {
                double averageProductLoad = (double)product.Value / truck.Trips;
                Console.WriteLine($"\tСреднее количество перевезенной продукции {product.Key}: {averageProductLoad}");
            }

            Console.WriteLine($"\tКоличество всей продукции: {totalProductCount}");
            Console.WriteLine($"\tВсего поездок: {truck.Trips}");
            Console.WriteLine($"\tСреднее количество перевезенной продукции: {truck.AverageLoad}");
        }
    }
}