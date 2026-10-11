


// класс предмета что будет на балансе шлемы, док-станции, зарядные шкафы
public abstract class Thing : IInventoryItem
{
    public string Name { get; }
    public int InventoryNumber { get; }

    protected Thing(string name, int inventoryNumber)
    {
        Name = name;
        InventoryNumber = inventoryNumber;
    }
}

public class Helmet : Thing
{
    public Helmet(string name, int inventoryNumber) : base(name, inventoryNumber)
    { }
}

public class DockStation : Thing, IEnergyConsumer
{
    public double DailyEnergyKwh { get; }

    public DockStation(string name, int inventoryNumber, double dailyEnergyKwh) :
    base(name, inventoryNumber)
    {
        DailyEnergyKwh = dailyEnergyKwh;
    }

}

public class ChargingCabinet : Thing, IEnergyConsumer
{
    public double DailyEnergyKwh { get; }

    public ChargingCabinet(string name, int inventoryNumber, double dailyEnergyKwh) :
    base(name, inventoryNumber)
    {
        DailyEnergyKwh = dailyEnergyKwh;
    }

}

