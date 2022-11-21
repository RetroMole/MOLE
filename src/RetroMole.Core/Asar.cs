#pragma warning disable CS0618, CS8625, CS8601

using System.Runtime.InteropServices;
using Serilog;

namespace RetroMole.Core;

/// <summary>
///     Contains various functions to apply patches.
/// </summary>
public static unsafe class Asar
{
    public const string DllPath = "asar";
    public const int ExpectedApiVersion = 303;

    /// <summary>
    ///     Converts Asar version to string
    /// </summary>
    /// <param name="ver">Version</param>
    /// <returns>String</returns>
    public static string Ver2Str(int ver)
    {
        //major*10000+minor*100+bugfix*1.
        //123456 = 12.3456
        var maj = ver / 10000;
        var min = (ver - maj * 10000) / 100;
        var fx = (ver - (maj * 10000 + min * 100)) / 1;
        return $"{maj}.{min}{fx}";
    }

    /// <summary>
    ///     Initializes Asar, should be done before doing anything.
    /// </summary>
    /// <returns>True if success</returns>
    [DllImport(DllPath, EntryPoint = "asar_init", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool AsarInit();

    /// <summary>
    ///     Closes Asar, should be done after finishing.
    /// </summary>
    /// <returns>True if success</returns>
    [DllImport(DllPath, EntryPoint = "asar_close", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool Close();

    /// <summary>
    ///     Returns the version, in the format major*10000+minor*100+bugfix*1.
    ///     This means that 1.2.34 would be returned as 10234.
    /// </summary>
    /// <returns>Asar version</returns>
    [DllImport(DllPath, EntryPoint = "asar_version", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    public static extern int Version();

    /// <summary>
    ///     Returns the API version, format major*100+minor. Minor is incremented on backwards compatible
    ///     changes; major is incremented on incompatible changes. Does not have any correlation with the
    ///     Asar version.
    ///     It's not very useful directly, since Asar.init() verifies this automatically.
    /// </summary>
    /// <returns>Asar API version</returns>
    [DllImport(DllPath, EntryPoint = "asar_apiversion", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    public static extern int ApiVersion();

    /// <summary>
    ///     Clears out all errors, warnings and printed statements, and clears the file cache.
    ///     Not useful for much, since patch() already does this.
    /// </summary>
    /// <returns>True if success</returns>
    [DllImport(DllPath, EntryPoint = "asar_reset", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool Reset();

    [DllImport(DllPath, EntryPoint = "asar_patch", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool _Patch(string patchLocation, byte* romData, int bufLen, int* romLength);

    [DllImport(DllPath, EntryPoint = "asar_patch_ex", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool _Patch_ex(ref RawPatchParams parameters);

    /// <summary>
    ///     Returns the maximum possible size of the output ROM from asar_patch().
    ///     Giving this size to buflen guarantees you will not get any buffer too small errors;
    ///     however, it is safe to give smaller buffers if you don't expect any ROMs larger
    ///     than 4MB or something. It's not very useful directly, since Asar.patch() uses this automatically.
    /// </summary>
    /// <returns>Maximum output size of the ROM.</returns>
    [DllImport(DllPath, EntryPoint = "asar_maxromsize", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    public static extern int MaxROMSize();

    [DllImport(DllPath, EntryPoint = "asar_geterrors", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern RawAsarError* _GetErrors(out int length);

    [DllImport(DllPath, EntryPoint = "asar_getwarnings", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern RawAsarError* _GetWarnings(out int length);

    [DllImport(DllPath, EntryPoint = "asar_getprints", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern void** _GetPrints(out int length);

    [DllImport(DllPath, EntryPoint = "asar_getalllabels", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern RawAsarLabel* _GetAllLabels(out int length);

    [DllImport(DllPath, EntryPoint = "asar_getlabelval", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int GetLabelVal(string labelName);

    /// <summary>
    ///     Gets contents of a define. If define doesn't exists, a null string will be generated.
    /// </summary>
    /// <param name="defineName">The define name.</param>
    /// <returns>The define content. If define has not found, this will be null.</returns>
    [DllImport(DllPath, EntryPoint = "asar_getdefine", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.AnsiBStr)]
    public static extern string GetDefine(string defineName);

    [DllImport(DllPath, EntryPoint = "asar_getalldefines", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern RawAsarDefine* _GetAllDefines(out int length);

    [DllImport(DllPath, EntryPoint = "asar_resolvedefines", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.AnsiBStr)]
    public static extern string ResolveDefines(string data, bool learnNew);

    [DllImport(DllPath, EntryPoint = "asar_math", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern double AsarMath(string math, out IntPtr error);

    [DllImport(DllPath, EntryPoint = "asar_getwrittenblocks", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern RawAsarWrittenBlock* _GetWrittenBlocks(out int length);

    /// <summary>
    ///     Gets mapper currently used by Asar.
    /// </summary>
    /// <returns>Returns mapper currently used by Asar.</returns>
    [DllImport(DllPath, EntryPoint = "asar_getmapper", CharSet = CharSet.Ansi,
        CallingConvention = CallingConvention.Cdecl)]
    public static extern MapperType GetMapper();

    /// <summary>
    ///     Generates the contents of a symbols file for in a specific format.
    /// </summary>
    /// <param name="format">The symbol file format to generate</param>
    /// <returns>Returns the textual contents of the symbols file.</returns>
    [DllImport(DllPath, EntryPoint = "asar_getsymbolsfile", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.AnsiBStr)]
    public static extern string GetSymbolsFile(string format = "wla");

    /// <summary>
    ///     Loads and initializes the DLL. You must call this before using any other Asar function.
    /// </summary>
    /// <returns>True if success</returns>
    public static bool Init()
    {
        try
        {
            if (ApiVersion() < ExpectedApiVersion || ApiVersion() / 100 > ExpectedApiVersion / 100)
            {
                Log.Error("[ASAR] Expected ApiVersion mismatch!");
                return false;
            }

            return AsarInit();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Applies a patch.
    /// </summary>
    /// <param name="patchLocation">The patch location.</param>
    /// <param name="romData">The rom data. It must not be headered.</param>
    /// <param name="includePaths">lists additional include paths</param>
    /// <param name="shouldReset">
    ///     specifies whether asar should clear out all defines, labels, etc from the last inserted file.<br />
    ///     Setting it to False will make Asar act like the currently patched file was directly appended to the previous one.
    /// </param>
    /// <param name="additionalDefines">specifies extra defines to give to the patch</param>
    /// <param name="stdIncludeFile">path to a file that specifes additional include paths</param>
    /// <param name="stdDefineFile">path to a file that specifes additional defines</param>
    /// <returns>True if no errors.</returns>
    public static bool Patch(string patchLocation, ref byte[] romData, string[] includePaths = null,
        bool shouldReset = true, Dictionary<string, string> additionalDefines = null,
        string stdIncludeFile = null, string stdDefineFile = null)
    {
        if (includePaths == null) includePaths = Array.Empty<string>();
        if (additionalDefines == null) additionalDefines = new Dictionary<string, string>();

        var includes = new byte*[includePaths.Length];
        var defines = new RawAsarDefine[additionalDefines.Count];

        try
        {
            for (var i = 0; i < includePaths.Length; i++)
                includes[i] = (byte*)Marshal.StringToCoTaskMemAnsi(includePaths[i]);

            var keys = additionalDefines.Keys.ToArray();

            for (var i = 0; i < additionalDefines.Count; i++)
            {
                var name = keys[i];
                var value = additionalDefines[name];
                defines[i].name = Marshal.StringToCoTaskMemAnsi(name);
                defines[i].contents = Marshal.StringToCoTaskMemAnsi(value);
            }

            var newsize = MaxROMSize();
            var length = romData.Length;

            if (length < newsize) Array.Resize(ref romData, newsize);

            bool success;

            fixed (byte* ptr = romData)
            fixed (byte** includepaths = includes)
            fixed (RawAsarDefine* additionalDefines2 = defines)
            {
                var param = new RawPatchParams
                {
                    patchloc = patchLocation,
                    romdata = ptr,
                    buflen = newsize,
                    romlen = &length,

                    should_reset = shouldReset,
                    includepaths = includepaths,
                    numincludepaths = includes.Length,
                    additional_defines = additionalDefines2,
                    additional_define_count = defines.Length,
                    stddefinesfile = stdDefineFile,
                    stdincludesfile = stdIncludeFile
                };
                param.structsize = Marshal.SizeOf(param);

                success = _Patch_ex(ref param);
            }

            if (length < newsize) Array.Resize(ref romData, length);

            return success;
        }
        finally
        {
            for (var i = 0; i < includes.Length; i++) Marshal.FreeCoTaskMem((IntPtr)includes[i]);

            foreach (var define in defines)
            {
                Marshal.FreeCoTaskMem(define.name);
                Marshal.FreeCoTaskMem(define.contents);
            }

            if (GetPrints().Length > 0)
            {
                Log.Information("[ASAR] Prints:");
                foreach (var p in GetPrints()) Log.Information("[ASAR]: {0}", p);
            }

            if (GetWarnings().Length > 0)
            {
                Log.Information("[ASAR] Warnings:");
                foreach (var w in GetWarnings()) Log.Warning("[ASAR]: {0}", w.Fullerrdata);
            }

            if (GetErrors().Length > 0)
            {
                Log.Information("[ASAR] Errors:");
                foreach (var e in GetErrors()) Log.Error("[ASAR]: {0}", e.Fullerrdata);
            }
        }
    }

    private static Asarerror[] CleanErrors(RawAsarError* ptr, int length)
    {
        var output = new Asarerror[length];

        // Copy unmanaged to managed memory to avoid potential errors in case the area
        // gets cleared by Asar.
        for (var i = 0; i < length; i++)
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
    ///     Gets all Asar current errors. They're safe to keep for as long as you want.
    /// </summary>
    /// <returns>All Asar's errors.</returns>
    public static Asarerror[] GetErrors()
    {
        var ptr = _GetErrors(out var length);
        return CleanErrors(ptr, length);
    }

    /// <summary>
    ///     Gets all Asar current warning. They're safe to keep for as long as you want.
    /// </summary>
    /// <returns>All Asar's warnings.</returns>
    public static Asarerror[] GetWarnings()
    {
        var ptr = _GetWarnings(out var length);
        return CleanErrors(ptr, length);
    }

    /// <summary>
    ///     Gets all prints generated by the patch
    ///     (Note: to see warnings/errors, check getwarnings() and geterrors()
    /// </summary>
    /// <returns>All prints</returns>
    public static string[] GetPrints()
    {
        var ptr = _GetPrints(out var length);

        var output = new string[length];

        for (var i = 0; i < length; i++) output[i] = Marshal.PtrToStringAnsi((IntPtr)ptr[i]);

        return output;
    }

    /// <summary>
    ///     Gets all Asar current labels. They're safe to keep for as long as you want.
    /// </summary>
    /// <returns>All Asar's labels.</returns>
    public static Asarlabel[] GetAllLabels()
    {
        var ptr = _GetAllLabels(out var length);
        var output = new Asarlabel[length];

        // Copy unmanaged to managed memory to avoid potential errors in case the area
        // gets cleared by Asar.
        for (var i = 0; i < length; i++)
        {
            output[i].Name = Marshal.PtrToStringAnsi(ptr[i].name);
            output[i].Location = ptr[i].location;
        }

        return output;
    }

    /// <summary>
    ///     Gets all Asar current defines. They're safe to keep for as long as you want.
    /// </summary>
    /// <returns>All Asar's defines.</returns>
    public static Asardefine[] GetAllDefines()
    {
        var ptr = _GetAllDefines(out var length);
        var output = new Asardefine[length];

        // Copy unmanaged to managed memory to avoid potential errors in case the area
        // gets cleared by Asar.
        for (var i = 0; i < length; i++)
        {
            output[i].Name = Marshal.PtrToStringAnsi(ptr[i].name);
            output[i].Contents = Marshal.PtrToStringAnsi(ptr[i].contents);
        }

        return output;
    }

    /// <summary>
    ///     Parse a string of math.
    /// </summary>
    /// <param name="math">The math string, i.e "1+1"</param>
    /// <param name="error">If occurs any error, it will showed here.</param>
    /// <returns>Product.</returns>
    public static double Math(string math, out string error)
    {
        var value = AsarMath(math, out var err);

        error = Marshal.PtrToStringAnsi(err);
        return value;
    }

    private static Asarwrittenblock[] CleanWrittenBlocks(RawAsarWrittenBlock* ptr, int length)
    {
        var output = new Asarwrittenblock[length];

        // Copy unmanaged to managed memory to avoid potential errors in case the area
        // gets cleared by Asar.
        for (var i = 0; i < length; i++)
        {
            output[i].Snesoffset = ptr[i].snesoffset;
            output[i].Numbytes = ptr[i].numbytes;
            output[i].Pcoffset = ptr[i].pcoffset;
        }

        return output;
    }

    /// <summary>
    ///     Gets all Asar blocks written to the ROM. They're safe to keep for as long as you want.
    /// </summary>
    /// <returns>All Asar's blocks written to the ROM.</returns>
    public static Asarwrittenblock[] GetWrittenBlocks()
    {
        var ptr = _GetWrittenBlocks(out var length);
        return CleanWrittenBlocks(ptr, length);
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
        [MarshalAs(UnmanagedType.I1)] public bool should_reset;
        public RawAsarDefine* additional_defines;
        public int additional_define_count;
        public string stdincludesfile;
        public string stddefinesfile;
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct RawAsarError
    {
        public readonly IntPtr fullerrdata;
        public readonly IntPtr rawerrdata;
        public readonly IntPtr block;
        public readonly IntPtr filename;
        public readonly int line;
        public readonly IntPtr callerfilename;
        public readonly int callerline;
        public readonly int errid;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RawAsarLabel
    {
        public readonly IntPtr name;
        public readonly int location;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RawAsarDefine
    {
        public IntPtr name;
        public IntPtr contents;
    }


    [StructLayout(LayoutKind.Sequential)]
    private struct RawAsarWrittenBlock
    {
        public readonly int pcoffset;
        public readonly int snesoffset;
        public readonly int numbytes;
    }
}

/// <summary>
///     Contains full information of a Asar error or warning.
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
///     Contains a label from Asar.
/// </summary>
public struct Asarlabel
{
    public string Name;
    public int Location;
}

/// <summary>
///     Contains a Asar define.
/// </summary>
public struct Asardefine
{
    public string Name;
    public string Contents;
}

/// <summary>
///     Contains full information on a block written to the ROM.
/// </summary>
public struct Asarwrittenblock
{
    public int Pcoffset;
    public int Snesoffset;
    public int Numbytes;
}

/// <summary>
///     Defines the ROM mapper used.
/// </summary>
public enum MapperType
{
    /// <summary>
    ///     Invalid map.
    /// </summary>
    InvalidMapper,

    /// <summary>
    ///     Standard LoROM.
    /// </summary>
    LoRom,

    /// <summary>
    ///     Standard HiROM.
    /// </summary>
    HiRom,

    /// <summary>
    ///     SA-1 ROM.
    /// </summary>
    Sa1Rom,

    /// <summary>
    ///     SA-1 ROM with 8 MB mapped at once.
    /// </summary>
    BigSa1Rom,

    /// <summary>
    ///     Super FX ROM.
    /// </summary>
    SfxRom,

    /// <summary>
    ///     ExLoROM.
    /// </summary>
    ExLoRom,

    /// <summary>
    ///     ExHiROM.
    /// </summary>
    ExHiRom,

    /// <summary>
    ///     No specific ROM mapping.
    /// </summary>
    NoRom
}

#pragma warning restore CS0618, CS8625, CS8601