using nanoFramework.Tools.Debugger;

namespace Mako.IoT.NFVersionInspector.Extensions
{
    public static class DeviceInfoExtensions
    {
        public static IEnumerable<Package> NativePackages(this INanoFrameworkDeviceInfo deviceInfo)
        {
            return deviceInfo.NativeAssemblies.Select(a =>
                new Package(a.Name, a.Version.ToString(), $"0x{a.Checksum.ToString("x8").ToUpper()}"));
        }
    }
}
