using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ExplorerExtensions.Helpers;

public class FileHelper
{
    [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern bool EzUnlockFileW(string path);

    [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern bool EzDeleteFileW(string path);
    /// <summary>
    /// 删除占用文件的所有应用程序
    /// </summary>
    /// <param name="fileName"></param>
    static void CloseApp(string filepath, string fileName)
    {
        Process tool = new Process();
        tool.StartInfo.FileName = filepath;
        tool.StartInfo.Arguments = fileName + " /accepteula";
        tool.StartInfo.UseShellExecute = false;
        tool.StartInfo.RedirectStandardOutput = true;
        tool.Start();
        tool.WaitForExit();
        string outputTool = tool.StandardOutput.ReadToEnd();
        string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";

        bool IsClosed = false;
        foreach (Match match in Regex.Matches(outputTool, matchPattern))
        {
            IsClosed = true;
            Process.GetProcessById(int.Parse(match.Value)).Kill();
        }
        if (!IsClosed)
        {
            string filename = Path.GetFileNameWithoutExtension(fileName);
            Process[] ProcessBuff = Process.GetProcessesByName(filename);
            for (int i = 0; i < ProcessBuff.Length; i++)
            {
                string path = ProcessBuff[i].MainModule.FileName;
                if (path == fileName)
                    ProcessBuff[i].Kill();
            }
        }
    }

    public static void WipeFile(string filename, int timesToWrite)
    {
        try
        {
            if (File.Exists(filename))
            {
                //设置文件的属性为正常，这是为了防止文件是只读
                File.SetAttributes(filename, FileAttributes.Normal);
                //计算扇区数目
                double sectors = Math.Ceiling(new FileInfo(filename).Length / 512.0);
                // 创建一个同样大小的虚拟缓存
                byte[] dummyBuffer = new byte[512];
                // 创建一个加密随机数目生成器
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                // 打开这个文件的FileStream
                FileStream inputStream = new FileStream(filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                for (int currentPass = 0; currentPass < timesToWrite; currentPass++)
                {
                    // 文件流位置
                    inputStream.Position = 0;
                    //循环所有的扇区
                    for (int sectorsWritten = 0; sectorsWritten < sectors; sectorsWritten++)
                    {
                        //把垃圾数据填充到流中
                        rng.GetBytes(dummyBuffer);
                        // 写入文件流中
                        inputStream.Write(dummyBuffer, 0, dummyBuffer.Length);
                    }
                }
                // 清空文件
                inputStream.SetLength(0);
                // 关闭文件流
                inputStream.Close();
                // 清空原始日期需要
                DateTime dt = new DateTime(2037, 1, 1, 0, 0, 0);
                File.SetCreationTime(filename, dt);
                File.SetLastAccessTime(filename, dt);
                File.SetLastWriteTime(filename, dt);
                // 删除文件
                File.Delete(filename);
            }
            else
            {
                //删除占用文件夹的所有应用程序
                var result = EzDeleteFileW(filename);
                Debug.WriteLine(result);
            }
        }
        catch (Exception)
        {
        }
    }
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