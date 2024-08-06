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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileKiller.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
        {
            PickFolderOutputTextBlock.Text = "";

            FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                PickFolderOutputTextBlock.Text = "Picked folder: " + folder.Name;
                FileHelper.DeleteFolderAsync(new DirectoryInfo(folder.Path));
                Debug.WriteLine("DONE");


            }
            else
            {
                PickFolderOutputTextBlock.Text = "Operation cancelled.";
            }
        }
    }
}
