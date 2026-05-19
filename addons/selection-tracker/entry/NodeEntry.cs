
#if TOOLS
using Godot;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class NodeEntry(Node node) : Entry {

  [Export]
  private string _cachedSceneName;
  [Export]
  private string _cachedScenePath;

  private Node _cachedNode = node;

  public override string DisplayName => _cachedNode.Name;

  public override void Locate() {
    if (_cachedNode == null) {
      return;
    }
    EditorInterface.Singleton.EditNode(_cachedNode);
  }

}
#endif
