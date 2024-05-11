using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class Country
{
    public int Id { get; set; }

    public string CountryName { get; set; }

    public virtual ICollection<University> Universities { get; set; } = new List<University>();
}
