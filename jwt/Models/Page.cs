// Copyright (c) IUA. All rights reserved.

namespace jwt.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Page
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? Author { get; set; }
}
