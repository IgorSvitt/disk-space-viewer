using STMLabs.Models;
using Directory = STMLabs.Models.Directory;
using File = STMLabs.Models.File;


namespace STMLabs.Service
{
    public class DirectoryMethods
    {
        private readonly Dictionary<string, long> _directorySizes = new();
        private readonly Logger<DirectoryMethods> _logger;

        public DirectoryMethods(Logger<DirectoryMethods> logger)
        {
            _logger = logger;
        }

        public async Task<CurrentDirectory> GetDirectories(string path)
        {
            var directory = new CurrentDirectory()
            {
                SubDirectories = new List<Directory>(),
                Files = new List<File>(),
                Name = new DirectoryInfo(path).Name,
                Path = new DirectoryInfo(path).FullName
            };

            var directoriesInfo = new DirectoryInfo(path);

            foreach (var directoryInfo in directoriesInfo.GetDirectories())
            {
                var subDirectory = new Directory()
                {
                    Name = directoryInfo.Name,
                    Path = directoryInfo.FullName
                };
                subDirectory.Size = await GetDirectorySize(subDirectory.Path);
                _directorySizes[subDirectory.Path] = subDirectory.Size;
                directory.SubDirectories.Add(subDirectory);
            }

            foreach (var fileInfo in directoriesInfo.GetFiles())
            {
                var file = new File()
                {
                    Name = fileInfo.Name,
                    Path = fileInfo.FullName,
                    Size = fileInfo.Length
                };
                _directorySizes[file.Path] = file.Size;
                directory.Files.Add(file);
            }

            directory.Files = directory.Files.OrderBy(x => x.Size).ToList();
            directory.SubDirectories = directory.SubDirectories.OrderBy(x => x.Size).ToList();


            return directory;
        }


        private async Task<long> GetDirectorySize(string path)
        {
            if (_directorySizes.TryGetValue(path, out var directorySize))
            {
                return directorySize;
            }

            var directoryInfo = new DirectoryInfo(path);
            var size = await GetDirectorySizeRecursive(directoryInfo);
            _directorySizes[path] = size;
            return size;
        }

        private async Task<long> GetDirectorySizeRecursive(DirectoryInfo directoryInfo)
        {
            long size = 0;

            try
            {
                var fileInfos = directoryInfo.GetFiles();

                foreach (FileInfo fileInfo in fileInfos)
                {
                    try
                    {
                        size += fileInfo.Length;
                        _directorySizes[fileInfo.FullName] = fileInfo.Length;
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            try
            {
                var directoryInfos = directoryInfo.GetDirectories();

                foreach (DirectoryInfo subDirectoryInfo in directoryInfos)
                {
                    try
                    {
                        long subDirectorySize = await GetDirectorySizeRecursive(subDirectoryInfo);
                        size += subDirectorySize;
                        _directorySizes[subDirectoryInfo.FullName] = subDirectorySize;
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
               _logger.LogError(ex.Message);
            }

            return size;
        }
    }
}