


// все объекты что стоят на балансе и имеют id (инвентарный номер)
public interface IInventoryItem
{
    string Name { get; }
    int InventoryNumber { get; }
}

// все объекты что потребляют энергию

public interface IEnergyConsumer
{
    double DailyEnergyKwh { get; }
}
