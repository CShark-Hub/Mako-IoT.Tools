namespace Mako.IoT.NFVersionInspector.Services;

public interface INuspecParser
{
    Package Parse(TextReader reader);
}