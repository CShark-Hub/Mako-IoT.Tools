# nanoFramework Version Inspector
This tool helps with managing nanoFramework assemblies versions between native assemblies flashed onto a board (firmware) and managed assemblies (nuget packages). More about assembly versioning can be found here: https://www.nanoframework.net/nuget-assembly-and-native-versions/.

# Usage examples
**Pre-requisites**
1. Connect your board with flashed nanoFramework firmware and note its COM port.
2. Run nfvi.exe board -p _com_port_ -n _board_name_ - it will fetch firmare data from the board and store list of native assemblies under _board_name_.

**Check if nuget package is compatible with your board's firmware**

nfvi.exe check -n _board_name_ -i _package_id_ -v _package_version_

**Find newest version of nuget package, which is compatible with your board's firmware**

nfvi.exe find -n _board_name_ -i _package_id_

**Display all assemblies referenced by .nfproj files in your solution**

nfvi.exe proj -t _path_to_your_solution_ 

**Check your board's firmware compatibility with your solution**

nfvi.exe proj -t _path_to_your_solution_ -n _board_name_

**Look for possible nuget packages upgrades**

nfvi.exe proj -t _path_to_your_solution_ -n _board_name_ -u
