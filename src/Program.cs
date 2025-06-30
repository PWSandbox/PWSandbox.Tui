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

namespace PWSandbox.Tui;

public static class Program
{
	public static void Main(string[] args)
	{
		if (args.Length == 0) Menu.Start();
		else if (args.Length == 2 && (args[0] == "--map-file" || args[0] == "-m"))
		{
			try
			{
				new PlayMenu(MapParser.ParseMapFromFile(args[1], MapFileVersion.V1_0)).Start();
			}
			catch (System.IO.FileNotFoundException)
			{
				Console.WriteLine("This file does not exist.");
			}
			catch (ArgumentException e) when (e.ParamName == "path")
			{
				Console.WriteLine("Please enter a valid path of the map file.");
			}
			catch (FormatException e) when (e.Message.Contains("(map: end)", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine("An error occured while parsing map: expected \"(map: end)\" in the end of file, but it was not found.");
			}
			catch (FormatException e) when (e.Message.Contains("(map: begin)", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine("An error occured while parsing map: expected \"(map: begin)\" after map header (\"?PWSandbox-Map 1.0;\"), but it was not found.");
			}
			catch (FormatException e) when (e.Message.Contains("map header", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine("This file is not a valid PWSandbox map or it is a map designed for a newer/older version of PWSandbox.");
			}
		}
		else
			Console.WriteLine($"PWSandbox.Tui (v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)
				?? "<Unknown!>"
			}): Wrong arguments!");
	}
}
