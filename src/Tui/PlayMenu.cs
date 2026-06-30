// https://pws.yarb00.dev

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using PWSandbox.Formats;

namespace PWSandbox.Tui;

internal readonly record struct Position(int X, int Y);

internal sealed class PlayMenu(Map map) : IMenu
{
	private static readonly FrozenDictionary<MapObject, char> CharacterByMapObject = new Dictionary<
		MapObject,
		char
	>()
	{
		[MapObject.Unknown] = '?',
		[MapObject.Void] = '.',
		[MapObject.Player] = '!',
		[MapObject.Finish] = '=',
		[MapObject.Wall] = '@',
		[MapObject.FakeWall] = '@', // Same as Wall
		[MapObject.Barrier] = '.', // Same as Void
	}.ToFrozenDictionary();

	private readonly MapObject[,] mapObjects = map.Objects;

	private Position? playerPosition = null;

	bool shouldExit = false;

	bool isAtFinish = false;

	public void ShowDialog()
	{
		while (!shouldExit)
		{
			Console.Clear();

			Console.WriteLine($"===== PWSandbox.Tui [Play] =====");

			ProcessMap();

			if (isAtFinish)
			{
				Console.WriteLine();
				Console.WriteLine("You have reached the finish!");
			}

			Console.WriteLine();
			Console.WriteLine(
				"""
				Controls:

				Escape - back to main menu
				W/Up - move up
				D/Down - move down
				S/Left - move left
				D/Right - move right
				"""
			);

			ProcessKey(Console.ReadKey(intercept: true).Key);
		}
	}

	private void ProcessKey(ConsoleKey key)
	{
		if (key == ConsoleKey.Escape)
		{
			shouldExit = true;
			return;
		}

		if (playerPosition is null)
			return;

		(int playerX, int playerY) = (Position)playerPosition;

		switch (key)
		{
			case ConsoleKey.UpArrow or ConsoleKey.W when !IsCollision(playerX, playerY - 1):
				playerY -= 1;
				break;

			case ConsoleKey.DownArrow
			or ConsoleKey.S when !IsCollision(playerX, playerY + 1):
				playerY += 1;
				break;

			case ConsoleKey.LeftArrow
			or ConsoleKey.A when !IsCollision(playerX - 1, playerY):
				playerX -= 1;
				break;

			case ConsoleKey.RightArrow
			or ConsoleKey.D when !IsCollision(playerX + 1, playerY):
				playerX += 1;
				break;

			default:
				return;
		}

		playerPosition = new(playerX, playerY);
	}

	private void ProcessMap()
	{
		isAtFinish = false;

		for (int y = 0; y < mapObjects.GetLength(0); y++)
		{
			for (int x = 0; x < mapObjects.GetLength(1); x++)
			{
				switch (mapObjects[y, x])
				{
					case MapObject.Player:
						playerPosition ??= new Position(x, y);
						if (playerPosition != new Position(x, y))
							Console.Write(CharacterByMapObject[MapObject.Void]);
						break;

					case MapObject.Finish when playerPosition == new Position(x, y):
						isAtFinish = true;
						goto default;

					default:
						if (playerPosition != new Position(x, y))
							Console.Write(CharacterByMapObject[mapObjects[y, x]]);
						break;
				}

				if (playerPosition == new Position(x, y))
					Console.Write(CharacterByMapObject[MapObject.Player]);
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
