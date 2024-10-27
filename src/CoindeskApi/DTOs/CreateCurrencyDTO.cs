using System;
using System.ComponentModel.DataAnnotations;

namespace CoindeskApi.DTOs;

public class CreateCurrencyDTO
{
    [Required]
    public required string Code { get; set; }
    [Required]
    public required string Name { get; set; }
}
