// https://pws.yarb00.dev

using System;

namespace PWSandbox.Tui;

internal class UpdateCheckMenu : IMenu
{
	public void Show()
	{
		Console.Clear();

		Console.WriteLine("""
			===== PWSandbox.Tui Updater =====
			Getting the update data...
			""");

		UpdateData updateData = Updater.GetUpdateData().GetAwaiter().GetResult();

		// IsUpdateAvailable returns null if update data doesn't contain the version information
		if (Updater.IsUpdateAvailable(updateData) is false) Console.WriteLine("You're using the latest version!");
		else Console.WriteLine($"""
			Version {updateData.LatestVersion?.ToString(3) ?? "[Invalid data]"} is available!

			Information about the update:
			{updateData.DetailsUrl?.ToString() ?? "[Invalid data]"}
			""");

		Console.WriteLine("""
			===== End of the Updater section =====
			Press Escape to return.
			""");

		ConsoleKey pressedKey;
		do pressedKey = Console.ReadKey(true).Key; while (pressedKey != ConsoleKey.Escape);
	}
}
