using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataViewer.Core.Contracts;
using Microsoft.Win32;

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

    [ObservableProperty]
    private bool _isBusy = false;

    [ObservableProperty]
    private bool _isFileLoaded = false;

    [ObservableProperty]
    private int _selectedRibbonTabIndex = 0;

    [ObservableProperty]
    private bool _isRibbonTabAnalyticsEnabled = false;

    [ObservableProperty]
    private string _selectedFileHistoryEntry = string.Empty;

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
    private async Task LoadFileFromDialog(CancellationToken cancellationToken)
    {
        OpenFileDialog fileDialog = new OpenFileDialog
        {
            Title = "Datei auswählen",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            RestoreDirectory = true,
            Filter = "Alle Dateien (*.*)|*.*|Text-Dateien (*.txt)|*.txt"
        };

        if (fileDialog.ShowDialog() == true)
        {
            string fileName = fileDialog.FileName;
            await this.LoadFile(fileName, cancellationToken);
        }
    }

    [RelayCommand]
    private async Task LoadFile(string filePath, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(filePath))
        {
            return;
        }

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
        try
        {
            this.IsBusy = true;

            string? data = await iOProvider.GetFileContentAsync(loadedFilePath, Encoding.UTF7, cancellationToken);
            await this.DisplayAnalyticsFromDataAsync(data, cancellationToken);
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
        finally
        {
            this.IsBusy = false;
        }
    }

    private async Task DisplayAnalyticsFromDataAsync(string data, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        this.IsFileLoaded = true;
        this.IsRibbonTabAnalyticsEnabled = true;
        this.SelectedRibbonTabIndex = 1;

        IEnumerable<string> analytics = await dataExtractor.GetAnalyticsFromData(data, cancellationToken);
    }

    [RelayCommand]
    private async Task DeleteFromHistory(string filePath, CancellationToken cancellationToken)
    {
        this.FileHistory.Remove(filePath);

        await settingsStorage.Write("FileHistory", this.FileHistory.ToArray(), cancellationToken);
    }

    [RelayCommand]
    private async Task CloseFile(CancellationToken cancellationToken)
    {
        await this.CloseFileAndClearDisplay(cancellationToken);
    }

    [RelayCommand]
    private async Task AppInfo(CancellationToken cancellationToken)
    {
        string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

        StringBuilder appInfoStringBuilder = new StringBuilder();
        appInfoStringBuilder.AppendLine("DataViewer by lk-code");
        appInfoStringBuilder.AppendLine($"Version: {version}");

        AdonisUI.Controls.MessageBox.Show(appInfoStringBuilder.ToString(), "Info", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Information);

        await Task.CompletedTask;
    }

    private async Task CloseFileAndClearDisplay(CancellationToken cancellationToken)
    {
        this.IsFileLoaded = false;
        this.IsRibbonTabAnalyticsEnabled = false;
        this.SelectedRibbonTabIndex = 0;
        this.LoadedFilePath = "";

        await Task.CompletedTask;
    }

    [RelayCommand]
    private void ExitApplication()
    {
        Application.Current.Shutdown();
    }
}
