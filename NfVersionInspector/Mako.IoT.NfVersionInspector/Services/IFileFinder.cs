namespace Mako.IoT.NFVersionInspector.Services;

public interface IFileFinder
{
    void ProcessNfprojFiles(string rootPath, Action<StreamReader> processAction);
    void ProcessNuspecFiles(string rootPath, IEnumerable<string> packages, Action<StreamReader> processAction);
}