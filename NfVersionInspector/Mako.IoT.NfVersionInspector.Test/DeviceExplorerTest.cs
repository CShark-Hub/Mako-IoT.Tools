using System.Linq;
using Mako.IoT.NFVersionInspector.Extensions;
using Mako.IoT.NFVersionInspector.Services;
using Xunit;
using Xunit.Abstractions;

namespace Mako.IoT.NfVersionInspector.Test
{
    public class DeviceExplorerTest
    {
        private readonly ITestOutputHelper _output;

        public DeviceExplorerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "Test when hardware connected")]
        public void GetNativeAssembliesFromDevice()
        {
            var result = new DeviceExplorer().GetBoardInfo("COM5").NativePackages().ToArray();
            
            foreach (var package in result)
            {
                _output.WriteLine($"{package.Id} {package.Version} {package.Checksum}");
            }
            
            Assert.NotEmpty(result);
        }
    }
}
