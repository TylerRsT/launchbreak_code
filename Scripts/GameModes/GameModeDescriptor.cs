using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "GM_Name", menuName = "Shovel/Game Mode Descriptor")]
    public class GameModeDescriptor : ScriptableObject
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public new string name => _name;

        /// <summary>
        /// 
        /// </summary>
        public GameObject gameModePrefab => _gameModePrefab;

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
        private GameObject _gameModePrefab = default;

        #endregion
    }
}