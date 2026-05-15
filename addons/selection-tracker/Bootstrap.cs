#if TOOLS
using Godot;

[Tool]
public partial class Bootstrap : EditorPlugin {

  private EditorDock _dock;

  public override void _EnterTree() {
    // Initialization of the plugin goes here.
    Control _dock_scene = GD.Load<PackedScene>("res://addons/selection-tracker/ui/panel.tscn").Instantiate<Control>();

    _dock = new EditorDock();
    _dock.AddChild(_dock_scene);
    _dock.Title = "Selection Tracker";

    _dock.DefaultSlot = EditorDock.DockSlot.Bottom;
    _dock.AvailableLayouts = EditorDock.DockLayout.Horizontal | EditorDock.DockLayout.Floating;

    AddDock(_dock);
  }

  public override void _ExitTree() {
    // Clean-up of the plugin goes here.
    RemoveDock(_dock);
    _dock.QueueFree();
  }
}
#endif
