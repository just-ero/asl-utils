using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using AslUtils;
using Spectre.Console;

internal partial class Program
{
    private const string CascadiaVersion = "2111.01";
    private const string CascadiaDownloadUrl = $"https://github.com/microsoft/cascadia-code/releases/download/v{CascadiaVersion}/CascadiaCode-{CascadiaVersion}.zip";

    private static readonly string _cascadiaDir = $"{_temp}/CascadiaCode-{CascadiaVersion}";
    private static readonly string _cascadiaZip = $"{_cascadiaDir}.zip";
    private static readonly string _fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

    private static bool CascadiaIsExtracted => Directory.Exists(_cascadiaDir);
    private static bool CascadiaIsInstalled => File.Exists($"{_cascadiaDir}/ttf/CascadiaCode.ttf");

    private static async Task<bool> TryDownloadCascadiaCode()
    {
        if (CascadiaIsExtracted || CascadiaIsInstalled)
        {
            return true;
        }

        return await SpectreHelpers.TryDownloadFile(CascadiaDownloadUrl, _cascadiaZip);
    }

    private static void ExtractCascadiaCode()
    {
        if (CascadiaIsExtracted || CascadiaIsInstalled)
        {
            return;
        }

        AnsiConsole.MarkupLine($"[Yellow]Extracting CascadiaCode-{CascadiaVersion}.zip...[/]");

        ZipFile.ExtractToDirectory(_cascadiaZip, _cascadiaDir);
    }

    private static readonly string[] _fonts =
    {
        "CascadiaCode.ttf",
        "CascadiaCodeItalic.ttf"
    };

    private static void InstallCascadiaCode()
    {
        if (File.Exists($"{_cascadiaDir}/ttf/CascadiaCode.ttf"))
        {
            AnsiConsole.MarkupLine("[Green]Cascadia Code already installed![/]");

            return;
        }

        AnsiConsole.MarkupLine("[Yellow]Installing fonts...[/]");

        foreach (string font in _fonts)
        {
            if (File.Exists($"{_cascadiaDir}/ttf/{font}"))
            {
                File.Move($"{_cascadiaDir}/ttf/{font}", $"{_fontsDir}/{font}");
            }
        }

        AnsiConsole.MarkupLine("[Green]Done.[/]");
    }
}
