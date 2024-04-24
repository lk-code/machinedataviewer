using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataViewer.Core.Contracts;
using DataViewer.Core.Models;
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

    [ObservableProperty]
    private ObservableCollection<string> _ports = new();

    /// <summary>
    /// Mittelwert
    /// </summary>
    [ObservableProperty]
    private double _calculatedHeadWidthAverage = 0;

    /// <summary>
    /// Spannweite
    /// </summary>
    [ObservableProperty]
    private double _calculatedHeadWidthSpan = 0;

    /// <summary>
    /// Standard-Abweichung
    /// </summary>
    [ObservableProperty]
    private double _calculatedHeadWidthVariance = 0;

    /// <summary>
    /// min. wert
    /// </summary>
    [ObservableProperty]
    private double _calculatedHeadWidthMinValue = 0;

    /// <summary>
    /// max. wert
    /// </summary>
    [ObservableProperty]
    private double _calculatedHeadWidthMaxValue = 0;

    /// <summary>
    /// Mittelwert
    /// </summary>
    [ObservableProperty]
    private double _calculatedDistanceAverage = 0;

    /// <summary>
    /// Spannweite
    /// </summary>
    [ObservableProperty]
    private double _calculatedDistanceSpan = 0;

    /// <summary>
    /// Standard-Abweichung
    /// </summary>
    [ObservableProperty]
    private double _calculatedDistanceVariance = 0;

    /// <summary>
    /// min. wert
    /// </summary>
    [ObservableProperty]
    private double _calculatedDistanceMinValue = 0;

    /// <summary>
    /// max. wert
    /// </summary>
    [ObservableProperty]
    private double _calculatedDistanceMaxValue = 0;

    [ObservableProperty]
    private long _numberOfAnalyticsRows = 0;

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
        if (string.IsNullOrEmpty(filePath))
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

            // clean up
            data = null;
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

        _ = Task.WhenAll(
            this.LoadPortsFromData(data, cancellationToken),
            this.LoadAnalyticsFromData(data, cancellationToken)
        );
    }

    private async Task LoadPortsFromData(string data, CancellationToken cancellationToken)
    {
        List<string> ports = (await dataExtractor.GetPortsFromDataAsync(data, cancellationToken)).ToList();

        this.Ports.Clear();
        ports.ToList()
            .ForEach(x => this.Ports.Add(x));

        ports.Clear();
    }

    private async Task LoadAnalyticsFromData(string data, CancellationToken cancellationToken)
    {
        List<AnalyticsRow> analytics = (await dataExtractor.GetAnalyticsFromDataAsync(data, cancellationToken)).ToList();

        this.NumberOfAnalyticsRows = analytics.Count();

        // calculate head width
        double headWidthAverage = analytics.Average(x => x.HeadWidth);
        this.CalculatedHeadWidthAverage = headWidthAverage;
        double headWidthSpan = analytics.Max(x => x.HeadWidth) - analytics.Min(x => x.HeadWidth);
        this.CalculatedHeadWidthSpan = headWidthSpan;
        double headWidthVariance = analytics.Select(x => Math.Pow(x.HeadWidth - headWidthAverage, 2)).Sum() / analytics.Count();
        this.CalculatedHeadWidthVariance = headWidthVariance;
        double headWidthMinValue = analytics.Min(x => x.HeadWidth);
        this.CalculatedHeadWidthMinValue = headWidthMinValue;
        double headWidthMaxValue = analytics.Max(x => x.HeadWidth);
        this.CalculatedHeadWidthMaxValue = headWidthMaxValue;

        // calculate distance
        double distanceAverage = analytics.Average(x => x.Distance);
        this.CalculatedDistanceAverage = distanceAverage;
        double distanceSpan = analytics.Max(x => x.Distance) - analytics.Min(x => x.Distance);
        this.CalculatedDistanceSpan = distanceSpan;
        double distanceVariance = analytics.Select(x => Math.Pow(x.Distance - distanceAverage, 2)).Sum() / analytics.Count();
        this.CalculatedDistanceVariance = distanceVariance;
        double distanceMinValue = analytics.Min(x => x.Distance);
        this.CalculatedDistanceMinValue = distanceMinValue;
        double distanceMaxValue = analytics.Max(x => x.Distance);
        this.CalculatedDistanceMaxValue = distanceMaxValue;

        analytics.Clear();
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
        this.LoadedFilePath = "";
        this.IsFileLoaded = false;
        this.SelectedRibbonTabIndex = 0;
        this.IsRibbonTabAnalyticsEnabled = false;
        this.Ports.Clear();
        this.CalculatedHeadWidthAverage = 0;
        this.CalculatedHeadWidthSpan = 0;
        this.CalculatedHeadWidthVariance = 0;
        this.CalculatedDistanceAverage = 0;
        this.CalculatedDistanceSpan = 0;
        this.CalculatedDistanceVariance = 0;
        this.NumberOfAnalyticsRows = 0;

        await Task.CompletedTask;
    }

    [RelayCommand]
    private void ExitApplication()
    {
        Application.Current.Shutdown();
    }
}
