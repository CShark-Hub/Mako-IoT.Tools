using Mako.IoT.NFVersionInspector.Services;

namespace Mako.IoT.NFVersionInspector.Commands
{
    public class ClearCommand
    {
        private readonly IStorage _storage;

        public ClearCommand(IStorage storage)
        {
            _storage = storage;
        }

        public int Execute(ClearOptions options)
        {
            if (options.ClearBoards)
                _storage.ClearBoardsInfo();

            if (options.ClearPackages)
                _storage.ClearPackages();

            return 0;
        }
    }
}
