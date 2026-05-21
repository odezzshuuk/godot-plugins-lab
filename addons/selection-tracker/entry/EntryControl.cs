#if TOOLS
using Godot;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class EntryControl : Control {

  [Signal] public delegate void RemoveRequestedEventHandler(Entry entry);
  [Signal] public delegate void FavoriteToggledEventHandler(Entry entry, bool isFavorite);

  [ExportGroup("References")]
  [Export] private Label _entryNameLabel;
  [Export] private TextureRect _entryIcon;
  [Export] private Button _locateButton;
  [Export] private Button _openButton;

  [ExportGroup("Style")]
  [Export] private Color _loadedColor = Colors.White;
  [Export] private Color _unloadedColor = new(0.85f, 0.73f, 0.33f);
  [Export] private Color _deletedColor = new(0.92f, 0.42f, 0.42f);
  [Export] private Color _defaultColor = Colors.White;
  [Export] private bool _showFavorites = true;
  [Export] private bool _hideOpenForNodes = true;

  private Entry _entry;

  public int Index { get; set; }

  public string EntryText => _entryNameLabel?.Text ?? string.Empty;

  public EditorInterface Editor { get; set; }

  public Entry Entry {
    get => _entry;
    set => SetupEntry(value);
  }

  public override void _EnterTree() {
    ConnectUi();
    ApplyStaticUi();
    SetupEntry(_entry);
  }

  public override void _ExitTree() {
    DisconnectUi();
  }

  public void Reset() {
    Entry = null;
  }


  private void ConnectUi() {
    if (_locateButton != null) {
      _locateButton.Pressed += OnPingPressed;
    }

    if (_openButton != null) {
      _openButton.Pressed += OnOpenPressed;
    }
  }

  private void DisconnectUi() {
    if (_locateButton != null) {
      _locateButton.Pressed -= OnPingPressed;
    }

    if (_openButton != null) {
      _openButton.Pressed -= OnOpenPressed;
    }
  }

  private void ApplyStaticUi() {

    Texture2D searchIcon = EditorInterface.Singleton.GetEditorTheme().GetIcon("Search", "EditorIcons");
    _locateButton.Icon = searchIcon;
    _locateButton.Text = "Find";

    Texture2D openIcon = EditorInterface.Singleton.GetEditorTheme().GetIcon("Folder", "EditorIcons");
    _openButton.Icon = openIcon;

  }

  private void SetupEntry(Entry value) {
    _entry = value;

    if (_entry == null) {
      Visible = false;
      return;
    }

    Visible = true;

    _entryNameLabel.Text = _entry.DisplayName;
    _entryNameLabel.Modulate = GetDisplayColor(_entry.CurrentRefState);

    _entryIcon.Texture = _entry.Icon;
    _entryIcon.Visible = _entry.Icon != null;

    bool hideOpen = _hideOpenForNodes && _entry.CurrentRefState.HasFlag(RefState.Node);
    _openButton.Visible = !hideOpen;
  }

  private void OnInfoGuiInput(InputEvent @event) {
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
      EmitSignal(SignalName.RemoveRequested, _entry);
      AcceptEvent();
    }
  }

  private void OnPingPressed() {
    _entry?.Locate();
  }

  private void OnOpenPressed() {
    _entry?.Open();
  }

  private Color GetDisplayColor(RefState refState) {
    if (refState.HasFlag(RefState.Deleted) || refState.HasFlag(RefState.Freed)) {
      return _deletedColor;
    }

    if (refState.HasFlag(RefState.Unloaded) || refState.HasFlag(RefState.External)) {
      return _unloadedColor;
    }

    if (refState.HasFlag(RefState.Loaded) || refState.HasFlag(RefState.Instanced) || refState.HasFlag(RefState.Resource)) {
      return _loadedColor;
    }

    return _defaultColor;
  }
}
#endif
