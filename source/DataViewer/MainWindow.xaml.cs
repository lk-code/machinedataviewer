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

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ((MainViewModel)this.DataContext).LoadedCommand.Execute(e);
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

    private void MainWindow_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0)
        {
            return;
        }

        string filePath = (string)e.AddedItems[0]!;
        ((MainViewModel)this.DataContext).LoadFileCommand.Execute(filePath);
    }

    private void MainWindow_OpenFileButton_Click(object sender, RoutedEventArgs e)
    {
        string filePath = ((MainViewModel)this.DataContext).SelectedFileHistoryEntry;

        ((MainViewModel)this.DataContext).LoadFileCommand.Execute(filePath);
    }

    private void MainWindow_DeleteFromFileHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        string filePath = ((MainViewModel)this.DataContext).SelectedFileHistoryEntry;

        ((MainViewModel)this.DataContext).DeleteFromHistoryCommand.Execute(filePath);
    }

    private void MainWindow_FileHistoryListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        string filePath = ((MainViewModel)this.DataContext).SelectedFileHistoryEntry;

        ((MainViewModel)this.DataContext).LoadFileCommand.Execute(filePath);
    }
}
