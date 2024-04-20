using System.Text;

namespace DataViewer.Core.Contracts;

public interface IIOProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetFileContentAsync(string filePath, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetFileContentAsync(string filePath, Encoding encoding, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task WriteFileContentAsync(string filePath, string content, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task WriteFileContentAsync(string filePath, string content, Encoding encoding, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateDirectoryAsync(string directoryPath, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateFileAsync(string filePath, CancellationToken cancellationToken);
}