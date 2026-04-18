// https://pws.yarb00.dev

using System;
using System.Collections.Generic;

namespace PWSandbox.Tui;

internal readonly record struct Position(int X, int Y);

internal class PlayMenu(Map map) : IMenu
{
	private static readonly Dictionary<MapObject, char> CharacterByMapObject = new()
	{
		[MapObject.Unknown] = '?',
		[MapObject.Void] = '.',
		[MapObject.Player] = '!',
		[MapObject.Finish] = '=',
		[MapObject.Wall] = '@',
		[MapObject.FakeWall] = '@', // Same color as Wall
		[MapObject.Barrier] = '.' // Same color as Void
	};

	private readonly MapObject[,] mapObjects = map.Objects;

	private Position? playerPosition = null;

	bool isExit = false, isOnFinish = false;

	public void Show()
	{
		while (!isExit)
		{
			Console.Clear();

			Console.WriteLine($"===== PWSandbox.Tui [Play] =====");

			ProcessMap();

			if (isOnFinish)
			{
				Console.WriteLine();
				Console.WriteLine("You have reached the finish!");
			}

			Console.WriteLine();
			Console.WriteLine("""
				Controls:

				Escape - back to main menu
				W/Up - move up
				D/Down - move down
				S/Left - move left
				D/Right - move right
				""");

			ProcessKey(Console.ReadKey(true).Key);
		}
	}

	private void ProcessKey(ConsoleKey key)
	{
		if (key == ConsoleKey.Escape)
		{
			isExit = true;
			return;
		}

		if (playerPosition is null) return;
		(int playerX, int playerY) = (Position)playerPosition;

		switch (key)
		{
			case ConsoleKey.W or ConsoleKey.UpArrow:
				if (!IsCollision(playerX, playerY - 1)) playerY -= 1;
				break;

			case ConsoleKey.S or ConsoleKey.DownArrow:
				if (!IsCollision(playerX, playerY + 1)) playerY += 1;
				break;

			case ConsoleKey.A or ConsoleKey.LeftArrow:
				if (!IsCollision(playerX - 1, playerY)) playerX -= 1;
				break;

			case ConsoleKey.D or ConsoleKey.RightArrow:
				if (!IsCollision(playerX + 1, playerY)) playerX += 1;
				break;

			default:
				return;
		}

		playerPosition = new(playerX, playerY);
	}

	private void ProcessMap()
	{
		isOnFinish = false;

		for (int y = 0; y < mapObjects.GetLength(0); y++)
		{
			for (int x = 0; x < mapObjects.GetLength(1); x++)
			{
				switch (mapObjects[y, x])
				{
					case MapObject.Player:
						playerPosition ??= new Position(x, y);
						if (playerPosition != new Position(x, y)) Console.Write(CharacterByMapObject[MapObject.Void]);
						break;

					case MapObject.Finish when playerPosition == new Position(x, y):
						isOnFinish = true;
						goto default;

					default:
						if (playerPosition != new Position(x, y)) Console.Write(CharacterByMapObject[mapObjects[y, x]]);
						break;
				}

				if (playerPosition == new Position(x, y)) Console.Write(CharacterByMapObject[MapObject.Player]);
			}

			Console.WriteLine();
		}
	}

	private bool IsCollision(int x, int y) => IsCollision(new(x, y));

	private bool IsCollision(Position coordinates)
	{
		MapObject @object;
		try
		{
			@object = mapObjects[coordinates.Y, coordinates.X];
		}
		catch (IndexOutOfRangeException)
		{
			return true;
		}

		return @object is MapObject.Wall or MapObject.Barrier;
	}
}
