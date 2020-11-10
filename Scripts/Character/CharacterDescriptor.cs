using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "CHAR_Name", menuName = "Shovel/Character Descriptor")]
    public class CharacterDescriptor : ScriptableObject
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public new string name => _name;

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<CharacterSkinLightDescriptor> skinDescriptors => _skinDescriptors;

        /// <summary>
        /// 
        /// </summary>
        public Color mainColor => _mainColor;

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
        private List<CharacterSkinLightDescriptor> _skinDescriptors = new List<CharacterSkinLightDescriptor>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Color _mainColor = default;

        #endregion
    }
}