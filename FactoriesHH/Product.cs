using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoriesHH;

/// <summary>
/// Класс, представляющий продукт.
/// </summary>
public class Product
{
    public string Name { get; }
    public int Weight { get; }
    public string PackagingType { get; }

    public Product(string name, int weight, string packagingType)
    {
        Name = name;
        Weight = weight;
        PackagingType = packagingType;
    }
}
