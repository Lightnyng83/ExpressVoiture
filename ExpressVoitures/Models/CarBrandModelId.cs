using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Models;

[Table("CarBrandModelId")]
public partial class CarBrandModelId
{
    [Key]
    [Column("CarBrandModelId")]
    public int CarBrandModelId1 { get; set; }

    public int CarBrandId { get; set; }

    public int CarModelId { get; set; }

    [ForeignKey("CarBrandId")]
    [InverseProperty("CarBrandModelIds")]
    public virtual CarBrand CarBrand { get; set; } = null!;

    [ForeignKey("CarModelId")]
    [InverseProperty("CarBrandModelIds")]
    public virtual CarModel CarModel { get; set; } = null!;

    [InverseProperty("CarBrandModel")]
    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
