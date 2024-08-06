using System.IO;
using FileKiller.Core.Services;

namespace FileKiller.WinUI.ViewModels;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileKiller.WinUI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

public partial class MainWindowViewModel:ObservableObject
{

    public static MainWindowViewModel? Instance { get; private set; }

    public string? Command { get; set; }

    [ObservableProperty]
    public string? _message;

    private readonly FileOperationService _fileService = new();
    public MainWindowViewModel()
    {
        Instance = this;
        _fileService.CurrentItemChanged += CurrentItemChanged;
    }

    private async void CurrentItemChanged(object? sender, ProgressingItemChangedEventArgs e)
    {
        MainWindow.Current.DispatcherQueue.TryEnqueue(() => {
            Message = "正在处理 " + e.ItemName;
        });
    }

    [ObservableProperty]
    private ObservableCollection<ItemViewModel> _items = [];

    [ObservableProperty]
    private ObservableCollection<ItemViewModel> _selectedItems = [];

    [RelayCommand]
    public void RemoveSelectedItems()
    {
        while(SelectedItems.Count > 0)
        {
            Items.Remove(SelectedItems[0]);
        }

    }

    [RelayCommand]
    public async Task DeleteItemsAsync()
    {
        try
        {
            var count = Items.Count;
            var num = 0;
            while (count > 0)
            {
                var result = true;
                var info = new DirectoryInfo(Items[num].Path);
                if (Directory.Exists(Items[num].Path))
                {
                    result = await _fileService.DeleteFolderAsync(new DirectoryInfo(Items[num].Path));
                }
                else
                {
                    result = await _fileService.DeleteFileAsync(Items[num].Path);
                }

                if (result)
                {
                    Items.RemoveAt(num);
                }
                else
                {
                    num++;
                }
                count -= 1;
            }
            Message = "完成";

        }
        catch (Exception e)
        {
            Message = "发生错误:" + e.Message;
        }

    }

    [RelayCommand]
    public async Task UnlockItemsAsync()
    {
        try
        {
            var count = Items.Count;
            var num = 0;
            while (count > 0)
            {
                var result = true;
                var info = new DirectoryInfo(Items[num].Path);
                if (Directory.Exists(Items[num].Path))
                {
                    result = await _fileService.UnlockFolderAsync(new DirectoryInfo(Items[num].Path));
                }
                else
                {
                    result = await _fileService.UnlockFileAsync(Items[num].Path);
                }

                if (result)
                {
                    Items.RemoveAt(num);
                }
                else
                {
                    num++;
                }
                count -= 1;
            }
            Message = "完成";
        }
        catch (Exception e)
        {
            Message = "发生错误:" + e.Message;
        }
    }


    [RelayCommand]
    public void AddItems(IEnumerable<IStorageItem> items)
    {
        foreach (var item in items)
        {
            if (Items.All(p => p.Path.Equals(item.Path,System.StringComparison.Ordinal))&&Items.Count >0) continue;
            var vm = new ItemViewModel(item.Path, item.IsOfType(StorageItemTypes.Folder)?ItemType.Directory:ItemType.File);
            Items.Add(vm);
        }
    }

    public async Task DeleteFolderAsync(StorageFolder folder)
    {
        IReadOnlyList<StorageFolder> folders = null;
        try
        {
            folders = await folder.GetFoldersAsync();
        }
        catch
        {
            //IGNORE
        }
        if (folders != null)
        {
            foreach (var item in folders)
            {
                await DeleteFolderAsync(item);
            }
        }
        IReadOnlyList<StorageFile> files = null;
        try
        {
            files = await folder.GetFilesAsync();
        }
        catch
        {
            //IGNORE
        }
        if (files != null)
        {
            foreach (var item in files)
            {
                var result = FileOperationService.EzDeleteFileW(item.Path);
                Debug.WriteLine(item.Path + ":" + result);
            }
        }

        await folder.DeleteAsync();

    }

}