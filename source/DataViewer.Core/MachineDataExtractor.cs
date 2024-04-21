using DataViewer.Core.Contracts;

namespace DataViewer.Core;

public class MachineDataExtractor : IDataExtractor
{
    public Task<IEnumerable<string>> GetAnalyticsFromData(string data, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<string>>(new List<string>());
    }
}
