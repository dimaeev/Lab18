using System;
using System.Collections.Generic;
using DirectorySyncApp.Model;

namespace DirectorySyncApp.View
{
  public interface ISyncView
  {
    string SourceDirectory { get; }
    string TargetDirectory { get; }

    void DisplayChanges(List<FileChange> changes);
    void ShowMessage(string message);
    void SetProgress(int percent);

    event Action OnCompare;
    event Action OnSyncLeftToRight;
    event Action OnSyncRightToLeft;
  }
}