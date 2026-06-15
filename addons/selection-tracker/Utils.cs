#if TOOLS
using Godot;
using System;
using System.Collections.Generic;

namespace Odezzshuuk.Editor.SelectionTracker;

public struct PopupMenuItem {
  public long id;
  public string text;
  public bool isSeparator;
  public Action callback;
}

public class PopupMenuHelper {

  private readonly List<(string text, bool isSeparator, Action callback)> _contextMenuItems = [];
  private PopupMenu _popupMenu;

  public PopupMenu ApplyTo(PopupMenu popupMenu) {

    for (int i = 0; i < _contextMenuItems.Count; i++) {
      if (_contextMenuItems[i].isSeparator) {
        popupMenu.AddSeparator();
      } else {
        popupMenu.AddItem(_contextMenuItems[i].text);
      }
    }
    _popupMenu = popupMenu;
    return _popupMenu;
  }

  public PopupMenuHelper AddItem(string text, Action callback) {
    _contextMenuItems.Add((text, false, callback));
    return this;
  }

  public PopupMenuHelper AddSeparator() {
    _contextMenuItems.Add((string.Empty, true, null));
    return this;
  }

  public void IsPressedCallback(long id) {
    _contextMenuItems[(int)id].callback?.Invoke();
  }

  public void ClearItems() {
    _contextMenuItems.Clear();
    _popupMenu?.Clear();
  }
}
#endif
