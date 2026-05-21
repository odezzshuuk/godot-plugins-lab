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

  [Export] protected string _cachedName;

  [Export] protected Texture2D _cachedIcon;
  [Export] protected RefState _cachedRefState = RefState.Unknown;

  protected GodotObject _cachedRef;

  public GodotObject Ref => ResolveReference();
  public virtual string DisplayName => "unknown";
  public Texture2D Icon => _cachedIcon;

  public virtual RefState CurrentRefState => RefState.Unknown;

  public Entry() { }

  public virtual bool Equals(Entry other) {
    if (other is null) {
      return false;
    }

    if (!ReferenceEquals(this, other)) {
      return false;
    }

    GodotObject thisRef = ResolveReference();
    GodotObject otherRef = other.ResolveReference();

    if (thisRef != null ^ otherRef != null) {
      return false;
    }

    // if (thisRef == null && otherRef == null) {
    //   return _instanceId == other._instanceId
    //     && string.Equals(_scenePath, other._scenePath, StringComparison.Ordinal)
    //     && string.Equals(_nodePath, other._nodePath, StringComparison.Ordinal)
    //     && string.Equals(_resourcePath, other._resourcePath, StringComparison.Ordinal)
    //     && _resourceUid == other._resourceUid;
    // }

    return thisRef.GetInstanceId() == otherRef.GetInstanceId();
  }

  public override bool Equals(object obj) {
    return obj is Entry other && Equals(other);
  }

  public override int GetHashCode() {
    return 0;
  }

  public virtual void Locate() { }

  public virtual void Open() {
    Locate();
  }

  public virtual GodotObject ResolveReference() { return null; }

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
