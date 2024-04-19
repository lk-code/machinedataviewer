using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataViewer.Core.Contracts;

namespace DataViewer.ViewModels;

public partial class MainViewModel(IIOProvider iOProvider,
    IDataExtractor dataExtractor) : ObservableObject
{
    [ObservableProperty]
    private string _title = "DataViewer by lk-code";

    [ObservableProperty]
    private bool _isDragDropUIVisible = false;

    [ObservableProperty]
    private string _loadedFilePath = "";

    [RelayCommand]
    private async Task DragEnter(System.Windows.DragEventArgs e, CancellationToken cancellationToken)
    {
        this.IsDragDropUIVisible = true;

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DragLeave(System.Windows.DragEventArgs e, CancellationToken cancellationToken)
    {
        this.IsDragDropUIVisible = false;

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task Drop(System.Windows.DragEventArgs e, CancellationToken cancellationToken)
    {
        this.IsDragDropUIVisible = false;

        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        string targetFile = files[0];

        this.LoadedFilePath = targetFile;

        await this.LoadDataAsync(this.LoadedFilePath, cancellationToken);
    }

    private async Task LoadDataAsync(string loadedFilePath, CancellationToken cancellationToken)
    {
        string data = await iOProvider.GetFileContentAsync(loadedFilePath, Encoding.UTF7, cancellationToken);

        if(string.IsNullOrEmpty(data))
        {
            return;
        }

        IEnumerable<string> analytics = await dataExtractor.GetAnalyticsFromData(data, cancellationToken);
    }
}
