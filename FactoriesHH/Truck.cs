using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    private int totalLoad;
    public int trips { get; private set; }

    public Truck(int capacity)
    {
        Capacity = capacity;
        totalLoad = 0;
        trips = 0;
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

            if (warehouse.isFinish) return;

            if (!warehouse.isCanUpload) continue;

            var load = warehouse.LoadTruck(Capacity);
            totalLoad += load.Values.Sum();
            trips++;

            AverageLoad = (double)totalLoad / trips;
            LoadComposition = string.Join(", ", load.Select(p => $"{p.Key}: {p.Value}"));

            // Обновляем статистику по перевезённым продуктам
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