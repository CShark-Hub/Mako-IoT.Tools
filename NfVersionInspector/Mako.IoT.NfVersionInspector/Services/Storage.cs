using Newtonsoft.Json;

namespace Mako.IoT.NFVersionInspector.Services
{
    public class Storage : IStorage
    {
        public const string Folder = "Mako.IoT.NFVersionInspector";
        public const string NugetCacheExt = ".nugetcache";
        public const string BoardCacheExt = ".boardcache";

        private readonly Settings _settings;

        public Storage(Settings settings)
        {
            _settings = settings;
        }

        public IEnumerable<Package> Load(string id)
        {
            return LoadFromFile(GetFilePath($"{id}.nugetcache")) ?? Array.Empty<Package>();
        }

        public void Save(string id, IEnumerable<Package> packages)
        {
            SaveToFile(GetFilePath($"{id}.nugetcache"), packages);
        }

        public void SaveBoardInfo(string name, IEnumerable<Package> packages)
        {
            SaveToFile(GetFilePath($"{name}.boardcache"), packages);
        }

        public IEnumerable<Package> LoadBoardInfo(string name)
        {
            return LoadFromFile(GetFilePath($"{name}.boardcache")) ?? throw new FileNotFoundException();
        }

        public IEnumerable<string> ListBoardInfo()
        {
            return Directory.GetFiles(GetFilePath(String.Empty), "*.boardcache")
                .Select(Path.GetFileNameWithoutExtension)!;
        }

        public IEnumerable<Package>? LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            try
            {
                using var reader = new StreamReader(fileName);
                var s = reader.ReadToEnd();
                reader.Close();
                return JsonConvert.DeserializeObject<IEnumerable<Package>>(s);
            }
            catch (Exception e)
            {

            }

            return null;
        }

        private void SaveToFile(string fileName, IEnumerable<Package> packages)
        {
            var s = JsonConvert.SerializeObject(packages);
            if (File.Exists(fileName))
                File.Delete(fileName);
            using var sw = new StreamWriter(fileName);
            sw.Write(s);
            sw.Close();
        }

        private string GetFilePath(string fileName)
        {
            var folder = Path.Combine(_settings.DataFolder, Folder);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return fileName == String.Empty ? folder : Path.Combine(folder, fileName);
        }

        public void ClearBoardsInfo()
        {
            foreach (var file in Directory.GetFiles(GetFilePath(String.Empty), "*.boardcache"))
            {
                File.Delete(file);
            }
        }

        public void ClearPackages()
        {
            foreach (var file in Directory.GetFiles(GetFilePath(String.Empty), "*.nugetcache"))
            {
                File.Delete(file);
            }
        }
    }
}
