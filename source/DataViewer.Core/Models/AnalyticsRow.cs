using System.Globalization;

namespace DataViewer.Core.Models;

public record AnalyticsRow(string dateValue,
        string timeValue,
        long profileNumber,
        double headWidth,
        double distance)
{
    public string DateValue { get; set; } = dateValue;
    public string TimeValue { get; set; } = timeValue;
    public DateTime DateTime => DateTime.ParseExact($"{DateValue} {TimeValue}", "MM/dd/yyyy HH:mm:ss:fff", CultureInfo.InvariantCulture);
    public long ProfileNumber { get; set; } = profileNumber;
    public double HeadWidth { get; set; } = headWidth;
    public double Distance { get; set; } = distance;
}
