using System.Net;
using Newtonsoft.Json.Linq;

namespace Mako.IoT.NFVersionInspector.Services
{
    public class NugetClient : INugetClient
    {
        private readonly INuspecParser _nuspecParser;

        public NugetClient(INuspecParser nuspecParser)
        {
            _nuspecParser = nuspecParser;
        }

        public Package GetFromNuspec(string id, string version, out string versionFound)
        {
            versionFound = version;
            using var client = new HttpClient();
            var response = client.GetAsync($"https://api.nuget.org/v3-flatcontainer/{id}/{version}/{id}.nuspec").GetAwaiter().GetResult();
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var versions = GetPackageVersions(id);
                versionFound = versions.FirstOrDefault(v => v.StartsWith(version)) ??
                               throw new PackageNotFoundException($"Package {id} not found on nuget");
                response = client
                    .GetAsync($"https://api.nuget.org/v3-flatcontainer/{id}/{versionFound}/{id}.nuspec")
                    .GetAwaiter().GetResult();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new PackageNotFoundException($"Package {id} {version} not found on nuget");

            response.EnsureSuccessStatusCode();
            using var r = new StreamReader(response.Content.ReadAsStream());
            return _nuspecParser.Parse(r);
        }

        public IEnumerable<string> GetPackageVersions(string id)
        {
            using var client = new HttpClient();
            var response = client.GetAsync($"https://api.nuget.org/v3-flatcontainer/{id}/index.json").GetAwaiter().GetResult();

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new PackageNotFoundException($"Package {id} not found on nuget");

            response.EnsureSuccessStatusCode();
            var j = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var jo = JObject.Parse(j);
            var list = jo["versions"].Select(s => (string)s).ToList();
            list.Reverse();
            return list;
        }
    }
}
