using System;
using System.IO;
using System.Threading.Tasks;
using AslUtils;
using Spectre.Console;

internal partial class Program
{
    private static string PromptForTraceSpyOutputDirectory()
    {
        var defaultDirectory = Path.GetDirectoryName(".")!;

        return AnsiConsole.Prompt(
            new TextPrompt<string>("[CornflowerBlue]Specify a directory for TraceSpy (leave blank to choose current directory):[/]")
            .DefaultValue(defaultDirectory)
            .HideDefaultValue()
            .ValidationErrorMessage("[Red]Directory does not exist.[/]")
            .Validate(Directory.Exists)
            .AllowEmpty());
    }

    private const string TraceSpyDownloadUrl = "https://github.com/smourier/TraceSpy/releases/latest/download/WpfTraceSpy.exe";

    private static async Task DownloadTraceSpy()
    {
        var traceSpyDir = PromptForTraceSpyOutputDirectory();
        var traceSpyExe = $"{traceSpyDir}/TraceSpy.exe";
        if (File.Exists(traceSpyExe))
        {
            return;
        }

        await SpectreHelpers.TryDownloadFile(TraceSpyDownloadUrl, traceSpyExe);
    }

    private const string TraceSpyConfigDir = "%APPDATA%/TraceSpy";
    private const string TraceSpyConfigDownloadUrl = "https://gist.github.com/just-ero/bdd5b0d8df6d2106830d978347b3443d/raw/WpfSettings.config";

    private static async Task DownloadTraceSpyConfig()
    {
        var configDir = Environment.ExpandEnvironmentVariables(TraceSpyConfigDir);
        Directory.CreateDirectory(configDir);

        var configFile = $"{configDir}/WpfSettings.config";
        if (File.Exists(configFile))
        {
            AnsiConsole.MarkupLine("[Red]WpfSettings.config already exists![/]");
            AnsiConsole.MarkupLine("[Yellow]Continuing will replace all of your settings (a backup will be created).[/]");

            if (!AnsiConsole.Confirm("[Blue]Are you sure you want to continue?[/]", false))
            {
                return;
            }

            File.Move(configFile, $"{configDir}/backup.WpfSettings.config", true);
        }

        await SpectreHelpers.TryDownloadFile(TraceSpyConfigDownloadUrl, configFile);

        AnsiConsole.MarkupLine("[Green]Done.[/]");
    }
}
