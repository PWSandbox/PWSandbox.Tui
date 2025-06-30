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
using System.Collections.Generic;

namespace PWSandbox.Tui;

public class PlayMenu
{
	private readonly MapObject[,] mapObjects;

	private (int x, int y)? playerPosition = null;

	bool isExit = false, isOnFinish = false;

	public PlayMenu(MapObject[,] mapObjects)
	{
		this.mapObjects = mapObjects;
	}

	public void Start()
	{
		while (true)
		{
			Console.Clear();

			Console.WriteLine("PWSandbox.Tui [Play]\n");

			ProcessMap();

			if (isOnFinish) Console.WriteLine("\nYou have reached the finish!");

			Console.WriteLine(
'\n' + @"Controls:

Escape - back to main menu
W/Up - move up
D/Down - move down
S/Left - move left
D/Right - move right"
			);

			ProcessKey(Console.ReadKey(true));

			if (isExit) break;
		}

		Console.Clear();
	}

	private void ProcessKey(ConsoleKeyInfo key)
	{
		if (key.Key == ConsoleKey.Escape)
		{
			isExit = true;

			return;
		}

		if (playerPosition is null ||
			!new List<ConsoleKey>
			{
				ConsoleKey.W, ConsoleKey.UpArrow,
				ConsoleKey.S, ConsoleKey.DownArrow,
				ConsoleKey.A, ConsoleKey.LeftArrow,
				ConsoleKey.D, ConsoleKey.RightArrow
			}.Contains(key.Key)) return;

		(int playerX, int playerY) = ((int, int))playerPosition;

		switch (key.Key)
		{
			case ConsoleKey.W or ConsoleKey.UpArrow:
				if (!IsCollision((playerX, playerY - 1))) playerY -= 1;
				break;
			case ConsoleKey.S or ConsoleKey.DownArrow:
				if (!IsCollision((playerX, playerY + 1))) playerY += 1;
				break;
			case ConsoleKey.A or ConsoleKey.LeftArrow:
				if (!IsCollision((playerX - 1, playerY))) playerX -= 1;
				break;
			case ConsoleKey.D or ConsoleKey.RightArrow:
				if (!IsCollision((playerX + 1, playerY))) playerX += 1;
				break;
		}

		playerPosition = (playerX, playerY);
	}

	private void ProcessMap()
	{
		isOnFinish = false;

		for (int y = 0; y < mapObjects.GetLength(0); y++)
		{
			for (int x = 0; x < mapObjects.GetLength(1); x++)
			{
				if (playerPosition == (x, y)) Console.Write('!');

				switch (mapObjects[y, x])
				{
					case MapObject.Unknown:
						if (playerPosition != (x, y)) Console.Write('?');
						break;

					case MapObject.Void:
						if (playerPosition != (x, y)) Console.Write(' ');
						break;

					case MapObject.Player:
						playerPosition ??= (x, y);
						if (playerPosition != (x, y)) Console.Write(' ');
						break;

					case MapObject.Finish:
						if (playerPosition == (x, y)) isOnFinish = true;
						if (playerPosition != (x, y)) Console.Write('=');
						break;

					case MapObject.Wall:
						if (playerPosition != (x, y)) Console.Write('@');
						break;

					case MapObject.FakeWall:
						if (playerPosition != (x, y)) Console.Write('@');
						break;

					case MapObject.Barrier:
						if (playerPosition != (x, y)) Console.Write(' ');
						break;
				}
			}

			Console.WriteLine();
		}
	}

	private bool IsCollision((int x, int y) coordinates)
	{
		bool isCollision = false;

		try
		{
			if (mapObjects[coordinates.y, coordinates.x] == MapObject.Wall
			|| mapObjects[coordinates.y, coordinates.x] == MapObject.Barrier)
				isCollision = true;
		}
		catch (IndexOutOfRangeException)
		{
			isCollision = true;
		}

		return isCollision;
	}
}
