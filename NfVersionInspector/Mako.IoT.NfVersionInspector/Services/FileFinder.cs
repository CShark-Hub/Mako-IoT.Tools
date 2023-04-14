using System.IO.Compression;

namespace Mako.IoT.NFVersionInspector.Services
{
    public class FileFinder : IFileFinder
    {
        public void ProcessNfprojFiles(string rootPath, Action<StreamReader> processAction)
        {
            foreach (var file in Directory.GetFiles(rootPath, "*.nfproj", SearchOption.AllDirectories))
            {
                using var reader = new StreamReader(File.OpenRead(file));
                processAction(reader);
                reader.Close();
            }
        }

        public void ProcessNuspecFiles(string rootPath, IEnumerable<string> packages, Action<StreamReader> processAction)
        {
            var packagesFolder = Directory.GetDirectories(rootPath, "packages", SearchOption.AllDirectories).First();
            foreach (var p in packages)
            {
                var path = Path.Combine(packagesFolder, p);
                if (Directory.Exists(path))
                {
                    var nupkg = Directory
                        .GetFiles(path, "*.nupkg", SearchOption.TopDirectoryOnly)
                        .FirstOrDefault();
                    if (nupkg != null)
                    {
                        using var zip = ZipFile.OpenRead(nupkg);
                        var nuspecEntry = zip.Entries.FirstOrDefault(e => e.Name.EndsWith(".nuspec"));
                        if (nuspecEntry != null)
                        {
                            using var reader = new StreamReader(nuspecEntry.Open());
                            processAction(reader);
                            reader.Close();
                        }
                    }
                }
            }
        }
    }
}
