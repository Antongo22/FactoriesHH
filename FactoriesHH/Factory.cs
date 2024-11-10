using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoriesHH;

/// <summary>
/// Класс, представляющий завод.
/// </summary>
public class Factory
{
    private string name;
    private int productionRate;
    private Warehouse warehouse;

    public Factory(string name, int productionRate, Warehouse warehouse)
    {
        this.name = name;
        this.productionRate = productionRate;
        this.warehouse = warehouse;
    }

    /// <summary>
    /// Запускает производство продукции на заводе.
    /// </summary>
    public void StartProduction(int day, int hour)
    {
        Thread.Sleep(100);
        var product = new Product(name, 1, "Standard");
        warehouse.AddProduct(product, productionRate);
        Console.WriteLine($"День - {day}, час - {hour}. Завод {name} произвел {productionRate} единиц продукта {name}.");
    }
}