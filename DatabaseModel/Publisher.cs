﻿using System;
using System.Collections.Generic;

namespace SQLLab2;

public partial class Publisher
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
