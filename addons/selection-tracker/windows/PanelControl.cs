#if TOOLS
using Godot;
using System;
using static Odezzshuuk.Editor.SelectionTracker.Constants;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class PanelControl : Control {

  [Export]
  private LineEdit _searchBar;

  [Export]
  private PopupMenu _contextMenu;

  [Export]
  private Node _containerPlaceHolder;

  private readonly PopupMenuHelper _popupMenuHelper = new();

  [Signal]
  public delegate void SearchTextChangedEventHandler();

  public (int id, string text, bool isSeparator, Action callback)[] ContextMenuItems { get; private set; }

  public override void _EnterTree() {
    if (SceneFilePath != SELECTION_ENTRIES_INSTANCE_PATH) {
      return;
    }
    _contextMenu.Clear();
    _popupMenuHelper.AddItem("Remove Deleted", () => GD.Print("Remove Deleted callback invoked"))
                    // .AddItem("Remove All", _container.RemoveAll)
                    .ApplyTo(_contextMenu);
    _contextMenu.IdPressed += _popupMenuHelper.IsPressedCallback;

    PackedScene sc;
    if (ResourceLoader.Exists(SELECTION_ENTRIES_INSTANCE_PATH)) {
      sc = ResourceLoader.Load<PackedScene>(SELECTION_ENTRIES_INSTANCE_PATH);
      // Initialization of the plugin goes here.
    } else {
      sc = Utils.InstantiateTemplateScene(SELECTION_ENTRIES_TEMPLATE_PATH, SELECTION_ENTRIES_INSTANCE_PATH);
    }

    GD.Print($"Place is null: {_containerPlaceHolder == null}");
    Node parent = _containerPlaceHolder.GetParent();
    int index = _containerPlaceHolder.GetIndex();
    // Control _container = sc.Instantiate<Control>();

    Node container = PluginHandle.Instance.containerNode;

    parent.RemoveChild(_containerPlaceHolder);
    parent.AddChild(container);
    parent.MoveChild(container, index);
  }

  public override void _ExitTree() {
    try {
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
    // 
  }


  private void FilterEntryies() {

  }

  private bool PassFilter() {
    return false;
  }

  private void StoreChangedCallback() {

  }

}
#endif
