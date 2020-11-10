using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerScoredAnimation : MonoBehaviour
    {
        #region Const

        private const string ScoredText = "SCORED!";
        private const string WinsText = "WINS!";

        private const string PlayerScoredSoundKey = "PlayerScored";
        private const string PlayerWinsSoundKey = "PlayerWins";

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="color"></param>
        /// <param name="win"></param>
        /// <returns></returns>
        public void StartAnimation(string playerName, Color color, bool win)
        {
            StartCoroutine(StartAnimationInternal(playerName, color, win));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="color"></param>
        /// <param name="win"></param>
        /// <returns></returns>
        private IEnumerator StartAnimationInternal(string playerName, Color color, bool win)
        {
            yield return new WaitForSeconds(1.0f);

            var soundInstances = GameMode.instance.EmitSound(win ? PlayerWinsSoundKey : PlayerScoredSoundKey);
            soundInstances.RegisterAsGameplayLooping();

            // Set variables needed

            var canvas = GameObject.FindObjectOfType<Canvas>();
            var charSpacing = 52;

            var x = canvas.transform.position.x;
            var y = canvas.transform.position.y;

            var playerNameContainer = DynamicText.GenerateDynamicText(playerName, new Vector2(canvas.transform.position.x - 512, canvas.transform.position.y + 32), color, charSpacing);
            var scoredContainer = DynamicText.GenerateDynamicText(win ? WinsText : ScoredText, new Vector2(canvas.transform.position.x + 512, canvas.transform.position.y), charSpacing);

            // Start Animation Sequence

            _animationSequence = DOTween.Sequence();

            for (int i = 0; i < playerNameContainer.letters.Count; i++)
            {
                _animationSequence.Join(playerNameContainer.letters[i].transform.DOBlendableLocalMoveBy(new Vector3(x + 1024, y), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);
            }

            for (int i = 0; i < scoredContainer.letters.Count; i++)
            {
                _animationSequence.Join(scoredContainer.letters[i].transform.DOBlendableLocalMoveBy(new Vector3(x - 1024, y), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);
            }

            yield return new WaitForSeconds(1.5f);

            _animationSequence = DOTween.Sequence();

            for (int i = 0; i < playerNameContainer.letters.Count; i++)
            {
                _animationSequence.Join(playerNameContainer.letters[i].transform.DOBlendableLocalMoveBy(new Vector3(x + 1024, y), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);
            }

            for (int i = 0; i < scoredContainer.letters.Count; i++)
            {
                _animationSequence.Join(scoredContainer.letters[i].transform.DOBlendableLocalMoveBy(new Vector3(x - 1024, y), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);
            }

            yield return _animationSequence.WaitForCompletion();

            // Destroy TextContainer Objects
            
            Destroy(playerNameContainer.gameObject);
            Destroy(scoredContainer.gameObject);

            soundInstances.UnregisterAsGameplayLooping();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string TempPlayerName => _tempPlayerName;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _tempPlayerName = default;

        private Sequence _animationSequence;

        #endregion
    }
}
