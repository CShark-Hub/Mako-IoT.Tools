namespace Mako.IoT.NFVersionInspector.Services;

public interface INfprojParser
{
    IEnumerable<string> GetPackagesPaths(TextReader reader);
}