using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class LaunchKeySpawner : MonoBehaviour
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void BeginSpawn()
        {
            /*_keySpawnParticleSystem.Play();

            StartCoroutine(DoSpawn());*/
            var firstSpawnAnimation = GetComponent<InGameStartAnimation>();
            var spawnAnimation = GetComponent<KeySpawnAnimation>();


            if (!_firstSpawn)
            {
                firstSpawnAnimation.StartAnimation(() =>
                {
                    var launchKeyAnimator = Instantiate(_launchKeyPrefab, transform.position, Quaternion.identity).GetComponent<SpriteAnimator>();

                    launchKeyAnimator.Play(launchKeyAnimator.animations[0]);
                    launchKeyAnimator.EmitSound(LaunchKeyPickable.KeySpawnSoundKey);
                });

                _firstSpawn = true;
            }
            else
            {
                spawnAnimation.StartAnimation(() =>
                {
                    var launchKeyAnimator = Instantiate(_launchKeyPrefab, transform.position, Quaternion.identity).GetComponent<SpriteAnimator>();

                    launchKeyAnimator.Play(launchKeyAnimator.animations[0]);
                    launchKeyAnimator.EmitSound(LaunchKeyPickable.KeySpawnSoundKey);
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /*private IEnumerator DoSpawn()
        {
            const float blinkInterval = 0.5f;
            const float interval = 0.1f;

            var elapsed = 0.0f;
            while (elapsed < (GameMode.instance as KeyBasedGameMode).launchKeySpawnDuration)
            {
                _targetRenderer.sprite = (elapsed % (blinkInterval * 2.0f)) <= blinkInterval ? _onSprite : _offSprite;
                yield return new WaitForSeconds(interval);
                elapsed += interval;
            }

            _keySpawnParticleSystem.Stop();
            _targetRenderer.sprite = _offSprite;

            Debug.Assert(_launchKeyPrefab != null);
            var launchKeyAnimator = Instantiate(_launchKeyPrefab, transform.position, Quaternion.identity).GetComponent<SpriteAnimator>();
            launchKeyAnimator.Play(launchKeyAnimator.animations[2], true);

            yield return new WaitUntil(() => !launchKeyAnimator.IsPlaying);

            launchKeyAnimator.Play(launchKeyAnimator.animations[0]);
            launchKeyAnimator.EmitSound(LaunchKeyPickable.KeySpawnSoundKey);

            yield break;
        }*/

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _launchKeyPrefab = default;

        private bool _firstSpawn = false;

        #endregion
    }
}