using System;
using System.IO;
using System.Runtime.InteropServices;

public class PlatformDirs
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

    public string UserPicturesDir()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    }

    public string UserVideosDir()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
    }

    public string UserMusicDir()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
    }

    public string UserRuntimeDir()
    {
        string path;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Runtime", _appName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Runtime", _appName);
        }
        else // Linux and other Unix-like OS
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", "runtime", _appName);
        }
        return path;
    }

    public string SiteDataDir()
    {
        string path;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), _appName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            path = Path.Combine("/Library/Application Support", _appName);
        }
        else // Linux and other Unix-like OS
        {
            path = Path.Combine("/usr/local/share", _appName);
        }
        return path;
    }

    public string SiteConfigDir()
    {
        string path;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), _appName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            path = Path.Combine("/Library/Application Support", _appName);
        }
        else // Linux and other Unix-like OS
        {
            path = Path.Combine("/etc/xdg", _appName);
        }
        return path;
    }

    public string SiteCacheDir()
    {
        string path;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Cache", _appName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            path = Path.Combine("/Library/Caches", _appName);
        }
        else // Linux and other Unix-like OS
        {
            path = Path.Combine("/var/cache", _appName);
        }
        return path;
    }

    public IEnumerable<string> IterDataDirs()
    {
        var dirs = new List<string>();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            dirs.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _appName));
            dirs.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), _appName));
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            dirs.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", _appName));
            dirs.Add(Path.Combine("/Library/Application Support", _appName));
        }
        else // Linux and other Unix-like OS
        {
            dirs.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", _appName));
            dirs.Add(Path.Combine("/usr/local/share", _appName));
        }
        return dirs;
    }

    public IEnumerable<string> IterCacheDirs()
    {
        var dirs = new List<string>();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            dirs.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Cache", _appName));
            dirs.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Cache", _appName));
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            dirs.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Caches", _appName));
            dirs.Add(Path.Combine("/Library/Caches", _appName));
        }
        else // Linux and other Unix-like OS
        {
            dirs.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", _appName));
            dirs.Add(Path.Combine("/var/cache", _appName));
        }
        return dirs;
    }

    public string GetPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "Windows";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "macOS / Darwin";
        }
        else
        {
            return "Linux";
        }
    }
}
