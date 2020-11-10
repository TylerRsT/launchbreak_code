using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class OccluderTrigger : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<Character>() == true)
            {
                MoveToLayer(collision.transform, LayerMask.NameToLayer("CharacterNoOcclusion"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<Character>() == true)
            {
                MoveToLayer(collision.transform, LayerMask.NameToLayer("Character"));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="layer"></param>
        private void MoveToLayer(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform child in root)
                MoveToLayer(child, layer);
        }

        #endregion
    }
}
