

// базовый класс транспорта

public abstract class Transport : IInventoryItem
{
    public string Name { get; }
    public int InventoryNumber { get; }

    // простота транспорта
    public int SimpleValue { get; }

    protected Transport(string name, int inventoryNumber, int simple)
    {
        Name = name;
        InventoryNumber = inventoryNumber;
        SimpleValue = simple;
    }
}

public class Bicycle : Transport
{
    public Bicycle(string name, int inventoryNumber, int simple)
    : base(name, inventoryNumber, simple)
    { }

}

// электросамокатик
public class ElectricScooter : Transport, IEnergyConsumer
{
    public double DailyEnergyKwh { get; }

    public ElectricScooter(string name, int inventoryNumber, int simple, double dailyEnergyKwh) :
    base(name, inventoryNumber, simple)
    {
        DailyEnergyKwh = dailyEnergyKwh;
    }

}


public class ElectricBicycle : Transport, IEnergyConsumer
{
    public double DailyEnergyKwh { get; }

    public ElectricBicycle(string name, int inventoryNumber, int simple, double dailyEnergyKwh) :
    base(name, inventoryNumber, simple)
    {
        DailyEnergyKwh = dailyEnergyKwh;
    }

}