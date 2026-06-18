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

  [ExportGroup("Style")]
  [Export] private Color _loadedColor = Colors.White;
  [Export] private Color _unloadedColor = new(0.85f, 0.73f, 0.33f);
  [Export] private Color _deletedColor = new(0.92f, 0.42f, 0.42f);
  [Export] private Color _defaultColor = Colors.White;
  [Export] private bool _showFavorites = true;

  [Export]
  private EntryModel _entry;

  private EntryStore _entryStore;

  private readonly PopupMenuHelper _popupMenuHelper = new();

  public int Index { get; set; }

  public string EntryText => _entryNameLabel?.Text ?? string.Empty;

  public EditorInterface Editor { get; set; }

  public EntryModel Entry {
    get => _entry;
    set => BindEntry(value);
  }

  public override void _EnterTree() {
    _locateButton.Pressed += OnPingPressed;
    _openButton.Pressed += OnOpenPressed;

    Texture2D searchIcon = EditorInterface.Singleton.GetEditorTheme().GetIcon("Search", "EditorIcons");
    _locateButton.Icon = searchIcon;

    Texture2D openIcon = EditorInterface.Singleton.GetEditorTheme().GetIcon("Folder", "EditorIcons");
    _openButton.Icon = openIcon;

    _contextMenu.Clear();
    _popupMenuHelper.AddItem("Remove", () => GD.Print($"Requesting removal of entry: {_entry.DisplayName}"))
                    .AddItem("Option 1", () => GD.Print("Option 1 selected"))
                    .AddItem("Option 2", () => GD.Print("Option 2 selected"))
                    .AddSeparator()
                    .AddItem("Remove All", () => {
                      GD.Print($"[{GetType().Name}] Owner: {Owner?.Name}(OwnerType: {Owner?.GetType().Name})");
                      PanelControl control = Owner.GetNodeOrNull<PanelControl>(".");
                      control?.ClearEntries();
                    })
                    .ApplyTo(_contextMenu);
    _contextMenu.IdPressed += _popupMenuHelper.IsPressedCallback;

  }

  // public override void _ExitTree() {
  //   try {
  //
  //     _locateButton.Pressed -= OnPingPressed;
  //     _openButton.Pressed -= OnOpenPressed;
  //     _contextMenu.IdPressed -= _popupMenuHelper.IsPressedCallback;
  //   } catch (Exception ex) {
  //     GD.PrintErr($"[{GetType().Name}] Error during cleanup: {ex.Message}");
  //   }
  // }

  public override void _GuiInput(InputEvent @event) {
    GUIInputCallback(@event);
  }

  public void Reset() {
    Entry = null;
  }

  private void BindEntry(EntryModel value) {
    _entry = value;

    if (_entry == null) {
      _entryNameLabel.Text = string.Empty;
      _entryIcon.Texture = null;
      _entryIcon.Visible = false;
      _openButton.Visible = false;
      return;
    }

    SetTextStyle();
    _entryIcon.Texture = _entry.Icon;
    _entryIcon.Visible = _entry.Icon != null;

    GD.Print($"Binding entry: {_entry.DisplayName} with state: {_entry.CurrentEntryState}");
    bool hideOpen = _entry.CurrentEntryState.HasFlag(EntryState.Accessible);
    _openButton.Visible = hideOpen;
  }

  private void AppendPopupMenu(PopupMenu menu) {
    menu.AddItem("Remove");
    menu.AddItem("Option 1");
    menu.AddItem("Option 2");
    menu.AddSeparator();
    menu.AddItem("Remove All");
  }

  private void GUIInputCallback(InputEvent @event) {
    if (_entry == null || @event is not InputEventMouseButton mouseButton || !mouseButton.Pressed) {
      return;
    }

    GD.Print($"Mouse Pressed input event: {@event} triggered");
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
      GD.Print($"Right-clicked on entry: {_entry.DisplayName}");
      _contextMenu.Position = DisplayServer.MouseGetPosition();
      _contextMenu.Popup();
      AcceptEvent();
    }
  }

  private void OnPingPressed() {
    _entry?.Locate();
  }

  private void OnOpenPressed() {
    _entry?.Open();
  }

  private void SetTextStyle() {
    GD.Print($"TextStyle entry is null: {_entry == null}");

    EntryState state = _entry
      .CurrentEntryState;

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
