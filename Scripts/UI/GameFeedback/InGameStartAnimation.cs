using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Elendow.SpritedowAnimator;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class InGameStartAnimation : MonoBehaviour
    {
        #region Const

        private string KeySpinSoundKey = "KeySpin";
        private string KeyDropSoundKey = "KeyDrop";
        private string GameStartSoundKey = "GameStart";

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
            var soundInstances = GameMode.instance.EmitSound(GameStartSoundKey);
            soundInstances.RegisterAsGameplayLooping();


            // Set variables needed

            var canvas = GameObject.FindObjectOfType<Canvas>();

            var x = canvas.transform.position.x;
            var y = canvas.transform.position.y;

            var readyContainer = DynamicText.GenerateDynamicText("GET RE DY", new Vector2(canvas.transform.position.x + 512, canvas.transform.position.y + 16), 52);
            var pContainer = DynamicText.GenerateDynamicText("P", new Vector2(canvas.transform.position.x - 512, canvas.transform.position.y + 16), 52);
            var nicContainer = DynamicText.GenerateDynamicText("NIC !", new Vector2(canvas.transform.position.x + 512, canvas.transform.position.y + 16), 52);

            // Add Text Container to a List to destroy later

            var textContainerList = new List<GameObject>();
            textContainerList.Add(readyContainer.gameObject);
            textContainerList.Add(pContainer.gameObject);
            textContainerList.Add(nicContainer.gameObject);

            GameObject keyLetter = null;

            // Replace 6th ("A") by the key graphic and join all the letters to the center

            for (int i = 0; i < readyContainer.letters.Count; i++)
            {
                if(i == 6)
                {
                    keyLetter = readyContainer.letters[i];
                    keyLetter.AddComponent<SpriteAnimator>();
                    var renderer = keyLetter.GetComponent<SpriteRenderer>();
                    renderer.sprite = _keyLetterSprite;
                    readyContainer.letters[i].transform.position = new Vector2(readyContainer.letters[i].transform.position.x, readyContainer.letters[i].transform.position.y -21);      
                }

                _animationSequence = DOTween.Sequence();
                _animationSequence.Join(readyContainer.letters[i].transform.DOBlendableLocalMoveBy(new Vector3(x - 1024, y), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);
            }

            yield return new WaitForSeconds(1);

            // Explode "GET READY" and replace with "PANIC"

            _animationSequence = DOTween.Sequence();

            for (int i = 0; i < readyContainer.letters.Count; i++)
            {
                if (i == 6)
                {
                    continue;
                }

                _animationSequence.Join(readyContainer.letters[i].transform.DOBlendableLocalMoveBy(new Vector3(x + RandomPosition(), y + RandomPosition()), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);
            }

            _animationSequence.Join(pContainer.letters[0].transform.DOBlendableLocalMoveBy(new Vector3(x + 928, y), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);

            for (int i = 0; i < nicContainer.letters.Count; i++)
            {
                _animationSequence.Join(nicContainer.letters[i].transform.DOBlendableLocalMoveBy(new Vector3(x - 916, y), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);
            }

            _animationSequence.Join(keyLetter.transform.DOBlendableLocalMoveBy(new Vector3(x - 156, 0), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);

            yield return new WaitForSeconds(1f);

            // Shake "PANIC"

            _animationSequence = DOTween.Sequence();

            _animationSequence.Join(pContainer.letters[0].transform.DOShakePosition(1f, 20f, 50));

            for (int i = 0; i < nicContainer.letters.Count; i++)
            {
                _animationSequence.Join(nicContainer.letters[i].transform.DOShakePosition(1f, 20f, 50));
            }

            _animationSequence.Join(keyLetter.transform.DOShakePosition(1f, 20f, 50));


            yield return new WaitForSeconds(1f);

            // Send every remaning letter except the key out of the screen

            _animationSequence = DOTween.Sequence();

            _animationSequence.Join(pContainer.letters[0].transform.DOBlendableLocalMoveBy(new Vector3(x - 928, y), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);

            for (int i = 0; i < nicContainer.letters.Count; i++)
            {
                _animationSequence.Join(nicContainer.letters[i].transform.DOBlendableLocalMoveBy(new Vector3(x + 972, y), 1f)).SetEase(Ease.OutBounce, 10f, 0.1f);
            }

            var keyLetterAnim = keyLetter.GetComponent<SpriteAnimator>();
            keyLetterAnim.Play(_letterToKeyAnimation, true);

            keyLetterAnim.onFinish.AddListener(() => StartCoroutine(FirstKeyPlacement(keyLetter, textContainerList, callback)));

            soundInstances.UnregisterAsGameplayLooping();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        private IEnumerator FirstKeyPlacement(GameObject key, List<GameObject> objectToDestroy, System.Action callback)
        {
            // Set Variables

            var keyPos = key.transform.position;

            var fakeLaunchKey = new GameObject();
            fakeLaunchKey.transform.position = keyPos;
            var spriteAnimator = fakeLaunchKey.AddComponent<SpriteAnimator>();

            Destroy(key);

            // Start Sequence

            spriteAnimator.Play(_keySpinAnimation);

            var soundInstances = new List<FMOD.Studio.EventInstance>(GameMode.instance.EmitSound(KeySpinSoundKey));
            soundInstances.RegisterAsGameplayLooping();

            Sequence sequence = DOTween.Sequence();

            sequence.Append(fakeLaunchKey.transform.DOJump(new Vector2(transform.position.x, transform.position.y + 8), 15.0f, 1, 0.4f));
            sequence.AppendCallback(() => spriteAnimator.StopAtFrame(9));
            sequence.AppendCallback(() =>
            {
                foreach (var item in soundInstances)
                {
                    item.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                }
            });
            sequence.AppendCallback(() => soundInstances.UnregisterAsGameplayLooping());
            sequence.AppendCallback(() => Destroy(fakeLaunchKey));
            sequence.AppendCallback(() => GameMode.instance.EmitSound(KeyDropSoundKey));

            yield return sequence.WaitForCompletion();

            foreach (var item in objectToDestroy)
            {
                Destroy(item.gameObject);
            }

            callback?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int RandomPosition()
        {
            var rndPos = (int)(Random.Range(512, 1024) * MathExtensions.RandOne());

            return rndPos;
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
