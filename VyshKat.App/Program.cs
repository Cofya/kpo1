using System;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

class Program
{
    static void Main()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IServiceCenter, ServiceCenter>();
        services.AddSingleton<InventoryNumberGenerator>();
        services.AddSingleton<InventoryService>();
        services.AddSingleton<TextReader>(Console.In);
        services.AddSingleton<TextWriter>(Console.Out);
        services.AddSingleton<App>();
        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<App>().Run();
    }
}