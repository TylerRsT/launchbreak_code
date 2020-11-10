using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(PlayerScoredAnimation))]
    public abstract class KeyBasedGameMode : GameMode
    {
        #region Const

        private const string KeyRedeemSoundKey = "KeyRedeem";
        private const string KeySpinSoundKey = "KeySpin";
        private const string RippleSoundKey = "Ripple";
        private const string PlayerScoredSoundKey = "PlayerScored";
        private const string GameStartSoundKey = "GameStart";
        private const string GameEndSoundKey = "GameEnd";
        private const string ShipExplosionSoundKey = "ShipExplosion";
        private const string ShipDamageSoundKey = "ShipDamage";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void RedeemKey(Character character)
        {
            base.RedeemKey(character);

            Telemetry.game.Incr("redeemKey");

            GameplayStatics.state = GameplayState.Cutscene;

            var currentLaunchKeyPosition = Vector3.zero;

            var launchKeyEquippable = character.currentEquippable as LaunchKeyEquippable;
            var soundInstances = new List<FMOD.Studio.EventInstance>(launchKeyEquippable.EmitSound(KeySpinSoundKey));
            soundInstances.RegisterAsGameplayLooping();

            if (launchKeyEquippable != null)
            {
                currentLaunchKeyPosition = launchKeyEquippable.transform.position;
            }
            else
            {
                var launchKeyPickable = FindObjectOfType<LaunchKeyPickable>();
                currentLaunchKeyPosition = launchKeyPickable.transform.position;
                Destroy(launchKeyPickable.gameObject);
            }

            var redeemedLaunchKey = Instantiate(_redeemedLaunchKeyPrefab,
                currentLaunchKeyPosition,
                Quaternion.identity);

            redeemedLaunchKey.GetComponent<SpriteAnimator>().RegisterAsCutscene();
            redeemedLaunchKey.GetComponent<SpriteRenderer>().material.SetColor("_DesiredColor", Pickable.blackOutlineColor);
            redeemedLaunchKey.transform.DOMove(character.spawner.nextKeySlot.transform.position + (Vector3.up * 20.0f), 0.5f)
                .SetAsCutscene().onComplete += (() =>
                {
                    redeemedLaunchKey.GetComponent<SpriteAnimator>().UnregisterAsCutscene();
                    StartCoroutine(InsertKey(redeemedLaunchKey, character.spawner));

                    foreach (var item in soundInstances)
                    {
                        item.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                        item.UnregisterAsGameplayLooping();
                    }

                });

            if(character.GetComponent<CharacterMarketingZoom>() == null)
            {
                gameObject.AddComponent<CameraScoreZoom>().ZoomIn(character.spawner.transform.position);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public override void OnCharacterSpawned(Character character)
        {
            base.OnCharacterSpawned(character);

            if(isGameOver)
            {
                var input = character.GetComponent<CharacterInput>();
                if(input != null)
                {
                    input.inputLocked = true;
                }
                character.SetGameplayState(CharacterGameplayState.Panicking);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            _launchKeySpawners.AddRange(FindObjectsOfType<LaunchKeySpawner>());
            Debug.Assert(_launchKeySpawners.Count > 0);

            base.Start();

            var dropManagers = FindObjectsOfType<DropManager>();
            foreach(var dropManager in dropManagers)
            {
                dropManager.Initialize(characterSpawners.ToArray());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (Debug.isDebugBuild)
            {
                if (InputHelper.AnyButtonDown("Gameplay_Activate"))
                {
                    if(!AIBehaviour.easyMode)
                    {
                        var characters = FindObjectsOfType<Character>();
                        var index = -1;
                        for(var i = 0; i < characters.Length; ++i)
                        {
                            if(characters[i].GetComponent<CharacterMarketingZoom>())
                            {
                                index = i;
                                break;
                            }
                        }

                        ++index;
                        index = Mathf.Min(characters.Length - 1, index);
                        characters[index].ToggleMarketingZoom();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitGame()
        {
            StartCoroutine(InitGameInternal());
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Step()
        {
            CharacterSpawner winner = null;
            foreach (var spawner in characterSpawners)
            {
                if (spawner.score >= _maxScore)
                {
                    winner = spawner;
                    break;
                }
            }

            if (winner == null)
            {
                TriggerHazardSteps();
                StartCoroutine(Reset(++_stepCount));
                return;
            }

            foreach (var spawner in characterSpawners)
            {
                spawner.canSpawn = false;
            }

            foreach (var character in FindObjectsOfType<Character>())
            {
                var input = character.GetComponent<CharacterInput>();
                if (input != null)
                {
                    input.inputLocked = true;
                }
            }

            StartCoroutine(GameOver(winner));
        }

        /// <summary>
        /// 
        /// </summary>
        public override int highestScore
        {
            get
            {
                var score = 0;
                foreach (var spawner in characterSpawners)
                {
                    score = Mathf.Max(score, spawner.score);
                }
                return score;
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Reset(int stepCount)
        {
            if (stepCount > 1 && launchKeySpawnCooldown > 0.0f)
            {
                yield return new WaitForSeconds(launchKeySpawnCooldown);
            }

            var random = Random.Range(0, launchKeySpawners.Count);
            launchKeySpawners[random].BeginSpawn();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SpawnCharacters()
        {
            var playersParams = GameModeParams.instance.playerParams.Where(x => x.isPlaying).ToArray();

            if (playersParams.Length == 2)
            {
                var selectedSpawner = characterSpawners[Random.Range(0, characterSpawners.Count)];
                Debug.Assert(selectedSpawner.nemesisSpawner != null);
                SpawnCharacter(playersParams[0], selectedSpawner);
                SpawnCharacter(playersParams[1], selectedSpawner.nemesisSpawner);
            }
            else
            {
                foreach (var playerParams in playersParams)
                {
                    CharacterSpawner selectedSpawner = null;
                    do
                    {
                        selectedSpawner = characterSpawners[Random.Range(0, characterSpawners.Count)];
                    } while (selectedSpawner.playerIndex != 0);

                    SpawnCharacter(playerParams, selectedSpawner);
                }
            }

            // Disable every spawner that is not used for this game.
            foreach (var spawner in characterSpawners)
            {
                if (spawner.teamIndex == -1)
                {
                    spawner.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerParams"></param>
        /// <param name="selectedSpawner"></param>
        private void SpawnCharacter(PlayerParams playerParams, CharacterSpawner selectedSpawner)
        {
            selectedSpawner.playerIndex = playerParams.playerIndex;
            selectedSpawner.teamIndex = playerParams.teamIndex;

            selectedSpawner.BeginSpawn();
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator InitGameInternal()
        {
            SpawnCharacters();
            yield return ShowControlsScheme();
            Step();
            hasGameStarted = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="winner"></param>
        /// <returns></returns>
        private IEnumerator GameOver(CharacterSpawner winner)
        {
            for (var i = 0; i < characterSpawners.Count; ++i)
            {
                if (characterSpawners[i].teamIndex == -1)
                {
                    continue;
                }
                characterSpawners[i].UpdateScoreInfo();
                Telemetry.game.Set($"player{i}Score", characterSpawners[i].score);
            }

            isGameOver = true;
            var character = winner.character;
            var aiBehaviourManager = character.GetComponentInChildren<AIBehaviourManager>();
            if(aiBehaviourManager != null)
            {
                Destroy(aiBehaviourManager.gameObject);
            }

            yield return winner.OpenPod();

            character.SetGameplayState(CharacterGameplayState.Leaving);

            yield return (character.gameplayStateHandler as CharacterGameplayState_Leaving).coroutine;

            yield return winner.ClosePod();

            StartCoroutine(TriggerExplosions());

            yield return new WaitForSeconds(1.0f);

            for (var i = 0; i < characterSpawners.Count; ++i)
            {
                if (characterSpawners[i].teamIndex == -1)
                {
                    continue;
                }

                Telemetry.game.Set($"player{i}Score", characterSpawners[i].score);

                if (characterSpawners[i] == winner)
                {
                    continue;
                }

                if (characterSpawners[i].character != null)
                {
                    characterSpawners[i].character.SetGameplayState(CharacterGameplayState.Panicking);
                }
            }

            yield return new WaitForSeconds(4.0f);

            GameplayStatics.TransitionToScene(GameConstants.VictoryScene);

            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redeemedLaunchKey"></param>
        /// <param name="spawner"></param>
        /// <returns></returns>
        private IEnumerator InsertKey(GameObject redeemedLaunchKey, CharacterSpawner spawner)
        {
            var spriteAnimator = redeemedLaunchKey.GetComponent<SpriteAnimator>();
            spriteAnimator.Play(spriteAnimator.animations[1]);
            yield return new WaitForSecondsCutscene(0.2f);

            redeemedLaunchKey.GetComponent<SpriteRenderer>().sortingLayerName = StaticSortingLayer.KeyAtSpawner.ToString();
            redeemedLaunchKey.transform.DOMove(spawner.nextKeySlot.transform.position, 0.05f)
                .SetAsCutscene().onComplete += (() =>
                {
                    GameplayStatics.state = GameplayState.GameRunning;

                    CameraShakeManager.instance.Shake(Bootstrap.instance.data.keyRedeemShake);
                    spawner.EmitSound(KeyRedeemSoundKey);

                    GetComponent<PlayerScoredAnimation>().StartAnimation(spawner.character.characterDescriptor.name,
                        spawner.character.characterDescriptor.mainColor, ++spawner.score >= _maxScore);

                    Step();

                    var redeemRepulser = new GameObject(nameof(RedeemRepulser), typeof(RedeemRepulser));
                    redeemRepulser.transform.position = spawner.transform.position;
                    redeemRepulser.layer = LayerMask.NameToLayer("RedeemRepulser");

                    if (spawner.character.GetComponent<CharacterMarketingZoom>() == null)
                    {
                        GetComponent<CameraScoreZoom>().ZoomOut(spawner.transform.position);
                    }
                    this.EmitSound(RippleSoundKey);
                });

            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator TriggerExplosions()
        {
            this.EmitSound(ShipDamageSoundKey);
            CameraShakeManager.instance.Shake(_lastShipHitGameCameraShake);

            yield return new WaitForSeconds(2f);

            var halfWidth = GameConstants.SceneWidth / 2.0f;
            var halfHeight = GameConstants.SceneHeight / 2.0f;
            var explosionSoundSkip = 0;

            do
            {
                var pos = new Vector2(Random.Range(halfWidth * -1.0f, halfWidth), Random.Range(halfHeight * -1.0f, halfHeight));
                var index = Random.Range(0, _endGameExplosionAnimations.Count);

                GameplayStatics.SpawnFireAndForgetAnimation(_endGameExplosionAnimations[index], pos, Quaternion.identity)
                   .GetComponent<SpriteRenderer>().sortingLayerID = SortingLayerHelper.GetSortingLayerValue(StaticSortingLayer.Default);

                explosionSoundSkip++;

                if (explosionSoundSkip > 5)
                {
                    this.EmitSound(ShipExplosionSoundKey, false);
                    explosionSoundSkip = 0;
                }

                CameraShakeManager.instance.Shake(_endGameCameraShake);

                if(_endGameExplosionRandomIntervalMax > _endGameExplosionIntervalMin)
                {
                    _endGameExplosionRandomIntervalMin -= _endGameExplosionDecrementValue;
                    _endGameExplosionRandomIntervalMax -= _endGameExplosionDecrementValue;
                }

                yield return new WaitForSeconds(Random.Range(_endGameExplosionRandomIntervalMin, _endGameExplosionRandomIntervalMax));
            } while (true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int maxScore => _maxScore;

        /// <summary>
        /// 
        /// </summary>
        public float launchKeySpawnCooldown => _launchKeySpawnCooldown;

        /// <summary>
        /// 
        /// </summary>
        public float launchKeySpawnDuration => _launchKeySpawnDuration;

        /// <summary>
        /// 
        /// </summary>
        protected IReadOnlyList<LaunchKeySpawner> launchKeySpawners => _launchKeySpawners;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _maxScore = 4;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _launchKeySpawnCooldown = 10.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _launchKeySpawnDuration = 3.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _redeemedLaunchKeyPrefab = default;
        
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<SpriteAnimation> _endGameExplosionAnimations = new List<SpriteAnimation>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ShakePreset _endGameCameraShake = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ShakePreset _lastShipHitGameCameraShake = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _endGameExplosionDecrementValue = 0.05f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _endGameExplosionIntervalMin = 0.05f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _endGameExplosionRandomIntervalMax = 0.3f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _endGameExplosionRandomIntervalMin = 0.25f;

        private List<LaunchKeySpawner> _launchKeySpawners = new List<LaunchKeySpawner>();

        private int _stepCount = 0;

        #endregion
    }
}