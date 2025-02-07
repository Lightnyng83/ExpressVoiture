using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Models;

[Table("Car")]
public partial class Car
{
    [Key]
    public int CarId { get; set; }

    public int CarBrandModelId { get; set; }

    public int Year { get; set; }

    [StringLength(350)]
    public string? ImageUrl { get; set; }

    public int SellingPrice { get; set; }

    [ForeignKey("CarBrandModelId")]
    [InverseProperty("Cars")]
    public virtual CarBrandModelId CarBrandModel { get; set; } = null!;
}
