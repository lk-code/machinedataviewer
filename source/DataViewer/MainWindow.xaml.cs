using System.Windows;
using AdonisUI.Controls;
using DataViewer.ViewModels;

namespace DataViewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : AdonisWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        this.DataContext = viewModel;
    }

    private void MainWindow_DragEnter(object sender, System.Windows.DragEventArgs e)
    {
        ((MainViewModel)this.DataContext).DragEnterCommand.Execute(e);

        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void MainWindow_DragLeave(object sender, DragEventArgs e)
    {
        ((MainViewModel)this.DataContext).DragLeaveCommand.Execute(e);
    }

    private void MainWindow_Drop(object sender, System.Windows.DragEventArgs e)
    {
        ((MainViewModel)this.DataContext).DropCommand.Execute(e);
    }
}
