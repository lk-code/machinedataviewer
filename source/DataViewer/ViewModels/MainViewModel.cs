using CommunityToolkit.Mvvm.ComponentModel;

namespace DataViewer.ViewModels;

public partial class MainViewModel() : ObservableObject
{
    [ObservableProperty]
    private string _title = "DataViewer by lk-code";
}
