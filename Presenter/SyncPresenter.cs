using System;
using System.Collections.Generic;
using DirectorySyncApp.Model;
using DirectorySyncApp.View;

namespace DirectorySyncApp.Presenter
{
  public class SyncPresenter
  {
    private readonly ISyncView _view;
    private readonly DirectorySynchronizer _synchronizer;

    public SyncPresenter(ISyncView view, DirectorySynchronizer synchronizer)
    {
      if (view == null)
      {
        throw new ArgumentNullException(nameof(view));
      }
      if (synchronizer == null)
      {
        throw new ArgumentNullException(nameof(synchronizer));
      }

      _view = view;
      _synchronizer = synchronizer;
    }

    private void SubscribeToEvents()
    {
      _view.OnCompare += HandleCompare;
      _view.OnSyncLeftToRight += () => HandleSync(_view.SourceDirectory, _view.TargetDirectory);
      _view.OnSyncRightToLeft += () => HandleSync(_view.TargetDirectory, _view.SourceDirectory);
    }

    private void HandleCompare()
    {
      try
      {
        ValidateDirectories();
        var changes = _synchronizer.CompareDirectories(_view.SourceDirectory, _view.TargetDirectory);
        _view.DisplayChanges(changes);
      }
      catch (Exception ex)
      {
        _view.ShowMessage($"Ошибка сравнения: {ex.Message}");
      }
    }

    private void HandleSync(string source, string target)
    {
      try
      {
        ValidateDirectories();
        _view.SetProgress(0);

        _synchronizer.Synchronize(source, target);

        _view.SetProgress(100);
        _view.ShowMessage("Синхронизация завершена успешно");
        HandleCompare(); // Обновляем список изменений
      }
      catch (Exception ex)
      {
        _view.ShowMessage($"Ошибка синхронизации: {ex.Message}");
      }
    }

    private void ValidateDirectories()
    {
      if (string.IsNullOrWhiteSpace(_view.SourceDirectory))
      {
        throw new ArgumentException("Не выбрана исходная директория");
      }

      if (string.IsNullOrWhiteSpace(_view.TargetDirectory))
      {
        throw new ArgumentException("Не выбрана целевая директория");
      }
    }
  }
}