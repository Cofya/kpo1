using System.Globalization;

// консольное приложение, ввод и вывод передаются снаружи, чтобы можно было тестировать
public class App
{
    private InventoryService _invService;
    private InventoryNumberGenerator _invNumbers;
    private TextReader _input;
    private TextWriter _output;

    public App(InventoryService invService, InventoryNumberGenerator invNumbers, TextReader input, TextWriter output)
    {
        _invService = invService;
        _invNumbers = invNumbers;
        _input = input;
        _output = output;
    }

    public void Run()
    {
        _output.WriteLine("-----Старт----");
        RunDemo();
        _output.WriteLine("----Демо кончилось!!!---");
        _output.WriteLine();
        RunMenu();
    }

    private void RunDemo()
    {
        _output.WriteLine();
        _output.WriteLine("--- демо ---");

        TryAdd(new ElectricScooter("Самокат1", _invNumbers.Next(), 8, 1.2));
        TryAdd(new Bicycle("Велосипед1", _invNumbers.Next(), 9));
        TryAdd(new ElectricBicycle("Электровелосипед1", _invNumbers.Next(), 4, 2.0));
        // этот не пройдет serviceCenter тк расход отрицательный
        TryAdd(new ElectricScooter("Самокат отриц", _invNumbers.Next(), 7, -10));
        // этот не пройдет serviceCenter тк простота вне диапазона
        TryAdd(new Bicycle("Велосипед отриц", _invNumbers.Next(), -15));

        _invService.AddThing(new Helmet("Шлем1", _invNumbers.Next()));
        _invService.AddThing(new DockStation("Док-станция1", _invNumbers.Next(), 0.8));
        _invService.AddThing(new ChargingCabinet("Зарядный шкаф1", _invNumbers.Next(), 1.5));

        PrintReport();
        PrintBeginners();
        PrintInventory();
    }

    private void RunMenu()
    {
        while (true)
        {
            _output.WriteLine();
            _output.WriteLine("--- меню ---");
            _output.WriteLine("1 - добавить самокат");
            _output.WriteLine("2 - добавить велосипед");
            _output.WriteLine("3 - добавить электровелосипед");
            _output.WriteLine("4 - добавить шлем");
            _output.WriteLine("5 - добавить док-станцию");
            _output.WriteLine("6 - добавить зарядный шкаф");
            _output.WriteLine("7 - отчет по парку");
            _output.WriteLine("8 - техника для новичков");
            _output.WriteLine("9 - инвентарные номера");
            _output.WriteLine("0 - выход");
            _output.Write("выбор: ");

            var choice = _input.ReadLine();
            if (choice == null || choice == "0")
            {
                break;
            }

            switch (choice)
            {
                case "1":
                    TryAdd(new ElectricScooter(AskText("название"), _invNumbers.Next(), AskInt("простота (1-10)"), AskDouble("расход кВт*ч в сутки")));
                    break;
                case "2":
                    TryAdd(new Bicycle(AskText("название"), _invNumbers.Next(), AskInt("простота (1-10)")));
                    break;
                case "3":
                    TryAdd(new ElectricBicycle(AskText("название"), _invNumbers.Next(), AskInt("простота (1-10)"), AskDouble("расход кВт*ч в сутки")));
                    break;
                case "4":
                    TryAddThing(new Helmet(AskText("название"), _invNumbers.Next()), "шлем");
                    break;
                case "5":
                    TryAddThing(new DockStation(AskText("название"), _invNumbers.Next(), AskDouble("расход кВт*ч в сутки")), "док-станция");
                    break;
                case "6":
                    TryAddThing(new ChargingCabinet(AskText("название"), _invNumbers.Next(), AskDouble("расход кВт*ч в сутки")), "зарядный шкаф");
                    break;
                case "7":
                    PrintReport();
                    break;
                case "8":
                    PrintBeginners();
                    break;
                case "9":
                    PrintInventory();
                    break;
                default:
                    _output.WriteLine("неизвестная команда");
                    break;
            }
        }
    }

    private void TryAdd(Transport transport)
    {
        if (_invService.AddTransport(transport))
        {
            _output.WriteLine($"принят: {transport.Name} (№{transport.InventoryNumber})");
        }
        else
        {
            _output.WriteLine($"отклонен техосмотром: {transport.Name}");
        }
    }

    private void TryAddThing(Thing thing, string kind)
    {
        if (_invService.AddThing(thing))
        {
            _output.WriteLine($"{kind} добавлен(а): {thing.Name} (№{thing.InventoryNumber})");
        }
        else
        {
            _output.WriteLine($"отклонен техосмотром: {thing.Name}");
        }
    }

    private void PrintReport()
    {
        _output.WriteLine();
        _output.WriteLine("--- отчет ---");
        _output.WriteLine($"единиц транспорта: {_invService.TransportCount}");
        _output.WriteLine($"единиц вещей: {_invService.ThingCount}");
        
        var energy = _invService.GetTotalEnergy().ToString("F2", CultureInfo.InvariantCulture);
        _output.WriteLine($"суммарное потребление: {energy} кВт*ч в сутки");
    }

    private void PrintBeginners()
    {
        _output.WriteLine();
        _output.WriteLine("--- для новичков ---");
        var list = _invService.GetBeginnerTransports();
        if (list.Count == 0)
        {
            _output.WriteLine("таких нет");
        }
        foreach (var t in list)
        {
            _output.WriteLine($"{t.Name} (простота {t.SimpleValue})");
        }
    }

    private void PrintInventory()
    {
        _output.WriteLine();
        _output.WriteLine("--- инвентарные номера ---");
        foreach (var item in _invService.GetAllItems())
        {
            _output.WriteLine($"№{item.InventoryNumber} - {item.Name}");
        }
    }

    private string AskText(string label)
    {
        _output.Write($"{label}: ");
        var text = _input.ReadLine();
        if (string.IsNullOrWhiteSpace(text))
        {
            return "Без названия";
        }
        return text;
    }

    private int AskInt(string label)
    {
        _output.Write($"{label}: ");
        if (int.TryParse(_input.ReadLine(), out int value))
        {
            return value;
        }
        _output.WriteLine("не число, будет 0");
        return 0;
    }

    private double AskDouble(string label)
    {
        _output.Write($"{label}: ");
        var text = (_input.ReadLine() ?? "").Replace(',', '.');
        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
        {
            return value;
        }
        _output.WriteLine("не число, будет 0");
        return 0;
    }
}