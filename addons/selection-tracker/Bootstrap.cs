#if TOOLS
using System.Linq;
using Godot;
using Godot.Collections;
using static Odezzshuuk.Editor.SelectionTracker.Constants;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class Bootstrap : EditorPlugin {

  private EditorDock _pluginDock;
  private FileSystemDock _fileSystemDock;

  private HistoryEntryStore _historyEntryStore;

  public override void _EnterTree() {
    MountDock();

    _historyEntryStore = EntryStore.GetStore<HistoryEntryStore>(HISTORY_SELECTION_PATH);
    GD.Print($"[{GetType().Name}] HistoryEntryStore loaded with {_historyEntryStore.Entries.Count} entries.");
    EditorInterface.Singleton.GetFileSystemDock().SelectionChanged += FileSystemSelectionChangedCallback;
    EditorInterface.Singleton.GetSelection().SelectionChanged += SelectionChangedCallback;

  }

  public override void _ExitTree() {
    // In Godot, Cleanup listeners are unnecessary because the plugin will be freed when exiting the editor, but it's good practice to clean up resources.
    // EditorInterface.Singleton.GetFileSystemDock().SelectionChanged -= FileSystemSelectionChangedCallback;

    // EditorInterface.Singleton.GetSelection().SelectionChanged -= SelectionChangedCallback;
    if (_pluginDock != null) {
      RemoveDock(_pluginDock);
      _pluginDock.QueueFree();
      _pluginDock = null;
    }

    _historyEntryStore = null;

  }

  private void MountDock() {
    // Initialization of the plugin goes here.
    Control _dock_scene = GD.Load<PackedScene>(MAIN_PANEL_RESOURCE_PATH).Instantiate<Control>();

    PluginHandle.Instance.IsActive = true;

    _pluginDock = new EditorDock();
    _pluginDock.AddChild(_dock_scene);
    _pluginDock.Title = "Selection Tracker";

    _pluginDock.DefaultSlot = EditorDock.DockSlot.Bottom;
    _pluginDock.AvailableLayouts = EditorDock.DockLayout.Horizontal | EditorDock.DockLayout.Floating;

    AddDock(_pluginDock);
  }

  private void SelectionChangedCallback() {

    GodotObject res = null;
    EditorInterface editor = EditorInterface.Singleton;
    Array<Node> selectedNodes = editor.GetSelection().GetSelectedNodes();
    if (selectedNodes.Count > 0) {
      res = selectedNodes[0];
      _historyEntryStore.RecordEntry(CreateNodeEntry(selectedNodes[0]));
    }

    // GodotObject inspectorObject = editor.GetInspector().GetEditedObject();
    //
    // if (inspectorObject != null) {
    //   res = inspectorObject;
    // }

    GD.Print(res?.ToString());
  }

  private void FileSystemSelectionChangedCallback() {
    string selectedPath = string.Empty;

    EditorInterface editor = EditorInterface.Singleton;
    string[] selectedPaths = editor.GetSelectedPaths();

    if (selectedPaths.Count() > 0) {
      string path = selectedPaths[0];
      GD.Print("Selected path: " + path);
      if (path.Trim().EndsWith("/")) {
        return;
      }
      _historyEntryStore.RecordEntry(CreateFileEntry(path));
    }

  }

  private Entry CreateNodeEntry(Node obj) {
    NodeEntry entry = new(obj);
    return entry;
  }

  private Entry CreateFileEntry(string path) {
    FileEntry entry = new(path);
    return entry;
  }
}
#endif
