using System;

namespace CoindeskApi.DTOs;

public class CurrencyDTO
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public string? Name { get; set; }
}
