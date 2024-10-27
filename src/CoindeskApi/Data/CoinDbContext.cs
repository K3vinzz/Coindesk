using System;
using CoindeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoindeskApi.Data;

public class CoinDbContext : DbContext
{
    public CoinDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Currency> Currencies { get; set; }

}
