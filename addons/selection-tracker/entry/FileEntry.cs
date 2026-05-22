#if TOOLS
using Godot;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class FileEntry : Entry {

  [Export]
  private Resource _loadedResource;
  [Export]
  private string _cachedFilePath;

  public override string DisplayName => _cachedFilePath.GetFile();

  public FileEntry() { }
  public FileEntry(string filePath) {
    _cachedFilePath = filePath;
    _cachedIcon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("File", "EditorIcons");
  }

  public override bool Equals(Entry other) {
    string check = "res://addons/selection-tracker/entry/FileEntry.cs";
    // if base equality check fails, the entries are not equal
    if (!base.Equals(other)) {
      return false;
    }

    if (other is not FileEntry otherFileEntry) {
      return false;
    }

    if (otherFileEntry._cachedFilePath == check && _cachedFilePath == check) {
      GD.Print($"{GetType().Name} Should be equal");
    }

    return _cachedFilePath == otherFileEntry._cachedFilePath;
  }

  public override int GetHashCode() {
    return _cachedFilePath.GetHashCode();
  }

  public override void Locate() {
    if (string.IsNullOrEmpty(_cachedFilePath)) {
      return;
    }
    EditorInterface.Singleton.SelectFile(_cachedFilePath);
  }

  public override void Open() {
    EditorInterface.Singleton.EditResource(_loadedResource);
  }

  protected long GetResourceUid(string resourcePath) {
    return string.IsNullOrEmpty(resourcePath)
      ? -1
      : ResourceLoader.GetResourceUid(resourcePath);
  }

  private void CacheFileInfo(string path) {

  }
}
#endif
