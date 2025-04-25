using System;
using System.Collections.Generic;
using System.IO;

namespace DirectorySyncApp.Model
{
  public class DirectorySynchronizer
  {
    public List<FileChange> CompareDirectories(string sourceDir, string targetDir)
    {
      ValidateDirectories(sourceDir, targetDir);

      var changes = new List<FileChange>();
      var sourceFiles = GetFileDictionary(sourceDir);
      var targetFiles = GetFileDictionary(targetDir);

      // Поиск новых и измененных файлов
      foreach (var file in sourceFiles)
      {
        if (!targetFiles.ContainsKey(file.Key))
        {
          changes.Add(new FileChange { FilePath = file.Key, ChangeType = FileChangeType.Created });
        }
        else if (file.Value != targetFiles[file.Key])
        {
          changes.Add(new FileChange { FilePath = file.Key, ChangeType = FileChangeType.Modified });
        }
      }

      // Поиск удаленных файлов
      foreach (var file in targetFiles)
      {
        if (!sourceFiles.ContainsKey(file.Key))
        {
          changes.Add(new FileChange { FilePath = file.Key, ChangeType = FileChangeType.Deleted });
        }
      }

      return changes;
    }

    public void Synchronize(string sourceDir, string targetDir)
    {
      ValidateDirectories(sourceDir, targetDir);
      var changes = CompareDirectories(sourceDir, targetDir);

      foreach (var change in changes)
      {
        string sourcePath = Path.Combine(sourceDir, change.FilePath);
        string targetPath = Path.Combine(targetDir, change.FilePath);

        try
        {
          switch (change.ChangeType)
          {
            case FileChangeType.Created:
            case FileChangeType.Modified:
              EnsureDirectoryExists(Path.GetDirectoryName(targetPath));
              File.Copy(sourcePath, targetPath, overwrite: true);
              break;
            case FileChangeType.Deleted:
              if (File.Exists(targetPath))
                File.Delete(targetPath);
              break;
          }
        }
        catch (Exception ex)
        {
          throw new Exception($"Ошибка при обработке файла {change.FilePath}: {ex.Message}");
        }
      }
    }

    private Dictionary<string, DateTime> GetFileDictionary(string path)
    {
      var dict = new Dictionary<string, DateTime>();
      var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

      foreach (var file in files)
      {
        string relativePath = file.Substring(path.Length).TrimStart(Path.DirectorySeparatorChar);
        dict[relativePath] = File.GetLastWriteTime(file);
      }

      return dict;
    }

    private void ValidateDirectories(params string[] paths)
    {
      foreach (var path in paths)
      {
        if (!Directory.Exists(path))
          throw new DirectoryNotFoundException($"Директория не существует: {path}");
      }
    }

    private void EnsureDirectoryExists(string path)
    {
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
    }
  }
}