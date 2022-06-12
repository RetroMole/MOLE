namespace RetroMole.Core.Utility;

public static class CommonDirectories
{
    public static string Home => (Environment.OSVersion.Platform == PlatformID.Unix
                || Environment.OSVersion.Platform == PlatformID.MacOSX)
                ?  Environment.GetEnvironmentVariable("HOME")                      // *nix systems have a HOME env var
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"); // windows systems have various env vars that may point to a "home" directory but this is the safest
    public static string Temp => Path.GetTempPath(); // checks various env vars on both *nix and windows and gets the first one that matches
    public static string Exec => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); // y e s.
}
