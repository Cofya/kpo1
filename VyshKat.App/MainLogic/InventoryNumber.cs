


public class InventoryNumberGenerator
{
    private int _number = 0;

    public int Next()
    {
        _number += 1;
        return _number;
    }
}