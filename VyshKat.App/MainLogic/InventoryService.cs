

using System.Net;


// класс, отвечающий за добавление транспорта / вещей
// вывод некоторой инфы по ним
public class InventoryService
{
    public const int MinBeginSimple = 6;

    private IServiceCenter _serviceCenter;

    private List<Transport> _transports = new();
    private List<Thing> _things = new();

    public InventoryService(IServiceCenter serviceCenter)
    {
        _serviceCenter = serviceCenter;
    }

    public int TransportCount => _transports.Count;
    public int ThingCount => _things.Count;

    public bool AddTransport(Transport transport)
    {
        if (_serviceCenter.Inspect(transport) == false)
        {
            return false;
        }
        _transports.Add(transport);
        return true;
    }

    // не понял по условию, нужен ли техосмотр вещей, допустим да
    public bool AddThing(Thing thing)
    {
        if (_serviceCenter.Inspect(thing) == false)
        {
            return false;
        }
        _things.Add(thing);
        return true;
    }

    public List<IInventoryItem> GetAllItems()
    {
        var result = new List<IInventoryItem>();
        result.AddRange(_transports);
        result.AddRange(_things);
        return result;
    }
    public double GetTotalEnergy()
    {
        var items = GetAllItems();
        double sum = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] is IEnergyConsumer energy)
            {
                sum += energy.DailyEnergyKwh;
            }
        }
        return sum;
    }

    public List<Transport> GetBeginnerTransports()
    {
        var result = new List<Transport>();

        foreach (var transport in _transports)
        {
            if (transport.SimpleValue >= MinBeginSimple)
            {
                result.Add(transport);
            }
        }

        return result;
    }
}