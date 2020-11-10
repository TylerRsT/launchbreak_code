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
    public class CharacterSpawner : MonoBehaviour
    {
        #region Const

        private const string PlayerSpawnSoundKey = "PlayerSpawn";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _keySlots = GetComponentsInChildren<KeySlot>().OrderBy(x => x.value).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerStay2D(Collider2D collision)
        {
            var character = collision.GetComponent<Character>();
            if (character != null && this.character == character &&
                character.gameplayState == CharacterGameplayState.LaunchKey)
            {
                var state = character.gameplayStateHandler as CharacterGameplayState_LaunchKey;
                state.DropInPlace();
                character.SetGameplayState(CharacterGameplayState.Default);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abilityId"></param>
        /// <returns></returns>
        public bool HasAbility(AbilityId abilityId)
        {
            return _abilities.ContainsKey(abilityId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAbility"></typeparam>
        /// <param name="abilityId"></param>
        /// <param name="ability"></param>
        /// <returns></returns>
        public bool TryGetAbility<TAbility>(AbilityId abilityId, out TAbility ability) where TAbility : class, IAbility
        {
            IAbility tempAbility;
            var result = _abilities.TryGetValue(abilityId, out tempAbility);
            if (result)
            {
                ability = tempAbility as TAbility;
                return true;
            }
            ability = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginSpawn()
        {
            StartCoroutine(Spawn());
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateScoreInfo()
        {
            GameModeParams.instance.playerParams[playerIndex - 1].score = score;
            GameModeParams.instance.playerParams[playerIndex - 1].killCount = killCount;
            GameModeParams.instance.playerParams[playerIndex - 1].deathCount = deathCount;
        }

        /// <summary>
        /// 
        /// </summary>
        public void TriggerAllTraps()
        {
            var trapsToRemove = new List<ITrapElement>();
            foreach (var trap in traps)
            {
                if (trap.Trigger())
                {
                    trapsToRemove.Add(trap);
                }
            }
            foreach (var trapToRemove in trapsToRemove)
            {
                traps.Remove(trapToRemove);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator OpenPod()
        {
            var animator = GetComponent<SpriteAnimator>();
            animator.Play(_openPodAnimation, true);

            var characters = FindObjectsOfType<Character>();

            foreach (var item in characters)
            {
                item.resources.HideSupply();
                item.resources.HideHealth();
            }

            yield return new WaitUntil(() => !animator.IsPlaying);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator ClosePod()
        {
            var animator = GetComponent<SpriteAnimator>();
            animator.Play(_openPodAnimation, true, true);

            yield return new WaitUntil(() => !animator.IsPlaying);

        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator Spawn()
        {
            if (!canSpawn)
            {
                yield break;
            }

            if (_firstSpawn)
            {
                _firstSpawn = false;
                FirstSpawn();
            }

            _playerSpawnParticleSystem.Play();

            yield return new WaitForSeconds(_spawnDelay);

            _playerSpawnParticleSystem.Stop();

            var animation = GameplayStatics.SpawnFireAndForgetAnimation(_playerSpawnAnimation, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(_materializationDelay);

            character = Instantiate(_characterPrefab, transform.position + spawnOffset, transform.rotation).GetComponent<Character>();
            var hitFlash = character.GetComponent<HitFlash>();
            hitFlash?.Flash(1.0f, 0.4f, 1f);
            var playerParams = GameModeParams.instance.playerParams[playerIndex - 1];
            character.GetComponent<GameplayTeam>().teamIndex = teamIndex;
            character.GetComponent<CharacterResources>().supply = _supply;

            character.spawner = this;
            character.characterDescriptor = playerParams.selectedCharacter;
            character.skinDescriptor = playerParams.selectedSkin;
            character.name = $"Character_{playerParams.selectedSkin.name}";
            character.SetConstructsFromLoadout(playerParams.loadout);

            character.TryAddBuff<InvincilityBuff>(1.0f);
            character.SetWeapon(0, _defaultWeapon);
            character.Orientate(_orientation, _orientation);

            var playerIdBehaviour = Instantiate(_playerIdSpritePrefab, transform.position + _playerIdSpritePrefab.transform.position, Quaternion.identity, character.transform)
                .GetComponent<CharacterPlayerIdBehaviour>();
            if (playerParams.isAiControlled)
            {
                playerIdBehaviour.playerIndex = -1;
            }
            else
            {
                playerIdBehaviour.playerIndex = playerIndex;
            }
            playerIdBehaviour.color = character.characterDescriptor.mainColor;

            this.EmitSound(PlayerSpawnSoundKey);

            var additiveArmor = GameMode.instance.highestScore - score;
            if (additiveArmor > 0)
            {
                var powerUp = new ArmorPowerUp();
                powerUp.powerUpData = _armorPowerUpData;
                powerUp.Apply(character, additiveArmor);
            }

            GameMode.instance.OnCharacterSpawned(character);

            if (playerParams.isAiControlled)
            {
                var behaviourManager = new GameObject(nameof(AIBehaviourManager)).AddComponent<AIBehaviourManager>();
                behaviourManager.gameObject.layer = LayerMask.NameToLayer("CharacterAI");
                behaviourManager.GetComponent<AINavigationController>().navMesh = _navMesh;
                behaviourManager.transform.SetParent(character.transform);
            }
            else
            {
                var characterInput = character.gameObject.AddComponent<CharacterInput>();
                characterInput.userIndex = playerParams.controllerIndex;
                characterInput.vibrationMultiplier = playerParams.vibrationMultiplier;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void FirstSpawn()
        {
            var characterColor = GameModeParams.instance.playerParams[playerIndex - 1].selectedCharacter.mainColor;

            GetComponent<SpriteRenderer>().material.SetColor("_DesiredColor", characterColor);

            // Spawn abilities
            foreach (var abilityLoadoutItem in GameModeParams.instance.playerParams[playerIndex - 1].loadout.items.OfType<AbilityLoadoutItem>())
            {
                var abilityType = AbilityTable.Get(abilityLoadoutItem.abilityId);
                IAbility ability = null;
                if (abilityType != null)
                {
                    ability = AbilityFactory.Instantiate(abilityType, abilityLoadoutItem.abilityData);
                }
                _abilities.Add(abilityLoadoutItem.abilityId, ability);
            }

            if (GameModeParams.instance.playerParams[playerIndex - 1].isAiControlled)
            {
                _navMesh = new NavMesh();
                _navMesh.Generate();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int supply
        {
            get { return _supply; }
            set { _supply = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int score
        {
            get { return _score; }
            set
            {
                nextKeySlot.isOn = true;
                _score = value;
                _scoreSpriteRenderer.sprite = _scoreSpriteAnimator.animations[0].GetFrame(value - 1);

                var keyBasedGameMode = GameMode.GetInstance<KeyBasedGameMode>();
                var maxScore = 0;

                if(keyBasedGameMode != null)
                {
                    maxScore = keyBasedGameMode.maxScore;
                }

                if(value == maxScore - 1)
                {
                    foreach (var lightAnimator in _lightAnimators)
                    {
                        lightAnimator.GetComponent<LightController>().Flicker(0.1f, 2);
                        lightAnimator.SpriteRenderer.sprite = lightAnimator.animations[0].GetFrame(1);
                    }
                }
                else if(value == maxScore)
                {
                    foreach(var lightAnimator in _lightAnimators)
                    {
                        lightAnimator.GetComponent<LightController>().ChangeColor(Color.green);
                        lightAnimator.SpriteRenderer.sprite = lightAnimator.animations[0].GetFrame(2);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public KeySlot nextKeySlot
        {
            get { return _keySlots[score]; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Character character { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int playerIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int teamIndex { get; set; } = -1;

        /// <summary>
        /// 
        /// </summary>
        public bool canSpawn { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public int lastConstructIndex { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int killCount = 0;

        /// <summary>
        /// 
        /// </summary>
        public int deathCount = 0;

        /// <summary>
        /// 
        /// </summary>
        public List<ITrapElement> traps { get; } = new List<ITrapElement>();

        /// <summary>
        /// 
        /// </summary>
        public CharacterSpawner nemesisSpawner => _nemesisSpawner;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 spawnOffset => _spawnOffset;

        /// <summary>
        /// 
        /// </summary>
        public WeaponDescriptor defaultWeapon => _defaultWeapon;

        /// <summary>
        /// 
        /// </summary>
        public DashDeflectAbilityData dashDeflectAbilityData => _dashDeflectAbilityData;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _scoreSpriteRenderer = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimator _scoreSpriteAnimator = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimator[] _lightAnimators = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Vector2 _orientation = Vector2.zero;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _characterPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private WeaponDescriptor _defaultWeapon = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _playerIdSpritePrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ParticleSystem _playerSpawnParticleSystem = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _playerSpawnAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _openPodAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private CharacterSpawner _nemesisSpawner = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _spawnDelay = 2.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _materializationDelay = 2.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Vector3 _spawnOffset = Vector3.zero;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private ArmorPowerUpData _armorPowerUpData = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private DashDeflectAbilityData _dashDeflectAbilityData = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _supply = 4;

        private int _score = 0;

        private bool _firstSpawn = true;

        private Dictionary<AbilityId, IAbility> _abilities = new Dictionary<AbilityId, IAbility>();

        private KeySlot[] _keySlots;

        private NavMesh _navMesh;

        #endregion
    }
}