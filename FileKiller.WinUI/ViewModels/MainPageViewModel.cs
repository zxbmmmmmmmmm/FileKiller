using System.IO;
using FileKiller.Core.Helpers;

namespace FileKiller.WinUI.ViewModels;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

public partial class MainPageViewModel:ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ItemViewModel> _items = [];

    [RelayCommand]
    public async Task DeleteItemsAsync()
    {
        foreach (var itemViewModel in Items)
        {
            itemViewModel.IsDeleted = await FileHelper.DeleteFolderAsync(new DirectoryInfo(itemViewModel.Path));
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
                var result = FileHelper.EzDeleteFileW(item.Path);
                Debug.WriteLine(item.Path + ":" + result);

            }
        }

        await folder.DeleteAsync();

    }

}