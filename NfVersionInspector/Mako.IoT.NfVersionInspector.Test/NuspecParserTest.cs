using System.IO;
using Mako.IoT.NFVersionInspector;
using Mako.IoT.NFVersionInspector.Services;
using Xunit;

namespace Mako.IoT.NfVersionInspector.Test
{
    public class NuspecParserTest
    {
        [Fact]
        public void ParseNuspec_given_nuspec_string_should_return_package()
        {
            var ns = new StringReader(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<package xmlns=""http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd"">
  <metadata>
    <id>nanoFramework.Windows.Storage</id>
    <version>1.4.5-preview.30</version>
    <title>nanoFramework.Windows.Storage</title>
    <authors>nanoFramework project contributors</authors>
    <owners>nanoframework,dotnetfoundation</owners>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <license type=""file"">LICENSE.md</license>
    <licenseUrl>https://aka.ms/deprecateLicenseUrl</licenseUrl>
    <icon>images\nf-logo.png</icon>
    <readme>docs\README.md</readme>
    <projectUrl>https://github.com/nanoframework/Windows.Storage</projectUrl>
    <description>This package includes the nanoFramework.Windows.Storage assembly for .NET nanoFramework C# projects.
This package requires a target with nanoFramework.Windows.Storage v100.0.2.0 (checksum 0x954A4192).</description>
    <releaseNotes></releaseNotes>
    <copyright>Copyright (c) .NET Foundation and Contributors</copyright>
    <tags>nanoFramework C# csharp netmf netnf nanoFramework.Windows.Storage</tags>
    <repository type=""git"" url=""https://github.com/nanoframework/nanoFramework.Windows.Storage"" commit=""87e2ddf7a9481b204b65a18c11f7fff357f40245"" />
    <dependencies>
      <dependency id=""nanoFramework.CoreLibrary"" version=""1.11.7"" />
      <dependency id=""nanoFramework.Windows.Storage.Streams"" version=""1.12.3"" />
      <dependency id=""nanoFramework.Runtime.Events"" version=""1.9.2"" />
      <dependency id=""nanoFramework.System.Text"" version=""1.1.2"" />
    </dependencies>
  </metadata>
</package>"
                );

            var result = new NuspecParser().Parse(ns);

            Assert.Equal("nanoFramework.Windows.Storage", result.Id);
            Assert.Equal("1.4.5-preview.30", result.Version);
            Assert.False(result.IsNative);
            Assert.Equal(5, result.Dependencies.Count);
            Assert.Contains(result.Dependencies,
                p => p.Id == "nanoFramework.Windows.Storage" && p.IsNative && p.Version== "100.0.2.0" && p.Checksum == "0x954A4192");
            Assert.Contains(result.Dependencies,
                p => p.Id == "nanoFramework.System.Text" && !p.IsNative && p.Version == "1.1.2");

        }
    }
}