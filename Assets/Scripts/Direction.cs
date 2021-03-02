using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North, East, South, West, Up, Down

}
public static class DirectionExt
{
    public static Direction Next(this Direction dir) //order: norh, east, south, west, 
    {
        switch(dir)
        {
            case Direction.North:
                return Direction.East;
            case Direction.East:
                return Direction.South;
            case Direction.South:
                return Direction.West;
            case Direction.West:
                return Direction.North;
        }
        return Direction.North;
    }

    public static Direction FullNext(this Direction dir) //order: norh, east, south, west, up, down 
    {
        switch (dir)
        {
            case Direction.North:
                return Direction.East;
            case Direction.East:
                return Direction.South;
            case Direction.South:
                return Direction.West;
            case Direction.West:
                return Direction.Up;
            case Direction.Up:
                return Direction.Down;
        }
        return Direction.North;
    }

    public static Vector3 getVector(this Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                return new Vector3(0, 0, 1);
            case Direction.East:
                return new Vector3(1, 0, 0);
            case Direction.South:
                return new Vector3(0, 0, -1);
            case Direction.West:
                return new Vector3(-1, 0, 0);
            case Direction.Up:
                return new Vector3(0, 1, 0);
            default:
                return new Vector3(0, -1, 0);
        }
    }
}