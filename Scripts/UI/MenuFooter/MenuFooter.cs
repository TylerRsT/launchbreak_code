using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class MenuFooter : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            var now = System.DateTime.Now;
            _timeText.text = $"{now.Hour.ToString("00")}:{now.Minute.ToString("00")}";
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _timeText = default;

        #endregion
    }
}