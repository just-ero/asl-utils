using System;
using System.IO;
using AslUtils;
using Spectre.Console;

var choices = AnsiConsole.Prompt(
    new MultiSelectionPrompt<string>()
    .Title("[Blue]Which components would you like to download and install?[/]")
    .PageSize(12)
    .AddChoiceGroup(VsCode,
        VsCodeProgram,
        VsCodeSettings,
        VsCodeExtensions)
    .AddChoiceGroup(TraceSpy,
        TraceSpyProgram,
        TraceSpyConfig)
    .AddChoiceGroup(Other,
        ContextMenu,
        AslHelp,
        EmuHelp,
        CascadiaCode));

Directory.CreateDirectory(_temp);

if (choices.Contains(VsCodeProgram))
{
    SpectreHelpers.WriteRule("Visual Studio Code");

    if (await TryDownloadVsCode())
    {
        await InstallVsCode();
        AnsiConsole.WriteLine();
    }
}

if (choices.Contains(VsCodeSettings))
{
    SpectreHelpers.WriteRule("Visual Studio Code Settings", false);

    await DownloadVsCodeSettings();
}

if (choices.Contains(VsCodeExtensions))
{
    SpectreHelpers.WriteRule("Visual Studio Code Extensions");

    await InstallVsCodeExtensions();
    AnsiConsole.WriteLine();
}

if (choices.Contains(TraceSpyProgram))
{
    SpectreHelpers.WriteRule("TraceSpy");

    await DownloadTraceSpy();
}

if (choices.Contains(TraceSpyConfig))
{
    SpectreHelpers.WriteRule("TraceSpy Config");

    await DownloadTraceSpyConfig();
    AnsiConsole.WriteLine();
}

if (choices.Contains(ContextMenu))
{
    SpectreHelpers.WriteRule("Context Menu");

    await CreateContextMenuEntry();
    AnsiConsole.WriteLine();
}

if (choices.Contains(AslHelp))
{
    SpectreHelpers.WriteRule("asl-help");

    await DownloadAslHelp();
    AnsiConsole.WriteLine();
}

if (choices.Contains(EmuHelp))
{
    SpectreHelpers.WriteRule("emu-help");

    await DownloadEmuHelp();
    AnsiConsole.WriteLine();
}

if (choices.Contains(CascadiaCode))
{
    SpectreHelpers.WriteRule("Cascadia Code");

    if (await TryDownloadCascadiaCode())
    {
        ExtractCascadiaCode();
        InstallCascadiaCode();
        AnsiConsole.WriteLine();
    }
}

AnsiConsole.Markup("[Green]asl-help execution finished![/] Press any key to exit... ");
Console.ReadKey(true);
