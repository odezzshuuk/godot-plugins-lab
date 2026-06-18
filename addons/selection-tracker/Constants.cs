#if TOOLS

namespace Odezzshuuk.Editor.SelectionTracker;

public static class Constants {

  public const string PLUGIN_DIR_PATH = "user://editor/selection-tracker/";

  public const string MAIN_PANEL_TEMPLATE_PATH = "res://addons/selection-tracker/scenes/panel.tscn";
  public const string MAIN_PANEL_INSTANCE_PATH = PLUGIN_DIR_PATH + "panel.tscn";

  // public const string HISTORY_SELECTION_PATH = "user://history-selection.tres";

  public const string SELECTION_ENTRIES_TEMPLATE_PATH = "res://addons/selection-tracker/scenes/selection-entries.tscn";
  public const string SELECTION_ENTRIES_INSTANCE_PATH = PLUGIN_DIR_PATH + "selection-entries.tscn";

}
#endif
