using System.Text;

namespace DataViewer.Core.Contracts;

public interface IIOProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetFileContentAsync(string filePath, Encoding encoding, CancellationToken cancellationToken);
}