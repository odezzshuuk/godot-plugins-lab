#if TOOLS
using Godot;
using System;
using System.Linq;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class NodeEntry : Entry {

  [Export]
  private string _cachedNodePath;
  [Export]
  private string _cachedSceneFileName;
  [Export]
  private string _cachedScenePath;

  [Export]
  protected ulong _instanceId;  // session-only

  [Export]
  protected string _cachedNodeType;

  private Node _cachedScene;
  private Node _cachedNode;

  public override string DisplayName => _cachedNode.Name != string.Empty
    ? _cachedNode.Name
    : _cachedName != string.Empty
      ? _cachedName
      : $"Missing Node ({_cachedNodeType})";

  public override EntryState CurrentEntryState {
    get {
      GD.Print($"Checking entry state for NodeEntry: {_cachedNode.Name}");
      if (!_cachedNode.IsQueuedForDeletion()) {
        GD.Print($"Node {_cachedNode.Name} is no longer valid. Marking as Deleted.");
        return EntryState.Deleted;
      }

      if (EditorInterface.Singleton.GetOpenScenes().Contains(_cachedScenePath)) {
        return EntryState.Loaded;
      } else {
        return EntryState.Unloaded;
      }
    }
  }

  public NodeEntry() { }

  public NodeEntry(Node node) {
    CacheNodeInfo(node);
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

  public override int GetHashCode() {
    return HashCode.Combine(_cachedNodePath);
  }

  public override void Locate() {
    if (_cachedNode == null) {
      return;
    }
    EditorInterface.Singleton.EditNode(_cachedNode);
  }

  public override void Open() {
    EditorInterface editor = EditorInterface.Singleton;
    if (CurrentEntryState.HasFlag(EntryState.Loaded)) {

    }

    if (CurrentEntryState.HasFlag(EntryState.Unloaded)) {
      editor.OpenSceneFromPath(_cachedScenePath);
      // TODO: restore node here ...
      editor.GetSelection().Clear();
      editor.GetSelection().AddNode(_cachedNode);
      editor.EditNode(_cachedNode);
      return;
    }
  }

  protected void CacheNodeInfo(Node node) {
    _cachedNode = node;
    _cachedNodePath = node.IsInsideTree() ? node.GetPath() : string.Empty;
    _cachedScenePath = GetScenePath(node);
    _cachedSceneFileName = _cachedScenePath.GetFile();
    _cachedScene = node.Owner;
    _cachedName = string.Concat(_cachedSceneFileName, "/", node.Name);
    _cachedNodeType = node.GetType().Name;
    _cachedIcon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("File", "EditorIcons");
    _instanceId = node.GetInstanceId();
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
