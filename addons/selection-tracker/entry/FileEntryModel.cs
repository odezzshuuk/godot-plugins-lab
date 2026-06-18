#if TOOLS
using Godot;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class FileEntryModel : EntryModel {

  [Export]
  private Resource _loadedResource;
  [Export]
  private string _cachedFilePath;

  public override string DisplayName => _cachedFilePath.GetFile();

  public override EntryState CurrentEntryState {
    get {
      if (FileAccess.FileExists(_cachedFilePath)) {
        return EntryState.Existed;
      } else {
        return EntryState.Deleted;
      }
    }
  }

  public FileEntryModel() { }
  public FileEntryModel(string filePath) {
    EditorInterface.Singleton.GetResourceFilesystem().FilesystemChanged += FileSystemChangedCallback;
    _cachedFilePath = filePath;
    _cachedName = filePath.GetFile();
    _cachedIcon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("File", "EditorIcons");
  }

  // public override bool Equals(object obj) {
  //   return obj is FileEntry other && Equals(other);
  // }

  public override bool Equals(EntryModel other) {
    // if base equality check fails, the entries are not equal
    if (!base.Equals(other)) {
      return false;
    }

    if (other is not FileEntryModel otherFileEntry) {
      return false;
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

  private void FileSystemChangedCallback() {

  }
}
#endif
