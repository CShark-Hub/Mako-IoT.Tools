namespace Mako.IoT.NFVersionInspector;

public class PackageNotFoundException : Exception
{
    public PackageNotFoundException():base()
    {
            
    }
    public PackageNotFoundException(string message) : base(message)
    {
    }
}