using System;
using CoindeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoindeskApi.Data;

public class DbInitializer
{
    public static void InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        SeedData(scope.ServiceProvider.GetService<CoinDbContext>());
    }

    private static void SeedData(CoinDbContext? context)
    {
        context!.Database.Migrate();
        if (context.Currencies.Any())
        {
            System.Console.WriteLine("Already have data");
            return;
        }

        var currencies = new List<Currency>()
        {
            new Currency
            {
                Code = "USD",
                Name = "美元"
            },
            new Currency
            {
                Code = "GBP",
                Name = "英鎊"
            },
            new Currency
            {
                Code = "EUR",
                Name = "歐元"
            }
        };

        context.AddRange(currencies);

        context.SaveChanges();

    }

}
