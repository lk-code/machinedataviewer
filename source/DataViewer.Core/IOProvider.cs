using System.Text;
using DataViewer.Core.Contracts;

namespace DataViewer.Core;

public class IOProvider : IIOProvider
{
    public async Task<string> GetFileContentAsync(string filePath, Encoding encoding, CancellationToken cancellationToken)
    {
        string contentAsAscii = await File.ReadAllTextAsync(filePath, encoding, cancellationToken);

        return contentAsAscii;
    }
}