using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LootableDescriptor : ScriptableObject
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public new string name => _name;

        /// <summary>
        /// 
        /// </summary>
        public Sprite icon => _icon;

        /// <summary>
        /// 
        /// </summary>
        public AILootInfo aiLootInfo => _aiLootInfo;

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
        private Sprite _icon = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private AILootInfo _aiLootInfo = new AILootInfo();

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class AILootInfo
    {
        public int interestValue;
    }
}