using System;
using System.Runtime.InteropServices;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MOLE_Back.Libs
{
    /// <summary>
    /// Imports functions from Lunar Compress DLL
    /// </summary>
    public static unsafe class LC
    {
        public const string dllPath = "lunarcompress.dll";

        public const uint
        CMPRS_LZ1 = 0,
        CMPRS_LZ2 = 1,
        CMPRS_LZ3 = 2,
        CMPRS_LZ4 = 3,
        CMPRS_LZ5 = 4,
        CMPRS_LZ6 = 5,
        CMPRS_LZ7 = 6,
        CMPRS_LZ8 = 7,
        CMPRS_LZ9 = 8,
        CMPRS_LZ10 = 9,
        CMPRS_LZ11 = 10,
        CMPRS_LZ12 = 11,
        CMPRS_LZ13 = 12,
        CMPRS_LZ14 = 13,
        CMPRS_LZ15 = 14,
        CMPRS_LZ16 = 15,
        CMPRS_LZ17 = 16,
        CMPRS_LZ18 = 17,

        CMPRS_RLE1 = 100,
        CMPRS_RLE2 = 101,
        CMPRS_RLE3 = 102,
        CMPRS_RLE4 = 103;

        public const uint
        XPAND_48_EXHIROM = 48,
        XPAND_48_EXHIROM_1 = (0x100 | 48),  //Higher compatibility, but uses up to 1 meg of the new space.  Do not use this unless the ROM doesn't load or has problems with the other options.
        XPAND_64_EXHIROM = 64,
        XPAND_64_EXHIROM_1 = (0x100 | 64),  //Higher compatibility, but uses up to 2 meg of the new space.  Do not use this unless the ROM doesn't load or has problems with the other options.
        XPAND_48_EXLOROM_1 = (0x1000 | 48), //For LoROMs that use the 00:8000-6F:FFFF
        XPAND_48_EXLOROM_2 = (0x2000 | 48), //For LoROMs that use the 80:8000-FF:FFFF map.
        XPAND_48_EXLOROM_3 = (0x4000 | 48), //Higher compatibility, but uses up most of the new space.  Do not use this unless the ROM doesn't load or has problems with the other options.
        XPAND_64_EXLOROM_1 = (0x1000 | 64), //For LoROMs that use the 00:8000-6F:FFFF
        XPAND_64_EXLOROM_2 = (0x2000 | 64), //For LoROMs that use the 80:8000-FF:FFFF map.
        XPAND_64_EXLOROM_3 = (0x4000 | 64); //Higher compatibility, but uses up most of the new space.  Do not use this unless the ROM doesn't load or has problems with the other options.

        public const uint
        FILE_READONLY = 0x00,
        FILE_READWRITE = 0x01,
        FILE_CREATEREADWRITE = 0x02,
        FILE_LOCKARRAYSIZE = 0x04,
        FILE_LOCKARRAYSIZE_2 = 0x08,
        FILE_CREATEARRAY = 0x10,
        FILE_SAVEONCLOSE = 0x20;

        public const uint
        NOSEEK = 0,
        SEEK = 1;

        public const uint
        ADDR_NOBANK = 0x00,
        ADDR_LOROM = 0x01, //LoROM
        ADDR_HIROM = 0x02, //HiROM
        ADDR_EXHIROM = 0x04, //Extended HiROM
        ADDR_EXLOROM = 0x08, //Extended LoROM
        ADDR_LOROM_2 = 0x10, //LoROM, always converts to 80:8000 map
        ADDR_HIROM_2 = 0x20, //HiROM, always converts to 40:0000 map up till 70:0000
        ADDR_EXROM = 0x04; //same as LC_EXHIROM (depreciated)


        public const uint 
        NOHEADER = 0,
        HEADER = 1;

        public const uint
        IPS_LOG = 0x80000000,
        IPS_QUIET = 0x40000000,
        IPS_EXTRA_WARNINGS = 0x20000000,
        IPS_FORCEFILE_SAVEAS = 0x10000000;

        public const uint 
        GFX_1BPP = 1,
        GFX_2BPP = 2,
        GFX_3BPP = 3,
        GFX_4BPP = 4,
        GFX_5BPP = 5,
        GFX_6BPP = 6,
        GFX_7BPP = 7,
        GFX_8BPP = 8,
        GFX_4BPP_GBA = 0x14, //unofficial support
        GFX_MODE7_8BPP = 0x28; //set aside for SWR

        static uint 
        RENDER_INVERT_TRANSPARENT = 0x01,
        RENDER_INVERT_OPAQUE = 0x02,
        RENDER_INVERT = (RENDER_INVERT_TRANSPARENT | RENDER_INVERT_OPAQUE),
        RENDER_RED_TRANSPARENT = 0x04,
        RENDER_RED_OPAQUE = 0x08,
        RENDER_RED = (RENDER_RED_TRANSPARENT | RENDER_RED_OPAQUE),
        RENDER_GREEN_TRANSPARENT = 0x10,
        RENDER_GREEN_OPAQUE = 0x20,
        RENDER_GREEN = (RENDER_GREEN_TRANSPARENT | RENDER_GREEN_OPAQUE),
        RENDER_BLUE_TRANSPARENT = 0x40,
        RENDER_BLUE_OPAQUE = 0x80,
        RENDER_BLUE = (RENDER_BLUE_TRANSPARENT | RENDER_BLUE_OPAQUE),
        RENDER_TRANSLUCENT = 0x0100,
        RENDER_HALF_COLOR = 0x0200, //half-color mode
        RENDER_SCREEN_ADD = 0x0400, //sub-screen addition
        RENDER_SCREEN_SUB = 0x0800, //sub-screen subtraction
        RENDER_PRIORITY_0 = 0x1000,
        RENDER_PRIORITY_1 = 0x2000,
        RENDER_PRIORITY_2 = 0x4000,
        RENDER_PRIORITY_3 = 0x8000,
        RENDER_DRAW = (RENDER_PRIORITY_0 | RENDER_PRIORITY_1 | RENDER_PRIORITY_2 | RENDER_PRIORITY_3),
        RENDER_OPAQUE = 0x010000,
        RENDER_SPRITE = 0x020000,
        RENDER_SPRITE_TRANSLUCENT = 0x040000,
        RENDER_2BPP_GFX = 0x080000,
        RENDER_TILE_16 = 0x100000,
        RENDER_TILE_32 = 0x200000,
        RENDER_TILE_64 = 0x400000;

        static uint 
        RATF_FORMAT = 0x000000FF,  //bitmask reserved for LC compressed format (DO NOT USE THIS VALUE AS A FLAG!)
        RATF_LOROM = 0x00000100,  //use LoROM banks
        RATF_HIROM = 0x00000200,  //use HiROM banks
        RATF_EXLOROM = 0x00010000,  //NOT same as RATF_LOROM
        RATF_EXHIROM = 0x00000400,  //NOT same as RATF_HIROM
        RATF_EXROM = 0x00000400,  //same as RATF_EXHIROM (depreciated)
        RATF_COMPRESSED = 0x00000800,  //data to erase is compressed; can decompress to get size using LC format specified
        RATF_NOERASERAT = 0x00001000,  //don't erase RAT tag
        RATF_NOWRITERAT = 0x00002000,  //don't write RAT tag
        RATF_NOERASEDATA = 0x00004000,  //don't erase user data
        RATF_NOWRITEDATA = 0x00008000,  //don't write user data
        RATF_EXPANDROM = 0x00020000,  //expand ROM if necessary (up to 32 mbits)
        RATF_NOSCANDATA = 0x00040000,  //don't scan user data to remove embedded RATs
        RATF_NOWRITE = 0x00080000,  //simulated write, almost like using RATF_NOWRITEDATA and NOWRITERAT, but unlike NOWRITERAT it looks for enough space for the RAT as well
        RATF_EXHIROM_RANGE = 0x00100000,     //just enforce range checks for ExHiROM yet allow crossing banks
        RATF_EXLOROM_RANGE = 0x00200000;  //just enforce range checks for ExLoROM yet allow crossing banks

        /// <summary>
        /// Returns the current version of the DLL as a uint<br/>
        /// For example, version 1.30 of the DLL would return "130".
        /// </summary>
        /// <returns>The current version of the DLL as a uint</returns>
        [DllImport(dllPath, EntryPoint = "LunarVersion", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Version();

        /// <summary>
        /// Open file for access by the DLL.<br/>
        /// If another file is already open, LunarCloseFile() will be used to close it first.<br/>
        /// The DLL does not prevent other applications from reading/writing to the file at the same time.
        /// </summary>
        /// <param name="FileName">Name of File</param>
        /// <param name="FileFlag">Indicates the mode to open the file in.<br/>
        /// FILE_READONLY        : Open existing file in read-only mode (default).<br/>
        /// FILE_READWRITE       : Open existing file in read and write mode.<br/>
        /// FILE_CREATEREADWRITE : Create a new file in read and write mode, erase the existing file(if any).</param>
        /// <returns>True on success, False on fail</returns>
        [DllImport(dllPath, EntryPoint = "LunarOpenFile", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool OpenFile(string FileName, uint FileFlag);

        /// <summary>
        /// "Open" a byte array in RAM for access by the DLL as though it were a file.<br/>
        /// If another file is already open, LunarCloseFile() will be used to close it first.<br/>
        /// You may still modify the array contents directly in your own application while it is "open" in the DLL, provided the array remains in the same memory location.<br/>
        /// The file mode flags will be used to indicate whether or not the DLL is allowed to write to the array.<br/>
        /// Even if you're using your own array, you must still specify one of the file mode flags so the DLL knows if it should treat the array as read-only or not.
        /// </summary>
        /// <param name="data">Byte array to "open"</param>
        /// <param name="FileFlag">indicates the mode to open the file in, plus optional flags.<br/>
        /// The modes and flags available are:<br/>
        /// FILE_READONLY        : Open existing file in read-only mode(default).<br/>
        /// FILE_READWRITE       : Open existing file in read and write mode.<br/>
        /// FILE_CREATEREADWRITE : Create a new file in read and write mode, erase the existing file(if any).<br/>
        /// FILE_LOCKARRAYSIZE   : Prevent the DLL from ever reading/writing past the end of the array. ROM expansion functions will have no effect.<br/>
        /// FILE_LOCKARRAYSIZE_2 : Prevent the DLL from reading/writing past the end of the array except for the LunarExpandROM function.</param>
        /// <param name="size">Size of the user-supplied array.</param>
        /// <returns>Pointer to array on success, 0 on fail</returns>
        [DllImport(dllPath, EntryPoint = "LunarOpenRAMFile", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenRAMFile(byte[] data, uint FileFlag, uint size);

        /// <summary>
        /// Load a file into RAM as a byte array.<br/>
        /// MUST PASS THE FILE_CREATEARRAY FLAG to instruct the DLL to create its own array and load a file into it<br/>
        /// The size variable will be used to indicate the minimum ize of the array to allocate... if the file size is larger than this, the size variable is ignored.<br/>
        /// This is only useful if you want to use the ROM expansion function later to increase the file size up to the size you requested.<br/>
        /// Note that the file itself will be kept open in the mode specified until you call the CloseFile() function.<br/>
        /// (Which makes this function rather useless as it is more optimal to load the file to an array yourself, close the file, and use the other version of the function by passing the array itself)<br/>
        /// If you want the contents of the array to be saved back to the file, you can call SaveRAMFile().  Or you can specify the FILE_SAVEONCLOSE option in OpenRAMFile() to save the file automatically when LunarCloseFile() is called.
        /// </summary>
        /// <param name="FileName">Name of the file to open.</param>
        /// <param name="FileFlag">indicates the mode to open the file in, plus optional flags.<br/>
        /// The modes and flags available are:<br/>
        /// FILE_READONLY        : Open existing file in read-only mode(default).<br/>
        /// FILE_READWRITE       : Open existing file in read and write mode.<br/>
        /// FILE_CREATEREADWRITE : Create a new file in read and write mode, erase the existing file(if any).<br/>
        /// FILE_LOCKARRAYSIZE   : Prevent the DLL from ever reading/writing past the end of the array. ROM expansion functions will have no effect.<br/>
        /// FILE_LOCKARRAYSIZE_2 : Prevent the DLL from reading/writing past the end of the array except for the ExpandROM function.<br/>
        /// FILE_CREATEARRAY (REQUIRED) : Instructs the DLL to create its own array and load a file into it using the specified file mode.<br/>
        /// FILE_SAVEONCLOSE     : Automatically save the contents of the byte array back to the file when CloseFile() is called. This has no effect if the file was opened in read-only mode.</param>
        /// <param name="size">The minimum size of the array to be allocated.</param>
        /// <returns>Pointer to array on success, 0 on fail</returns>
        [DllImport(dllPath, EntryPoint = "LunarOpenRAMFile", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenRAMFile(string FileName, uint FileFlag, uint size);

        /// <summary>
        /// Saves the currently open byte array in RAM to a file (see OpenRAMFile() for how to open a byte array as a file).<br/>
        /// If you used the FILE_CREATEARRAY option in OpenRAMFile(), the file name is optional.<br/>
        /// If you specify a file name, the old file will be closed and a new file will be created with the contents of the RAM array.<br/>
        /// Note that the FILE_SAVEONCLOSE option for OpenRAMFile() will NOT cause the array contents to be written to the old file in this case.<br/>
        /// The new file will then remain open in the same mode used to open the old file until CloseFile() is called.<br/>
        /// If you do not specify a file name and the original file was opened in read-only mode, the function will not allow you to save the RAM array to the file!<br/>
        /// If you did not use the FILE_CREATEARRAY option in OpenRAMFile(), a file name is required.<br/>
        /// Note however that if you used your own array, the DLL will not retain ownership of the created file.<br/>
        /// Ie, the file will immediately be closed once the save is complete and subsequent calls to this function will still require a file name.<br/>
        /// If you specify the name of a file that already exists, it will be overwritten.
        /// </summary>
        /// <param name="FileName">Path and name of the file to save as.<br/>
        /// You can set this to NULL if you used the FILE_CREATEARRAY flag in OpenRAMFile and you wish to save to the same file you opened, but only if it was not opened in read-only mode.</param>
        /// <returns>Non-zero on success, 0 on fail.</returns>
        [DllImport(dllPath, EntryPoint = "LunarSaveRAMFile", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SaveRAMFile(string FileName);

        /// <summary>
        /// Closes the file or RAM byte array currently open in the DLL.
        /// </summary>
        /// <returns>True on success, False on fail.</returns>
        [DllImport(dllPath, EntryPoint = "LunarCloseFile", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool CloseFile();

        /// <summary>
        /// Returns the size of the file that is currently open in the DLL in bytes.
        /// </summary>
        /// <returns>The size of the file that is currently open in bytes, 0 on fail</returns>
        [DllImport(dllPath, EntryPoint = "LunarGetFileSize", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetFileSize();

        /// <summary>
        /// Reads data from the currently open file into a byte array.
        /// </summary>
        /// <param name="Destination">Destination byte array.</param>
        /// <param name="Size">Number of bytes to read.</param>
        /// <param name="Address">File offset to get data from.</param>
        /// <param name="Seek">LC_NOSEEK or LC_SEEK.</param>
        /// <returns>Non-zero on success, 0 on fail.</returns>
        [DllImport(dllPath, EntryPoint = "LunarReadFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ReadFile(byte[] Destination, uint Size, uint Address, uint Seek);

        /// <summary>
        /// Writes data from a byte array to the currently open file.
        /// </summary>
        /// <param name="Source">Source byte array.</param>
        /// <param name="Size">Number of bytes to read.</param>
        /// <param name="Address">File offset to get data from.</param>
        /// <param name="Seek">LC_NOSEEK or LC_SEEK.</param>
        /// <returns>Non-zero on success, 0 on fail.</returns>
        [DllImport(dllPath, EntryPoint = "LunarWriteFile", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint WriteFile(byte[] Source, uint Size, uint Address, uint Seek);

        /// <summary>
        /// Changes the bytes used for scanning for free space and erasing data.<br/>
        /// This function affects ROM expansion and erasing, even in RAT functions.<br/>
        /// It allows you to set up to 2 different bytes to check for when scanning for free space, and the byte to use when expanding the ROM or erasing an area.<br/>
        /// For example, if you wanted to use both 00 and FF to represent free space and 00 to erase data, you would pass 0xFF.<br/>
        /// To only use FF to represent free space and to erase data, you would pass 0xFFFFFF.<br/>
        /// You should set the byte to use for erasing to the same value as one of the 2 free space bytes, for obvious reasons.<br/>
        /// When the DLL is first loaded, all 3 bytes are set to 0 by default.
        /// </summary>
        /// <param name="value">lower 8 bits are for free space byte 1,<br/>
        /// next 8 bits are for free space byte 2,<br/>
        /// and next 8 bits are for byte to use for erasing.</param>
        /// <returns>Non-zero on success, 0 on fail.</returns>
        [DllImport(dllPath, EntryPoint = "LunarSetFreeBytes", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SetFreeBytes(uint value);

        /// <summary>
        /// Converts a SNES ROM Address to a PC file offset.<br/>
        /// Do NOT specify an ExROM type if your ROM is not larger than 32 Mbit!<br/>
        /// For 64Mbit Headered ExLoROMs you should not store anything past $7F:01FF<br/>
        /// For 64Mbit Headered ExHiROMs you should not store anything in the ranges $7E:0200-7E:81FF and $7F:0200-7F:81FF aswell as the area from 0000-7FFF of banks $70 - $77
        /// </summary>
        /// <param name="Pointer">SNES address to convert.</param>
        /// <param name="ROMType">One of the following values:<br/>
        /// LC_LOROM   : 00-32 Mbit LoROM ROMs (0-4 MB)<br/>
        /// LC_LOROM_2 : Same as above<br/>
        /// LC_HIROM   : 00-32 Mbit HiROM ROMs (0-4 MB)<br/>
        /// LC_HIROM_2 : Same as above<br/>
        /// LC_EXLOROM : 33-64 Mbit LoROM ROMs (4.x-8 MB)<br/>
        /// LC_EXHIROM : 33-64 Mbit HiROM ROMs (4.x-8 MB)</param>
        /// <param name="Header">LC_NOHEADER or LC_HEADER (0x200 bytes)<br/>
        /// (The header will be included in the return value)</param>
        /// <returns>The PC file offset of the SNES ROM address, Undefined value for non-ROM addresses.</returns>
        [DllImport(dllPath, EntryPoint = "LunarSNEStoPC", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SNEStoPC(uint Pointer, uint ROMType, uint Header);

        /// <summary>
        /// Converts PC file offset to SNES ROM address.<br/>
        /// Do NOT specify an ExROM type if your ROM is not larger than 32 Mbit!<br/>
        /// For 64Mbit Headered ExLoROMs you should not store anything past $7F:01FF<br/>
        /// For 64Mbit Headered ExHiROMs you should not store anything in the ranges $7E:0200-7E:81FF and $7F:0200-7F:81FF aswell as the area from 0000-7FFF of banks $70 - $77
        /// </summary>
        /// <param name="Pointer">PC file offset to convert.</param>
        /// <param name="ROMType">One of the following values:<br/>
        /// LC_LOROM   : 00-32 Mbit LoROM ROMs (0-4 MB) (Only uses 80:8000 map for offsets >= 0x38000)<br/>
        /// LC_LOROM_2 : Same as above but always uses 80:8000 map. (Safer)<br/>
        /// LC_HIROM   : 00-32 Mbit HiROM ROMs (0-4 MB)<br/>
        /// LC_HIROM_2 : Same as above but always uses 40:0000 map up to 70:0000, then uses the C0:0000 map. (Safer)<br/>
        /// LC_EXLOROM : 33-64 Mbit LoROM ROMs (4.x-8 MB)<br/>
        /// LC_EXHIROM : 33-64 Mbit HiROM ROMs (4.x-8 MB)</param>
        /// <param name="Header">LC_NOHEADER or LC_HEADER (0x200 bytes)</param>
        /// <returns>The SNES ROM address of the PC file offset,  Undefined value for non-ROM addresses.</returns>
        [DllImport(dllPath, EntryPoint = "LunarPCtoSNES", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint PCtoSNES(uint Pointer, uint ROMType, uint Header);

        /// <summary>
        /// Decompress data from the currently open file into an array.
        /// If the size of the decompressed data is greater than MaxDataSize, the data will be truncated to fit into the array.<br/>
        /// Note however that the size value returned by the function will not be the truncated size.<br/>
        /// If Destination=NULL and/or MaxDataSize=0, no data will be copied to the array but the function will still decompress the data so it can return the size and store the LastROMPosition.<br/>
        /// In general, a max limit of 0x10000 bytes is supported for the uncompressed data, which is the size of a HiROM SNES bank.A few formats may have lower limits depending on their design.
        /// </summary>
        /// <param name="Destination">Destination byte array for decompressed data.</param>
        /// <param name="AddressToStart">File offset to start at.</param>
        /// <param name="MaxDataSize">Maximum number of bytes to copy into dest.</param>
        /// <param name="Format">Compression format (see table at https://pastebin.com/DABVHUeW ).</param>
        /// <param name="Format2">Must be zero unless otherwise stated by the Compression format table</param>
        /// <param name="LastROMPosition">An optional pointer that the function will use to store the file offset of the next byte that comes after the compressed data.<br/>
        /// This could be used to calculate the size of the compressed data after calling the function, using the simple formula LastROMPosition-AddressToStart.<br/>
        /// You can pass NULL if you don't needthis value.</param>
        /// <returns>The size of the decompressed data.  A value of zero indicates either failure or a decompressed structure of size 0.</returns>
        [DllImport(dllPath, EntryPoint = "LunarDecompress", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Decompress(byte[] Destination, uint AddressToStart, uint MaxDataSize, uint Format, uint Format2, uint* LastROMPosition);

        [DllImport(dllPath, EntryPoint = "LunarRecompress", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Recompress(byte[] Source, byte[] Destination, uint DataSize, uint MaxDataSize, uint Formta, uint Format2);

        [DllImport(dllPath, EntryPoint = "LunarEraseArea", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool EraseArea(uint Address, uint Size);

        [DllImport(dllPath, EntryPoint = "LunarExpandROM", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ExpandROM(uint Mbits);

        [DllImport(dllPath, EntryPoint = "LunarVerifyFreeSpace", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint VerifyFreeSpace(uint AddressStart, uint AddressEnd, uint Size, uint BankType);

        [DllImport(dllPath, EntryPoint = "LunarIPSCreate", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint IPSCreate(IntPtr hwnd, string IPSFileName, string CleanROMFileName, string NewROMFileName, uint Flags);

        [DllImport(dllPath, EntryPoint = "LunarIPSApply", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint IPSApply(IntPtr hwnd, string IPSFileName, string ROMFileName, uint Flags);

        [DllImport(dllPath, EntryPoint = "LunarCreatePixelMap", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool CreatePixelMap(byte[] Source, byte[] Destination, uint NumTiles, uint GFXType);

        [DllImport(dllPath, EntryPoint = "LunarCreateBppMap", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool CreateBppMap(byte[] Source, byte[] Destination, uint NumTiles, uint GFXType);

        [DllImport(dllPath, EntryPoint = "LunarSNEStoPCRGB", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SNEStoPCRGB(uint SNESColor);

        [DllImport(dllPath, EntryPoint = "LunarPCtoSNESRGB", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint PCtoSNESRGB(uint PCColor);

        [DllImport(dllPath, EntryPoint = "LunarRender8x8", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Render8x8(int TheMapBits, int TheWidth, int TheHeight, int DisplayAtX, int DisplayAtY, byte[] PixelMap, uint* PCPalette, uint Map8Tile, uint Extra);

        [DllImport(dllPath, EntryPoint = "LunarWriteRatArea", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint WriteRatArea(void* TheData, uint Size, uint PreferredAddress, uint MinRange, uint MaxRange, uint Flags);

        [DllImport(dllPath, EntryPoint = "LunarEraseRatArea", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint EraseRatArea(uint Address, uint Size, uint Flags);

        [DllImport(dllPath, EntryPoint = "LunarGetRatAreaSize", CharSet=CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetRatAreaSize(uint Address, uint Flags);
    }
}
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member