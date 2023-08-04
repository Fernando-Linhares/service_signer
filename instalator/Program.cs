namespace Instalator;

public class Program
{
    public static void Main(string[] args)
    {
        var os = new Plataform();

        if(os.Name.Equals("linux"))
        {
            Helpers.Info("Starting Instalation ...");

            bool applyiedFoldersLinux = Helpers.ApplyFoldersLinux();

            Helpers.Info("Installation Complete");
        }
    }
}