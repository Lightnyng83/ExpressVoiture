using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Models;

[Table("CarBrandModel")]
public partial class CarBrandModel
{
    [Key]
    [Column("CarBrandModelId")]
    public int CarBrandModelId { get; set; }

    public int CarBrandId { get; set; }

    public int CarModelId { get; set; }

    [ForeignKey("CarBrandId")]
    [InverseProperty("CarBrandModels")]
    public virtual CarBrand CarBrand { get; set; } = null!;

    [ForeignKey("CarModelId")]
    [InverseProperty("CarBrandModels")]
    public virtual CarModel CarModel { get; set; } = null!;


    [InverseProperty("CarBrandModel")]
    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
