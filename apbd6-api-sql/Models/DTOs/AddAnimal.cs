using System.ComponentModel.DataAnnotations;

namespace Tutorial5.Models.DTOs;

public class AddAnimal
{
    
    // [Required]
    // // [MinLength(1)]
    // // [MaxLength(200)]
    // public int IdAnimal { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(200)]
    public string Name { get; set; }
    
    // nullable
    [MinLength(3)]
    [MaxLength(200)]
    public string? Description { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(200)]
    public string Category { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(200)]
    public string Area { get; set; }
    
}