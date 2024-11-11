namespace FactoriesHH;

/// <summary>
/// Класс, представляющий продукт.
/// </summary>
public class Product
{
    public string Name { get; private set; }
    public int Weight { get; private set; }
    public string PackagingType { get; private set; }

    public Product(string name, int weight, string packagingType)
    {
        Name = name;
        Weight = weight;
        PackagingType = packagingType;
    }
}
