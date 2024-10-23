using System;
using System.IO;
using System.Runtime.InteropServices;

public class platformdirs
{
    private string _appName;
    private string _appAuthor;

    public PlatformDirs(string appName = null, string appAuthor = null)
    {
        _appName = appName;
        _appAuthor = appAuthor;
    }

    public string UserDataDir()
    {
        string path;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _appName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", _appName);
        }
        else // Linux and other Unix-like OS
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", _appName);
        }
        return path;
    }

    public string UserConfigDir()
    {
        string path;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _appName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", _appName);
        }
        else // Linux and other Unix-like OS
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", _appName);
        }
        return path;
    }

    public string UserCacheDir()
    {
        string path;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Cache", _appName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Caches", _appName);
        }
        else // Linux and other Unix-like OS
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", _appName);
        }
        return path;
    }

    public string UserDocumentsDir()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    public string UserDownloadsDir()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        }
        else // macOS, Linux, and other Unix-like OS
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        }
    }

    /*user_downloads_dir
    user_pictures_dir
    user_videos_dir
    user_music_dir
    user_runtime_dir
    site_data_dir
    site_config_dir
    site_cache_dir
    iter_data_dirs
    iter_cache_dirs*/

    public string PlatformDirs()
    {
        // Currently active platform
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "Windows";
        }
        elif(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "macOS / Darwin";
        }
        else
        {
            return "Linux";
        }
    }

    public string windows()
    {

    }





    platformdirs.windows.user_data_dir // %USERPROFILE%\AppData\Local\$appauthor\$appname or %USERPROFILE%\AppData\Roaming\$appauthor\$appname
    platformdirs.windows.site_data_dir // C:\ProgramData\$appauthor\$appname
}


// var dirs = new PlatformDirs("MyApp", "MyCompany");
// Console.WriteLine($"User data directory: {dirs.UserDataDir()}");
// Console.WriteLine($"User config directory: {dirs.UserConfigDir()}");
// Console.WriteLine($"User cache directory: {dirs.UserCacheDir()}");
// Console.WriteLine($"User documents directory: {dirs.UserDocumentsDir()}");
// Console.WriteLine($"User downloads directory: {dirs.UserDownloadsDir()}");
