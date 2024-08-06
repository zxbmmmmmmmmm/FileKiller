using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FileKiller.Core.Helpers;

public static class FileHelper
{
    [DllImport("NativeLibs/EzUnlock.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern bool EzUnlockFileW(string path);

    [DllImport("NativeLibs/EzUnlock.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern bool EzDeleteFileW(string path);

    public static event EventHandler<ProgressingItemChangedEventArgs>? ItemChanged;
    public static string? Message { get; set; }
    public static async Task<bool> DeleteFolderAsync(DirectoryInfo folder)
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


    public static async Task<bool> UnlockFolderAsync(DirectoryInfo folder)
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

    public static async Task<bool> UnlockFileAsync(string path)
    {
        return await Task.Run<bool>(() =>
        {
            //FileHelper.ItemChanged.Invoke();
            var result = EzUnlockFileW(path);
            Debug.WriteLine(path + ":" + result);
            return result;
        });
    }
    public static async Task<bool> DeleteFileAsync(string path)
    {
        return await Task.Run<bool>(() =>
        {
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