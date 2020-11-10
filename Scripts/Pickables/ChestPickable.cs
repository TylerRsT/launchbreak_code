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
    public class ChestPickable : Pickable
    {
        #region Const

        private const float LootMinDistance = 12.0f;
        private const float LootMaxDistance = 16.0f;

        private const int PowerUpBuildAnimationIndex = 0;
        private const int PowerUpAnimationIndex = 1;
        private const int WeaponBuildAnimationIndex = 2;
        private const int WeaponAnimationIndex = 3;
        private const int SpawnAnimationIndex = 4;

        private const string SpawnChestSoundKey = "SpawnChest";
        private const string OpenChestSoundKey = "OpenChest";
        private const string ChestRewardSoundKey = "ChestReward";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damages"></param>
        private void OnReceiveDamages(int damages)
        {
            BeginOpenChest();
        }

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        protected override void OnPickup(PickupResponse response)
        {
            base.OnPickup(response);

            BeginOpenChest();
        }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void OnReceivingBullet(BulletResponse response)
        {
            if (response.collider == GetComponent<BoxCollider2D>())
            {
                response.received = true;
                SendPickup(response.bullet.instigator as Character, false);
                _hasOpened = true;
            }
            else
            {
                response.received = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentDropBeacon"></param>
        public void Initialize(DropBeacon parentDropBeacon)
        {
            _parentDropBeacon = parentDropBeacon;
            var spriteAnimator = GetComponent<SpriteAnimator>();

            this.EmitSound(SpawnChestSoundKey);

            if (parentDropBeacon.lootableToSpawn is PowerUpDescriptor)
            {
                spriteAnimator.Play(spriteAnimator.animations[PowerUpBuildAnimationIndex], true);
                spriteAnimator.onFinish.AddListener(() =>
                {
                    spriteAnimator.Play(spriteAnimator.animations[PowerUpAnimationIndex]);
                });
            }
            else if (parentDropBeacon.lootableToSpawn is WeaponDescriptor)
            {
                spriteAnimator.Play(spriteAnimator.animations[WeaponBuildAnimationIndex], true);
                spriteAnimator.onFinish.AddListener(() =>
                {
                    spriteAnimator.Play(spriteAnimator.animations[WeaponAnimationIndex]);
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginOpenChest()
        {
            if(_hasOpened)
            {
                return;
            }

            Telemetry.game.Incr("chest_pickup");

            this.EmitSound(OpenChestSoundKey);

            _hasOpened = true;
            GetComponent<BoxCollider2D>().enabled = false;

            KillFloatingAnim();
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.material = Bootstrap.instance.data.spriteFlashMaterial;
            _shakeTween = transform.DOShakePosition(0.4f, 5.0f, 30, fadeOut: false);
            _shakeTween.onComplete += (() =>
            {
                _shakeTween = null;
                var spriteAnimator = GetComponent<SpriteAnimator>();
                spriteAnimator.Play(spriteAnimator.animations[SpawnAnimationIndex], true);
                spriteRenderer.material = Bootstrap.instance.data.spriteDefaultMaterial;
                StartCoroutine(OpenChest(spriteAnimator));
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator OpenChest(SpriteAnimator spriteAnimator)
        {
            yield return new WaitUntil(() => spriteAnimator.CurrentFrame >= _spawnPickableFrame);
            var selectedLootDescriptor = _parentDropBeacon.lootableToSpawn;

            GameObject pickablePrefab = null;
            switch (selectedLootDescriptor)
            {
                case PowerUpDescriptor powerUpDescriptor:
                    pickablePrefab = _powerUpPickablePrefab;
                    break;
                case WeaponDescriptor weaponDescriptor:
                    pickablePrefab = _weaponPickablePrefab;
                    break;
                default:
                    Debug.Assert(false);
                    yield break;
            }

            Debug.Assert(pickablePrefab != null);
            var lootPickable = Instantiate(pickablePrefab, transform.position, transform.rotation).GetComponent<LootPickable>();
            lootPickable.Initialize(selectedLootDescriptor);
            lootPickable.DoRandomJumpAnim(LootMinDistance, LootMaxDistance, () =>
            {
                lootPickable.isPickable = true;
            });

            this.EmitSound(ChestRewardSoundKey);

            _parentDropBeacon.state = DropBeacon.State.Off;
            Destroy(gameObject);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool hasOpened => _hasOpened;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _powerUpPickablePrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _weaponPickablePrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _spawnPickableFrame = 0;

        private DropBeacon _parentDropBeacon;

        private Tween _shakeTween;

        private bool _hasOpened = false;

        #endregion
    }
}