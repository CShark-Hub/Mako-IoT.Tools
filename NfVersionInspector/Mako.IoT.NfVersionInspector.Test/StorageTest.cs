using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mako.IoT.NFVersionInspector;
using Mako.IoT.NFVersionInspector.Services;
using Xunit;

namespace Mako.IoT.NfVersionInspector.Test
{
    public class StorageTest
    {
        [Fact]
        public void Save_given_packages_list_should_save_it_to_file()
        {
            var settings = new Settings
            {
                DataFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
            };

            try
            {
                var p = new HashSet<Package>(new Package[]
                {
                    new Package("id.1", "version.1", new List<Package>
                    {
                        { new Package("id.2", "version.1") },
                        { new Package("id.3", "version.1") }
                    }),
                    new Package("id.1", "version.2"),
                    new Package("id.1", "version.3"),
                    new Package("id.1", "version.4"),
                    new Package("id.1", "version.5"),
                    new Package("id.1", "version.6"),
                    new Package("id.1", "version.7"),
                    new Package("id.1", "version.8"),
                });

                new Storage(settings).Save("id.1", p);

                Assert.True(File.Exists(Path.Combine(settings.DataFolder, Storage.Folder,
                    $"id.1{Storage.NugetCacheExt}")));
            }
            finally
            {
                Directory.Delete(settings.DataFolder, true);
            }
        }

        [Fact]
        public void Load_given_file_should_return_list_of_packages()
        {
            var settings = new Settings
            {
                DataFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
            };

            var fileContents =
                @"[{""Id"":""id.1"",""Version"":""version.1"",""IsNative"":false,""Checksum"":"""",""Dependencies"":[{""Id"":""id.2"",""Version"":""version.1"",""IsNative"":false,""Checksum"":"""",""Dependencies"":[]},{""Id"":""id.3"",""Version"":""version.1"",""IsNative"":false,""Checksum"":"""",""Dependencies"":[]}]},{""Id"":""id.1"",""Version"":""version.2"",""IsNative"":false,""Checksum"":"""",""Dependencies"":[]},{""Id"":""id.1"",""Version"":""version.3"",""IsNative"":false,""Checksum"":"""",""Dependencies"":[]},{""Id"":""id.1"",""Version"":""version.4"",""IsNative"":false,""Checksum"":"""",""Dependencies"":[]},{""Id"":""id.1"",""Version"":""version.5"",""IsNative"":false,""Checksum"":"""",""Dependencies"":[]},{""Id"":""id.1"",""Version"":""version.6"",""IsNative"":false,""Checksum"":"""",""Dependencies"":[]},{""Id"":""id.1"",""Version"":""version.7"",""IsNative"":false,""Checksum"":"""",""Dependencies"":[]},{""Id"":""id.1"",""Version"":""version.8"",""IsNative"":false,""Checksum"":"""",""Dependencies"":[]}]";

            try
            {
                Directory.CreateDirectory(Path.Combine(settings.DataFolder, Storage.Folder));

                using var fs = new StreamWriter(Path.Combine(settings.DataFolder, Storage.Folder, $"id.1{Storage.NugetCacheExt}"));
                fs.Write(fileContents);
                fs.Close();

                var p = new Storage(settings).Load("id.1");
                Assert.Equal(8, p.Count());
            }
            finally
            {
                Directory.Delete(settings.DataFolder, true);
            }
        }
    }
}
