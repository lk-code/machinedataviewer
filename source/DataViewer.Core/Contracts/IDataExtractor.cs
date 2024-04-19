namespace DataViewer.Core.Contracts;
public interface IDataExtractor
{
    Task<IEnumerable<string>> GetAnalyticsFromData(string data, CancellationToken cancellationToken);
}
