namespace Sundew.Injection.Testing;

using System.IO;

public static class DemoProjectInfo
{
    public static string GetPath(string path)
    {
        var demoPath = Path.Combine(string.Format(@"..{0}..{0}..{0}..{0}..{0}..{0}..{0}..{0}", Path.DirectorySeparatorChar), path);
        if (!Directory.Exists(demoPath))
        {
            demoPath = Path.Combine(string.Format(@"..{0}..{0}..{0}..{0}", Path.DirectorySeparatorChar), path);
        }

        return demoPath;
    }

    public static bool TryFindDirectoryUpwards(string path, out string? foundPath)
    {
        foundPath = path;
        var parentPath = Directory.GetParent(path)?.FullName;
        while (foundPath != null && !Directory.Exists(foundPath))
        {
            if (parentPath == null)
            {
                foundPath = null;
                return false; 
            }

            parentPath = Directory.GetParent(parentPath)?.FullName;
            foundPath = !string.IsNullOrEmpty(parentPath) ? Path.Combine(parentPath, path) : null;
        }

        return !string.IsNullOrEmpty(foundPath);
    }

    public static string FindDirectoryUpwards(string path)
    {
        if (TryFindDirectoryUpwards(path, out var foundPath))
        {
            return foundPath!;
        }

        throw new DirectoryNotFoundException($"Directory from the path: {path} was not found");
    }
}