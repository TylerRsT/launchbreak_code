using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class MathExtensions
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Mod(int a, int b)
        {
            while(a < 0)
            {
                a += b;
            }
            return a % b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Mod(float a, float b)
        {
            while(a < b)
            {
                a += b;
            }
            return a % b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color MakeTransparent(this Color color)
        {
            return ChangeOpacity(color, 0.0f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color ChangeOpacity(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 point, float radians)
        {
            float s = Mathf.Sin(radians);
            float c = Mathf.Cos(radians);
            return new Vector2(
                point.x * c - point.y * s,
                point.y * c + point.x * s
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static float RandOne()
        {
            var i = Random.Range(0, 2);
            return i == 0 ? 1.0f : -1.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreSameSign(float a, float b)
        {
            if(a == 0.0f || b == 0.0f)
            {
                return true;
            }

            return (a < 0.0f && b < 0.0f) || (a > 0.0f && b > 0.0f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreSameSign(int a, int b)
        {
            if (a == 0 || b == 0)
            {
                return true;
            }

            return (a < 0 && b < 0) || (a > 0 && b > 0);
        }

        #endregion
    }
}