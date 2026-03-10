namespace OrderManagementSystem.Lab.Lab2.FactoryMethod;

public static class OrderCreatorFactory
{
    public static OrderCreator GetCreator(string orderType)
    {
        return orderType?.Trim().ToLowerInvariant() switch
        {
            "standard" => new StandardOrderCreator(),
            "express" => new ExpressOrderCreator(),
            "bulk" => new BulkOrderCreator(),
            _ => throw new ArgumentException("Unsupported order type. Use standard, express, or bulk.")
        };
    }
}
