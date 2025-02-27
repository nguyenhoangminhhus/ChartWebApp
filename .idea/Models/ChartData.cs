using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using Microsoft.VisualBasic.CompilerServices;

namespace ChartWebApp.Models;

public class ChartData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Ignore]
    public int Id { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime DateTime { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:n8}", ApplyFormatInEditMode = true)]
    public decimal MarketPrice { get; set; }
}