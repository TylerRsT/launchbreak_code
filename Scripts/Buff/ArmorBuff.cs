using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class ArmorBuff : CharacterBuff
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bullet"></param>
        /// <returns></returns>
        public override bool ReceiveBullet(Bullet bullet)
        {
            UnityEngine.Events.UnityAction animationFinishedHandler = null;
            animationFinishedHandler = () =>
            {
                _armorAnimator.Play(powerUpData.armorAnimation);
                _armorAnimator.onFinish.RemoveListener(animationFinishedHandler);
            };

            this.EmitSound(powerUpData.armorHitSounds);
            _armorAnimator.Play(powerUpData.armorHitAnimation, true);
            _armorAnimator.onFinish.AddListener(animationFinishedHandler);

            return base.ReceiveBullet(bullet);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            var armorObject = new GameObject("Armor");
            armorObject.transform.SetParent(character.transform);
            armorObject.transform.localPosition = Vector3.zero;

            _armorAnimator = armorObject.AddComponent<SpriteAnimator>();
            _armorAnimator.Play(powerUpData.armorAnimation);

            var tileObjectRenderer = armorObject.AddComponent<TileObjectRenderer>();
            tileObjectRenderer.isChild = true;
            tileObjectRenderer.childLevel = 1;
            tileObjectRenderer.type = TileObjectType.Dynamic;
            tileObjectRenderer.additive = 16;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            if(character.resources.armor <= 0)
            {
                Destroy(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.EmitSound(powerUpData.armorDestroySounds);

            if (_armorAnimator != null)
            {
                Destroy(_armorAnimator.gameObject);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ArmorPowerUpData powerUpData { get; set; }

        #endregion

        #region Fields


        private SpriteAnimator _armorAnimator;

        #endregion
    }
}
