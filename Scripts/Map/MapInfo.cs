using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "MAPINF_Name", menuName = "Shovel/Map Info")]
    public class MapInfo : ScriptableObject
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static MapInfo New(string name, string sceneName)
        {
            var mapInfo = CreateInstance<MapInfo>();
            mapInfo._name = name;
            mapInfo._sceneName = sceneName;
            return mapInfo;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public new string name => _name;

        /// <summary>
        /// 
        /// </summary>
        public Sprite mapBackground => _mapBackground;

        /// <summary>
        /// 
        /// </summary>
        public Sprite mapThumbnail => _mapThumbnail;

        /// <summary>
        /// 
        /// </summary>
        public Sprite mapIcon => _mapIcon;

        /// <summary>
        /// 
        /// </summary>
        public string sceneName => _sceneName;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _name = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _mapBackground = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _mapThumbnail = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _mapIcon = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _sceneName = string.Empty;

        #endregion
    }
}