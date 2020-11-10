using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class BulletReaction : MonoBehaviour
    {
        #region Const

        private const string OnReceivingBulletMethodName = "OnReceivingBullet";

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulletResponse"></param>
        public void SendBulletReceive(BulletResponse bulletResponse)
        {
            SendMessage(OnReceivingBulletMethodName, bulletResponse, SendMessageOptions.DontRequireReceiver);
            if (_acceptBullets)
            {
                bulletResponse.received = true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool acceptBullets
        {
            get { return _acceptBullets; }
            set { _acceptBullets = value; }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _acceptBullets = true;

        #endregion
    }
}