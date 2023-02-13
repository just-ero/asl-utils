using System;
using System.IO;
using Microsoft.Win32;

internal partial class Program
{
    private const string UninstallKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
    private const string DisplayNameValueName = "DisplayName";

    private static string _installDirectory = "%LOCALAPPDATA%/Programs/Microsoft VS Code";
    private static string ExpandedInstallDirectory => Environment.ExpandEnvironmentVariables(_installDirectory);
    private static string CodeCmd => $"{ExpandedInstallDirectory}/bin/code.cmd";

    private static bool _vsCodeIsInstalled;
    private static bool VsCodeIsInstalled
    {
        get
        {
            if (_vsCodeIsInstalled)
            {
                return true;
            }

            if (CheckForVsCodeInRegistry(Registry.CurrentUser))
            {
                _vsCodeIsInstalled = true;
                return true;
            }

            if (CheckForVsCodeInRegistry(Registry.LocalMachine))
            {
                _vsCodeIsInstalled = true;
                return true;
            }

            if (File.Exists(CodeCmd))
            {
                _vsCodeIsInstalled = true;
                return true;
            }

            return false;
        }
    }

    private static bool CheckForVsCodeInRegistry(RegistryKey? key)
    {
        key?.OpenSubKey(UninstallKeyName);
        if (key is null)
        {
            return false;
        }

        foreach (string subKeyName in key.GetSubKeyNames())
        {
            using RegistryKey? subKey = key.OpenSubKey(subKeyName);
            if (subKey is null)
            {
                continue;
            }

            if (subKey.GetValue(DisplayNameValueName) is string displayName
                && displayName.StartsWith("Microsoft Visual Studio Code", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
