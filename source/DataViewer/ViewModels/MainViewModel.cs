using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataViewer.Core.Contracts;
using static AdonisUI.Helpers.HwndInterop;

namespace DataViewer.ViewModels;

public partial class MainViewModel(ISettingsStorage settingsStorage,
    IIOProvider iOProvider,
    IDataExtractor dataExtractor) : ObservableObject
{
    [ObservableProperty]
    private string _title = "DataViewer by lk-code";

    [ObservableProperty]
    private bool _isDragDropUIVisible = false;

    [ObservableProperty]
    private string _loadedFilePath = "";

    [ObservableProperty]
    private ObservableCollection<string> _fileHistory = new();

    [RelayCommand]
    private async Task DragEnter(System.Windows.DragEventArgs e, CancellationToken cancellationToken)
    {
        this.IsDragDropUIVisible = true;

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task Loaded(CancellationToken cancellationToken)
    {
        string[]? fileHistory = await settingsStorage.Read<string[]>("FileHistory", cancellationToken);

        this.FileHistory.Clear();
        if (fileHistory is not null)
        {
            fileHistory.ToList()
                .ForEach(x => this.FileHistory.Add(x));
        }
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

        await this.LoadFile(targetFile, cancellationToken);
    }

    [RelayCommand]
    private async Task LoadFile(string filePath, CancellationToken cancellationToken)
    {
        this.LoadedFilePath = filePath;

        if (this.FileHistory.Contains(filePath))
        {
            this.FileHistory.Remove(filePath);
        }
        this.FileHistory.Insert(0, filePath);

        await settingsStorage.Write("FileHistory", this.FileHistory.ToArray(), cancellationToken);

        await this.LoadDataAsync(this.LoadedFilePath, cancellationToken);
    }

    private async Task LoadDataAsync(string loadedFilePath, CancellationToken cancellationToken)
    {
        string? data = null;
        try
        {
            data = await iOProvider.GetFileContentAsync(loadedFilePath, Encoding.UTF7, cancellationToken);
        }
        catch (System.IO.FileNotFoundException)
        {
            AdonisUI.Controls.MessageBox.Show("Datei wurde nicht gefunden.", "Fehler", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            AdonisUI.Controls.MessageBox.Show("Datei wurde nicht gefunden.", "Fehler", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            AdonisUI.Controls.MessageBox.Show(ex.Message, "Fehler", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
        }

        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        IEnumerable<string> analytics = await dataExtractor.GetAnalyticsFromData(data, cancellationToken);
    }
}
