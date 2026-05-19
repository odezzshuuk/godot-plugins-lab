#if TOOLS
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Odezzshuuk.Editor.SelectionTracker;

[Tool]
public partial class MostVisitedEntryStore : EntryStore {

  private readonly struct EntryVisit(Entry entry, int count, int firstIndex) {
    public Entry Entry { get; } = entry;
    public int Count { get; } = count;
    public int FirstIndex { get; } = firstIndex;
  }

  [Export] private Array<Entry> _slidingWindow = [];
  [Export] private int _sizeLimit = 200;

  public override Array<Entry> Entries {
    get {
      List<EntryVisit> visits = [];

      for (int index = 0; index < _slidingWindow.Count; index++) {
        Entry entry = _slidingWindow[index];
        if (entry == null) {
          continue;
        }

        int visitIndex = FindVisitIndex(visits, entry);
        if (visitIndex >= 0) {
          EntryVisit existingVisit = visits[visitIndex];
          visits[visitIndex] = new EntryVisit(existingVisit.Entry, existingVisit.Count + 1, existingVisit.FirstIndex);
          continue;
        }

        visits.Add(new EntryVisit(entry, 1, index));
      }

      SortVisits(visits);

      Array<Entry> orderedEntries = [];
      for (int index = 0; index < visits.Count; index++) {
        orderedEntries.Add(visits[index].Entry);
      }

      return orderedEntries;
    }
  }

  public override void RecordEntry(Entry entry) {
    if (entry == null) {
      return;
    }

    _slidingWindow.Add(entry);

    if (_slidingWindow.Count > _sizeLimit) {
      _slidingWindow.RemoveAt(0);
    }

    EmitChanged();
  }

  public override void RemoveEntry(Entry entry) {
    bool removed = false;
    for (int index = _slidingWindow.Count - 1; index >= 0; index--) {
      if (_slidingWindow[index]?.Equals(entry) != true) {
        continue;
      }

      _slidingWindow.RemoveAt(index);
      removed = true;
    }

    if (removed) {
      EmitChanged();
    }
  }

  private static int FindVisitIndex(List<EntryVisit> visits, Entry entry) {
    for (int index = 0; index < visits.Count; index++) {
      if (visits[index].Entry?.Equals(entry) == true) {
        return index;
      }
    }

    return -1;
  }

  private static void SortVisits(List<EntryVisit> visits) {
    for (int current = 1; current < visits.Count; current++) {
      EntryVisit candidate = visits[current];
      int insertIndex = current - 1;

      while (insertIndex >= 0 && ShouldComeBefore(candidate, visits[insertIndex])) {
        visits[insertIndex + 1] = visits[insertIndex];
        insertIndex--;
      }

      visits[insertIndex + 1] = candidate;
    }
  }

  private static bool ShouldComeBefore(EntryVisit left, EntryVisit right) {
    if (left.Count != right.Count) {
      return left.Count > right.Count;
    }

    return left.FirstIndex < right.FirstIndex;
  }
}
#endif
