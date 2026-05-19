#if TOOLS
// using System.Linq;
using Godot;
using Godot.Collections;
using static Odezzshuuk.Editor.SelectionTracker.Constants;

namespace Odezzshuuk.Editor.SelectionTracker;

public interface IEntryStore {
  Array<Entry> Entries { get; }
  int CurrentSelectionIndex { get; set; }
  void RecordEntry(Entry entry);
  void RemoveEntry(Entry entry);
}


[Tool]
public abstract partial class EntryStore : Resource, IEntryStore {

  public static T GetStore<T>(string path) where T : EntryStore, new() {
    if (ResourceLoader.Exists(path)) {
      return ResourceLoader.Load<T>(path);
    }
    T newStore = new();
    ResourceSaver.Save(newStore, path);
    return newStore;
  }

  public abstract Array<Entry> Entries { get; }
  public virtual int CurrentSelectionIndex { get; set; }

  public abstract void RecordEntry(Entry entry);
  public abstract void RemoveEntry(Entry entry);
}


[Tool]
public partial class HistoryEntryStore : EntryStore {

  public static HistoryEntryStore Instance { get; private set; }

  [Export]
  private Array<Entry> _entries = [];
  public override Array<Entry> Entries {
    get {
      Array<Entry> reversedEntries = _entries.Duplicate();
      return reversedEntries;
    }
  }

  private readonly int _sizeLimit = 100;
  private int _currentSelectionIndex = -1;

  public override void RecordEntry(Entry entry) {
    if (entry == null) {
      return;
    }

    if (_currentSelectionIndex > 0 && _entries.Count > _currentSelectionIndex && entry.Equals(_entries[_currentSelectionIndex])) {
      return;
    }

    int existingIndex = FindIndex(entry);
    if (existingIndex >= 0) {
      // _entries.RemoveAt(existingIndex);
      _entries.RemoveAt(existingIndex);
    }

    _entries.Add(entry);

    while (_entries.Count > _sizeLimit) {
      _entries.RemoveAt(0);
    }

    ResetCurrentSelection();
    EmitChanged();
  }

  public override void RemoveEntry(Entry entry) {
    int existingIndex = FindIndex(entry);
    if (existingIndex < 0) {
      return;
    }

    _entries.RemoveAt(existingIndex);
    EmitChanged();
  }

  public void RemoveAll() {
    if (_entries.Count == 0) {
      return;
    }

    _entries.Clear();
    ResetCurrentSelection();
    EmitChanged();
  }

  public void RemoveAll(string filter) {
    ResetCurrentSelection();
    EmitChanged();
  }

  public Entry PreviousSelection() {
    if (_entries.Count == 0) {
      return null;
    }

    _currentSelectionIndex--;
    if (_currentSelectionIndex < 0) {
      _currentSelectionIndex = 0;
    }

    return _entries[_currentSelectionIndex];
  }

  public Entry NextSelection() {
    if (_entries.Count == 0) {
      return null;
    }

    _currentSelectionIndex++;
    if (_currentSelectionIndex >= _entries.Count) {
      _currentSelectionIndex = _entries.Count - 1;
    }

    return _entries[_currentSelectionIndex];
  }

  public void ResetCurrentSelection() {
    _currentSelectionIndex = -1;
  }


  private int FindIndex(Entry entry) {
    for (int index = 0; index < _entries.Count; index++) {
      if (_entries[index]?.Equals(entry) == true) {
        return index;
      }
    }

    return -1;
  }

  private void Save() {
    if (ResourceSaver.Save(this, HISTORY_SELECTION_PATH) != Error.Ok) {
      GD.PushError($"Failed to save history entry store to '{HISTORY_SELECTION_PATH}'");
    }
  }
}


#endif
