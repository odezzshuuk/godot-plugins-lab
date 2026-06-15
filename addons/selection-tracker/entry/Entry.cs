#if TOOLS
using Godot;
using System;

namespace Odezzshuuk.Editor.SelectionTracker;

[Flags]
public enum EntryState {
  Node = 1 << 0,
  File = 1 << 1,

  Accessible = 1 << 4,
  Unaccessible = 1 << 5,

  Loaded = Accessible | Node,
  Unloaded = Unaccessible | Node,
  Freed = (1 << 6) | Node,
  Playing = (1 << 7) | Node,

  Existed = Accessible | File,

  Deleted = 1 << 8,

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
  [Export] protected EntryState _cachedRefState = EntryState.Unknown;


  public virtual string DisplayName => _cachedName ?? $"Unnamed {GetType().Name}" ?? "Empty";
  public Texture2D Icon => _cachedIcon;

  public virtual EntryState CurrentEntryState => EntryState.Unknown;

  public Entry() { }


  public override bool Equals(object obj) {
    return obj is Entry other && Equals(other);
  }

  public override int GetHashCode() {
    return HashCode.Combine(_cachedName);
  }

  public virtual bool Equals(Entry other) {
    if (other is null) {
      return false;
    }

    if (ReferenceEquals(this, other)) {
      return true;
    }

    return other.GetType() == GetType();
  }

  public virtual void Locate() { }

  public virtual void Open() {
    Locate();
  }

  public virtual GodotObject ResolveReference() { return null; }


}
#endif
