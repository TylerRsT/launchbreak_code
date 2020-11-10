using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class DamageReceiver : DamageComponent
    {
        #region Const

        private const string OnReceiveDamagesMethodName = "OnReceiveDamages";
        private const string OnNoMoreHealthMethodName = "OnNoMoreHealth";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageInfo"></param>
        public override void TakeDamages(DamageInfo damageInfo)
        {
            _health = Mathf.Max(0, _health - damageInfo.damages);
            SendMessage(OnReceiveDamagesMethodName, damageInfo.damages, SendMessageOptions.DontRequireReceiver);

            if (_canDie && _health == 0)
            {
                SendMessage(OnNoMoreHealthMethodName, SendMessageOptions.DontRequireReceiver);
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _health = 0;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _canDie = true;

        #endregion
    }
}