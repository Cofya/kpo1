using Microsoft.Extensions.DependencyInjection;
using Xunit;

// заглушка техосмотра: отвечает заранее заданным значением
// и запоминает, что ей показали
public class StubServiceCenter : IServiceCenter
{
    private bool _decision;
    public List<IInventoryItem> Inspected { get; } = new();

    public StubServiceCenter(bool decision)
    {
        _decision = decision;
    }

    public bool Inspect(IInventoryItem item)
    {
        Inspected.Add(item);
        return _decision;
    }
}


// тесты на inventoryServices
public class InventoryServiceTests
{
    [Fact]
    public void AddTransport_InspectionPassed_TransportIsAccepted()
    {
        // arrange
        var service = new InventoryService(new StubServiceCenter(true));

        // act
        var result = service.AddTransport(new Bicycle("b", 1, 5));

        // assert
        Assert.True(result);
        Assert.Equal(1, service.TransportCount);
    }

    [Fact]
    public void AddTransport_InspectionFailed_TransportIsNotAccepted()
    {
        // arrange
        var service = new InventoryService(new StubServiceCenter(false));

        // act
        var result = service.AddTransport(new Bicycle("b", 1, 5));

        // assert
        Assert.False(result);
        Assert.Equal(0, service.TransportCount);
        Assert.Empty(service.GetAllItems());
    }

    [Fact]
    public void AddTransport_SendsThisTransportToInspection()
    {
        // arrange
        var serviceCenter = new StubServiceCenter(true);
        var service = new InventoryService(serviceCenter);
        var bike = new Bicycle("b", 1, 5);

        // act
        service.AddTransport(bike);

        // assert
        Assert.Same(bike, Assert.Single(serviceCenter.Inspected));
    }

    [Fact]
    public void AddThing_InspectionPassed_ThingIsAccepted()
    {
        // arrange
        var service = new InventoryService(new StubServiceCenter(true));

        // act
        var result = service.AddThing(new Helmet("h", 1));

        // assert
        Assert.True(result);
        Assert.Equal(1, service.ThingCount);
    }

    [Fact]
    public void AddThing_InspectionFailed_ThingIsNotAccepted()
    {
        // arrange
        var service = new InventoryService(new StubServiceCenter(false));

        // act
        var result = service.AddThing(new Helmet("h", 1));

        // assert
        Assert.False(result);
        Assert.Equal(0, service.ThingCount);
    }

    [Fact]
    public void AddTransport_ServiceCenterIsReplacedThroughDiContainer()
    {
        // arrange: в контейнере вместо настоящего техосмотра регистрируем заглушку
        var services = new ServiceCollection();
        services.AddSingleton<IServiceCenter>(new StubServiceCenter(false));
        services.AddSingleton<InventoryService>();
        var service = services.BuildServiceProvider().GetRequiredService<InventoryService>();

        // act
        var result = service.AddTransport(new Bicycle("b", 1, 9));

        // assert
        Assert.False(result);
    }

    [Fact]
    public void GetTotalEnergy_SumsTransportAndThings()
    {
        // arrange
        var service = new InventoryService(new StubServiceCenter(true));
        service.AddTransport(new ElectricScooter("s", 1, 5, 1.0));
        service.AddTransport(new Bicycle("b", 2, 5));
        service.AddThing(new DockStation("d", 3, 0.5));
        service.AddThing(new Helmet("h", 4));

        // act
        var total = service.GetTotalEnergy();

        // assert
        Assert.Equal(1.5, total);
    }

    [Fact]
    public void GetTotalEnergy_RejectedItemsAreNotCounted()
    {
        // arrange
        var service = new InventoryService(new StubServiceCenter(false));
        service.AddTransport(new ElectricScooter("s", 1, 5, 1.0));
        service.AddThing(new ChargingCabinet("c", 2, 2.0));

        // act
        var total = service.GetTotalEnergy();

        // assert
        Assert.Equal(0.0, total);
    }

    [Theory]
    [InlineData(5, false)]
    [InlineData(6, true)]
    [InlineData(10, true)]
    public void GetBeginnerTransports_UsesSimpleValueThresholdSix(int simple, bool expected)
    {
        // arrange
        var service = new InventoryService(new StubServiceCenter(true));
        service.AddTransport(new Bicycle("b", 1, simple));

        // act
        var beginners = service.GetBeginnerTransports();

        // assert
        Assert.Equal(expected, beginners.Count == 1);
    }

    [Fact]
    public void GetAllItems_ReturnsTransportAndThingsTogether()
    {
        // arrange
        var service = new InventoryService(new StubServiceCenter(true));
        service.AddTransport(new Bicycle("b", 1, 5));
        service.AddThing(new Helmet("h", 2));

        // act
        var numbers = service.GetAllItems().Select(i => i.InventoryNumber).ToList();

        // assert
        Assert.Equal(new List<int> { 1, 2 }, numbers);
    }
}

public class ServiceCenterTests
{
    private readonly ServiceCenter _center = new();

    [Fact]
    public void Inspect_NormalBicycle_Accepted()
    {
        // arrange
        var bike = new Bicycle("b", 1, 5);

        // act
        var result = _center.Inspect(bike);

        // assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(-15)]
    [InlineData(0)]
    [InlineData(11)]
    public void Inspect_SimpleValueOutOfRange_Rejected(int simple)
    {
        // arrange
        var bike = new Bicycle("b", 1, simple);

        // act
        var result = _center.Inspect(bike);

        // assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Inspect_ElectricTransportWithBadEnergy_Rejected(double energy)
    {
        // arrange
        var scooter = new ElectricScooter("s", 1, 5, energy);

        // act
        var result = _center.Inspect(scooter);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void Inspect_ElectricBicycleWithNormalEnergy_Accepted()
    {
        // arrange
        var bike = new ElectricBicycle("e", 1, 5, 2.0);

        // act
        var result = _center.Inspect(bike);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void Inspect_ThingWithBadEnergy_Rejected()
    {
        // arrange
        var dock = new DockStation("d", 1, -1.0);

        // act
        var result = _center.Inspect(dock);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void Inspect_HelmetWithoutEnergy_Accepted()
    {
        // arrange
        var helmet = new Helmet("h", 1);

        // act
        var result = _center.Inspect(helmet);

        // assert
        Assert.True(result);
    }
}


// тесты на генерацию айдишника
public class InventoryNumberGeneratorTests
{
    [Fact]
    public void Next_FirstNumberIsOne()
    {
        // arrange
        var generator = new InventoryNumberGenerator();

        // act
        var first = generator.Next();

        // assert
        Assert.Equal(1, first);
    }

    [Fact]
    public void Next_ReturnsDifferentNumbers()
    {
        // arrange
        var generator = new InventoryNumberGenerator();

        // act
        var first = generator.Next();
        var second = generator.Next();

        // assert
        Assert.NotEqual(first, second);
    }
}

public class AppTests
{
    // запускает приложение с заданным вводом, возвращает сервис и весь вывод
    private static (InventoryService service, string output) RunApp(string input)
    {
        var output = new StringWriter();
        var service = new InventoryService(new ServiceCenter());
        var app = new App(service, new InventoryNumberGenerator(), new StringReader(input), output);

        app.Run();

        return (service, output.ToString());
    }

    [Fact]
    public void Run_Demo_FillsInventory()
    {
        // arrange
        var input = "0";

        // act
        var (service, _) = RunApp(input);

        // assert: из 5 демо-транспортов принято 3, вещей 3
        Assert.Equal(3, service.TransportCount);
        Assert.Equal(3, service.ThingCount);
        Assert.Equal(5.5, service.GetTotalEnergy(), 3);
    }

    [Fact]
    public void Run_InputEnded_DoesNotFail()
    {
        // arrange
        var input = "";

        // act
        var exception = Record.Exception(() => RunApp(input));

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void Run_AddEasyBicycle_AppearsInBeginnersList()
    {
        // arrange
        var input = "2\nМой велик\n9\n0";

        // act
        var (service, _) = RunApp(input);

        // assert
        Assert.Contains(service.GetBeginnerTransports(), t => t.Name == "Мой велик");
    }

    [Fact]
    public void Run_AddScooterWithCommaEnergy_ParsesEnergy()
    {
        // arrange
        var input = "1\nСамокат2\n7\n1,5\n0";

        // act
        var (service, _) = RunApp(input);

        // assert
        var scooter = service.GetAllItems().OfType<ElectricScooter>().Single(s => s.Name == "Самокат2");
        Assert.Equal(1.5, scooter.DailyEnergyKwh, 3);
    }

    [Fact]
    public void Run_AddBicycleWithBadNumber_IsNotAccepted()
    {
        // arrange: вместо простоты вводим текст, получится 0
        var input = "2\nПлохой\nabc\n0";

        // act
        var (service, _) = RunApp(input);

        // assert
        Assert.DoesNotContain(service.GetAllItems(), i => i.Name == "Плохой");
    }

    [Fact]
    public void Run_AddHelmet_ThingCountGrows()
    {
        // arrange
        var input = "4\nШлем2\n0";

        // act
        var (service, _) = RunApp(input);

        // assert
        Assert.Equal(4, service.ThingCount);
    }

    [Fact]
    public void Run_AddDockStationWithBadEnergy_IsNotAccepted()
    {
        // arrange
        var input = "5\nДок2\n-1\n0";

        // act
        var (service, _) = RunApp(input);

        // assert
        Assert.Equal(3, service.ThingCount);
    }

    [Fact]
    public void Run_UnknownCommand_DoesNotChangeInventory()
    {
        // arrange
        var input = "xyz\n0";

        // act
        var (service, _) = RunApp(input);

        // assert
        Assert.Equal(3, service.TransportCount);
        Assert.Equal(3, service.ThingCount);
    }

    [Fact]
    public void Run_ReportCommand_PrintsTotalEnergy()
    {
        // arrange
        var input = "7\n0";

        // act
        var (_, output) = RunApp(input);

        // assert: проверяем число
        Assert.Contains("5.50", output);
    }
}