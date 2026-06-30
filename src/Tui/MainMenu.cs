// https://pws.yarb00.dev

using System;
using PWSandbox.Formats;

namespace PWSandbox.Tui;

internal sealed class MainMenu : IMenu
{
	public void ShowDialog()
	{
		bool shouldExit = false;

		while (!shouldExit)
		{
			Console.Clear();

			Console.WriteLine(
				$"""
				===== PWSandbox.Tui v{Program.FriendlyVersion} =====
				M. Load map
				A. About PWSandbox.Tui
				U. Check for updates
				Q. Quit
				"""
			);

			switch (char.ToUpper(Console.ReadKey(intercept: true).KeyChar))
			{
				case 'M':
					Console.WriteLine();
					Console.WriteLine("Enter map file (*.pws_map) location: ");
					string? filePath = Console.ReadLine();
					if (filePath is null)
						goto case 'Q';
					LoadMapInteractively(filePath);
					continue;

				case 'A':
					new AboutMenu().ShowDialog();
					continue;

				case 'U':
					new UpdateCheckMenu().ShowDialog();
					continue;

				case 'Q':
					shouldExit = true;
					break;

				default:
					continue;
			}
		}
	}

	public static void LoadMapInteractively(string filePath)
	{
		Map map;
		try
		{
			map = Map.ParseFromFile(filePath);
		}
		catch (Exception e) when (e is FormatException or NotSupportedException)
		{
			Console.WriteLine(
				$"""
				===== PWSandbox.Tui v{Program.FriendlyVersion} =====

				Map file is not valid!
				It's either made for a newer version of PWSandbox or just written incorrectly.

				Contact the map maker and let them know about this problem.
				(If you are the map maker and map file is being loaded with the right version of PWSandbox,
				then you wrote map file in a wrong way. Check detailed message.)

				===== Details: =====
				{e.Message}

				Press Escape to return.
				"""
			);

			ConsoleKey pressedKey;

			do pressedKey = Console.ReadKey(intercept: true).Key;
			while (pressedKey != ConsoleKey.Escape);

			return;
		}
		catch (Exception e)
		{
			Console.WriteLine(
				$"""
				===== PWSandbox.Tui v{Program.FriendlyVersion} =====

				An error occurred while trying to read the map file:
				{e.Message}

				Press Escape to return.
				"""
			);

			ConsoleKey pressedKey;

			do pressedKey = Console.ReadKey(intercept: true).Key;
			while (pressedKey != ConsoleKey.Escape);

			return;
		}

		new PlayMenu(map).ShowDialog();
	}
}
