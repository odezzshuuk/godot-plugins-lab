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

  [Export]
  private PopupMenu _contextMenu;

  private HistoryEntryStore _entryStore;

  public override void _EnterTree() {
    _entryStore = EntryStore.GetStore<HistoryEntryStore>(HISTORY_SELECTION_PATH);
    _entryStore.Changed += StoreChangedCallback;
    _contextMenu.IdPressed += id => {
      GD.Print($"Context menu item with id {id} pressed");
      // Handle context menu actions based on the id
    };
    LoadEntries();

  }

  public override void _ExitTree() {
    _entryStore.Changed -= StoreChangedCallback;
  }

  public override void _GuiInput(InputEvent @event) {
    if (@event is InputEventMouseButton mouseEvent) {
      if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed) {
        GD.Print("Right click event handled in HistoryWindowControl");
        _contextMenu.Position = DisplayServer.MouseGetPosition();
        _contextMenu.Popup();
        AcceptEvent();
      }
    }
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

  private void ClearEntries() {
    _entryStore.RemoveAll();
  }

  private void ContextMenuPressedCallbac(long id) {
    switch (id) {
      case 0:
        ClearEntries();
        break;
      case 1:
        GD.Print("Option 1 selected");
        break;
      default:
        GD.Print($"Unknown context menu item with id {id} pressed");
        break;
    }

  }


  // private void  

}
#endif
