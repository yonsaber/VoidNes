using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace VoidNes;

public static class DllMap
{
    private static readonly string s_os;

    private static Dictionary<Assembly, Dictionary<string, string>> s_mapping =
        new Dictionary<Assembly, Dictionary<string, string>>();

    static DllMap()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            s_os = "osx";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            s_os = "linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            s_os = "windows";
        }
        else
        {
            throw new NotSupportedException("Your platform is not supported");
        }
    }

    public static void Register(Assembly assembly)
    {
        var configPath = assembly.Location + ".config";
        Console.WriteLine(configPath);
        if (File.Exists(configPath))
        {
            var assemblyMapping = new Dictionary<string, string>();
            s_mapping[assembly] = assemblyMapping;

            var root = XElement.Load(configPath);
            foreach (var map in root.Elements("dllmap"))
            {
                var mapOS = (string)map.Attribute("os");
                if (mapOS.Contains(s_os) && (string)map.Attribute("cpu") != "armv8")
                {
                    var dll = (string)map.Attribute("dll");
                    var target = (string)map.Attribute("target");
                    assemblyMapping[dll] = target;
                }
            }

            try
            {
                NativeLibrary.SetDllImportResolver(assembly, MapAndLoad);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static IntPtr MapAndLoad(
        string libraryName,
        Assembly assembly,
        DllImportSearchPath? dllImportSearchPath
    )
    {
        var finalName = libraryName;
        Console.WriteLine(finalName);

        if (s_mapping.TryGetValue(assembly, out var mapping) &&
            mapping.TryGetValue(libraryName, out var target))
        {
            finalName = target;
        }
        Console.WriteLine(finalName);
        return NativeLibrary.Load(finalName, assembly, dllImportSearchPath);
    }
}


