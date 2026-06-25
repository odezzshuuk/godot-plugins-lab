#if TOOLS
using Godot;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class EntryControl : Control {

  [ExportGroup("References")]
  [Export] private RichTextLabel _entryNameLabel;
  [Export] private TextureRect _entryIcon;
  [Export] private Button _locateButton;
  [Export] private Button _openButton;
  [Export] private PopupMenu _contextMenu;
  [Export] private PackedScene _dragPreview;

  [ExportGroup("Style")]
  [Export] private Color _loadedColor = Colors.White;
  [Export] private Color _unloadedColor = new(0.85f, 0.73f, 0.33f);
  [Export] private Color _deletedColor = new(0.92f, 0.42f, 0.42f);
  [Export] private Color _defaultColor = Colors.White;
  [Export] private bool _showFavorites = true;

  [Export]
  private EntryModel _entry;

  private readonly PopupMenuHelper _popupMenuHelper = new();

  public int Index { get; set; }

  public string EntryText => _entryNameLabel?.Text ?? string.Empty;

  public EditorInterface Editor { get; set; }

  public EntryModel Entry {
    get => _entry;
    set => BindEntry(value);
  }

  public override void _EnterTree() {

    Texture2D searchIcon = EditorInterface.Singleton.GetEditorTheme().GetIcon("Search", "EditorIcons");
    _locateButton.Icon = searchIcon;

    Texture2D openIcon = EditorInterface.Singleton.GetEditorTheme().GetIcon("Folder", "EditorIcons");
    _openButton.Icon = openIcon;

    _contextMenu.Clear();
    _popupMenuHelper.AddItem("Remove All", RemoveAllEntries)
                    .AddItem("Remove", () => GD.Print($"Requesting removal of entry: {_entry.DisplayName}"))
                    .AddSeparator()
                    .AddItem("Get State", () => GD.Print($"Entry state: {_entry.CurrentEntryState}"))
                    .AddItem("Entry Info", () => GD.Print(_entry))
                    .ApplyTo(_contextMenu);
    // GetParent().PrintTreePretty();
  }


  public override void _GuiInput(InputEvent @event) {
    GUIInputCallback(@event);
  }

  public override Variant _GetDragData(Vector2 position) {
    string dragData = $"EntryControl: {_entry.DisplayName}";
    Node root = _dragPreview.Instantiate();
    DragPreviewControl dragPreview = root.GetNode<DragPreviewControl>(".");
    dragPreview.PreviewData = _entry;
    SetDragPreview(dragPreview);
    return _entry;
  }

  private void BindEntry(EntryModel value) {
    _entry = value;
    _entryIcon.Texture = _entry.Icon;
    _entryIcon.Visible = _entry.Icon != null;
    bool hideOpen = _entry.CurrentEntryState.HasFlag(EntryState.Accessible);
    _openButton.Visible = hideOpen;
    _entryNameLabel.Modulate = _loadedColor;
    _entryNameLabel.Text = _entry.DisplayName;

    _entry.onStateUpdated += StateUpdatedCallback;
  }

  private void GUIInputCallback(InputEvent @event) {
    if (_entry == null || @event is not InputEventMouseButton mouseButton || !mouseButton.Pressed) {
      return;
    }

    if (mouseButton.ButtonIndex == MouseButton.Left) {
      if (mouseButton.DoubleClick) {
        _entry.Open();
      } else {
        _entry.Locate();
      }

      AcceptEvent();
      return;
    }

    if (mouseButton.ButtonIndex == MouseButton.Right) {
      _contextMenu.Position = DisplayServer.MouseGetPosition();
      _contextMenu.Popup();
      AcceptEvent();
    }
  }

  private void RemoveAllEntries() {
    foreach (Node child in GetParent().GetNode<ContainerControl>(".").GetChildren()) {
      child.QueueFree();
    }
  }

  private void LocatePressedCallback() {
    _entry.Locate();
  }

  private void OpenPressedCallback() {
    _entry.Open();
  }

  private void ContextMenuIdPressedCallback(int id) {
    _popupMenuHelper.InvokeCallbackById(id);
  }

  private void StateUpdatedCallback(EntryState state) {

    if (state.HasFlag(EntryState.Deleted) || state.HasFlag(EntryState.Freed)) {
      _entryNameLabel.Modulate = _deletedColor;
      _entryNameLabel.Text = $"[s]{_entry.DisplayName}[/s]";
    }

    if (state.HasFlag(EntryState.Unaccessible)) {
      _entryNameLabel.Modulate = _unloadedColor;
      _entryNameLabel.Text = _entry.DisplayName;
    }

    if (state.HasFlag(EntryState.Accessible)) {
      _entryNameLabel.Modulate = _loadedColor;
      _entryNameLabel.Text = _entry.DisplayName;
    }
  }
}
#endif
