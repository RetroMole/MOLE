// using System.Runtime.Loader;
// using RetroMole.Core.Interfaces;
// using Serilog;
//
// namespace RetroMole.Core;
//
// public static class Global
// {
//     //--------------------Packages--------------------//
//     public static Package[] Packages =
//         Directory.GetFileSystemEntries(
//                 Path.Combine(ExecPath, "Packages"),
//                 "*.dll", SearchOption.AllDirectories
//             )
//             .Concat(
//                 Directory.GetFileSystemEntries(
//                     Path.Combine(ExecPath, "Packages"),
//                     "*.mole.pckg", SearchOption.AllDirectories
//                 )
//             )
//             .SelectMany(p =>
//             {
//                 var ctx = new AssemblyLoadContext($"{Path.GetFullPath(p)}_{DateTime.Now.ToFileTimeUtc()}");
//                 switch (Path.GetExtension(p)?.ToUpper())
//                 {
//                     case ".DLL":
//                         return Import.AssemblyPackages(ctx.LoadFromAssemblyPath(Path.GetFullPath(p)));
//                     case ".MOLE.PCKG":
//                         return Import.CompressedPackages(p);
//                     default:
//                         Log.Error($"Uhh... how did you.. BAD PACKAGE @ {p}");
//                         return null;
//                 }
//             })
//             .ToArray();
// }