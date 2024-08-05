using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FileKiller.Core.Helpers;

public class FileHelper
{
    [DllImport("EzUnlock.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern bool EzUnlockFileW(string path);

    [DllImport("EzUnlock.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern bool EzDeleteFileW(string path);
    public static bool DeleteFolder(DirectoryInfo folder)
    {
        var result = true;
        foreach (var item in folder.GetDirectories())
        {
            if (!DeleteFolder(item)) result = false;
        }

        foreach (var item in folder.GetFiles("*.*", SearchOption.AllDirectories))
        {
            var fileResult = EzDeleteFileW(item.FullName);
            if (fileResult!) result = false;
            Debug.WriteLine(item.FullName + ":" + fileResult);
        }

        try
        {
            folder.Delete();
            return result;

        }
        catch { return false; }
    }

}