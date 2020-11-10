using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CharacterTransformSorter : MonoBehaviour
    {
        #region Const

        private const int DefaultCharacterOrder = 2;

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            var ordered = _tileObjectRenderers.Where(x => x != null).OrderByDescending(x => x.transform.position.y).ToArray();
            for(var i = 0; i < ordered.Length; ++i)
            {
                ordered[i].additive = DefaultCharacterOrder + i;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public void AddCharacter(Character character)
        {
            _tileObjectRenderers.Add(character.GetComponent<TileObjectRenderer>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public void RemoveCharacter(Character character)
        {
            _tileObjectRenderers.Remove(character.GetComponent<TileObjectRenderer>());
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private HashSet<TileObjectRenderer> _tileObjectRenderers { get; } = new HashSet<TileObjectRenderer>();

        #endregion
    }
}