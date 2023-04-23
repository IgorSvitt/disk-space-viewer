namespace STMLabs.Models;

public class CurrentDirectory
{
    public string Name { get; set; }
    public string Path { get; set; }
    public List<File> Files { get; set; }
    public List<Directory> SubDirectories { get; set; }
}