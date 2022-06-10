using System;
using System.Reflection;
namespace RetroMole;
public static class Launch
{
	public static Core.Interfaces.Package[] pckgs;
	public static void Main(string[] args)
	{
		var pckgsDir = Path.Combine(Core.Utility.CommonDirectories.Exec, "Packages");

		var pckgAsms = Directory.GetFileSystemEntries(pckgsDir, "*.dll", SearchOption.AllDirectories);
		var compressedPckgsFiles = Directory.GetFileSystemEntries(pckgsDir, "*.mole.package", SearchOption.AllDirectories);

		List<Core.Interfaces.Package> pckgList = new();
		
		foreach (var f in pckgAsms)
			pckgList.AddRange(Core.Utility.Import.AssemblyPackages(Assembly.LoadFrom(f)));

		foreach (var f in compressedPckgsFiles)
			pckgList.AddRange(Core.Utility.Import.CompressedPackages(f));

		pckgs = pckgList.ToArray();

		pckgs.First().Controllers.First().Main(() => {
			Gui.Main.TestUI();
		});
	}
}