using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LoadoutItem : ScriptableObject
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public abstract void Load();

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public new string name => _name;

        /// <summary>
        /// 
        /// </summary>
        public string description => _description;

        /// <summary>
        /// 
        /// </summary>
        public Sprite loadoutIcon => _loadoutIcon;

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
        private string _description = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _loadoutIcon = default;

        #endregion
    }
}