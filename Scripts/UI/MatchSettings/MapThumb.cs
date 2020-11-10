using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class MapThumb : MonoBehaviour
    {
        #region Const

        private const string RandomMapInfoName = "Random";

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public MapInfo mapInfo
        {
            get
            {
                return _mapInfo;
            }
            set
            {
                _mapInfo = value;
                if(value != null && value.mapIcon != null)
                {
                    var image = GetComponent<Image>();
                    image.sprite = value.mapIcon;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isRandom => _mapInfo.name == RandomMapInfoName;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private MapInfo _mapInfo = default;

        #endregion
    }
}