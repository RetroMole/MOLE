using System;
using System.Runtime.InteropServices;

namespace SDL2
{
    public static class SDL
    {
        const string nativeLibName = "SDL2";
        /* window refers to an SDL_Window*, icon to an SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_SetWindowIcon(
            IntPtr window,
            IntPtr icon
        );

		/* These are for SDL_LoadBMP, which is a macro in the SDL headers. */
		/* IntPtr refers to an SDL_Surface* */
		/* THIS IS AN RWops FUNCTION! */
		[DllImport(nativeLibName, EntryPoint = "SDL_LoadBMP_RW", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr INTERNAL_SDL_LoadBMP_RW(
			IntPtr src,
			int freesrc
		);
		public static IntPtr SDL_LoadBMP(string file)
		{
			IntPtr rwops = SDL_RWFromFile(file, "rb");
			return INTERNAL_SDL_LoadBMP_RW(rwops, 1);
		}

		/* IntPtr refers to an SDL_RWops* */
		[DllImport(nativeLibName, EntryPoint = "SDL_RWFromFile", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr INTERNAL_SDL_RWFromFile(
			byte[] file,
			byte[] mode
		);
		public static IntPtr SDL_RWFromFile(
			string file,
			string mode
		)
		{
			return INTERNAL_SDL_RWFromFile(
				UTF8_ToNative(file),
				UTF8_ToNative(mode)
			);
		}
		internal static byte[] UTF8_ToNative(string s)
		{
			if (s == null)
			{
				return null;
			}

			// Add a null terminator. That's kind of it... :/
			return System.Text.Encoding.UTF8.GetBytes(s + '\0');
		}

		/* This is public because SDL_DropEvent needs it! */
		public static unsafe string UTF8_ToManaged(IntPtr s, bool freePtr = false)
		{
			if (s == IntPtr.Zero)
			{
				return null;
			}

			/* We get to do strlen ourselves! */
			byte* ptr = (byte*)s;
			while (*ptr != 0)
			{
				ptr++;
			}

			/* TODO: This #ifdef is only here because the equivalent
			 * .NET 2.0 constructor appears to be less efficient?
			 * Here's the pretty version, maybe steal this instead:
			 *
			string result = new string(
				(sbyte*) s, // Also, why sbyte???
				0,
				(int) (ptr - (byte*) s),
				System.Text.Encoding.UTF8
			);
			 * See the CoreCLR source for more info.
			 * -flibit
			 */
#if NETSTANDARD2_0
			/* Modern C# lets you just send the byte*, nice! */
			string result = System.Text.Encoding.UTF8.GetString(
				(byte*) s,
				(int) (ptr - (byte*) s)
			);
#else
			/* Old C# requires an extra memcpy, bleh! */
			int len = (int)(ptr - (byte*)s);
			if (len == 0)
			{
				return string.Empty;
			}
			char* chars = stackalloc char[len];
			int strLen = System.Text.Encoding.UTF8.GetChars((byte*)s, len, chars, len);
			string result = new string(chars, 0, strLen);
#endif

			/* Some SDL functions will malloc, we have to free! */
			if (freePtr)
			{
				SDL_free(s);
			}
			return result;
		}

		[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SDL_free(IntPtr memblock);
	}
}
