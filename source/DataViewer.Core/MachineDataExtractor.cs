using DataViewer.Core.Contracts;
using DataViewer.Core.Models;
using System.Text;

namespace DataViewer.Core;

public class MachineDataExtractor : IDataExtractor
{
    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetPortsFromDataAsync(string data, CancellationToken cancellationToken)
    {
        string portsBlock = await GetBlockFromDataAsync(data, 3, cancellationToken);

        List<string> ports = new List<string>();
        portsBlock.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                 .Skip(1)
                 .ToList()
                 .ForEach(x => ports.Add(x));

        return ports;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AnalyticsRow>> GetAnalyticsFromDataAsync(string data, CancellationToken cancellationToken)
    {
        string analyticsBlock = await GetBlockFromDataAsync(data, 4, cancellationToken);

        List<AnalyticsRow> analytics = new List<AnalyticsRow>();
        analyticsBlock.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                      .Skip(1)
                      .ToList()
                      .ForEach(x =>
                      {
                          string[] values = x.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                          analytics.Add(new AnalyticsRow(
                              values[0],
                              values[1],
                              Convert.ToInt64(values[2]),
                              Convert.ToDouble(values[3].Replace(".", ",")),
                              Convert.ToDouble(values[4].Replace(".", ","))));
                      });

        return analytics;
    }

    private async Task<string> GetBlockFromDataAsync(string data, int blockIndex, CancellationToken cancellationToken)
    {
        using (StringReader reader = new StringReader(data))
        {
            StringBuilder currentBlock = new StringBuilder();
            int currentBlockIndex = 0;

            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(line))
                {
                    if (currentBlockIndex == blockIndex)
                    {
                        return currentBlock.ToString();
                    }

                    currentBlockIndex++;
                    currentBlock.Clear();
                }
                else
                {
                    currentBlock.AppendLine(line);
                }
            }

            return currentBlock.ToString();
        }
    }
}