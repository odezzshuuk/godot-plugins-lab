#if TOOLS
using Godot;
using static Odezzshuuk.Editor.SelectionTracker.Constants;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class HistoryWindowControl : Control {

  [Export]
  private PackedScene _entryTemplate;

  [Export]
  private LineEdit _searchBar;

  [Export]
  private Node _entryContainer;

  private EntryStore _entryStore;

  public override void _EnterTree() {
    foreach (Node child in _entryContainer.GetChildren()) {
      child.QueueFree();
    }

    _entryStore = EntryStore.GetStore<HistoryEntryStore>(HISTORY_SELECTION_PATH);
    _entryStore.Changed += StoreChangedCallback;
  }

  public override void _ExitTree() {
    _entryStore.Changed -= StoreChangedCallback;
  }

  private void StoreChangedCallback() {
    LoadEntries();
  }

  private void LoadEntries() {
    int entryCounter = 0;
    int elementCount = _entryContainer.GetChildren().Count;

    GD.Print("entries count: " + _entryStore.Entries.Count);

    foreach (Entry entry in _entryStore.Entries) {
      if (entryCounter < elementCount) {
        // reuse existing entry node
        Node entryNode = _entryContainer.GetChild(entryCounter);
        EntryControl control = entryNode.GetNodeOrNull<EntryControl>(".");
        control.Entry = entry;
      } else {
        // create new entry node
        Node entryNode = _entryTemplate.Instantiate();
        _entryContainer.AddChild(entryNode);
        EntryControl control = entryNode.GetNodeOrNull<EntryControl>(".");
        control.Entry = entry;
      }
      entryCounter++;
    }
    GD.Print("counter: " + entryCounter);
    GD.Print("element count: " + elementCount);

    if (entryCounter < elementCount) {
      // remove extra entry nodes
      for (int index = elementCount - 1; index >= entryCounter; index--) {
        Node entryNode = _entryContainer.GetChild(index);
        entryNode.QueueFree();
      }
    }
  }

  private void FilterEntryies() {

  }

  private bool PassFilter() {
    return false;
  }


  // private void  

}
#endif
