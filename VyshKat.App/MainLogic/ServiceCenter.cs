using System.Net;



public interface IServiceCenter
{
    bool Inspect(IInventoryItem item);
}

public class ServiceCenter : IServiceCenter
{
    public bool Inspect(IInventoryItem item)
    {
        if (item is Transport transport)
        {
            // простота должна быть От 1 до 10
            if (transport.SimpleValue < 1 || transport.SimpleValue > 10)
            {
                return false;
            }
        }


        if (item is IEnergyConsumer energy)
        {
            if (energy.DailyEnergyKwh <= 0)
            {
                return false;
            }
        }

        return true;
    }
}