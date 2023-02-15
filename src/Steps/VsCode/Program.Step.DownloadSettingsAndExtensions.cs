using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AslUtils;
using Spectre.Console;

internal partial class Program
{
    private const string SettingsDir = "%APPDATA%/Code/User";
    private const string SettingsUrl = "https://gist.github.com/just-ero/bdd5b0d8df6d2106830d978347b3443d/raw/settings.json";

    private static async Task DownloadVsCodeSettings()
    {
        var settingsDir = Environment.ExpandEnvironmentVariables(SettingsDir);
        _ = Directory.CreateDirectory(settingsDir);

        var settingsFile = $"{settingsDir}/settings.json";
        if (File.Exists(settingsFile))
        {
            AnsiConsole.MarkupLine("[Red]settings.json already exists![/]");
            AnsiConsole.MarkupLine("[Yellow]Continuing will replace all of your settings (a backup will be created).[/]");

            if (!AnsiConsole.Confirm("[Blue]Are you sure you want to continue?[/]", false))
            {
                return;
            }

            File.Move(settingsFile, $"{settingsDir}/backup.settings.json", true);
        }

        if (!VsCodeIsInstalled)
        {
            PromptForVsCodeInstallDirectory();
        }

        _ = await SpectreHelpers.TryDownloadFile(SettingsUrl, settingsFile);

        AnsiConsole.MarkupLine("[Green]Done.[/]");
    }

    private const string ExtensionsUrl = "https://gist.github.com/just-ero/bdd5b0d8df6d2106830d978347b3443d/raw/extensions.json";

    private static async Task InstallVsCodeExtensions()
    {
        if (!VsCodeIsInstalled)
        {
            PromptForVsCodeInstallDirectory();
        }

        string[] extensions;

        try
        {
            using var client = new HttpClient();
            using var stream = await client.GetStreamAsync(ExtensionsUrl);

            var jsonResponse = JsonSerializer.Deserialize<string[]>(stream);
            if (jsonResponse is null)
            {
                return;
            }

            extensions = jsonResponse;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return;
        }

        AnsiConsole.MarkupLine("[Yellow]Installing extensions...[/]");
        AnsiConsole.WriteLine();

        using var installer = Process.Start(CodeCmd, string.Join(' ', extensions.Select(e => $"--install-extension {e}")));
        await installer.WaitForExitAsync();

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[Green]Done.[/]");
    }

    private static void PromptForVsCodeInstallDirectory()
    {
        AnsiConsole.MarkupLine("[Red]Visual Studio Code does not appear to be installed or could not be located.[/]");
        AnsiConsole.WriteLine();

        _installDirectory = AnsiConsole.Prompt(
            new TextPrompt<string>("[Blue]Please provide Visual Studio Code's install directory:[/]")
            .Validate(static path =>
            {
                path = Environment.ExpandEnvironmentVariables(path);

                if (!Directory.Exists(path))
                {
                    return ValidationResult.Error("[Red]Directory does not exist.[/]");
                }

                if (!File.Exists($"{path}/bin/code.cmd"))
                {
                    return ValidationResult.Error("[Red]Visual Studio Code is not installed there.[/]");
                }
                else
                {
                    return ValidationResult.Success();
                }
            }));
    }
}
