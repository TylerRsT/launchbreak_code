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
    public class RaycastWeaponEquippable : HeatBasedWeaponEquippable
    {
        #region Const

        private const string SolidLayerName = "Solid";
        private const string CharacterLayerName = "Character";

        private const string HeadShotText = "Headshot!";
        private const string HeadShotSoundKey = "HeadShot";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool CanFire()
        {
            var condition = elapsedSinceLastFire >= weaponDescriptor.rateOfFire;
            if (condition || isOverheating)
            {
                Vibrate(0.5f, 0.1f);
            }
            return condition;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Fire()
        {
            if (isOverheating)
            {
                this.EmitSound(weaponDescriptor.emptySounds, false);
                return;
            }

            DoRay(_fireEffectTransform.position, character.targetOrientation, true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="direction"></param>
        /// <param name="originalShot"></param>
        private void DoRay(Vector2 startPoint, Vector2 direction, bool originalShot)
        {
            var rebounced = false;

            var raycastHit = Physics2D.Raycast(startPoint, direction, GameConstants.SceneWidth, LayerMask.GetMask(SolidLayerName, CharacterLayerName));
            if (raycastHit.collider != null)
            {
                var damageComponent = raycastHit.collider.GetComponent<DamageComponent>();
                if (damageComponent != null)
                {
                    var opponent = damageComponent as Character;
                    var takeDamages = opponent == null;

                    if (opponent != null)
                    {
                        if(opponent.GetComponent<InvincilityBuff>() != null)
                        {
                            takeDamages = false;
                        }
                        else
                        {
                            var dashState = opponent.gameplayStateHandler as CharacterGameplayState_Dash;
                            if (dashState != null)
                            {
                                takeDamages = false;
                                if (dashState.isActive)
                                {
                                    rebounced = true;
                                    var bounceDirection = direction * -1.0f;
                                    dashState.dashDeflectAbility.SpawnDeflectAnimation(dashState, opponent.transform);
                                    DoRay((Vector2)opponent.transform.position + bounceDirection * (GameConstants.HalfTileSize + 1.0f), bounceDirection, false);
                                }
                            }
                            else
                            {
                                opponent.Push(character.targetOrientation, 2000.0f, 0.1f);
                                takeDamages = true;
                            }
                        }
                    }

                    if (takeDamages)
                    {
                        damageComponent.TakeDamages(new DamageInfo
                        {
                            provider = this,
                            damages = (int)weaponDescriptor.damages,
                            damageType = weaponDescriptor.damageType,
                        });

                        if(opponent != null && opponent.resources.health <= 0)
                        {
                            SpawnHeadshot(opponent.transform);
                        }
                    }
                }
            }

            this.EmitSound(weaponDescriptor.fireSounds, true);
            DoKnockbackSequence();

            AddHeat(weaponDescriptor.heatPerShot);
            elapsedSinceLastFire = 0.0f;

            // Spawn fire effect.
            if (originalShot && weaponDescriptor.fireEffectPrefab != null)
            {
                Instantiate(weaponDescriptor.fireEffectPrefab, startPoint, muzzleTransform.rotation);
            }

            if (weaponDescriptor.shotTrailRenderer != null)
            {
                var shotTrail = Instantiate(weaponDescriptor.shotTrailRenderer, startPoint, muzzleTransform.rotation);
                shotTrail.transform.DOMove(raycastHit.point, 0.1f);
            }

            if (!rebounced && weaponDescriptor.impactAnimation != null)
            {
                GameplayStatics.SpawnFireAndForgetAnimation(weaponDescriptor.impactAnimation, raycastHit.point, Quaternion.identity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        private void SpawnHeadshot(Transform transform)
        {
            var textMesh = new GameObject().AddComponent<TextMeshProUGUI>();

            textMesh.transform.rotation = Quaternion.identity;
            textMesh.transform.position = transform.position;
            textMesh.transform.SetParent(FindObjectOfType<Canvas>().transform);
            textMesh.text = HeadShotText;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontSize = 16.0f;
            textMesh.autoSizeTextContainer = true;

            textMesh.gameObject.AddComponent<LootTextComponent>().AnimateText();

            this.EmitSound(HeadShotSoundKey, false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        private new RaycastWeaponDescriptor weaponDescriptor => base.weaponDescriptor as RaycastWeaponDescriptor;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform _fireEffectTransform = default;

        #endregion
    }
}