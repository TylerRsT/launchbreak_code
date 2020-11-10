using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class MatchSettingsManager : ControllersInfoProvider
    {
        #region Const

        private const float ScreenWidth = 640.0f;
        private const float BackButtonWidth = 128.0f;

        private const int MinimumNumberOfPlayers = 2;

        private const string SubmitInputName = "UI_Submit";
        private const string CancelInputName = "UI_Cancel";
        private const string NextInputName = "UI_Next";
        private const string PrevInputName = "UI_Prev";
        private const string SettingsInputName = "UI_Settings";
        private const string AddCPUInputName = "UI_AddCPU";
        private const string RemoveCPUInputName = "UI_RemoveCPU";

        private const string BackStartSoundKey = "BackStart";
        private const string BackEndSoundKey = "BackEnd";

        private const string GameModesFolder = "Content/GameModes";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override bool hasKeyboardOnly => _joystickNames == null || _joystickNames.All(x => string.IsNullOrEmpty(x));

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _characterSettings = GetComponentsInChildren<CharacterSettings>();
            _mapSettings = GetComponentInChildren<MapSettings>();

            GameModeParams.instance.firstGame = true;

            if (!showMaps)
            {
                _joystickNames = null;
                for (var i = 0; i < _characterSettings.Length; ++i)
                {
                    _characterSettings[i].playerIndex = i;
                    _characterSettings[i].controllerIndex = i == 0 ? 0 : UINavigation.InvalidUserIndex;
                    _characterSettings[i].matchSettingsManager = this;
                }
            }
            else
            {
                for (var i = 0; i < _characterSettings.Length; ++i)
                {
                    _characterSettings[i].playerIndex = i;
                    _characterSettings[i].matchSettingsManager = this;

                    var playerParams = GameModeParams.instance.playerParams[i];

                    if (!playerParams.isPlaying)
                    {
                        _characterSettings[i].controllerIndex = i == 0 ? 0 : UINavigation.InvalidUserIndex;
                        continue;
                    }

                    _characterSettings[i].hasSelected = true;
                    if (playerParams.controllerIndex == -1)
                    {
                        _characterSettings[i].useCPU = true;
                        continue;
                    }

                    _characterSettings[i].controllerIndex = playerParams.controllerIndex;
                    _characterSettings[i].characterDescriptor = playerParams.selectedCharacter;
                }
            }

            if(Debug.isDebugBuild)
            {
                _availableGameModes = Resources.LoadAll<GameModeDescriptor>(GameModesFolder);
            }
            else
            {
                _availableGameModes = Bootstrap.instance.data.gameModes.ToArray();
            }

            ChangeGameMode(0);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            if (Debug.isDebugBuild && _gameModeSelectionPanel != null)
            {
                _gameModeSelectionPanel.SetActive(true);
            }

            if(showMaps)
            {
                _state = State.MapSelection;
                var position = transform.position;

                position.x = ScreenWidth * -1.0f;
                transform.position = position;
            }

            SetCurrentStateActive(true);
            if(!MenuMusicController.isPlaying)
            {
                MenuMusicController.StartMusic();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if(_gameStarted)
            {
                return;
            }

            if (InputHelper.AnyButtonDown(SettingsInputName))
            {
                var settingPanel = GameObject.FindObjectOfType<SettingsPanelNavigationHandler>();

                if (settingPanel == null)
                {
                    var allNavigation = GetComponentsInChildren<UINavigation>();
                    foreach(var navigation in allNavigation)
                    {
                        navigation.Unfocus();
                    }

                    SettingsPanelNavigationHandler.OpenSettingsPanel(() =>
                    {
                        foreach (var navigation in allNavigation)
                        {
                            navigation.Focus();
                        }
                    });
                }
                else
                {
                    SettingsPanelNavigationHandler.CloseSettingsPanel();
                }
            }

            if (InputHelper.AnyButtonDown(AddCPUInputName))
            {
                var nextCharacterCPU = _characterSettings.FirstOrDefault(x => !x.isJoining);
                if (nextCharacterCPU != null)
                {
                    nextCharacterCPU.useCPU = true;
                }
            }

            if (InputHelper.AnyButtonDown(RemoveCPUInputName))
            {
                var lastCharacterCPU = _characterSettings.LastOrDefault(x => x.useCPU);
                if (lastCharacterCPU != null)
                {
                    lastCharacterCPU.useCPU = false;
                }
            }

            switch (_state)
            {
                case State.CharacterSelection:
                    UpdateCharacterSelection();
                    break;
                case State.MapSelection:
                    UpdateMapSelection();
                    break;
                case State.Transition:
                    _elapsedSinceBack = Mathf.Max(0.0f, _elapsedSinceBack - GameplayStatics.gameDeltaTime);
                    UpdateBackMask();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            if(InputHelper.AnyAxis(NextInputName) != 0.0f)
            {
                if(!_isNextPressed)
                {
                    ChangeGameMode(1);
                }
                _isNextPressed = true;
            }
            else
            {
                _isNextPressed = false;
            }

            if (InputHelper.AnyAxis(PrevInputName) != 0.0f)
            {
                if (!_isPrevPressed)
                {
                    ChangeGameMode(-1);
                }
                _isPrevPressed = true;
            }
            else
            {
                _isPrevPressed = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characterDescriptor"></param>
        /// <returns></returns>
        public bool IsCharacterAvailable(CharacterDescriptor characterDescriptor)
        {
            foreach(var characterSettings in _characterSettings)
            {
                if(characterSettings.hasSelected && !characterSettings.useCPU && characterSettings.characterDescriptor == characterDescriptor)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        public void OnCharacterSelected(CharacterSettings selector)
        {
            foreach(var characterSettings in _characterSettings)
            {
                if(characterSettings == selector)
                {
                    continue;
                }

                if(characterSettings.characterDescriptor == selector.characterDescriptor)
                {
                    characterSettings.GotoNextAvailableCharacter();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GotoMapSelection()
        {
            Transition(State.MapSelection);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCharacterSelection()
        {
            if(CheckTransitionBack(State.None))
            {
                GameplayStatics.TransitionToScene(GameConstants.TitleScreenScene);
                return;
            }

            UpdateControllers();

            var allReady = _characterSettings.Where(x => x.isJoining).All(x => x.hasSelected);
            var playersReady = _characterSettings.Where(x => x.isReady).ToArray();
            var playersReadyCount = playersReady.Count(x => x.isReady);
            var playersJoinCount = _characterSettings.Count(x => x.isJoining);

            var enoughPlayers = playersReadyCount >= (Debug.isDebugBuild ? 1 : MinimumNumberOfPlayers);

            if(playersJoinCount < 2 && playersReadyCount >= 1 && !Debug.isDebugBuild)
            {
                _playerMinPrompt.Open();
            }
            else if(_playerMinPrompt.isOpen)
            {
                _playerMinPrompt.Close();
            }

            if (allReady && enoughPlayers)
            {
                if (!_continuePrompt.isOpen)
                {
                    _continuePrompt.Open();
                }
            }
            else if(_continuePrompt.isOpen)
            {
                _continuePrompt.Close();
            }

            if(_continuePrompt.isOpen)
            {
                int controllerIndex;
                if (InputHelper.AnyButtonDown(SubmitInputName, out controllerIndex))
                {
                    if (playersReady.Any(x => x.controllerIndex == controllerIndex && x.isReady)
                        || playersReady.All(x => x.useCPU))
                    {
                        GotoMapSelection();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateControllers()
        {
            var joystickNames = Input.GetJoystickNames();
            if (_joystickNames == null)
            {
                _joystickNames = new string[GameConstants.MaxPlayerCount];
            }

            var differentIndex = -1;

            for (var i = 0; i < joystickNames.Length; ++i)
            {
                if (_joystickNames[i] != joystickNames[i])
                {
                    differentIndex = i;
                    break;
                }
            }

            if (differentIndex >= 0)
            {
                for (var i = 0; i < GameConstants.MaxPlayerCount; ++i)
                {
                    _characterSettings[i].ResetSettings();
                    _characterSettings[i].controllerIndex = i + 1;
                }

                var hasSetKeyboardIndex = false;
                for (var i = differentIndex; i < GameConstants.MaxPlayerCount; ++i)
                {
                    if(hasSetKeyboardIndex)
                    {
                        _characterSettings[i].controllerIndex = UINavigation.InvalidUserIndex;
                    }
                    else if (i >= joystickNames.Length)
                    {
                        _characterSettings[i].controllerIndex = 0;
                        hasSetKeyboardIndex = true;
                    }
                }
            }

            System.Array.Copy(joystickNames, 0, _joystickNames, 0, joystickNames.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateMapSelection()
        {
            if(CheckTransitionBack(State.CharacterSelection))
            {
                return;
            }

            if(_mapSettings.isReady && !_continuePrompt.isOpen)
            {
                StartGame();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateLoadoutSelection()
        {
            if(CheckTransitionBack(State.MapSelection))
            {
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prevState"></param>
        /// <returns></returns>
        private bool CheckTransitionBack(State prevState)
        {
            if(InputHelper.AnyButtonDown(CancelInputName))
            {
                this.EmitSound(BackStartSoundKey);
            }

            if (InputHelper.AnyButton(CancelInputName))
            {
                _elapsedSinceBack += GameplayStatics.gameDeltaTime;
                if (_elapsedSinceBack >= _backTimeout)
                {
                    this.EmitSound(BackEndSoundKey);
                    Transition(prevState);
                    return true;
                }
            }
            else
            {
                _elapsedSinceBack = Mathf.Max(0.0f, _elapsedSinceBack - GameplayStatics.gameDeltaTime);
            }

            UpdateBackMask();

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateBackMask()
        {
            var percentage = Mathf.Clamp(_elapsedSinceBack / _backTimeout, 0.0f, 1.0f);
            var backMaskX = BackButtonWidth - (BackButtonWidth * percentage);

            var backMaskPosition = _backMaskImage.transform.localPosition;
            backMaskPosition.x = backMaskX;
            _backMaskImage.transform.localPosition = backMaskPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextState"></param>
        private void Transition(State nextState)
        {
            var position = 0.0f;
            switch(nextState)
            {
                case State.CharacterSelection: position = 0.0f; break;
                case State.MapSelection: _continuePrompt.Close(); position = ScreenWidth * -1.0f; break;
            }

            Transition(position, () =>
            {
                SetState(nextState);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="callback"></param>
        private void Transition(float targetPosition, TweenCallback callback)
        {
            SetState(State.Transition);
            transform.DOMoveX(targetPosition, _transitionDuration).SetEase(_transitionEase, _easeAmplitude, _easeDuration)
                .onComplete += (callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextState"></param>
        private void SetState(State nextState)
        {
            if(nextState == _state)
            {
                return;
            }

            SetCurrentStateActive(false);
            _state = nextState;
            SetCurrentStateActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        private void SetCurrentStateActive(bool active)
        {
            switch(_state)
            {
                case State.CharacterSelection:
                    _backButton.gameObject.SetActive(true);
                    foreach (var characterSettings in _characterSettings)
                    {
                        characterSettings.SetFocus(active);
                    }
                    break;
                case State.MapSelection:
                    _backButton.gameObject.SetActive(true);
                    _mapSettings.SetFocus(active);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartGame()
        {
            var bootstrapData = Bootstrap.instance.data;

            var cpuCharacters = new List<CharacterDescriptor>();
            foreach(var character in bootstrapData.characters)
            {
                if(IsCharacterAvailable(character))
                {
                    cpuCharacters.Add(character);
                }
            }

            for(var i = 0; i < _characterSettings.Length; ++i)
            {
                var characterSettings = _characterSettings[i];
                var playerParams = GameModeParams.instance.playerParams[i];
                playerParams.controllerIndex = characterSettings.controllerIndex;

                playerParams.isPlaying = characterSettings.isJoining;
                if(!characterSettings.isJoining)
                {
                    continue;
                }

                playerParams.selectedCharacter = characterSettings.characterDescriptor;
                playerParams.selectedSkin = characterSettings.characterSkinDescriptor;

                if(characterSettings.useCPU)
                {
                    var rand = Random.Range(0, cpuCharacters.Count);
                    playerParams.selectedCharacter = cpuCharacters[rand];

                    var skinLightDescriptor = cpuCharacters[rand].skinDescriptors.First();
                    skinLightDescriptor.Load();
                    playerParams.selectedSkin = skinLightDescriptor.skinDescriptor;

                    playerParams.controllerIndex = -1;
                    cpuCharacters.RemoveAt(rand);
                }

                playerParams.loadout[0] = bootstrapData.defaultConstruct;

                if(characterSettings.controllerIndex > 0)
                {
                    this.StartVibrationCoroutine(characterSettings.controllerIndex, 0.7f, 0.7f, 0.3f);
                }
            }

            GameModeParams.instance.selectedMap = _mapSettings.selectedMap.sceneName;
            GameModeParams.instance.selectedGameModePrefab = _availableGameModes[_gameModeIndex].gameModePrefab;
            StartCoroutine(LoadGame());

            _gameStarted = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadGame()
        {
            MenuMusicController.StopMusic();
            yield return new WaitForSecondsRealtime(1.0f);
            GameplayStatics.TransitionToScene(_mapSettings.selectedMap.sceneName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void ChangeGameMode(int value)
        {
            var gameModeCount = _availableGameModes.Length;
            _gameModeIndex += value;

            while(_gameModeIndex < 0)
            {
                _gameModeIndex += gameModeCount;
            }

            _gameModeIndex %= gameModeCount;
            _gameModeNameText.text = _availableGameModes[_gameModeIndex].name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static bool showMaps { get; set; }

        #endregion

        #region Fields

        private static string[] _joystickNames = null;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UIButton _backButton = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _backMaskImage = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _gameModeNameText = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _transitionDuration = 0.5f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Ease _transitionEase = Ease.OutBounce;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _easeDuration = 0.1f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _easeAmplitude = 10.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _backTimeout = 2.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _gameModeSelectionPanel = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private PlayerNeededPrompt _playerMinPrompt = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private PlayerNeededPrompt _continuePrompt = default;

        private CharacterSettings[] _characterSettings;
        private MapSettings _mapSettings;

        private State _state = State.CharacterSelection;

        private float _elapsedSinceBack = 0.0f;

        private int _gameModeIndex = 0;
        private bool _isPrevPressed = false;
        private bool _isNextPressed = false;
        private bool _gameStarted = false;

        private GameModeDescriptor[] _availableGameModes;

        #endregion

        #region Nested

        /// <summary>
        /// 
        /// </summary>
        private enum State
        {
            Transition,
            None,
            CharacterSelection,
            MapSelection,
        }

        #endregion
    }
}