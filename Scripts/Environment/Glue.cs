using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class Glue : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(GetComponent<ConstructionValidator>() != null)
            {
                return;
            }

            var character = collision.GetComponent<Character>();
            if(character != null && !_multipliersPerCharacter.ContainsKey(character) && !character.isGlued && collision is CapsuleCollider2D)
            {
                character.isGlued = true;
                character.speedMultipliers.Add(_speedMultiplier);
                _multipliersPerCharacter.Add(character, _speedMultiplier);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            var character = collision.GetComponent<Character>();
            if (character != null && character.isGlued && _multipliersPerCharacter.ContainsKey(character))
            {
                _multipliersPerCharacter.Remove(character);
                character.speedMultipliers.Remove(_speedMultiplier);
                character.isGlued = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void OnConstructed(ConstructLoadoutItem item)
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _speedMultiplier = 0.4f;

        private Dictionary<Character, float> _multipliersPerCharacter = new Dictionary<Character, float>();

        #endregion
    }
}