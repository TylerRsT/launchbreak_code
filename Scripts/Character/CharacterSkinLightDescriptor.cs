using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "SKIN_Light_Name", menuName = "Shovel/Character Skin Light Descriptor")]
    public class CharacterSkinLightDescriptor : ScriptableObject
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            _skinDescriptor = Resources.Load<CharacterSkinDescriptor>(_skinDescriptorPath);
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
        public Sprite selectSprite => _selectSprite;

        /// <summary>
        /// 
        /// </summary>
        public Sprite bannerSprite => _bannerSprite;

        /// <summary>
        /// 
        /// </summary>
        public string skinDescriptorPath => _skinDescriptorPath;

        /// <summary>
        /// 
        /// </summary>
        public CharacterSkinDescriptor skinDescriptor => _skinDescriptor;

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
        private Sprite _selectSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _bannerSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _skinDescriptorPath = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private CharacterSkinDescriptor _skinDescriptor;

        #endregion
    }
}