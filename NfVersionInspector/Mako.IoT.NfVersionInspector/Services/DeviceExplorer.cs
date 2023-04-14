using nanoFramework.Tools.Debugger;
using nanoFramework.Tools.Debugger.PortSerial;
using nanoFramework.Tools.Debugger.WireProtocol;

namespace Mako.IoT.NFVersionInspector.Services
{
    public class DeviceExplorer : IDeviceExplorer
    {
        public INanoFrameworkDeviceInfo GetBoardInfo(string portName)
        {
            var port = PortBase.CreateInstanceForSerial(false, null);
            var device = new NanoDevice<NanoSerialDevice>();
            device.DeviceId = portName;
            device.ConnectionPort = new PortSerial((PortSerialManager)port, device);
            device.Transport = TransportType.Serial;

            var connectResult = device.ConnectionPort.ConnectDevice();
            if (connectResult != ConnectPortResult.Connected)
                throw new Exception($"Can't connect to device on port {portName}: {connectResult}");

            device.CreateDebugEngine();
            if (!device.DebugEngine.Connect(false, true))
                throw new Exception($"Can't connect to nanoCLR on device at {portName}");

            var info = device.GetDeviceInfo(true);

            device.Disconnect(true);

            return info;
        }
    }
}
