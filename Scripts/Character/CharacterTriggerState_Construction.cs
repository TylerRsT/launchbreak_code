using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CharacterTriggerState_Construction : CharacterTriggerStateHandler
    {
        #region Const

        private const float AutoChangeCooldown = 0.5f;
        private const float StickTolerance = 0.5f;
        private const float BuildDeniedSoundInterval = 1f;

        private const string CostChildName = "Cost";
        private const string LoadoutBGChildName = "LoadoutBG";
        private const string LoadoutIconChildName = "LoadoutIcon";

        private const string BuildDeniedSoundKey = "BuildDenied";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public override void Initialize(Character character)
        {
            base.Initialize(character);

            _loadoutBGSpriteRenderer = character.transform.Find(LoadoutBGChildName).GetComponent<SpriteRenderer>();
            _loadoutIconSpriteRenderer = _loadoutBGSpriteRenderer.transform.Find(LoadoutIconChildName).GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            _hasPlaced = false;

            _currentConstructIndex = character.spawner.lastConstructIndex;

            InstantiateConstructionSpace();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Update()
        {
            base.Update();

            if(_elapsedSinceBuildDeniedSound > 0f)
            {
                _elapsedSinceBuildDeniedSound -= GameplayStatics.gameFixedDeltaTime;
            }

            _cooldownBeforeAutoChange = Mathf.Max(0.0f, _cooldownBeforeAutoChange - GameplayStatics.gameDeltaTime);

            var characterPosition = character.transform.position;

            float x = (GameConstants.TileSize * (Mathf.Ceil(characterPosition.x / GameConstants.TileSize) - 1.0f)) + GameConstants.HalfTileSize;
            float y = (GameConstants.TileSize * (Mathf.Ceil((characterPosition.y - GameConstants.HalfTileSize) / GameConstants.TileSize) - 1.0f)) + GameConstants.HalfTileSize;

            _currentConstructionSpace.transform.position = new Vector3(x, y, 0.0f);

            var collisions = new List<Collider2D>();
            _currentConstructionSpace.boxCollider.GetContacts(new ContactFilter2D
            {
                useTriggers = true,
            }, collisions);

            if (character.constructs.Length == 0)
            {
                return;
            }

            var constructionRequest = new ConstructionSpaceRequest(_currentConstructionSpace, collisions);
            var constructItem = character.constructs[_currentConstructIndex];

            var accepted = character.resources.supply >= constructItem.supplyCost;

            if (accepted)
            {
                foreach (var collision in collisions)
                {
                    collision.gameObject.SendMessage(ConstructionSpace.OnConstructionSpaceRequestedMethodName, constructionRequest, SendMessageOptions.DontRequireReceiver);
                    if (!constructionRequest.accepted)
                    {
                        break;
                    }
                }
                accepted = constructionRequest.accepted;
            }

            _currentConstructionSpace.canBePlaced = accepted;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Exit()
        {
            base.Exit();

            if(!_currentConstructionSpace.isPlaced)
            {
                GameObject.Destroy(_currentConstructionSpace.gameObject);
            }

            _loadoutBGSpriteRenderer.enabled = false;
            _loadoutIconSpriteRenderer.enabled = false;

            character.resources.HideSupply();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Fire()
        {
            base.Fire();

            if(character.constructs.Length == 0)
            {
                return;
            }

            if(_hasPlaced && _currentConstructionSpace.transform.position == _lastPlacementPosition)
            {
                return;
            }

            var constructItem = character.constructs[_currentConstructIndex];
            if (_currentConstructionSpace.canBePlaced)
            {
                _currentConstructionSpace.isPlaced = true;
                _lastPlacementPosition = _currentConstructionSpace.transform.position;
                character.resources.TakeSupply(constructItem.supplyCost);

                var isTempConstruct = constructItem == character.tempConstruct;

                InstantiateConstructionSpace();

                _hasPlaced = true;
            }
            else if(character.resources.supply < constructItem.supplyCost)
            {
                if(_elapsedSinceBuildDeniedSound <= 0f)
                {
                    character.EmitSound(BuildDeniedSoundKey);
                    _elapsedSinceBuildDeniedSound = BuildDeniedSoundInterval;
                }
                character.resources.SupplyAnim(CharacterResources.SupplyAnimReason.NotEnough);
            }
            else if(!_currentConstructionSpace.canBePlaced)
            {
                if (_elapsedSinceBuildDeniedSound <= 0f)
                {
                    character.EmitSound(BuildDeniedSoundKey);
                    _elapsedSinceBuildDeniedSound = BuildDeniedSoundInterval;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Switch()
        {
            CycleBlueprint(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="additive"></param>
        private void CycleBlueprint(int additive)
        {
            if(character.constructs.Length == 0)
            {
                _currentConstructIndex = 0;
                character.spawner.lastConstructIndex = _currentConstructIndex;
                return;
            }

            var lastConstructIndex = _currentConstructIndex;
            _currentConstructIndex += additive;
            while(_currentConstructIndex < 0)
            {
                _currentConstructIndex += character.constructs.Length;
            }
            _currentConstructIndex = _currentConstructIndex % character.constructs.Length;

            if (_currentConstructIndex != lastConstructIndex)
            {
                character.spawner.lastConstructIndex = _currentConstructIndex;
                UpdateBlueprint();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InstantiateConstructionSpace()
        {
            _currentConstructionSpace =
                GameObject.Instantiate(character.constructionSpacePrefab, character.transform.position, Quaternion.Euler(Vector3.zero))
                .GetComponent<ConstructionSpace>();
            UpdateBlueprint();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateBlueprint()
        {
            if (character.constructs.Length == 0)
            {
                return;
            }

            if(_currentConstructIndex >= character.constructs.Length)
            {
                _currentConstructIndex = 0; // Safe guard for construct rogue.
            }

            var constructItem = character.constructs[_currentConstructIndex];
            _loadoutBGSpriteRenderer.enabled = true;
            _loadoutIconSpriteRenderer.enabled = true;
            _loadoutIconSpriteRenderer.sprite = constructItem.loadoutIcon;
            _currentConstructionSpace.InstantiateBlueprint(constructItem, character);
        }

        #endregion

        #region Fields

        private ConstructionSpace _currentConstructionSpace;

        private Vector3 _lastPlacementPosition = Vector3.zero;

        private int _currentConstructIndex = 0;

        private float _cooldownBeforeAutoChange = 0.0f;
        //private int _lastTargetOrientation = 0;

        private bool _hasPlaced = false;

        private float _elapsedSinceBuildDeniedSound = 0f;

        private SpriteRenderer _loadoutBGSpriteRenderer;
        private SpriteRenderer _loadoutIconSpriteRenderer;

        #endregion
    }
}