﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Models;

[Table("CarModel")]
public partial class CarModel
{
    [Key]
    public int CarModelId { get; set; }

    [Column("CarModel")]
    [StringLength(50)]
    public string CarModelName { get; set; } = null!;

    [InverseProperty("CarModel")]
    public virtual ICollection<CarBrandModel> CarBrandModels { get; set; } = new List<CarBrandModel>();
}
