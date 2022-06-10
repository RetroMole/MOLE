using System;
using System.Reflection;
namespace RetroMole;
public static class Launch
{
	public static void Main(string[] args)
	{
		var pckgsDir = Path.Combine(Core.Utility.CommonDirectories.Exec, "Packages");
		var pckgs = Directory.GetFileSystemEntries(pckgsDir, "*.dll", SearchOption.AllDirectories)
			.Concat(Directory.GetFileSystemEntries(pckgsDir, "*.mole.package", SearchOption.AllDirectories))
			.SelectMany(p => 
					p.EndsWith(".dll")          ? Core.Utility.Import.AssemblyPackages(Assembly.LoadFrom(p))
			   	  : p.EndsWith(".mole.package") ? Core.Utility.Import.CompressedPackages(p)
			   	  : throw new Exception($"Uhh... how did you.. BAD PACKAGE @ {p}"))
			.ToArray();

		pckgs.First().Controllers.First().Main(() => {
			Gui.Main.TestUI();
		});
	}
}