using Godot;

[Tool]
public partial class BaseWindow : Control {

  [Export]
  private BaseEntry _entryTemplate;

  [Export]
  private LineEdit _searchBar;

  [Export]
  private Node _entryContainer;

  public override void _EnterTree() {
    GD.Print("BaseWindow _Ready called");
    // Node entryInstance = _entryTemplate.Duplicate();
    // AddChild(entryInstance);
  }

}
