using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [HideInInspector]
    public sealed class GrenadeExplosion : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (_updateCount % frequency == 0)
            {
                var index = _updateCount / frequency;

                if (index >= explosions.Count)
                {
                    Destroy(gameObject);
                    return;
                }

                foreach (var pos in explosions[index])
                {
                    GameplayStatics.SpawnFireAndForgetAnimation(explosionAnimation, pos, Quaternion.identity)
                        .GetComponent<SpriteRenderer>().sortingLayerID = SortingLayerHelper.GetSortingLayerValue(StaticSortingLayer.Default);
                }
            }

            ++_updateCount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public List<List<Vector2>> explosions { get; private set; } = new List<List<Vector2>>();

        /// <summary>
        /// 
        /// </summary>
        public int frequency { get; set; } = 1;
        
        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation explosionAnimation { get; set; }

        #endregion

        #region Fields

        private int _updateCount = 0;

        #endregion
    }
}