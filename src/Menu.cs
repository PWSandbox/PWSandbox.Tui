// This file is a part of PWSandbox.Tui ( https://github.com/PWSandbox/PWSandbox.Tui )
// PWSandbox.Tui is licensed under the MIT (Expat) License:

/* MIT License
 *
 * Copyright (c) 2025 yarb00
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.IO;

namespace PWSandbox.Tui;

public static class Menu
{
	public static void Start()
	{
		bool isExit = false;

		while (true)
		{
			Console.Clear();

			Console.WriteLine($"""
				PWSandbox.Tui{(Program.AppVersion is not null ? $" v{Program.AppVersion.ToString(3)}" : "")}
				https://github.com/PWSandbox/PWSandbox.Tui

				==========

				M. Load map from file
				Q. Quit

				""");

			switch (char.ToUpper(Console.ReadKey(true).KeyChar))
			{
				case 'M':
					Console.WriteLine("Enter map file (*.pws_map) location: ");
					(PlayMenu? playMenu, string? errorText) = GetLoadedPlayMenu(Console.ReadLine() ?? string.Empty);

					if (errorText is not null)
					{
						Console.WriteLine(errorText);
						Console.ReadKey(true);
					}

					playMenu?.Start();

					continue;

				case 'Q':
					isExit = true;
					break;

				default:
					continue;
			}

			if (isExit) break;
		}

		Console.Clear();
	}

	public static (PlayMenu? playMenu, string? errorText) GetLoadedPlayMenu(string mapFileLocation)
	{
		MapObject[,] mapObjects;
		try
		{
			mapObjects = MapParser.ParseMapFromFile(mapFileLocation);
		}
		catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
		{
			return (null, "This file does not exist.");
		}
		catch (ArgumentException e) when (e.ParamName == "path")
		{
			return (null, "Please enter a valid path of the map file.");
		}
		catch (ArgumentException e) when (e.ParamName == "filePath")
		{
			return (null, "The file you selected is empty and does not contain a valid PWSandbox map.");
		}
		catch (Exception ex) when (ex is FormatException or NotSupportedException)
		{
			return (null, $"""
				Map file is not valid!
				It's either made for a newer version of PWSandbox or just written incorrectly.

				Contact map maker and let them know about this problem.
				(If you are the map maker and map file is being loaded with the right version of PWSandbox,
				then you wrote map file in a wrong way. Check detailed message.)

				Detailed message: "{ex.Message}"
				""");
		}
		catch (IOException)
		{
			return (null, "An error occurred when reading this file. Please check it's not blocked by anithing.");
		}

		return (new PlayMenu(mapObjects), null);
	}
}
