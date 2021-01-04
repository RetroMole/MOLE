using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MOLE_Back.Libs
{
    /// <summary>
    /// Contains various functions to apply patches.
    /// </summary>
    public static unsafe class Asar
    {
        public const string dllPath = "asar.dll";
        public const int ExpectedApiVersion = 303;
        public static string ver2str(int ver)
        {
            //major*10000+minor*100+bugfix*1.
            //123456 = 12.3456
            int maj = ver/10000;
            int min = (ver-(maj*10000))/100; 
            int fx  = (ver-((maj*10000)+(min*100)))/1;
            return String.Format("{0}.{1}{2}",maj,min,fx);
        }

        [DllImport(dllPath, EntryPoint = "asar_init", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool _Init();

        /// <summary>
        /// Closes Asar DLL. Call this when you're done using Asar functions.
        /// </summary>
        [DllImport(dllPath, EntryPoint = "asar_close", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Close();

        /// <summary>
        /// Returns the version, in the format major*10000+minor*100+bugfix*1.
        /// This means that 1.2.34 would be returned as 10234.
        /// </summary>
        /// <returns>Asar version</returns>
        [DllImport(dllPath, EntryPoint = "asar_version", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Version();

        /// <summary>
        /// Returns the API version, format major*100+minor. Minor is incremented on backwards compatible
        ///  changes; major is incremented on incompatible changes. Does not have any correlation with the
        ///  Asar version.
        /// It's not very useful directly, since Asar.init() verifies this automatically.
        /// </summary>
        /// <returns>Asar API version</returns>
        [DllImport(dllPath, EntryPoint = "asar_apiversion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ApiVersion();

        /// <summary>
        /// Clears out all errors, warnings and printed statements, and clears the file cache.
        /// Not useful for much, since patch() already does this.
        /// </summary>
        /// <returns>True if success</returns>
        [DllImport(dllPath, EntryPoint = "asar_reset", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Reset();

        [DllImport(dllPath, EntryPoint = "asar_patch", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool _Patch(string patchLocation, byte* romData, int bufLen, int* romLength);

        [DllImport(dllPath, EntryPoint = "asar_patch_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool _Patch_ex(ref RawPatchParams parameters);

        /// <summary>
        /// Returns the maximum possible size of the output ROM from asar_patch().
        /// Giving this size to buflen guarantees you will not get any buffer too small errors;
        /// however, it is safe to give smaller buffers if you don't expect any ROMs larger
        /// than 4MB or something. It's not very useful directly, since Asar.patch() uses this automatically.
        /// </summary>
        /// <returns>Maximum output size of the ROM.</returns>
        [DllImport(dllPath, EntryPoint = "asar_maxromsize", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MaxROMSize();

        [DllImport(dllPath, EntryPoint = "asar_geterrors", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern RawAsarError* _GetErrors(out int Length);

        [DllImport(dllPath, EntryPoint = "asar_getwarnings", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern RawAsarError* _GetWarnings(out int Length);

        [DllImport(dllPath, EntryPoint = "asar_getprints", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void** _GetPrints(out int Length);

        [DllImport(dllPath, EntryPoint = "asar_getalllabels", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern RawAsarLabel* _GetAllLabels(out int Length);

        [DllImport(dllPath, EntryPoint = "asar_getlabelval", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetLabelVal(string LabelName);

        /// <summary>
        /// Gets contents of a define. If define doesn't exists, a null string will be generated.
        /// </summary>
        /// <param name="DefineName">The define name.</param>
        /// <returns>The define content. If define has not found, this will be null.</returns>
        [DllImport(dllPath, EntryPoint = "asar_getdefine", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.AnsiBStr)]
        public static extern string GetDefine(string DefineName);

        [DllImport(dllPath, EntryPoint = "asar_getalldefines", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern RawAsarDefine* _GetAllDefines(out int Length);

        /// <summary>
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="LearnNew"></param>
        /// <returns></returns>
        [DllImport(dllPath, EntryPoint = "asar_resolvedefines", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.AnsiBStr)]
        public static extern string ResolveDefines(string Data, bool LearnNew);

        [DllImport(dllPath, EntryPoint = "asar_math", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double _Math(string Math, out IntPtr Error);

        [DllImport(dllPath, EntryPoint = "asar_getwrittenblocks", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern RawAsarWrittenBlock* _GetWrittenBlocks(out int length);

        /// <summary>
        /// Gets mapper currently used by Asar.
        /// </summary>
        /// <returns>Returns mapper currently used by Asar.</returns>
        [DllImport(dllPath, EntryPoint = "asar_getmapper", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern MapperType GetMapper();

        /// <summary>
        /// Generates the contents of a symbols file for in a specific format.
        /// </summary>
        /// <param name="format">The symbol file format to generate</param>
        /// <returns>Returns the textual contents of the symbols file.</returns>
        [DllImport(dllPath, EntryPoint = "asar_getsymbolsfile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.AnsiBStr)]
        public static extern string GetSymbolsFile(string format = "wla");

        /// <summary>
        /// Loads and initializes the DLL. You must call this before using any other Asar function.
        /// </summary>
        /// <returns>True if success</returns>
        public static bool Init()
        {
            try
            {
                if (ApiVersion() < ExpectedApiVersion || (ApiVersion() / 100) > (ExpectedApiVersion / 100))
                {
                    return false;
                }

                if (!_Init())
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct RawPatchParams
        {
            public int structsize;
            public string patchloc;
            public byte* romdata;
            public int buflen;
            public int* romlen;
            public byte** includepaths;
            public int numincludepaths;
            [MarshalAs(UnmanagedType.I1)]
            public bool should_reset;
            public RawAsarDefine* additional_defines;
            public int additional_define_count;
            public string stdincludesfile;
            public string stddefinesfile;
        };

        /// <summary>
        /// Applies a patch.
        /// </summary>
        /// <param name="PatchLocation">The patch location.</param>
        /// <param name="ROMData">The rom data. It must not be headered.</param>
        /// <param name="IncludePaths">lists additional include paths</param>
        /// <param name="ShouldReset">specifies whether asar should clear out all defines, labels, etc from the last inserted file.<br/> 
        /// Setting it to False will make Asar act like the currently patched file was directly appended to the previous one.</param>
        /// <param name="AdditionalDefines">specifies extra defines to give to the patch</param>
        /// <param name="STDIncludeFile">path to a file that specifes additional include paths</param>
        /// <param name="STDDefineFile">path to a file that specifes additional defines</param>
        /// <returns>True if no errors.</returns>
        public static bool Patch(string PatchLocation, ref byte[] ROMData, string[] IncludePaths = null,
            bool ShouldReset = true, Dictionary<string, string> AdditionalDefines = null,
            string STDIncludeFile = null, string STDDefineFile = null)
        {
            if (IncludePaths == null)
            {
                IncludePaths = new string[0];
            }
            if (AdditionalDefines == null)
            {
                AdditionalDefines = new Dictionary<string, string>();
            }

            var includes = new byte*[IncludePaths.Length];
            var defines = new RawAsarDefine[AdditionalDefines.Count];

            try
            {
                for (int i = 0; i < IncludePaths.Length; i++)
                {
                    includes[i] = (byte*)Marshal.StringToCoTaskMemAnsi(IncludePaths[i]);
                }

                var keys = AdditionalDefines.Keys.ToArray();

                for (int i = 0; i < AdditionalDefines.Count; i++)
                {
                    var name = keys[i];
                    var value = AdditionalDefines[name];
                    defines[i].name = Marshal.StringToCoTaskMemAnsi(name);
                    defines[i].contents = Marshal.StringToCoTaskMemAnsi(value);
                }

                int newsize = MaxROMSize();
                int length = ROMData.Length;

                if (length < newsize)
                {
                    Array.Resize(ref ROMData, newsize);
                }

                bool success;

                fixed (byte* ptr = ROMData)
                fixed (byte** includepaths = includes)
                fixed (RawAsarDefine* additional_defines = defines)
                {
                    var param = new RawPatchParams
                    {
                        patchloc = PatchLocation,
                        romdata = ptr,
                        buflen = newsize,
                        romlen = &length,

                        should_reset = ShouldReset,
                        includepaths = includepaths,
                        numincludepaths = includes.Length,
                        additional_defines = additional_defines,
                        additional_define_count = defines.Length,
                        stddefinesfile = STDDefineFile,
                        stdincludesfile = STDIncludeFile
                    };
                    param.structsize = Marshal.SizeOf(param);

                    success = _Patch_ex(ref param);
                }

                if (length < newsize)
                {
                    Array.Resize(ref ROMData, length);
                }

                return success;
            }
            finally
            {
                for (int i = 0; i < includes.Length; i++)
                {
                    Marshal.FreeCoTaskMem((IntPtr)includes[i]);
                }

                foreach (var define in defines)
                {
                    Marshal.FreeCoTaskMem(define.name);
                    Marshal.FreeCoTaskMem(define.contents);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RawAsarError
        {
            public IntPtr fullerrdata;
            public IntPtr rawerrdata;
            public IntPtr block;
            public IntPtr filename;
            public int line;
            public IntPtr callerfilename;
            public int callerline;
            public int errid;
        };

        private static Asarerror[] CleanErrors(RawAsarError* ptr, int length)
        {
            Asarerror[] output = new Asarerror[length];

            // Copy unmanaged to managed memory to avoid potential errors in case the area
            // gets cleared by Asar.
            for (int i = 0; i < length; i++)
            {
                output[i].Fullerrdata = Marshal.PtrToStringAnsi(ptr[i].fullerrdata);
                output[i].Rawerrdata = Marshal.PtrToStringAnsi(ptr[i].rawerrdata);
                output[i].Block = Marshal.PtrToStringAnsi(ptr[i].block);
                output[i].Filename = Marshal.PtrToStringAnsi(ptr[i].filename);
                output[i].Line = ptr[i].line;
                output[i].Callerfilename = Marshal.PtrToStringAnsi(ptr[i].callerfilename);
                output[i].Callerline = ptr[i].callerline;
                output[i].ErrorId = ptr[i].errid;
            }

            return output;
        }

        /// <summary>
        /// Gets all Asar current errors. They're safe to keep for as long as you want.
        /// </summary>
        /// <returns>All Asar's errors.</returns>
        public static Asarerror[] GetErrors()
        {
            RawAsarError* ptr = _GetErrors(out int length);
            return CleanErrors(ptr, length);
        }

        /// <summary>
        /// Gets all Asar current warning. They're safe to keep for as long as you want.
        /// </summary>
        /// <returns>All Asar's warnings.</returns>
        public static Asarerror[] GetWarnings()
        {
            RawAsarError* ptr = _GetWarnings(out int length);
            return CleanErrors(ptr, length);
        }

        /// <summary>
        /// Gets all prints generated by the patch
        /// (Note: to see warnings/errors, check getwarnings() and geterrors()
        /// </summary>
        /// <returns>All prints</returns>
        public static string[] GetPrints()
        {
            void** ptr = _GetPrints(out int length);

            string[] output = new string[length];

            for (int i = 0; i < length; i++)
            {
                output[i] = Marshal.PtrToStringAnsi((IntPtr)ptr[i]);
            }

            return output;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RawAsarLabel
        {
            public IntPtr name;
            public int location;
        }

        /// <summary>
        /// Gets all Asar current labels. They're safe to keep for as long as you want.
        /// </summary>
        /// <returns>All Asar's labels.</returns>
        public static Asarlabel[] GetAllLabels()
        {
            RawAsarLabel* ptr = _GetAllLabels(out int length);
            Asarlabel[] output = new Asarlabel[length];

            // Copy unmanaged to managed memory to avoid potential errors in case the area
            // gets cleared by Asar.
            for (int i = 0; i < length; i++)
            {
                output[i].Name = Marshal.PtrToStringAnsi(ptr[i].name);
                output[i].Location = ptr[i].location;
            }

            return output;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RawAsarDefine
        {
            public IntPtr name;
            public IntPtr contents;
        }
        /// <summary>
        /// Gets all Asar current defines. They're safe to keep for as long as you want.
        /// </summary>
        /// <returns>All Asar's defines.</returns>
        public static Asardefine[] GetAllDefines()
        {
            RawAsarDefine* ptr = _GetAllDefines(out int length);
            Asardefine[] output = new Asardefine[length];

            // Copy unmanaged to managed memory to avoid potential errors in case the area
            // gets cleared by Asar.
            for (int i = 0; i < length; i++)
            {
                output[i].Name = Marshal.PtrToStringAnsi(ptr[i].name);
                output[i].Contents = Marshal.PtrToStringAnsi(ptr[i].contents);
            }

            return output;
        }

        /// <summary>
        /// Parse a string of math.
        /// </summary>
        /// <param name="Math">The math string, i.e "1+1"</param>
        /// <param name="Error">If occurs any error, it will showed here.</param>
        /// <returns>Product.</returns>
        public static double Math(string Math, out string Error)
        {
            double value = _Math(Math, out IntPtr err);

            Error = Marshal.PtrToStringAnsi(err);
            return value;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct RawAsarWrittenBlock
        {
            public int pcoffset;
            public int snesoffset;
            public int numbytes;
        };

        private static Asarwrittenblock[] CleanWrittenBlocks(RawAsarWrittenBlock* ptr, int length)
        {
            Asarwrittenblock[] output = new Asarwrittenblock[length];

            // Copy unmanaged to managed memory to avoid potential errors in case the area
            // gets cleared by Asar.
            for (int i = 0; i < length; i++)
            {
                output[i].Snesoffset = ptr[i].snesoffset;
                output[i].Numbytes = ptr[i].numbytes;
                output[i].Pcoffset = ptr[i].pcoffset;
            }

            return output;
        }

        /// <summary>
        /// Gets all Asar blocks written to the ROM. They're safe to keep for as long as you want.
        /// </summary>
        /// <returns>All Asar's blocks written to the ROM.</returns>
        public static Asarwrittenblock[] GetWrittenBlocks()
        {
            RawAsarWrittenBlock* ptr = _GetWrittenBlocks(out int length);
            return CleanWrittenBlocks(ptr, length);
        }
    }

    /// <summary>
    /// Contains full information of a Asar error or warning.
    /// </summary>
    public struct Asarerror
    {
        public string Fullerrdata;
        public string Rawerrdata;
        public string Block;
        public string Filename;
        public int Line;
        public string Callerfilename;
        public int Callerline;
        public int ErrorId;
    }

    /// <summary>
    /// Contains a label from Asar.
    /// </summary>
    public struct Asarlabel
    {
        public string Name;
        public int Location;
    }

    /// <summary>
    /// Contains a Asar define.
    /// </summary>
    public struct Asardefine
    {
        public string Name;
        public string Contents;
    }

    /// <summary>
    /// Contains full information on a block written to the ROM.
    /// </summary>
    public struct Asarwrittenblock
    {
        public int Pcoffset;
        public int Snesoffset;
        public int Numbytes;
    }

    /// <summary>
    /// Defines the ROM mapper used.
    /// </summary>
    public enum MapperType
    {
        /// <summary>
        /// Invalid map.
        /// </summary>
        InvalidMapper,

        /// <summary>
        /// Standard LoROM.
        /// </summary>
        LoRom,

        /// <summary>
        /// Standard HiROM.
        /// </summary>
        HiRom,

        /// <summary>
        /// SA-1 ROM.
        /// </summary>
        Sa1Rom,

        /// <summary>
        /// SA-1 ROM with 8 MB mapped at once.
        /// </summary>
        BigSa1Rom,

        /// <summary>
        /// Super FX ROM.
        /// </summary>
        SfxRom,

        /// <summary>
        /// ExLoROM.
        /// </summary>
        ExLoRom,

        /// <summary>
        /// ExHiROM.
        /// </summary>
        ExHiRom,

        /// <summary>
        /// No specific ROM mapping.
        /// </summary>
        NoRom
    }
}
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

