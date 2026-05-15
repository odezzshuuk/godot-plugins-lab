using Godot;

[Tool]
public partial class BaseEntry : Control {

  [Export]
  private Button _searchButton;

  public override void _EnterTree() {

	GD.Print("BaseEntry _Ready called");
	Texture2D searchIcon = EditorInterface.Singleton.GetEditorTheme().GetIcon("Search", "EditorIcons");
	_searchButton.Icon = searchIcon;
	_searchButton.Text = "Find";
  }

  public override void _Process(double delta) {
  }
}
