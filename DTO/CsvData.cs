using CsvHelper.Configuration.Attributes;

namespace ChartWebApp.DTO;

public class CsvData
{
    [Name("Date")]
    public String Date { get; set; }
    [Name("Market Price EX1")]
    public decimal MarketPrice { get; set; }
}