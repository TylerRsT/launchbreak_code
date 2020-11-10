using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class DeathmatchGameMode : KeyBasedGameMode
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void LateUpdate()
        {
            base.LateUpdate();

            if(!_hasGameStarted)
            {
                return;
            }

            var aliveCharactersCount = 0;
            var aliveCharacter = null as Character;

            foreach(var spawner in characterSpawners)
            {
                if(spawner.teamIndex != -1)
                {
                    if(spawner.character != null && spawner.character.gameplayState != CharacterGameplayState.Dead)
                    {
                        aliveCharacter = spawner.character;
                        ++aliveCharactersCount;
                    }
                }
            }

            if(aliveCharactersCount == 1)
            {
                _hasGameStarted = false;
                RedeemKey(aliveCharacter);
            }
            else if(aliveCharactersCount == 0)
            {
                _hasGameStarted = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Reset(int stepCount)
        {
            foreach(var pickable in FindObjectsOfType<Pickable>())
            {
                Destroy(pickable.gameObject);
            }

            foreach (var dropBeacon in FindObjectsOfType<DropBeacon>())
            {
                dropBeacon.state = DropBeacon.State.Off;
            }

            foreach (var construct in FindObjectsOfType<Construct>())
            {
                Destroy(construct.gameObject);
            }

            yield return base.Reset(stepCount);

            foreach (var spawner in characterSpawners)
            {
                if (spawner.character != null)
                {
                    Destroy(spawner.character.gameObject);
                }
            }

            if (stepCount > 1)
            {
                yield return new WaitForSeconds(_timeBetweenRounds);
            }

            foreach (var spawner in characterSpawners)
            {
                if (spawner.teamIndex != -1)
                {
                    spawner.supply = 4;
                    spawner.BeginSpawn();
                }
            }

            foreach(var dropBeacon in FindObjectsOfType<DropBeacon>())
            {
                dropBeacon.state = DropBeacon.State.On;
            }

            _hasGameStarted = true;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _timeBetweenRounds = 2.0f;

        private bool _hasGameStarted = false;

        #endregion
    }
}