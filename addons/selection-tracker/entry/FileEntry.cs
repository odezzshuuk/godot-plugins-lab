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

  private void CacheFileInfo(string path) {

  }
}
#endif
