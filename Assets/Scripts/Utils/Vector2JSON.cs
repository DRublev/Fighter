﻿using System;

using UnityEngine;
namespace Assets.Scripts
{
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
        public override string ToString()
        {
            string result = string.Empty;
            result += X.ToString();
            result += "; ";
            result += Y.ToString();
            return result;
        }

    }
}
