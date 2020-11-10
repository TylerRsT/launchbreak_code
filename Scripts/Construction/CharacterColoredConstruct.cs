using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    [RequireComponent(typeof(Construct))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterColoredConstruct : MonoBehaviour
    {
        #region Const

        private const string DesiredColorParamName = "_DesiredColor";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void Start()
        {
            var construct = GetComponent<Construct>();
            if(construct.instigator == null)
            {
                return;
            }

            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.material = Bootstrap.instance.data.colorSwapMaterial;
            spriteRenderer.material.SetColor(DesiredColorParamName, construct.instigator.characterDescriptor.mainColor);

            var hitFlash = GetComponent<HitFlash>();
            if (hitFlash != null)
            {
                hitFlash.defaultMaterial = spriteRenderer.material;
            }
        }

        #endregion
    }
}