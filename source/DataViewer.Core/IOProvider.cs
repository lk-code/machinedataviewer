using System.Text;
using DataViewer.Core.Contracts;

namespace DataViewer.Core;

public class IOProvider : IIOProvider
{
    private readonly Encoding DefaultEncoding = Encoding.UTF8;

    /// <inheritdoc />
    public async Task<string> GetFileContentAsync(string filePath, CancellationToken cancellationToken)
    {
        return await this.GetFileContentAsync(filePath, DefaultEncoding, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> GetFileContentAsync(string filePath, Encoding encoding, CancellationToken cancellationToken)
    {
        return await File.ReadAllTextAsync(filePath, encoding, cancellationToken);
    }

    /// <inheritdoc />
    public async Task WriteFileContentAsync(string filePath, string content, CancellationToken cancellationToken)
    {
        await WriteFileContentAsync(filePath, content, DefaultEncoding, cancellationToken);
    }

    /// <inheritdoc />
    public async Task WriteFileContentAsync(string filePath, string content, Encoding encoding, CancellationToken cancellationToken)
    {
        await File.WriteAllTextAsync(filePath, content, encoding, cancellationToken);
    }

    /// <inheritdoc />
    public async Task CreateDirectoryAsync(string directoryPath, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        Directory.CreateDirectory(directoryPath);
    }

    /// <inheritdoc />
    public async Task CreateFileAsync(string filePath, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        File.Create(filePath);
    }
}