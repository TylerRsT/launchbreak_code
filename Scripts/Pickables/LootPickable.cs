using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class LootPickable : Pickable
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerUp"></param>
        public virtual void Initialize(LootableDescriptor lootableDescriptor)
        {
            isPickable = false;

            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = lootableDescriptor.icon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        protected void SpawnPickupText(string text)
        {
            if(!LootTextComponent.canSpawn)
            {
                return;
            }

            var textMesh = new GameObject().AddComponent<TextMeshProUGUI>();

            textMesh.transform.rotation = Quaternion.identity;
            textMesh.transform.position = transform.position;
            textMesh.transform.SetParent(FindObjectOfType<Canvas>().transform);
            textMesh.text = text;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontSize = 16.0f;
            textMesh.autoSizeTextContainer = true;

            textMesh.gameObject.AddComponent<LootTextComponent>().AnimateText();
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class LootTextComponent : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            StopAllCoroutines();
            TweenExtensions.SafeKill(ref _tween);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void AnimateText()
        {
            StartCoroutine(DoAnimateText());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator DoAnimateText()
        {
            _tween = transform.DOLocalMoveY(transform.localPosition.y + 20.0f, 0.25f);
            yield return _tween.WaitForCompletion();

            yield return new WaitForSeconds(1.0f);

            _tween = transform.GetComponent<TextMeshProUGUI>().DOFade(0.0f, 0.25f);
            yield return _tween.WaitForCompletion();

            Destroy(gameObject);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static bool canSpawn { get; set; } = true;

        #endregion

        #region Fields

        private Tween _tween;

        #endregion
    }
}