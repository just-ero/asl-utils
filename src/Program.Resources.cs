using System.IO;

internal partial class Program
{
    private static readonly string _temp = $"{Path.GetTempPath()}/asl-help";

    private const string VsCode = "Visual Studio Code: Free, open source code editor";
    private const string VsCodeProgram = "Visual Studio Code";
    private const string VsCodeSettings = "Settings: Creates default settings for better quality-of-life.";
    private const string VsCodeExtensions = "Extensions: Installs some useful extensions.";

    private const string TraceSpy = "Trace Spy: Trace viewer for displaying error and debug output";
    private const string TraceSpyProgram = "Trace Spy";
    private const string TraceSpyConfig = "Config: Creates a custom config for a much better user experience.";

    private const string Other = "Other components";
    private const string ContextMenu = "Context menu: Adds a 'New LiveSplit Auto Splitter' entry to the file context menu.";
    private const string AslHelp = "asl-help: An ASL helper library with many helpful features, especially for Unity games.";
    private const string EmuHelp = "emu-help: An ASL helper library for emulators.";
    private const string CascadiaCode = "Cascadia Code: A modern, sleek monospace font from Microsoft.";
}
