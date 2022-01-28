﻿
using Serilog;
using System.Reflection;

namespace RetroMole.Core.Utility
{
    public abstract class DynamicAssemblyLoader
    {
        public static bool VerifyAssembly(string AssemblyPath, byte[] PublicKeyToken, out Assembly asm)
        {
            asm = null;
            try
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(AssemblyPath);
                if (!Enumerable.SequenceEqual(assemblyName.GetPublicKeyToken(), PublicKeyToken))
                    Log.Warning("Assembly @ \"{0}\" PublicKeyToken mismatched!!", AssemblyPath);
                
                // Load anyway for now cuz i can't get the public key tokens to match lmfao fml ;_;
                asm = Assembly.LoadFile(AssemblyPath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool VerifyAssembly<T>(string AssemblyPath, byte[] PublicKeyToken, out Dictionary<string, T> Implementations)
        {
            // Verify with public key
            var v = VerifyAssembly(AssemblyPath, PublicKeyToken, out var asm);
            Implementations = new();
            if (!v) return false; // Unsafe

            // Verify Implementations
            var Types = asm.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(T))).ToArray();
            if (Types.Length == 0) return false;

            foreach (var t in Types)
            {
                Implementations.Add($"{t.Assembly.GetName().Name}|{t.FullName}", (T)Activator.CreateInstance(t));
            }

            return true;
        }
    }
}
