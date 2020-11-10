using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "CONS_Name", menuName = "Shovel/Construct Loadout Item")]
    public class ConstructLoadoutItem : LoadoutItem
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void Load()
        {
            gameObjectPrefab = Resources.Load<GameObject>(_gameObjectPrefabPath);
            Debug.Assert(gameObjectPrefab != null);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int supplyCost => _supplyCost;

        /// <summary>
        /// 
        /// </summary>
        public int health => _health;

        /// <summary>
        /// 
        /// </summary>
        public float lifetime => _lifetime;

        /// <summary>
        /// 
        /// </summary>
        public GameObject gameObjectPrefab { get; private set; }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _supplyCost = 0;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _health = 0;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _lifetime = 0.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _gameObjectPrefabPath = string.Empty;

        #endregion
    }
}