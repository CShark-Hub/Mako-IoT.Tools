using nanoFramework.Tools.Debugger;

namespace Mako.IoT.NFVersionInspector.Services;

public interface IDeviceExplorer
{
    INanoFrameworkDeviceInfo GetBoardInfo(string portName);
}