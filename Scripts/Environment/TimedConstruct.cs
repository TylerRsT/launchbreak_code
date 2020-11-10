using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    [RequireComponent(typeof(Construct))]
    public class TimedConstruct : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            _elapsed += GameplayStatics.gameDeltaTime;

            if(_duration > 0.0f && _elapsed >= _duration)
            {
                this.DestroyConstruct();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void OnConstructed(ConstructLoadoutItem item)
        {
            _duration = item.lifetime;
        }

        #endregion

        #region Fields

        [SerializeField]
        private float _duration = 0.0f;

        private float _elapsed = 0.0f;

        #endregion
    }
}