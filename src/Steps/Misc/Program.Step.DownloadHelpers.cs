using System.IO;
using System.Threading.Tasks;
using AslUtils;
using Spectre.Console;

internal partial class Program
{
    private const string AslHelpRepo = "https://github.com/just-ero/asl-help";
    private const string AslHelpDownloadUrl = $"{AslHelpRepo}/raw/main/lib/asl-help";

    private static string? _liveSplitDirectory = null;

    private static async Task DownloadAslHelp()
    {
        PromptForLiveSplitInstallDirectory();

        string filePath = $"{_liveSplitDirectory}/Components/asl-help";
        if (await SpectreHelpers.TryDownloadFile(AslHelpDownloadUrl, filePath, true))
        {
            AnsiConsole.MarkupLine($"[Green]asl-help's GitHub page can be found at {AslHelpRepo}.[/]");
        }
    }

    private const string EmuHelpRepo = "https://github.com/Jujstme/emu-help";
    private const string EmuHelpDownloadUrl = $"{EmuHelpRepo}/raw/master/lib/emu-help";

    private static async Task DownloadEmuHelp()
    {
        PromptForLiveSplitInstallDirectory();

        string filePath = $"{_liveSplitDirectory}/Components/emu-help";
        if (await SpectreHelpers.TryDownloadFile(EmuHelpDownloadUrl, filePath, true))
        {
            AnsiConsole.MarkupLine($"[Green]emu-help's documentation can be found at {EmuHelpRepo}.[/]");
        }
    }

    private static void PromptForLiveSplitInstallDirectory()
    {
        _liveSplitDirectory ??= AnsiConsole.Prompt(
            new TextPrompt<string>("[Blue]Please provide LiveSplit's install directory:[/]")
            .Validate(dir =>
            {
                if (!Directory.Exists(dir))
                {
                    return ValidationResult.Error("[Red]Directory does not exist.[/]");
                }

                if (!File.Exists($"{dir}/LiveSplit.exe"))
                {
                    return ValidationResult.Error("[Red]LiveSplit is not installed there.[/]");
                }

                return ValidationResult.Success();
            }));
    }
}
