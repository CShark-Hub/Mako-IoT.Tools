using CommandLine;
using Mako.IoT.NFVersionInspector;
using Mako.IoT.NFVersionInspector.Commands;
using Mako.IoT.NFVersionInspector.Services;
using Microsoft.Extensions.DependencyInjection;


var sp = new ServiceCollection()
    //commands
    .AddSingleton<BoardCommand>()
    .AddSingleton<ProjCommand>()
    .AddSingleton<CheckCommand>()
    .AddSingleton<FindCommand>()
    .AddSingleton<ClearCommand>()
    //services
    .AddSingleton<ICache, Cache>()
    .AddSingleton<IDependencyFinder, DependencyFinder>()
    .AddSingleton<IDeviceExplorer, DeviceExplorer>()
    .AddSingleton<IFileFinder, FileFinder>()
    .AddSingleton<INfprojParser, NfprojParser>()
    .AddSingleton<INugetClient, NugetClient>()
    .AddSingleton<INuspecParser, NuspecParser>()
    .AddSingleton<IStorage, Storage>()
    //settings
    .AddSingleton(new Settings
    {
        DataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
    })
    .BuildServiceProvider();

Parser.Default.ParseArguments<BoardOptions, ProjOptions, CheckOptions, FindOptions, ClearOptions>(args)
    .MapResult(
        (BoardOptions o) => sp.GetRequiredService<BoardCommand>().Execute(o),
        (ProjOptions o) => sp.GetRequiredService<ProjCommand>().Execute(o),
        (CheckOptions o) => sp.GetRequiredService<CheckCommand>().Execute(o),
        (FindOptions o) => sp.GetRequiredService<FindCommand>().Execute(o),
        (ClearOptions o) => sp.GetRequiredService<ClearCommand>().Execute(o),
        errors => 1);
