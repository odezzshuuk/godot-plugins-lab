#if TOOLS
using Godot;
using System;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class NodeEntryModel : EntryModel {

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

  [Export]
  private Node _cachedNode;

  private Node _cachedScene;

  private SceneTree _cachedSceneTree;

  private EntryState _currentEntryState;


  public override string DisplayName => _cachedNode.Name != string.Empty
    ? _cachedNode.Name
    : _cachedName != string.Empty
      ? _cachedName
      : $"Missing Node ({_cachedNodeType})";

  public override EntryState CurrentEntryState => _currentEntryState;

  public NodeEntryModel() { }

  public NodeEntryModel(Node node) {
    CacheNodeInfo(node);
    node.GetTree().NodeRemoved += NodeRemovedCallback;
    node.GetTree().SceneChanged += SceneChangedCallback;

  }


  public override bool Equals(EntryModel other) {
    if (!base.Equals(other)) {
      return false;
    }

    if (other is not NodeEntryModel otherNodeEntry) {
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
    _cachedSceneTree = node.GetTree();
    _cachedScene = node.Owner;
    _cachedName = string.Concat(_cachedSceneFileName, "/", node.Name);
    _cachedNodeType = node.GetType().Name;

    _cachedIcon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("File", "EditorIcons");
    _instanceId = node.GetInstanceId();
    _currentEntryState = EntryState.Loaded;
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

  private void NodeRemovedCallback(Node node) {
    if (node == _cachedNode) {
      _currentEntryState = EntryState.Deleted;
    }
  }

  private void SceneChangedCallback() {
    if (_cachedScene != _cachedSceneTree.CurrentScene) {
      _currentEntryState = EntryState.Unloaded;
    }
  }

}
#endif
