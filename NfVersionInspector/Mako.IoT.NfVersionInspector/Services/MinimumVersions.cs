namespace Mako.IoT.NFVersionInspector.Services
{
    public class MinimumVersions
    {
        private static readonly IDictionary<string, Version> _minimumVersions = new Dictionary<string, Version>
        {
            {"nanoFramework.CoreLibrary", new Version(1, 10, 5, 4)}
        };

        public static bool IsMinimumVersion(Package package)
        {
            if (_minimumVersions.ContainsKey(package.Id))
                return Version.Parse(package.Version.Replace("-preview", "")) >= _minimumVersions[package.Id];

            return true;
        }
    }
}
