// https://pws.yarb00.dev

using System;

namespace PWSandbox.Tui;

internal class AboutMenu : IMenu
{
#if DEBUG
	private const string buildType = "Debug";
#else
	private const string buildType = "Release";
#endif

	public void Show()
	{
		Console.Clear();

		Console.WriteLine($"""
			===== About PWSandbox.Tui =====
			Version {Program.FriendlyVersion}, {buildType} build

			===== Description: =====
			Cross-platform console version of PWSandbox.
			Website: {Program.Website}

			===== License: =====
			{Program.License}

			===== End of the About section =====
			Press Escape to return.
			""");

		ConsoleKey pressedKey;
		do pressedKey = Console.ReadKey(true).Key; while (pressedKey != ConsoleKey.Escape);
	}
}
