using System;
using System.Buffers;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Spectre.Console;

namespace AslUtils;

internal static class SpectreHelpers
{
    public static async Task<bool> TryDownloadFile(string downloadUrl, string targetFilePath, bool overwrite = false)
    {
        if (File.Exists(targetFilePath) && !overwrite)
        {
            return true;
        }

        using var client = new HttpClient();
        using var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode
            || response.Content.Headers.ContentLength is not long length)
        {
            AnsiConsole.MarkupLine($"[Red]Web request failed: {(int)response.StatusCode} {response.StatusCode}.[/]");
            return false;
        }

        return await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var description = $"[Yellow]Downloading {Path.GetFileName(targetFilePath)}...[/]";
                var task = ctx.AddTask(description, maxValue: length);
                var buffer = ArrayPool<byte>.Shared.Rent(81920);

                try
                {
                    using var source = await response.Content.ReadAsStreamAsync();
                    using var destination = File.OpenWrite(targetFilePath);

                    int bytesRead;
                    while ((bytesRead = await source.ReadAsync(buffer)) != 0)
                    {
                        await destination.WriteAsync(buffer.AsMemory(0, bytesRead));
                        task.Increment(bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteException(ex);
                    return false;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }

                return true;
            });
    }

    public static void WriteRule(string title, bool additionalBreak = true)
    {
        AnsiConsole.Write(new Rule(title)
        {
            Border = BoxBorder.Heavy,
            Justification = Justify.Left
        });

        if (additionalBreak)
        {
            AnsiConsole.WriteLine();
        }
    }
}
