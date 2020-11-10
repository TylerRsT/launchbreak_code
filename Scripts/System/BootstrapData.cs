using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "Bootstrap", menuName = "Shovel/Bootstrap Data")]
    public class BootstrapData : ScriptableObject
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            UpdateAlphabetMap();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnValidate()
        {
            UpdateAlphabetMap();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void UpdateAlphabetMap()
        {
            _alphabetMap.Clear();
            foreach(var entry in _alphabetEntryMap)
            {
                if (!string.IsNullOrWhiteSpace(entry.name))
                {
                    _alphabetMap[entry.name[0]] = entry.value;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<CharacterDescriptor> characters => _characters;

        /// <summary>
        /// 
        /// </summary>
        public ConstructLoadoutItem defaultConstruct => _defaultConstruct;

        /// <summary>
        /// 
        /// </summary>
        public IList<ConstructLoadoutItem> constructs => _constructs;

        /// <summary>
        /// 
        /// </summary>
        public IList<AbilityLoadoutItem> abilities => _abilities;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<GameModeDescriptor> gameModes => _gameModes;

        /// <summary>
        /// 
        /// </summary>
        public ShakePreset keyRedeemShake => _keyRedeemShake;

        /// <summary>
        /// 
        /// </summary>
        public GameObject supplyPickablePrefab => _supplyPickablePrefab;

        /// <summary>
        /// 
        /// </summary>
        public GameObject weaponPickablePrefab => _weaponPickablePrefab;

        /// <summary>
        /// 
        /// </summary>
        public GameObject titleScreenPanelPrefab => _titleScreenPanelPrefab;

        /// <summary>
        /// 
        /// </summary>
        public GameObject pausePanelPrefab => _pausePanelPrefab;

        /// <summary>
        /// 
        /// </summary>
        public GameObject rematchPanelPrefab => _rematchPanelPrefab;

        /// <summary>
        /// 
        /// </summary>
        public GameObject settingsPanelPrefab => _settingsPanelPrefab;

        /// <summary>
        /// 
        /// </summary>
        public GameObject controlsSchemePanelPrefab => _controlsSchemePanelPrefab;


        /// <summary>
        /// 
        /// </summary>
        public GameObject emptyTileSpritePrefab => _emptyTileSpritePrefab;

        /// <summary>
        /// 
        /// </summary>
        public Material spriteDefaultMaterial => _spriteDefaultMaterial;

        /// <summary>
        /// 
        /// </summary>
        public Material spriteFlashMaterial => _spriteFlashMaterial;

        /// <summary>
        /// 
        /// </summary>
        public Material colorSwapMaterial => _colorSwapMaterial;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<char, Sprite> alphabetMap => _alphabetMap;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<CharacterDescriptor> _characters = new List<CharacterDescriptor>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ConstructLoadoutItem _defaultConstruct = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<ConstructLoadoutItem> _constructs = new List<ConstructLoadoutItem>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<AbilityLoadoutItem> _abilities = new List<AbilityLoadoutItem>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<GameModeDescriptor> _gameModes = new List<GameModeDescriptor>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ShakePreset _keyRedeemShake = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _supplyPickablePrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _weaponPickablePrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _titleScreenPanelPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _pausePanelPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _rematchPanelPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _settingsPanelPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _controlsSchemePanelPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _emptyTileSpritePrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Material _spriteDefaultMaterial = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Material _spriteFlashMaterial = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Material _colorSwapMaterial = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<AlphabetEntry> _alphabetEntryMap = new List<AlphabetEntry>();

        private Dictionary<char, Sprite> _alphabetMap = new Dictionary<char, Sprite>();

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class AlphabetEntry
    {
        public string name;
        public Sprite value;
    }
}