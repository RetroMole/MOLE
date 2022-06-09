using System;
using System.Reflection;

namespace RetroMole;
public static class Launch
{
	public static string root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
	public static Core.Package[] pckgs = Core.PackageLoader.LoadAllInDirRecurse(Path.Combine(root, "Packages"));
	public static void Main(string[] args)
	{
		pckgs.First().Controllers.First().Main(() => {
			Gui.Main.TestUI();
		});
	}
}