using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class LaunchKeyPickable : Pickable
    {
        #region Const

        public const string KeySpawnSoundKey = "KeySpawn";
        public const string KeyDropSoundKey = "KeyDrop";
        private const string KeyPickupSoundKey = "KeyPickup";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            _contextualButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if(_elapsed < _durationBeforeFocus)
            {
                _elapsed += GameplayStatics.gameDeltaTime;
                if(_elapsed >= _durationBeforeFocus)
                {
                    GetComponent<SpriteAnimator>().Play(_focusAnimation);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        protected override void OnPickup(PickupResponse response)
        {
            base.OnPickup(response);

            Debug.Assert(_launchKeyEquippablePrefab != null);
            var character = response.character;
            character.SetEquippable(Instantiate(_launchKeyEquippablePrefab, character.transform.position, Quaternion.identity, character.transform).GetComponent<LaunchKeyEquippable>());
            character.SetGameplayState(CharacterGameplayState.LaunchKey);

            response.Accept();

            this.EmitSound(KeyPickupSoundKey);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();

            _contextualButton.gameObject.SetActive(charactersStepping.Count > 0);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _launchKeyEquippablePrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _durationBeforeFocus = 10.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _focusAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _contextualButton = default;

        private float _elapsed;

        #endregion
    }
}