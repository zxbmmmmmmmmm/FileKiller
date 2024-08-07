using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FileKiller.Core.Services;

public class FileOperationService
{
    [DllImport("NativeLibs/EzUnlock.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern bool EzUnlockFileW(string path);

    [DllImport("NativeLibs/EzUnlock.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern bool EzDeleteFileW(string path);

    public event EventHandler<ProgressingItemChangedEventArgs>? CurrentItemChanged;
    public string? Message { get; set; }
    public async Task<bool> DeleteFolderAsync(DirectoryInfo folder)
    {
        var result = true;
        foreach (var item in folder.GetDirectories())
        {
            if (!await DeleteFolderAsync(item)) result = false;
        }

        foreach (var item in folder.GetFiles("*.*", SearchOption.AllDirectories))
        {
            if(!await DeleteFileAsync(item.FullName))result = false;
        }

        try
        {
            folder.Delete();
            return result;
        }
        catch { return false; }
    }


    public async Task<bool> UnlockFolderAsync(DirectoryInfo folder)
    {
        var result = true;
        foreach (var item in folder.GetDirectories())
        {
            if (!await UnlockFolderAsync(item)) result = false;
        }

        foreach (var item in folder.GetFiles("*.*", SearchOption.AllDirectories))
        {
            if (!await UnlockFileAsync(item.FullName)) result = false;
        }

        return result;
    }

    public async Task<bool> UnlockFileAsync(string path)
    {
        return await Task.Run<bool>(() =>
        {
            CurrentItemChanged?.Invoke(this, new ProgressingItemChangedEventArgs(path));
            //FileHelper.ItemChanged.Invoke();
            var result = EzUnlockFileW(path);
            Debug.WriteLine(path + ":" + result);
            return result;
        });
    }
    public async Task<bool> DeleteFileAsync(string path)
    {
        return await Task.Run<bool>(() =>
        {
            CurrentItemChanged?.Invoke(this, new ProgressingItemChangedEventArgs(path));
            var result = EzDeleteFileW(path);
            Debug.WriteLine(path + ":" + result);
            return result;
        });
    }
}
public class ProgressingItemChangedEventArgs(string item) : EventArgs
{
    public string ItemName { get; } = item;
}