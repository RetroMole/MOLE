using System;
using System.Reflection;

namespace RetroMole.Core;
public static class PackageLoader
{
	public static Package[] LoadPackagesFromAssembly(string path)
	{
		var asm = Assembly.LoadFrom(path);
		return asm.DefinedTypes
			.Where(t => t.BaseType.FullName == "RetroMole.Core.Package")
			.Select((p, i) => (Package)asm.CreateInstance(p.FullName))
			.ToArray();
	}

	public static Package LoadPackagesCompressed(string path)
	{
		throw new NotImplementedException();
	}

	public static Package[] LoadAllInDirRecurse(string path)
	{
		var asms = Directory.GetFiles(path).Where(f => f.EndsWith(".dll"));
		List<Package> res = new();
		foreach (var a in asms)
			res.AddRange(LoadPackagesFromAssembly(a));
		return res.ToArray();
	}
}
