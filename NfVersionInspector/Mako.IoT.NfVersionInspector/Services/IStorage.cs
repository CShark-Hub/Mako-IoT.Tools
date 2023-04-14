namespace Mako.IoT.NFVersionInspector.Services;

public interface IStorage
{
    IEnumerable<Package> Load(string id);
    void Save(string id, IEnumerable<Package> packages);
    void SaveBoardInfo(string name, IEnumerable<Package> packages);
    IEnumerable<Package> LoadBoardInfo(string name);
    IEnumerable<string> ListBoardInfo();
    IEnumerable<Package>? LoadFromFile(string fileName);
    void ClearBoardsInfo();
    void ClearPackages();
}