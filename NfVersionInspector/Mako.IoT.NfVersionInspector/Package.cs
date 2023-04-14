using Newtonsoft.Json;

namespace Mako.IoT.NFVersionInspector
{
    public class Package
    {
        public Package(string id, string version) : this(id, version, false, String.Empty, new List<Package>())
        {
        }

        public Package(string id, string version, IList<Package> dependencies) : this(id, version, false, String.Empty, dependencies)
        {
        }

        public Package(string id, string version, string checksum):this(id, version, true, checksum, new List<Package>())
        {
        }

        [JsonConstructor]
        public Package(string id, string version, bool isNative, string checksum, IList<Package> dependencies)
        {
            Id = id;
            Version = version;
            IsNative = isNative;
            Checksum = checksum;
            Dependencies = dependencies;
        }


        public string Id { get; }
        public string Version { get; }
        public bool IsNative { get; }
        public string Checksum { get; }
        public IList<Package> Dependencies { get; }

        protected bool Equals(Package other)
        {
            return GetIdWithPrefix() == other.GetIdWithPrefix() && Version == other.Version && IsNative == other.IsNative && Checksum == other.Checksum;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Package)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetIdWithPrefix(), Version, IsNative, Checksum);
        }

        protected string GetIdWithPrefix() => Id.StartsWith("nanoFramework.") ? Id : $"nanoFramework.{Id}";

        public override string ToString()
        {
            return $"{Id} {Version} {Checksum}";
        }
    }

}
