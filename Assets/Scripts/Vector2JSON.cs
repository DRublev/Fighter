using System;
using UnityEngine;
[Serializable]
public class Vector2JSON
{
    public Vector2JSON(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
    public int X;
    public int Y;
    public Vector2 ToVector2()
    {
        return new Vector2(X, Y);
    }

}
