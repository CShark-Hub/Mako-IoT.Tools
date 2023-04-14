using System;
using System.IO;
using System.Linq;
using Mako.IoT.NFVersionInspector;
using Mako.IoT.NFVersionInspector.Services;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Mako.IoT.NfVersionInspector.Test
{
    public class NugetClientTest
    {
        private readonly ITestOutputHelper _output;

        public NugetClientTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GetFromNuspec_given_package_details_should_get_packages_nuspec()
        {
            string id = "nanoFramework.Windows.Storage";
            string version = "1.4.5-preview.30";

            string nuspecString = String.Empty;
            var parser = new Mock<INuspecParser>();
            parser.Setup(s => s.Parse(It.IsAny<TextReader>()))
                .Callback<TextReader>(reader => nuspecString = reader.ReadToEnd())
                .Returns(new Package("test", "0.0.0.0"));

            var result = new NugetClient(parser.Object)
                .GetFromNuspec("nanoFramework.Windows.Storage", "1.4.5-preview.30", out var versionFound);

            _output.WriteLine(nuspecString);

            Assert.Contains(id, nuspecString);
            Assert.Contains(version, nuspecString);
            Assert.Equal(version, versionFound);
        }

        [Fact]
        public void GetPackageVersions_given_package_id_should_return_list_of_versions()
        {
            string id = "nanoFramework.Windows.Storage";

            var parser = new Mock<INuspecParser>();
            var result = new NugetClient(parser.Object).GetPackageVersions(id);

            Assert.True(result.Any());
        }
    }
}
