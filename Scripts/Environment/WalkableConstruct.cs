using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Construct))]
    public class WalkableConstruct : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (this.IsInConstruction())
            {
                return;
            }

            if(collision.GetComponent<Character>() != null)
            {
                _health = -1;
                if(_health <= 0 && !_beingDestroyed)
                {
                    _beingDestroyed = true;
                    this.DestroyConstruct();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void OnConstructed(ConstructLoadoutItem item)
        {
            _health = item.health;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _health = 0;

        private bool _beingDestroyed = false;

        #endregion
    }
}