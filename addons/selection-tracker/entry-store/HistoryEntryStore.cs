#if TOOLS
// using System.Linq;
using Godot;
using Godot.Collections;
using System;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class HistoryEntryStore : EntryStore {

  [Export]
  private Array<Entry> _entries = [];

  public override Array<Entry> Entries {
    get {
      Array<Entry> reversedEntries = _entries.Duplicate();
      reversedEntries.Reverse();
      return reversedEntries;
    }
  }

  private readonly int _sizeLimit = 100;
  private int _currentSelectionIndex = -1;

  public override void RecordEntry(Entry entry) {
    GD.Print($"Recording entry: {entry?.DisplayName}");
    if (entry == null) {
      return;
    }

    if (_currentSelectionIndex > 0 && _entries.Count > _currentSelectionIndex && entry.Equals(_entries[_currentSelectionIndex])) {
      return;
    }

    int existingIndex = FindEntryIndex(entry);
    if (existingIndex != -1) {
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
    int existingIndex = FindEntryIndex(entry);
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

  private int FindEntryIndex<T>(IEquatable<T> entry) {
    for (int index = 0; index < _entries.Count; index++) {
      if (_entries[index]?.Equals(entry) == true) {
        return index;
      }
    }

    return -1;
  }
}


#endif
