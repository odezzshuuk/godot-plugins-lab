#if TOOLS
using Godot;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class FileEntry(string filePath) : Entry {

  private string _cachedFilePath = filePath;

  public override string DisplayName => System.IO.Path.GetFileName(_cachedFilePath);

  public override void Locate() {
    if (string.IsNullOrEmpty(_cachedFilePath)) {
      return;
    }
    EditorInterface.Singleton.SelectFile(_cachedFilePath);
  }
}
#endif
