using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [AIBehaviour]
    public class AIBehaviour_Pickable : AIBehaviour
    {
        #region Const

        private const float CloseDistance = 100.0f;
        private const float MidDistance = 250.0f;
        private const float FarDistance = 500.0f;

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            base.Update();

            var characterPosition = character.transform.position;
            timeBeforeLaunchKey = Mathf.Max(0.0f, timeBeforeLaunchKey - GameplayStatics.gameDeltaTime);

            foreach(var pickable in Pickable.availablePickables)
            {
                if (pickable == null)
                {
                    continue;
                }

                var distance = Vector2.Distance(characterPosition, pickable.transform.position);

                switch(pickable)
                {
                    case LaunchKeyPickable launchKeyPickable:
                        if (timeBeforeLaunchKey <= 0.0f)
                        {
                            CheckLaunchKey(launchKeyPickable, distance);
                        }
                        break;
                    case ChestPickable chestPickable:
                        CheckChest(chestPickable, distance);
                        break;
                    case PowerUpPickable powerUpPickable:
                        CheckPowerUp(powerUpPickable, distance);
                        break;
                    case WeaponPickable weaponPickable:
                        CheckWeapon(weaponPickable, distance);
                        break;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="launchKeyPickable"></param>
        /// <param name="distance"></param>
        private void CheckLaunchKey(LaunchKeyPickable launchKeyPickable, float distance)
        {
            var launchKeyScore = GetScore(distance, 500.0f, 0.8f, 0.5f, 0.2f);

            var spawnerDistance = Vector2.Distance(launchKeyPickable.transform.position, character.spawner.transform.position);
            var spawnerScore = GetScore(spawnerDistance, 100.0f, 0.8f, 0.5f, 0.0f);

            var finalScore = launchKeyScore + spawnerScore;
            behaviourManager.SetTask(new AITask_Pickable(launchKeyPickable), finalScore);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chestPickable"></param>
        /// <param name="distance"></param>
        private void CheckChest(ChestPickable chestPickable, float distance)
        {
            if(chestPickable.hasOpened)
            {
                return;
            }

            var chestScore = GetScore(distance, 100.0f, 0.8f, 0.2f, 0.0f);
            behaviourManager.SetTask(new AITask_Pickable(chestPickable), chestScore);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerUpPickable"></param>
        /// <param name="distance"></param>
        private void CheckPowerUp(PowerUpPickable powerUpPickable, float distance)
        {
            var score = GetScore(distance, powerUpPickable.powerUpDescriptor.aiLootInfo.interestValue, 0.8f, 0.5f, 0.2f);
            behaviourManager.SetTask(new AITask_Pickable(powerUpPickable), score);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weaponPickable"></param>
        /// <param name="distance"></param>
        private void CheckWeapon(WeaponPickable weaponPickable, float distance)
        {
            var interestValue = weaponPickable.weaponDescriptor.aiLootInfo.interestValue;
            if(!character.weapons.Any(x => x != null && x.weaponDescriptor.aiLootInfo.interestValue >= interestValue))
            {
                var weaponScore = GetScore(distance, interestValue, 0.8f, 0.5f, 0.2f);
                behaviourManager.SetTask(new AITask_Pickable(weaponPickable), weaponScore);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="score"></param>
        /// <param name="closeMultiplier"></param>
        /// <param name="midMultiplier"></param>
        /// <param name="farMultiplier"></param>
        /// <returns></returns>
        private float GetScore(float distance, float score, float closeMultiplier, float midMultiplier, float farMultiplier)
        {
            if (distance > CloseDistance && distance <= MidDistance)
            {
                score *= closeMultiplier;
            }
            else if (distance > MidDistance && distance <= FarDistance)
            {
                score *= midMultiplier;
            }
            else if (distance > FarDistance)
            {
                score *= farMultiplier;
            }

            return score;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float timeBeforeLaunchKey { get; set; }

        #endregion
    }
}