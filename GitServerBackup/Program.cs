using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GitServerBackup
{
    class Program
    {
        private static string _rootPath;
        private static string _backupTargetPath;
        static void Main(string[] args)
        {
            System.Console.WriteLine("============================================================================");
            System.Console.WriteLine("GitServerBackup - backup of git repositories folder on a Windows server");
            System.Console.WriteLine("Copyright: Tikal Knowledge LTD.");
            System.Console.WriteLine("============================================================================");
            if (args.Length != 2)
            {
                System.Console.WriteLine("Usage: <Root folder to backup> <Target backup folder>");
                Environment.Exit(1);
            }
            _rootPath = args[0];
            _backupTargetPath = args[1];
            Validate();
            Backup(_rootPath);
        }

        static void Validate()
        {
            System.Console.WriteLine("Folder to backup: " + _rootPath);
            System.Console.WriteLine("Target backup folder: " + _backupTargetPath);
            System.Console.WriteLine("==================================================");
            System.Console.WriteLine("");
            if (!Directory.Exists(_rootPath))
            {
                System.Console.WriteLine("The directory "+_rootPath+" doesn't exist!");
                Environment.Exit(2);
            }
            if (!Directory.Exists(_backupTargetPath))
            {
                try
                {
                    Directory.CreateDirectory(_backupTargetPath);
                }
                catch(Exception ex)
                {
                    System.Console.WriteLine("Failed to create the directory " + _backupTargetPath + ". Error: "+ex.Message);
                    Environment.Exit(3);
                }
            }
        }

        static void Backup(string folder)
        {
            if (0 == LaunchCommandLineApp("git", "rev-parse", folder))
            {
                string folderName = folder.Substring(folder.LastIndexOf("\\")+1);
                if (0 == LaunchCommandLineApp("git", "bundle create "+_backupTargetPath+"\\"+folderName+".bundle --all", folder))
                {
                    System.Console.WriteLine("git bundle " + folder);
                }
                System.Console.WriteLine("git bundle " + folder);
            }
            else
            {
                try
                {
                    string[] dirs = Directory.GetDirectories(folder);
                    foreach (string dir in dirs)
                    {
                        Backup(dir);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Failed to parse directory " + folder + ". Error: " + ex.Message);
                    Environment.Exit(4);
                }
            }
        }

        static int LaunchCommandLineApp(string executable, string arguments, string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = executable;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = path;
            startInfo.Arguments = arguments;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                    return exeProcess.ExitCode;
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine("Failed to run command [" + executable + " " + arguments + "]. Error: " + ex.Message);
                return 999;
            }
        }
    }
}
