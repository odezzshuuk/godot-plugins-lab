#if TOOLS
using Godot;
using Godot.Collections;

namespace Odezzshuuk.Editor.SelectionTracker;

public interface IEntryStore {
  Array<Entry> Entries { get; }
  int CurrentSelectionIndex { get; set; }
  void RecordEntry(Entry entry);
  void RemoveEntry(Entry entry);
}


[Tool]
public abstract partial class EntryStore : Resource, IEntryStore {

  [Export]
  protected string _storePath;

  public static T GetStore<T>(string path) where T : EntryStore, new() {
    if (ResourceLoader.Exists(path)) {
      T entryStore = ResourceLoader.Load<T>(path);
      // entryStore.Changed += entryStore.Save;
      return entryStore;
    }
    T newStore = new() {
      _storePath = path
    };
    newStore.Changed += newStore.Save;
    ResourceSaver.Save(newStore, path);
    return newStore;
  }

  public abstract Array<Entry> Entries { get; }
  public virtual int CurrentSelectionIndex { get; set; }

  public abstract void RecordEntry(Entry entry);
  public abstract void RemoveEntry(Entry entry);

  private void Save() {
    GD.Print($"[{GetType().Name}]: entry store Resource at {_storePath} saved");
    if (ResourceSaver.Save(this, _storePath) != Error.Ok) {
      GD.PushError($"Failed to save history entry store to '{_storePath}'");
    }

  }
}

#endif
