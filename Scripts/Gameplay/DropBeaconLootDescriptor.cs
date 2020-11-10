using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "LOOT_Name", menuName = "Shovel/Drop Beacon Loot Descriptor")]
    public class DropBeaconLootDescriptor : ScriptableObject
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<LootableItem> lootableItems => _lootableItems;
        
        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<LootableItem> _lootableItems = new List<LootableItem>();

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public struct LootableItem
    {
        public LootableDescriptor descriptor;
        public float odds;
    }
}