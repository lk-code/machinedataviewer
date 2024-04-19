using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DataViewer.ViewModels;

public partial class MainViewModel() : ObservableObject
{
    [ObservableProperty]
    private string _title = "DataViewer by lk-code";

    [ObservableProperty]
    private bool _isDragDropUIVisible = false;

    [ObservableProperty]
    private string _loadedFilePath = "";

    [RelayCommand]
    private void DragEnter(System.Windows.DragEventArgs e)
    {
        this.IsDragDropUIVisible = true;
    }

    [RelayCommand]
    private void DragLeave(System.Windows.DragEventArgs e)
    {
        this.IsDragDropUIVisible = false;
    }

    [RelayCommand]
    private void Drop(System.Windows.DragEventArgs e)
    {
        this.IsDragDropUIVisible = false;

        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        string targetFile = files[0];

        this.LoadedFilePath = targetFile;
    }
}
