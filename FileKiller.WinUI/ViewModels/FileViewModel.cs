namespace FileKiller.WinUI.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;

public partial class ItemViewModel(string path, ItemType type) : ObservableObject
{
    public string Name { get; private set; } = path.Split(["/","\\"], System.StringSplitOptions.RemoveEmptyEntries).Last();
    public string Path { get; private set; } = path;
    public ItemType Type { get; private set; } = type;

    [ObservableProperty]
    private bool _isDeleted;
    [ObservableProperty]
    private double _progress;
}
public enum ItemType
{
    File,
    Directory
}