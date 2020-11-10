using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConstructComponentExtensions
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        public static void DestroyConstruct(this Component component)
        {
            DestroyConstruct(component.gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        public static void DestroyConstruct(this GameObject gameObject)
        {
            gameObject.GetComponent<Construct>()?.BeginDestroy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool IsInConstruction(this Component component)
        {
            return IsInConstruction(component.gameObject);
        }

        public static bool IsInConstruction(this GameObject gameObject)
        {
            return gameObject.GetComponent<ConstructionValidator>() != null;
        }

        #endregion
    }
}