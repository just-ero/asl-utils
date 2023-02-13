using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AslUtils;
using Microsoft.Win32;
using Spectre.Console;

internal partial class Program
{
    private const string RegFileDownloadUrl = "https://gist.githubusercontent.com/just-ero/bdd5b0d8df6d2106830d978347b3443d/raw/livesplit-auto-splitter.reg";
    private const string AslTemplateDownloadUrl = "https://gist.githubusercontent.com/just-ero/bdd5b0d8df6d2106830d978347b3443d/raw/AslTemplate.asl";
    private const string TemplatesDir = "%APPDATA%/Microsoft/Windows/Templates";

    private static async Task CreateContextMenuEntry()
    {
        var tempaltesDir = Environment.ExpandEnvironmentVariables(TemplatesDir);
        var template = $"{tempaltesDir}/AslTemplate.asl";
        if (!await SpectreHelpers.TryDownloadFile(AslTemplateDownloadUrl, template))
        {
            return;
        }

        var regFile = $"{_temp}/livesplit-auto-splitter.reg";
        if (!await SpectreHelpers.TryDownloadFile(RegFileDownloadUrl, regFile))
        {
            return;
        }

        AnsiConsole.MarkupLine("[Yellow]Starting registry editor as administrator...[/]");

        ProcessStartInfo regEditSi = new("regedit.exe", regFile)
        {
            Verb = "runas",
            UseShellExecute = true
        };

        using var regEdit = Process.Start(regEditSi);

        AnsiConsole.MarkupLine("[Yellow]Please confirm the pop-ups.[/]");

        await regEdit!.WaitForExitAsync();

        AnsiConsole.MarkupLine("[Green]Done.[/]");
        AnsiConsole.MarkupLine("[Yellow]A restart may be required.[/]");
    }
}
