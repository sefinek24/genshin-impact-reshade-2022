using System.Diagnostics;

namespace BuildStellaMod;

internal static class Program
{
	public static readonly string RootPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..");
	private static readonly string ReleaseBuildPath = Path.Combine(RootPath, "Build", "Release", "net8.0-windows");

	private static async Task Main()
	{
		Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
		Console.WriteLine($"Release folder path: {ReleaseBuildPath}");
		Console.WriteLine();

		foreach (var process in Process.GetProcessesByName("inject64"))
		{
			try
			{
				process.Kill();
				Console.WriteLine($"Process {process.ProcessName} (ID: {process.Id}) has been terminated.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to terminate {process.ProcessName} (ID: {process.Id}): {ex.Message}");
			}
		}

		await Utils.DeleteDirectoryIfExistsAsync(ReleaseBuildPath).ConfigureAwait(false);

		await Utils.RestorePackages().ConfigureAwait(false);

		string[] projects =
		[
			@"Stella.Launcher\1-1. Stella Mod Launcher.csproj",
			@"Stella.Worker\2-1. Genshin Stella Mod.csproj",
			@"Stella.Configuration\3. Configuration .NET 8.csproj",
			@"Stella.PrepareOld\4. Prepare Stella.csproj",
			@"Stella.Info-48793142\5. Information 48793142.csproj",
			@"Stella.DeviceIdentifier\6. Device Identifier.csproj"
		];

		foreach (string project in projects) await PrepareCompilationAsync(Path.Combine(RootPath, project)).ConfigureAwait(false);

		Console.WriteLine("\nPress ENTER to exit the program...");
		Console.ReadLine();
	}

	private static async Task PrepareCompilationAsync(string projectPath)
	{
		string binPath = Path.Combine(Path.GetDirectoryName(projectPath)!, "bin");
		// string objPath = Path.Combine(Path.GetDirectoryName(projectPath)!, "obj");

		await Utils.DeleteDirectoryIfExistsAsync(binPath).ConfigureAwait(false);
		// await Utils.DeleteDirectoryIfExistsAsync(objPath).ConfigureAwait(false);

		Console.WriteLine();

		bool compilationSuccess = Utils.CompileProject(projectPath);
		Console.WriteLine();

		if (compilationSuccess)
		{
			Console.WriteLine($"----------------------------- COMPILED {Path.GetFileName(projectPath)} -----------------------------");
			Utils.CopyFiles(ReleaseBuildPath);
		}
		else
		{
			Console.WriteLine($"----------------------------- FAILED TO COMPILE {Path.GetFileName(projectPath)} -----------------------------");
		}
	}
}
