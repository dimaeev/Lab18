namespace DirectorySyncApp.Model
{
  public enum FileChangeType
  {
    Created,
    Modified,
    Deleted
  }

  public class FileChange
  {
    public string FilePath { get; set; }
    public FileChangeType ChangeType { get; set; }
  }
}