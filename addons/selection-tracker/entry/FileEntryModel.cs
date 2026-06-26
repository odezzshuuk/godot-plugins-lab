#if TOOLS
using Godot;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class FileEntryModel : EntryModel {

  [Export] private Resource _cachedResource;
  [Export] private string _cachedFilePath;

  // public override Variant Ref => _cachedResource;
  public override string DisplayName => _cachedFilePath.GetFile();

  public override EntryState CurrentEntryState {
    get => _cachedRefState;
    set {
      _cachedRefState = value;
      onStateUpdated.Invoke(value);
    }
  }

  public FileEntryModel() { }
  public FileEntryModel(string filePath) {
    EditorInterface.Singleton.GetResourceFilesystem().FilesystemChanged += FileSystemChangedCallback;
    _cachedFilePath = filePath;
    string ft = EditorInterface.Singleton.GetResourceFilesystem().GetFileType(filePath);
    _cachedIcon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon(ft, "EditorIcons");
    _cachedRefState = EntryState.Existed;
    _cachedResource = ResourceLoader.Load(filePath);

    _dragPayloadType = "files";
    _dragPayloadData = filePath;
  }

  // public override bool Equals(object obj) {
  //   return obj is FileEntry other && Equals(other);
  // }

  public override bool Equals(EntryModel other) {
    // if base equality check fails, the entries are not equal
    if (!base.Equals(other)) {
      if (other.DisplayName == "entry.tscn" && DisplayName == "entry.tscn") {
        GD.Print("base check: Should be equal");
      }
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
    EditorInterface.Singleton.EditResource(_cachedResource);
  }

  public override Variant GetRef() {
    return _cachedResource;
  }

  protected long GetResourceUid(string resourcePath) {
    return string.IsNullOrEmpty(resourcePath)
      ? -1
      : ResourceLoader.GetResourceUid(resourcePath);
  }


  private void FileSystemChangedCallback() {
    if (FileAccess.FileExists(_cachedFilePath)) {
      CurrentEntryState = EntryState.Existed;
    } else {
      CurrentEntryState = EntryState.Deleted;
    }
  }
}
#endif
