using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AslUtils;
using Spectre.Console;

internal partial class Program
{
    private const string VsCodeDownloadUrl = "https://code.visualstudio.com/sha/download?build=stable&os=win32-x64-user";
    private const string InstallerFileName = "VSCodeUserSetup-x64.exe";
    private static readonly string _installerFilePath = $"{_temp}/{InstallerFileName}";

    private static async Task<bool> TryDownloadVsCode()
    {
        if (VsCodeIsInstalled || File.Exists(_installerFilePath))
        {
            return true;
        }

        AnsiConsole.MarkupLine("[Yellow]Fetching Visual Studio Code installer...[/]");

        return await SpectreHelpers.TryDownloadFile(VsCodeDownloadUrl, _installerFilePath);
    }

    private static readonly string[] _options =
    {
        "/SILENT",
        "/NORESTART"
    };

    private static readonly string[] _tasks =
    {
        "!runcode",
        "addcontextmenufiles",
        "addcontextmenufolders",
        "associatewithfiles",
        "addtopath"
    };

    private static async Task InstallVsCode()
    {
        if (VsCodeIsInstalled)
        {
            AnsiConsole.MarkupLine("[Green]Visual Studio Code already installed![/]");
            return;
        }

        AnsiConsole.MarkupLine("[Yellow]Starting Visual Studio Code installer...[/]");
        using var installer = Process.Start(_installerFilePath, $"{string.Join(' ', _options)} /MERGETASKS={string.Join(',', _tasks)}");

        AnsiConsole.MarkupLine("[Yellow]Installing Visual Studio Code. This may take a while...[/]");
        await installer.WaitForExitAsync();

        if (installer.ExitCode == 0)
        {
            AnsiConsole.MarkupLine("[Green]Done.[/]");

            _vsCodeIsInstalled = true;
        }
        else
        {
            AnsiConsole.MarkupLine($"[Red]Installer did not finish correctly![/] (Exit code [Red]{installer.ExitCode}[/])");
        }
    }
}
