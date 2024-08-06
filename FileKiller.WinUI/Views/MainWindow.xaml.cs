using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage;
using FileKiller.Core.Helpers;
using Windows.ApplicationModel.DataTransfer;
using FileKiller.WinUI.ViewModels;
using FileKiller.WinUI.Helpers;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileKiller.WinUI.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; } = new();

        public MainWindow()
        {
            this.InitializeComponent();
            MainGrid.Drop += OnDrop;
            MainGrid.DragOver += OnDragOver;
            MainGrid.Loaded += OnLoaded;
            this.SetWindowSize(800, 400);
            this.ExtendsContentIntoTitleBar = true;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Items.Count > 0)
            {
                InfoGrid.Visibility = Visibility.Collapsed;
                await Task.Delay(300);
                await ViewModel.DeleteItemsCommand.ExecuteAsync(null);
                await Task.Delay(300);
                this.Close();
            }
        }

        private async void OnActivated(object sender, WindowActivatedEventArgs args)
        {

        }

        private async void OnDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                ViewModel.AddItems(items);
            }
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }
        //private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
        //{
        //    PickFolderOutputTextBlock.Text = "";

        //    var openPicker = new Windows.Storage.Pickers.FolderPicker();

        //    var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

        //    WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        //    openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
        //    openPicker.FileTypeFilter.Add("*");

        //    StorageFolder folder = await openPicker.PickSingleFolderAsync();
        //    if (folder != null)
        //    {
        //        StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
        //        PickFolderOutputTextBlock.Text = "Picked folder: " + folder.Name;
        //        FileHelper.DeleteFolderAsync(new DirectoryInfo(folder.Path));
        //        Debug.WriteLine("DONE");


        //    }
        //    else
        //    {
        //        PickFolderOutputTextBlock.Text = "Operation cancelled.";
        //    }
        //}
    }
}
