using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class WalkCollider : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            transform.parent.SendMessage(nameof(OnCollisionEnter2D), collision, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionStay2D(Collision2D collision)
        {
            transform.parent.SendMessage(nameof(OnCollisionStay2D), collision, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionExit2D(Collision2D collision)
        {
            transform.parent.SendMessage(nameof(OnCollisionExit2D), collision, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            transform.parent.SendMessage(nameof(OnTriggerEnter2D), collision, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerStay2D(Collider2D collision)
        {
            transform.parent.SendMessage(nameof(OnTriggerStay2D), collision, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            transform.parent.SendMessage(nameof(OnTriggerExit2D), collision, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulletResponse"></param>
        private void OnReceivingBullet(BulletResponse bulletResponse)
        {
            transform.parent.SendMessage(nameof(OnReceivingBullet), bulletResponse, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashAttack"></param>
        private void OnReceivingDashAttack(CharacterDashAttack dashAttack)
        {
            dashAttack.Accept();
            transform.parent.SendMessage(nameof(OnReceivingDashAttack), dashAttack, SendMessageOptions.DontRequireReceiver);
        }

        #endregion
    }
}