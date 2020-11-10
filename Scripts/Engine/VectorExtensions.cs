using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class VectorExtensions
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector2 Straighten(this Vector2 vec)
        {
            if(vec == Vector2.zero)
            {
                return vec;
            }

            var absoluteVec = new Vector2(Mathf.Abs(vec.x), Mathf.Abs(vec.y));
            if(absoluteVec.x == absoluteVec.y)
            {
                return vec;
            }

            if(absoluteVec.x > absoluteVec.y)
            {
                if(vec.x > 0.0f)
                {
                    vec.x = 1.0f;
                }
                else
                {
                    vec.x = -1.0f;
                }
                vec.y = 0.0f;
            }
            else
            {
                if(vec.y > 0.0f)
                {
                    vec.y = 1.0f;
                }
                else
                {
                    vec.y = -1.0f;
                }
                vec.x = 0.0f;
            }
            return vec;
        }

        #endregion
    }
}