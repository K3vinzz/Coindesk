using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoindeskApi.Models;

public class Currency
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public string? Name { get; set; }
}
