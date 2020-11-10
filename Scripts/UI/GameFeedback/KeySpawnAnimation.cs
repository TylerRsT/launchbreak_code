using DG.Tweening;
using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class KeySpawnAnimation : MonoBehaviour
    {
        #region Const

        private const string UILayerName = "HUD";

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public void StartAnimation(System.Action callback = null)
        {
            StartCoroutine(StartAnimationInternal(callback));
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator StartAnimationInternal(System.Action callback)
        {
            // Set variables needed

            var canvas = GameObject.FindObjectOfType<Canvas>();

            var x = canvas.transform.position.x;
            var y = canvas.transform.position.y;

            var launchKey = new GameObject("LaunchKey");
            launchKey.layer = LayerMask.NameToLayer(UILayerName);
            launchKey.AddComponent<SpriteAnimator>();
            var renderer = launchKey.GetComponent<SpriteRenderer>();
            renderer.sortingLayerID = SortingLayerHelper.GetSortingLayerValue(StaticSortingLayer.UIOverlay);
            renderer.sprite = _keyLetterSprite;
            launchKey.transform.position = new Vector2(launchKey.transform.position.x + 512, launchKey.transform.position.y);
                

            _animationSequence = DOTween.Sequence();
            _animationSequence.Append(launchKey.transform.DOBlendableLocalMoveBy(new Vector3(x - 570, y), 1f));

            yield return _animationSequence.WaitForCompletion();

            var keyLetterAnim = launchKey.GetComponent<SpriteAnimator>();
            keyLetterAnim.Play(_letterToKeyAnimation, true);

            keyLetterAnim.onFinish.AddListener(() => StartCoroutine(FirstKeyPlacement(launchKey, callback)));

            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        private IEnumerator FirstKeyPlacement(GameObject key, System.Action callback)
        {
            // Set Variables

            var keyPos = key.transform.position;

            var canvas = GameObject.FindObjectOfType<Canvas>();

            var x = canvas.transform.position.x;
            var y = canvas.transform.position.y;

            var fakeLaunchKey = new GameObject();
            fakeLaunchKey.layer = LayerMask.NameToLayer(UILayerName);
            fakeLaunchKey.transform.position = keyPos;
            var spriteAnimator = fakeLaunchKey.AddComponent<SpriteAnimator>();
            var renderer = fakeLaunchKey.GetComponent<SpriteRenderer>();
            renderer.sortingLayerID = renderer.sortingLayerID = SortingLayerHelper.GetSortingLayerValue(StaticSortingLayer.UIOverlay);

            Destroy(key);

            // Start Sequence

            spriteAnimator.Play(_keySpinAnimation);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(fakeLaunchKey.transform.DOJump(new Vector3(x, y, 0), 20.0f, 1, 0.7f));
            sequence.AppendCallback(() => spriteAnimator.StopAtFrame(9));
            sequence.AppendCallback(() => Destroy(fakeLaunchKey));

            yield return sequence.WaitForCompletion();

            callback?.Invoke();
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _keyLetterSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _letterToKeyAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _keySpinAnimation = default;

        private Sequence _animationSequence;

        #endregion
    }
}

