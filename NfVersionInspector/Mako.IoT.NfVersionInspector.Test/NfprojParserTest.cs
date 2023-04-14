using System.IO;
using System.Linq;
using Mako.IoT.NFVersionInspector;
using Mako.IoT.NFVersionInspector.Services;
using Xunit;

namespace Mako.IoT.NfVersionInspector.Test
{
    public class NfprojParserTest
    {
        [Fact]
        public void GetPackagesPaths_given_nfproj_should_extract_packages_paths()
        {
            var r = new StringReader(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""Current"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup Label=""Globals"">
    <NanoFrameworkProjectSystemPath>$(MSBuildExtensionsPath)\nanoFramework\v1.0\</NanoFrameworkProjectSystemPath>
  </PropertyGroup>
  <Import Project=""$(NanoFrameworkProjectSystemPath)NFProjectSystem.Default.props"" Condition=""Exists('$(NanoFrameworkProjectSystemPath)NFProjectSystem.Default.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectTypeGuids>{11A8DD76-328B-46DF-9F39-F559912D0360};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
    <ProjectGuid>aaf97c66-9575-4388-91d1-b90a18ef37c5</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <FileAlignment>512</FileAlignment>
    <RootNamespace>CShark.Mako.Iot.Client</RootNamespace>
    <AssemblyName>CShark.Mako.Iot.Client</AssemblyName>
    <TargetFrameworkVersion>v1.0</TargetFrameworkVersion>
  </PropertyGroup>
<Import Project=""$(NanoFrameworkProjectSystemPath)NFProjectSystem.props"" Condition=""Exists('$(NanoFrameworkProjectSystemPath)NFProjectSystem.props')"" />
  <ItemGroup>
<Compile Include=""Configuration\LoggerConfig.cs"" />
    <Compile Include=""Services\Scheduler.cs"" />
    <Compile Include=""Services\FileStorageService.cs"" />
  </ItemGroup>
<ItemGroup>
    <Reference Include=""mscorlib"">
      <HintPath>..\packages\nanoFramework.CoreLibrary.1.11.7\lib\mscorlib.dll</HintPath>
    </Reference>
    <Reference Include=""nanoFramework.Logging"">
      <HintPath>..\packages\nanoFramework.Logging.1.0.0\lib\nanoFramework.Logging.dll</HintPath>
    </Reference>
    <Reference Include=""nanoFramework.Runtime.Events"">
      <HintPath>..\packages\nanoFramework.Runtime.Events.1.9.2\lib\nanoFramework.Runtime.Events.dll</HintPath>
    </Reference>
    <Reference Include=""nanoFramework.Runtime.Native"">
      <HintPath>..\packages\nanoFramework.Runtime.Native.1.5.2\lib\nanoFramework.Runtime.Native.dll</HintPath>
    </Reference>
    <Reference Include=""nanoFramework.System.Collections"">
      <HintPath>..\packages\nanoFramework.System.Collections.1.3.0\lib\nanoFramework.System.Collections.dll</HintPath>
    </Reference>
    <Reference Include=""nanoFramework.System.Text"">
      <HintPath>..\packages\nanoFramework.System.Text.1.1.2\lib\nanoFramework.System.Text.dll</HintPath>
    </Reference>
    <Reference Include=""System.Net"">
      <HintPath>..\packages\nanoFramework.System.Net.1.7.1\lib\System.Net.dll</HintPath>
    </Reference>
    <Reference Include=""System.Threading"">
      <HintPath>..\packages\nanoFramework.System.Threading.1.0.3\lib\System.Threading.dll</HintPath>
    </Reference>
    <Reference Include=""Windows.Devices.Wifi"">
      <HintPath>..\packages\nanoFramework.Windows.Devices.Wifi.1.3.4-preview.30\lib\Windows.Devices.Wifi.dll</HintPath>
    </Reference>
    <Reference Include=""Windows.Storage"">
      <HintPath>..\packages\nanoFramework.Windows.Storage.1.4.5-preview.30\lib\Windows.Storage.dll</HintPath>
    </Reference>
    <Reference Include=""Windows.Storage.Streams"">
      <HintPath>..\packages\nanoFramework.Windows.Storage.Streams.1.12.3\lib\Windows.Storage.Streams.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <None Include=""packages.config"" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include=""..\nanoFramework.Json\nanoFramework.Json.nfproj"" />
    <ProjectReference Include=""..\nanoframework.M2Mqtt\nanoFramework.M2Mqtt.nfproj"" />
    <ProjectReference Include=""..\NetworkHelper\NetworkHelper.nfproj"" />
  </ItemGroup>
  <Import Project=""$(NanoFrameworkProjectSystemPath)NFProjectSystem.CSharp.targets"" Condition=""Exists('$(NanoFrameworkProjectSystemPath)NFProjectSystem.CSharp.targets')"" />
  <ProjectExtensions>
    <ProjectCapabilities>
      <ProjectConfigurationsDeclaredAsItems />
    </ProjectCapabilities>
  </ProjectExtensions>
</Project>"
                );

            var result = new NfprojParser().GetPackagesPaths(r).ToArray();
            Assert.Equal(11, result.Count());
            Assert.Contains("nanoFramework.CoreLibrary.1.11.7", result);
            Assert.Contains("nanoFramework.Windows.Devices.Wifi.1.3.4-preview.30", result);
            Assert.Contains("nanoFramework.Windows.Storage.Streams.1.12.3", result);

        }
    }
}
