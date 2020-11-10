using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public enum StaticSortingLayer
    {
        Floor,
        AboveFloor,
        Spawner,
        KeySlotTop,
        KeyAtSpawner,
        KeySlotBottom,
        AboveSpawner,

        Default,
        Top,
        Pickable,
        Weapon,
        Particles,
        CharacterHUD,
        UI,
        UIOverlay,
    }

    /// <summary>
    /// 
    /// </summary>
    public static class SortingLayerHelper
    {
        #region Const

        public const string LightLayerFormat = "LightLayer{0}";

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        [Pure]
        public static int GetSortingLayerValue(string layerName)
        {
            return SortingLayer.NameToID(layerName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortingLayer"></param>
        /// <returns></returns>
        [Pure]
        public static int GetSortingLayerValue(StaticSortingLayer sortingLayer)
        {
            return GetSortingLayerValue(sortingLayer.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Pure]
        public static int GetLightSortingLayerValue(int number)
        {
            return GetSortingLayerValue(string.Format(LightLayerFormat, number));
        }

        #endregion
    }
}