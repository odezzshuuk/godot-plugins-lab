#if TOOLS
using Godot;
using System;

namespace Odezzshuuk.Editor.SelectionTracker;

[Flags]
public enum RefState {
  Node = 1 << 0,
  Resource = 1 << 1,
  Scene = 1 << 2,

  Loaded = (1 << 4) | Node,
  Unloaded = (1 << 5) | Node,
  Freed = (1 << 6) | Node,
  Playing = (1 << 7) | Node,

  Deleted = 1 << 8,

  Instanced = Loaded | Scene,
  External = Unloaded | Scene,

  Unknown = 0,
  All = ~0,
}

/// <summary>
/// A scene node can usually be restored from its scene path plus node path.
/// Unsaved scene nodes and transient runtime-only nodes can only be restored while they are still alive.
/// Resources are restored from their resource path when available.
/// </summary>
[Tool]
public partial class Entry : Resource, IEquatable<Entry> {

  [Export] protected ulong _instanceId;
  [Export] protected string _scenePath = string.Empty;
  [Export] protected string _nodePath = string.Empty;
  [Export] protected string _resourcePath = string.Empty;
  [Export] protected long _resourceUid = -1;

  [Export] protected RefState _cachedRefState = RefState.Unknown;
  protected Texture2D _cachedRefIcon;
  protected GodotObject _cachedRef;

  public GodotObject Ref => ResolveReference();
  public virtual string DisplayName => "unknown";
  public Texture2D Icon => _cachedRefIcon;

  public virtual RefState CurrentRefState {
    get {
      GodotObject resolved = ResolveReference();

      if (resolved is Node node) {
        if (!node.IsInsideTree()) {
          return RefState.Unloaded;
        }

        return Engine.IsEditorHint() ? RefState.Loaded : RefState.Playing;
      }

      if (resolved is PackedScene) {
        return RefState.Scene;
      }

      if (resolved is Resource) {
        return RefState.Resource;
      }

      if (HasNodeIdentity()) {
        return string.IsNullOrEmpty(_scenePath)
          ? RefState.Freed
          : RefState.Unloaded;
      }

      if (HasResourceIdentity()) {
        return ResourceLoader.Exists(_resourcePath)
          ? _cachedRefState
          : RefState.Deleted;
      }

      return RefState.Unknown;
    }
  }

  public Entry() { }

  public Entry(GodotObject obj, Texture2D icon = null) {
    CacheIdentity(obj);
    CacheRefInfo(obj, icon);
  }

  public void Rebind(GodotObject obj, Texture2D icon = null) {
    CacheIdentity(obj);
    CacheRefInfo(obj, icon);
  }

  public virtual bool Equals(Entry other) {
    if (other is null) {
      return false;
    }

    if (ReferenceEquals(this, other)) {
      return true;
    }

    GodotObject thisRef = ResolveReference();
    GodotObject otherRef = other.ResolveReference();

    if (thisRef != null ^ otherRef != null) {
      return false;
    }

    if (thisRef == null && otherRef == null) {
      return _instanceId == other._instanceId
        && string.Equals(_scenePath, other._scenePath, StringComparison.Ordinal)
        && string.Equals(_nodePath, other._nodePath, StringComparison.Ordinal)
        && string.Equals(_resourcePath, other._resourcePath, StringComparison.Ordinal)
        && _resourceUid == other._resourceUid;
    }

    return thisRef.GetInstanceId() == otherRef.GetInstanceId();
  }

  public override bool Equals(object obj) {
    return obj is Entry other && Equals(other);
  }

  public override int GetHashCode() {
    GodotObject resolved = ResolveReference();
    if (resolved != null) {
      return resolved.GetInstanceId().GetHashCode();
    }

    return HashCode.Combine(_instanceId, _scenePath, _nodePath, _resourcePath, _resourceUid);
  }

  public virtual void Locate() {

    GodotObject resolved = ResolveReference(EditorInterface.Singleton);
    if (resolved is Node node) {
      EditorSelection selection = EditorInterface.Singleton.GetSelection();
      selection.Clear();
      selection.AddNode(node);
      EditorInterface.Singleton.EditNode(node);
      return;
    }

    if (resolved is Resource resource) {
      EditorInterface.Singleton.EditResource(resource);
      if (!string.IsNullOrEmpty(_resourcePath)) {
        EditorInterface.Singleton.SelectFile(_resourcePath);
      }
    }
  }

  public virtual void Open(EditorInterface editor) {
    if (!string.IsNullOrEmpty(_scenePath) && ResourceLoader.Exists(_scenePath, nameof(PackedScene))) {
      editor.OpenSceneFromPath(_scenePath);

      GodotObject reopened = ResolveReference(editor);
      if (reopened is Node reopenedNode) {
        EditorSelection selection = editor.GetSelection();
        selection.Clear();
        selection.AddNode(reopenedNode);
        editor.EditNode(reopenedNode);
        return;
      }
    }

    GodotObject resolved = ResolveReference(editor);
    if (resolved is Resource resource) {
      editor.EditResource(resource);
      if (!string.IsNullOrEmpty(_resourcePath)) {
        editor.SelectFile(_resourcePath);
      }
      return;
    }

    Locate();
  }

  public virtual GodotObject ResolveReference() {
    return ResolveReference((Node)null);
  }

  public virtual GodotObject ResolveReference(EditorInterface editor) {
    return ResolveReference(editor?.GetEditedSceneRoot());
  }

  public virtual GodotObject ResolveReference(Node editedSceneRoot) {
    if (IsAlive(_cachedRef)) {
      return _cachedRef;
    }

    if (!string.IsNullOrEmpty(_resourcePath) && ResourceLoader.Exists(_resourcePath)) {
      _cachedRef = ResourceLoader.Load(_resourcePath);
      return _cachedRef;
    }

    if (editedSceneRoot != null && !string.IsNullOrEmpty(_nodePath)) {
      if (!string.IsNullOrEmpty(_scenePath) && !string.Equals(GetScenePath(editedSceneRoot), _scenePath, StringComparison.Ordinal)) {
        return null;
      }

      Node node = editedSceneRoot.GetNodeOrNull(_nodePath);
      if (node != null) {
        _cachedRef = node;
        return _cachedRef;
      }
    }

    return null;
  }

  protected void CacheRefInfo(GodotObject obj, Texture2D icon = null) {
    if (!IsAlive(obj)) {
      return;
    }

    _cachedRef = obj;
    _cachedRefIcon = icon;
  }

  protected void CacheIdentity(GodotObject obj) {
    _instanceId = 0;
    _scenePath = string.Empty;
    _nodePath = string.Empty;
    _resourcePath = string.Empty;
    _resourceUid = -1;
    _cachedRefState = RefState.Unknown;

    if (!IsAlive(obj)) {
      return;
    }

    _instanceId = obj.GetInstanceId();

    switch (obj) {
      case Node node:
        _cachedRefState = RefState.Node;
        _scenePath = GetScenePath(node);
        _nodePath = node.IsInsideTree() ? node.GetPath() : string.Empty;
        break;

      case PackedScene packedScene:
        _cachedRefState = RefState.Scene;
        _resourcePath = packedScene.ResourcePath;
        _resourceUid = GetResourceUid(_resourcePath);
        break;

      case Resource resource:
        _cachedRefState = RefState.Resource;
        _resourcePath = resource.ResourcePath;
        _resourceUid = GetResourceUid(_resourcePath);
        break;
    }
  }

  protected static bool IsAlive(GodotObject obj) {
    return obj != null && IsInstanceValid(obj);
  }

  protected bool HasNodeIdentity() {
    return _cachedRefState.HasFlag(RefState.Node) || !string.IsNullOrEmpty(_nodePath);
  }

  protected bool HasResourceIdentity() {
    return _cachedRefState.HasFlag(RefState.Resource)
      || _cachedRefState.HasFlag(RefState.Scene)
      || !string.IsNullOrEmpty(_resourcePath);
  }

  protected static string GetScenePath(Node node) {
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

  protected static long GetResourceUid(string resourcePath) {
    return string.IsNullOrEmpty(resourcePath)
      ? -1
      : ResourceLoader.GetResourceUid(resourcePath);
  }
}
#endif
