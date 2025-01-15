using System;
using Microsoft.Win32;
using System.Diagnostics;

namespace DefenderControl
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Defender Control\n1. Disable Defender\n2. Enable Defender\nChoose an option:");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DisableDefender();
                    break;
                case "2":
                    EnableDefender();
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        static void DisableDefender()
        {
            try
            {
                Console.WriteLine("Disabling Windows Defender...");

                // Disable Defender using Registry
                SetRegistryValue(@"SOFTWARE\\Policies\\Microsoft\\Windows Defender", "DisableAntiSpyware", 1);
                SetRegistryValue(@"SOFTWARE\\Policies\\Microsoft\\Windows Defender", "DisableRealtimeMonitoring", 1);
                SetRegistryValue(@"SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Spynet", "SpynetReporting", 0);
                SetRegistryValue(@"SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Spynet", "SubmitSamplesConsent", 0);

                // Stop Defender services
                ExecuteCommand("sc stop WinDefend");
                ExecuteCommand("sc config WinDefend start= disabled");

                Console.WriteLine("Windows Defender has been disabled completely.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void EnableDefender()
        {
            try
            {
                Console.WriteLine("Enabling Windows Defender...");

                // Enable Defender using Registry
                SetRegistryValue(@"SOFTWARE\\Policies\\Microsoft\\Windows Defender", "DisableAntiSpyware", 0);
                SetRegistryValue(@"SOFTWARE\\Policies\\Microsoft\\Windows Defender", "DisableRealtimeMonitoring", 0);
                SetRegistryValue(@"SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Spynet", "SpynetReporting", 1);
                SetRegistryValue(@"SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Spynet", "SubmitSamplesConsent", 1);

                // Start Defender services
                ExecuteCommand("sc config WinDefend start= auto");
                ExecuteCommand("sc start WinDefend");

                Console.WriteLine("Windows Defender has been enabled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void SetRegistryValue(string path, string name, int value)
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(path))
            {
                if (key == null)
                    throw new Exception($"Failed to access registry path: {path}");

                key.SetValue(name, value, RegistryValueKind.DWord);
            }
        }

        static void ExecuteCommand(string command)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C {command}";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
        }
    }
}
