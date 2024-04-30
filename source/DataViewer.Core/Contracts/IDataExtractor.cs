using DataViewer.Core.Models;

namespace DataViewer.Core.Contracts;
public interface IDataExtractor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<string>> GetProgramsFromDataAsync(string data, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<string>> GetPortsFromDataAsync(string data, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<AnalyticsRow>> GetAnalyticsFromDataAsync(string data, CancellationToken cancellationToken);
}
