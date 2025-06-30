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
using System.Linq;

namespace PWSandbox.Tui;

public enum MapObject
{
	Unknown = -1, Void,
	Player,
	Finish,
	Wall, FakeWall, Barrier
}

public enum MapFileVersion
{
	V1_0
}

public static class MapParser
{
	public static MapObject[,] ParseMapFromFile(string filePath, MapFileVersion mapFileVersion)
	{
		try
		{
			return ParseMapFromStringArray(File.ReadAllLines(filePath), mapFileVersion);
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static MapObject[,] ParseMapFromStringArray(string[] mapLines, MapFileVersion mapFileVersion)
	{
		try
		{
			return mapFileVersion switch
			{
				MapFileVersion.V1_0 => ParseMapV1_0(mapLines),
				_ => throw new NotImplementedException()
			};
		}
		catch (Exception)
		{
			throw;
		}
	}

	#region Parsers

	private static MapObject[,] ParseMapV1_0(string[] mapLines)
	{
		for (int y = 0; y < 3; y++)
		{
			mapLines = mapLines.Where(str => !string.IsNullOrWhiteSpace(str)).ToArray();

			switch (y)
			{
				case 0:
					if (mapLines[0].TrimStart().StartsWith("?PWSandbox-Map 1.0;", true, null))
					{
						mapLines[0] = mapLines[0].TrimStart().Remove(0, "?PWSandbox-Map 1.0;".Length);
						continue;
					}
					else
					{
						throw new FormatException($"String array doesn't contain map header or version of standard in map header is unsupported");
					}

				case 1:
					if (mapLines[0].TrimStart().StartsWith("(map: begin)", true, null))
					{
						mapLines[0] = mapLines[0].TrimStart().Remove(0, "(map: begin)".Length);
						continue;
					}
					else
					{
						throw new FormatException($"Expected \"(map: begin)\" block after map header (\"?PWSandbox-Map 1.0;\") in string array, but it was not found");
					}

				case 2:
					if (mapLines[^1].TrimEnd().EndsWith("(map: end)", true, null))
					{
						mapLines[^1] = mapLines[^1].TrimEnd().Remove(mapLines[^1].Length - "(map: end)".Length, "(map: end)".Length);
						mapLines = mapLines.Where(str => !string.IsNullOrWhiteSpace(str)).ToArray();

						break;
					}
					else
					{
						throw new FormatException($"Expected \"(map: end)\" block in the end of string array, but it was not found");
					}
			}
		}

		int maxX = 0;
		for (int y = 0; y < mapLines.Length; y++)
			if (maxX < mapLines[y].Length)
				maxX = mapLines[y].Length;

		MapObject[,] mapObjects = new MapObject[mapLines.Length, maxX];

		for (int y = 0; y < mapLines.Length; y++)
			for (int x = 0; x < mapLines[y].Length; x++)
				mapObjects[y, x] = mapLines[y][x] switch
				{
					' ' => MapObject.Void,
					'!' => MapObject.Player,
					'=' => MapObject.Finish,
					'@' => MapObject.Wall,
					'#' => MapObject.FakeWall,
					'*' => MapObject.Barrier,
					_ => MapObject.Unknown
				};

		return mapObjects;
	}

	#endregion
}
