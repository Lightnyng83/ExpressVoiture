using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Models;

[Table("CarBrand")]
public partial class CarBrand
{
    [Key]
    public int CarBrandId { get; set; }

    [Column("CarBrand")]
    [StringLength(50)]
    public string CarBrandName { get; set; } = null!;

    [InverseProperty("CarBrand")]
    public virtual ICollection<CarBrandModelId> CarBrandModelIds { get; set; } = new List<CarBrandModelId>();
}
