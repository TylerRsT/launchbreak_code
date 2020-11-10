using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicText : MonoBehaviour
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="spacing"></param>
        /// <returns></returns>
        public static DynamicText GenerateDynamicText(string text, Vector2 position, float spacing = 48)
        {
            return GenerateDynamicText(text, position, Color.white, spacing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="spacing"></param>
        /// <returns></returns>
        public static DynamicText GenerateDynamicText(string text, Vector2 position, Color color, float spacing = 48)
        {
            GameObject[] objList = new GameObject[text.Length];
            var midPos = text.Length / 2;

            var uiLayer = LayerMask.NameToLayer("HUD");

            var containerObj = new GameObject("DynamicTextContainer");
            containerObj.layer = uiLayer;
            var dynamicText = containerObj.AddComponent<DynamicText>();
            containerObj.transform.position = GameObject.FindObjectOfType<Canvas>().transform.position;

            for (int i = 0; i < text.Length; i++)
            {
                var multiplier = i - midPos;
                var charObj = new GameObject(text[i].ToString());
                charObj.layer = uiLayer;
                charObj.transform.parent = containerObj.transform;
                charObj.transform.position = position + new Vector2(position.x + (spacing * multiplier), position.y);
                charObj.AddComponent(typeof(SpriteRenderer));
                var renderer = charObj.GetComponent<SpriteRenderer>();
                renderer.sortingLayerID = SortingLayerHelper.GetSortingLayerValue(StaticSortingLayer.UIOverlay);
                renderer.material = Bootstrap.instance.data.colorSwapMaterial;
                renderer.material.SetColor("_DesiredColor", color);
                Sprite sprite;

                if(Bootstrap.instance.data.alphabetMap.TryGetValue(char.ToLower(text[i]), out sprite))
                {
                    renderer.sprite = sprite;
                }

                objList[i] = charObj;
            }

            foreach (var item in objList)
            {
                containerObj.GetComponent<DynamicText>().letters.Add(item);
            }

            return dynamicText;
        }

        #endregion

        #region Properties

        public List<GameObject> letters => _letters;

        #endregion

        #region Fields

        private List<GameObject> _letters = new List<GameObject>();

        #endregion
    }
}
