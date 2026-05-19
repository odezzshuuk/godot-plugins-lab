#if TOOLS
using Godot;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class HistoryWindowControl : Control {

  [Export]
  private EntryControl _entryTemplate;

  [Export]
  private PackedScene _entryScene;

  [Export]
  private LineEdit _searchBar;

  [Export]
  private Node _entryContainer;

  private EntryStore _entryStore;

  public override void _EnterTree() {
    // Node entryInstance = _entryTemplate.Duplicate();
    // AddChild(entryInstance);
    // EntryStore.GetResource();
    _entryStore = EntryStore.GetStore<HistoryEntryStore>(Constants.HISTORY_SELECTION_PATH);
    _entryStore.Changed += StoreChangedCallback;
  }

  public override void _ExitTree() {
    _entryStore.Changed -= StoreChangedCallback;
  }

  private void StoreChangedCallback() {
    LoadEntries();
  }

  private void LoadEntries() {
    foreach (Entry entry in _entryStore.Entries) {
      if (_entryTemplate == null) {
        GD.PrintErr("Entry template is not assigned.");
        return;
      }
      Node entryNode = _entryTemplate.Duplicate();
      _entryContainer.AddChild(entryNode);
      EntryControl control = entryNode.GetNodeOrNull<EntryControl>(".");
      control.Entry = entry;
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
