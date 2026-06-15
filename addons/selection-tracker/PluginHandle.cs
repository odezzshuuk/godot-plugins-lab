#if TOOLS
using System;

namespace Odezzshuuk.Editor.SelectionTracker;

public class PluginState {
  public bool IsActive { get; set; }
  private PluginState() { }
}

public class PluginSettings {

  private PluginSettings() { }
}

public class PluginHandle {

  private static readonly Lazy<PluginHandle> s_Instance = new(() => new PluginHandle());

  public static PluginHandle Instance => s_Instance.Value;

  public bool IsActive { get; set; }

  private PluginHandle() { IsActive = false; }

}

#endif
