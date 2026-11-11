namespace TUnitPoc.Tests;

internal static class Products
{
    public static Product Keyboard => new("KB-01", "Clavier mécanique", 89.90m);
    public static Product Mouse => new("MS-01", "Souris", 24.50m);
    public static Product Cable => new("CB-01", "Câble USB-C", 9.99m);
}
