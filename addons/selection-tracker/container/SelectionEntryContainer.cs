#if TOOLS
using Godot;
using System;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class SelectionEntryContainer : Control {

  // [Export]
  // private Array<Entry> GetChildren() = [];

  [Export]
  private PackedScene _entryTemplate;

  // public override Array<Entry> Entries {
  //   get {
  //     Array<Entry> reversedEntries = GetChildren().Duplicate();
  //     reversedEntries.Reverse();
  //     return reversedEntries;
  //   }
  // }

  private readonly int _sizeLimit = 100;
  private int _currentSelectionIndex = -1;

  public void RecordEntry(EntryModel entry) {
    if (entry == null) {
      return;
    }

    if (_currentSelectionIndex > 0 && GetChildCount() > _currentSelectionIndex && entry.Equals(GetChildren()[_currentSelectionIndex])) {
      return;
    }

    int existingIndex = FindEntryIndex(entry);
    if (existingIndex != -1) {
      GetChildren().RemoveAt(existingIndex);
    }

    Node entryNode = _entryTemplate.Instantiate();
    EntryControl entryControl = entryNode.GetNode<EntryControl>(".");
    entryControl.Entry = entry;

    GetChildren().Add(entryNode);

    while (GetChildren().Count > _sizeLimit) {
      // remove the last
      GetChildren().RemoveAt(GetChildren().Count - 1);
    }

    ResetCurrentSelection();
  }

  public void RemoveEntry(EntryModel entry) {
    int existingIndex = FindEntryIndex(entry);
    if (existingIndex < 0) {
      return;
    }

    GetChildren().RemoveAt(existingIndex);
  }

  public void RemoveAll() {
    if (GetChildren().Count == 0) {
      return;
    }

    GetChildren().Clear();
    ResetCurrentSelection();
  }

  public void RemoveAll(string filter) {
    ResetCurrentSelection();
  }

  public Node PreviousSelection() {
    if (GetChildren().Count == 0) {
      return null;
    }

    _currentSelectionIndex--;
    if (_currentSelectionIndex < 0) {
      _currentSelectionIndex = 0;
    }

    return GetChildren()[_currentSelectionIndex];
  }

  public Node NextSelection() {
    if (GetChildren().Count == 0) {
      return null;
    }

    _currentSelectionIndex++;
    if (_currentSelectionIndex >= GetChildren().Count) {
      _currentSelectionIndex = GetChildren().Count - 1;
    }

    return GetChildren()[_currentSelectionIndex];
  }

  public void ResetCurrentSelection() {
    _currentSelectionIndex = -1;
  }

  private int FindEntryIndex<T>(IEquatable<T> entry) {
    for (int index = 0; index < GetChildren().Count; index++) {
      if (GetChildren()[index]?.Equals(entry) == true) {
        return index;
      }
    }

    return -1;
  }
}


#endif
