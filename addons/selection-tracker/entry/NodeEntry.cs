#if TOOLS
using Godot;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class NodeEntry : Entry {

  [Export]
  private string _cachedNodePath;
  [Export]
  private string _cachedSceneName;
  [Export]
  private string _cachedScenePath;

  [Export]
  protected ulong _instanceId;

  [Export]
  protected string _cachedNodeType;

  private Node _cachedScene;

  private Node _cachedNode;


  public override string DisplayName => _cachedNode?.Name ?? "Empty";

  public override RefState CurrentRefState {
    get {
      if (_cachedNode == null) {
        return RefState.Unloaded;
      }

      if (!_cachedNode.IsInsideTree()) {
        return RefState.Freed;
      }

      return RefState.Loaded;
    }
  }

  public NodeEntry() { }

  public NodeEntry(Node node) {
    CacheNodeInfo(node);
  }

  public override void Locate() {
    if (_cachedNode == null) {
      return;
    }
    EditorInterface.Singleton.EditNode(_cachedNode);
  }

  public override bool Equals(Entry other) {
    if (!base.Equals(other)) {
      return false;
    }

    if (other is not NodeEntry otherNodeEntry) {
      return false;
    }

    return otherNodeEntry._cachedNode == _cachedNode ||
      otherNodeEntry._instanceId == _instanceId;
  }

  public override void Open() {
    EditorInterface editor = EditorInterface.Singleton;
    if (CurrentRefState.HasFlag(RefState.Loaded)) {

    }

    if (CurrentRefState.HasFlag(RefState.Unloaded)) {
      editor.OpenSceneFromPath(_cachedScenePath);
      // TODO: restore node here ...
      editor.GetSelection().Clear();
      editor.GetSelection().AddNode(_cachedNode);
      editor.EditNode(_cachedNode);
      return;
    }
  }

  public override int GetHashCode() {
    return _instanceId.GetHashCode();
  }

  protected void CacheNodeInfo(Node node) {
    _cachedNode = node;
    _cachedNodePath = node.IsInsideTree() ? node.GetPath() : string.Empty;
    _cachedScenePath = GetScenePath(node);
    _cachedScene = node.Owner;
    _cachedIcon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("File", "EditorIcons");
  }

  protected string GetScenePath(Node node) {
    if (!string.IsNullOrEmpty(node.SceneFilePath)) {
      return node.SceneFilePath;
    }

    Node current = node;
    while (current.GetParent() != null) {
      current = current.GetParent();
      if (!string.IsNullOrEmpty(current.SceneFilePath)) {
        return current.SceneFilePath;
      }
    }
    return string.Empty;
  }

}
#endif
