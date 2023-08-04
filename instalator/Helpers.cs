using System.Diagnostics;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Instalator;

public class Helpers
{
   public static bool ApplyFoldersLinux()
   {
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        string? originalUsername = Environment.GetEnvironmentVariable("SUDO_USER");

        if(originalUsername is not null)
            homeDirectory = $"/home/{originalUsername}";

        string folder = $"{homeDirectory}/.svc-sgn";

        try
        {
            if(Directory.Exists(folder))
            {
                Info("Instalation Reloading Files...");

                var commandRemoveFolderRootRecursive = new ProcessStartInfo
                {
                    FileName = "/bin/rm",
                    Arguments = $"-r {folder}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var commandReloadDaemon = new ProcessStartInfo
                {
                    FileName = "/bin/systemctl",
                    Arguments = "daemon-reload",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process.Start(commandRemoveFolderRootRecursive);

                Process.Start(commandReloadDaemon);

                Folders(folder);
            }

            Info("Instalation Loading Files...");

            Folders(folder);

            string storagePath = $"{folder}/storage";

            string logsPath = $"{folder}/storage/logs";

            string pdfsPath = $"{folder}/storage/pdfs";

            string dotnevFile = File.ReadAllText(".env-vars-template");

            dotnevFile += $"\nSTORAGE_PATH={storagePath}";

            dotnevFile += $"\nLOGS_PATH={logsPath}";

            dotnevFile += $"\nPDFS_PATH={pdfsPath}";

            File.WriteAllText($"{folder}/.env", dotnevFile);

            File.WriteAllText(
                $"{folder}/init/svc-sgn-i",
                "#!/usr/bin/bash"
                + "\nchmod -R 777 ~/.svc-sgn"
                + "\ncd ~/.svc-sgn"
                + "\nbin/signer"
            );

            Ok("Instalation 1 / 4 - OK");

            Info("Configuring Files ...");

            CopyDirectory("build/linux-x64", $"{folder}/bin");

            Ok("Instalation  2 / 4 - OK");

            Info("Setup Application ...");

            var startInfoPermissionInitialize = new ProcessStartInfo
            {
                FileName = "/bin/chmod",
                Arguments = $"+x {folder}/init/svc-sgn-i",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            File.WriteAllText(
                "/etc/systemd/system/svc-sgn-i.service",
                "[Unit]"
                + "\nDescription=Signer Service - Websocket Server To Sign Pdf Files Using Client Application Signer"
                + "\nAfter=network.target"
                + "\n[Service]"
                + "\nRestart=on-failure"
                + "\nExecStart=" + folder + "/init/svc-sgn-i"
                + "\n[Install]"
                + "\nWantedBy=multi-user.target"
            );

            var reloadServices = new ProcessStartInfo
            {
                FileName = "/bin/systemctl",
                Arguments = "daemon-reload",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var enableStartService = new ProcessStartInfo
            {
                FileName = "/bin/systemctl",
                Arguments = "enable svc-sgn-i",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var commandStartService  = new ProcessStartInfo
            {
                FileName = "/bin/systemctl",
                Arguments = "start svc-sgn-i",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(reloadServices);

            Process.Start(startInfoPermissionInitialize);

            Process.Start(commandStartService);

            Process.Start(enableStartService);

            Ok("Setup Defined  3 / 4 - OK");

            Info("Testing Application Files ...");

            TestConnectionWebScoketByDefaultService().Wait();

            Ok("Application Tested 4 / 4 - OK");

            return true;
        }
        catch (System.Exception exception)
        {
            Fail(exception.Message);

            return false;
        }
   }

   public static async Task TestConnectionWebScoketByDefaultService()
   {
        try
        {
            string uri = "ws://localhost:2514";

            using ClientWebSocket wsClient = new ClientWebSocket();

            await wsClient.ConnectAsync(new Uri(uri), CancellationToken.None);

            Ok("OK - TEST 1 / 2 - Connection Webscoket");
            
            byte[] buffer = new byte[1024];

            string strMessage = "{ \"command\":\"setup.environment\" }";

            byte[] messageBytes = Encoding.UTF8.GetBytes(strMessage, 0, strMessage.Length);

            await wsClient.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            WebSocketReceiveResult result = await wsClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Ok("OK - TEST 2 / 2 - Send Message");

            await wsClient.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closing",
                CancellationToken.None
            );
        }
        catch (System.Exception exception)
        {
            Fail("FAIL ON TESTING ...");

            Fail(exception.Message);
        }
   }

    public static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        string[] files = Directory.GetFiles(sourceDir);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            string destinationFilePath = Path.Combine(destDir, fileName);

            File.Copy(file, destinationFilePath, true);
        }

        string[] subDirectories = Directory.GetDirectories(sourceDir);

        foreach (string subDirectory in subDirectories)
        {
            string subDirectoryName = Path.GetFileName(subDirectory);

            string destinationSubFolderPath = Path.Combine(destDir, subDirectoryName);

            CopyDirectory(subDirectory, destinationSubFolderPath);
        }
    }

    public static void Folders(string root)
    {
        Directory.CreateDirectory(root);
        Directory.CreateDirectory($"{root}/bin");
        Directory.CreateDirectory($"{root}/init");
        Directory.CreateDirectory($"{root}/storage");
        Directory.CreateDirectory($"{root}/storage/logs");
        Directory.CreateDirectory($"{root}/storage/pdfs");
        Directory.CreateDirectory($"{root}/storage/preferencies");
    }

    public static void Ok(string message)
    {
        Print(message, Message.Success);
    }
    
    public static void Fail(string message)
    {
        Print(message, Message.Error);
    }

    public static void Info(string message)
    {
        Print(message, Message.Info);
    }

    public static void Warning(string message)
    {
        Print(message, Message.Warning);
    }

    public static void Print(string message, Message status)
    {
        switch(status)
        {
            case Message.Success:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
            break;

            case Message.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ResetColor();
            break;

            case Message.Info:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(message);
                Console.ResetColor();
            break;

            case Message.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(message);
                Console.ResetColor();
            break;
        };
    }
}