using System.Runtime.InteropServices;

namespace Instalator;

public class Plataform
{
    public string Name { get; set; }

    public Plataform()
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Name = "windows";
        }

        if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Name = "linux";
        }

        if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Name = "macos";
        }
    }
}