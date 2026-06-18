#if TOOLS
using Godot;
using Godot.Collections;
using System.Linq;
using static Odezzshuuk.Editor.SelectionTracker.Constants;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class Bootstrap : EditorPlugin {

  private EditorDock _pluginDock;
  private FileSystemDock _fileSystemDock;

  private SelectionEntryContainer _container;

  private PackedScene _panelPackedScene;
  private PackedScene _containerPackedScene;

  private readonly PluginHandle _pluginHandle = PluginHandle.Instance;

  public override void _EnterTree() {
    using (DirAccess dir = DirAccess.Open(PLUGIN_DIR_PATH)) {
      if (dir == null) {
        DirAccess.MakeDirRecursiveAbsolute(PLUGIN_DIR_PATH);
      }
    }

    SetupPanel();
    SetupContainer();

    // EditorInterface.Singleton.GetFileSystemDock().SelectionChanged += FileSystemSelectionChangedCallback;
    // EditorInterface.Singleton.GetSelection().SelectionChanged += SelectionChangedCallback;
  }

  public override void _ExitTree() {
    // In Godot, Cleanup listeners are unnecessary because the plugin will be freed when exiting the editor, but it's good practice to clean up resources.
    // EditorInterface.Singleton.GetFileSystemDock().SelectionChanged -= FileSystemSelectionChangedCallback;
    // EditorInterface.Singleton.GetSelection().SelectionChanged -= SelectionChangedCallback;

    Error res = _containerPackedScene.Pack(_container);
    if (res == Error.Ok) {
      res = ResourceSaver.Save(_containerPackedScene, SELECTION_ENTRIES_INSTANCE_PATH);
    }
  }

  private void SetupPanel() {

    // Initialization of the plugin goes here.
    // if (ResourceLoader.Exists(MAIN_PANEL_INSTANCE_PATH)) {
    //   _panelPackedScene = ResourceLoader.Load<PackedScene>(MAIN_PANEL_INSTANCE_PATH);
    // } else {
    //   // Load Template Scene, instantiate it, pack it again and save it to new path
    //   _panelPackedScene = Utils.InstantiateTemplateScene(MAIN_PANEL_TEMPLATE_PATH, MAIN_PANEL_INSTANCE_PATH);
    // }
    //
    // _pluginHandle.panelNode = _panelPackedScene.Instantiate();
    //
    // _pluginDock = new EditorDock();
    // _pluginDock.AddChild(_pluginHandle.panelNode);
    // _pluginDock.Title = "Selections Tracker";
    //
    // _pluginDock.DefaultSlot = EditorDock.DockSlot.Bottom;
    // _pluginDock.AvailableLayouts = EditorDock.DockLayout.Horizontal | EditorDock.DockLayout.Floating;
    //
    // AddDock(_pluginDock);
  }

  private void SetupContainer() {
    if (ResourceLoader.Exists(SELECTION_ENTRIES_INSTANCE_PATH)) {
      _containerPackedScene = ResourceLoader.Load<PackedScene>(SELECTION_ENTRIES_INSTANCE_PATH);
    } else {
      // Load Template Scene, instantiate it, pack it again and save it to new path
      _containerPackedScene = Utils.InstantiateTemplateScene(SELECTION_ENTRIES_TEMPLATE_PATH, SELECTION_ENTRIES_INSTANCE_PATH);
    }

    _pluginHandle.containerNode = _containerPackedScene.Instantiate<Control>();
    _container = _pluginHandle.containerNode.GetNode<SelectionEntryContainer>(".");
  }

  private void SelectionChangedCallback() {

    GodotObject res = null;
    EditorInterface editor = EditorInterface.Singleton;
    Array<Node> selectedNodes = editor.GetSelection().GetSelectedNodes();
    if (selectedNodes.Count > 0) {
      res = selectedNodes[0];
      _container.RecordEntry(CreateNodeEntry(selectedNodes[0]));
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
      _container.RecordEntry(CreateFileEntry(path));
    }
  }

  private EntryModel CreateNodeEntry(Node obj) {
    NodeEntryModel entry = new(obj);
    return entry;
  }

  private EntryModel CreateFileEntry(string path) {
    FileEntryModel entry = new(path);
    return entry;
  }
}
#endif
