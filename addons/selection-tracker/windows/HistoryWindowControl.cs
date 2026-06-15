#if TOOLS
using Godot;
using System;
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

  private readonly PopupMenuHelper _popupMenuHelper = new();

  public (int id, string text, bool isSeparator, Action callback)[] ContextMenuItems { get; private set; }

  public override void _EnterTree() {
    if (!PluginHandle.Instance.IsActive) {
      return;
    }

    _entryStore = EntryStore.GetStore<HistoryEntryStore>(HISTORY_SELECTION_PATH);
    _entryStore.Changed += StoreChangedCallback;

    LoadEntries();

    _contextMenu.Clear();
    _popupMenuHelper.AddItem("Remove Deleted", () => GD.Print("Remove Deleted callback invoked"))
                    .AddItem("Remove All", _entryStore.RemoveAll)
                    .ApplyTo(_contextMenu);
    _contextMenu.IdPressed += _popupMenuHelper.IsPressedCallback;
  }

  public override void _ExitTree() {
    if (!PluginHandle.Instance.IsActive) {
      return;
    }

    try {
      _entryStore.Changed -= StoreChangedCallback;
      _contextMenu.IdPressed -= _popupMenuHelper.IsPressedCallback;
    } catch (Exception ex) {
      GD.PrintErr($"[{GetType().Name}] Error during cleanup: {ex.Message}");
    }
  }

  public override void _GuiInput(InputEvent @event) {
    if (@event is InputEventMouseButton mouseEvent) {
      if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed) {
        _contextMenu.Position = DisplayServer.MouseGetPosition();
        _contextMenu.Popup();
        AcceptEvent();
      }
    }
  }

  public void ClearEntries() {
    _entryStore.RemoveAll();
  }

  private void LoadEntries() {
    int entryCounter = 0;
    int entryNodeCount = _entryContainer.GetChildren().Count;

    GD.Print($"Loading entries: EntryCount={_entryStore.Entries.Count}, ExistingEntryNodes={entryNodeCount}");

    foreach (Entry entry in _entryStore.Entries) {
      if (entryCounter < entryNodeCount) {
        // reuse existing entry node
        Node entryNode = _entryContainer.GetChild(entryCounter);
        EntryControl control = entryNode.GetNodeOrNull<EntryControl>(".");
        control.Entry = entry;
      } else {
        // create new entry node
        Node entryNode = _entryTemplate.Instantiate();
        _entryContainer.AddChild(entryNode);
        entryNode.Owner = this;
        EntryControl control = entryNode.GetNodeOrNull<EntryControl>(".");
        control.Entry = entry;
      }
      entryCounter++;
    }

    if (entryCounter < entryNodeCount) {
      // remove extra entry nodes
      for (int index = entryNodeCount - 1; index >= entryCounter; index--) {
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

  private void StoreChangedCallback() {
    LoadEntries();
  }
}
#endif
