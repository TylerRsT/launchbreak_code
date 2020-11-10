using DG.Tweening;
using Elendow.SpritedowAnimator;
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
    [RequireComponent(typeof(UINavigation))]
    public class CharacterSettings : MonoBehaviour
    {
        #region Const

        private const string CharacterConfirmSoundKey = "CharacterConfirm";
        private const string JoinSoundKey = "Join";
        private const string NavigateSoundKey = "Navigate";
        private const string CancelSoundKey = "Cancel";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            GetComponent<UINavigation>().userIndex = _controllerIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _leftArrowSequence = DOTween.Sequence();
            _leftArrowSequence.Append(_leftArrow.DOLocalMoveX(_leftArrow.transform.localPosition.x - 2.0f, 0.3f));
            _leftArrowSequence.Append(_leftArrow.DOLocalMoveX(_leftArrow.transform.localPosition.x, 0.3f));
            _leftArrowSequence.SetLoops(-1).SetUpdate(true);

            _rightArrowSequence = DOTween.Sequence();
            _rightArrowSequence.Append(_rightArrow.DOLocalMoveX(_rightArrow.transform.localPosition.x + 2.0f, 0.3f));
            _rightArrowSequence.Append(_rightArrow.DOLocalMoveX(_rightArrow.transform.localPosition.x, 0.3f));
            _rightArrowSequence.SetLoops(-1).SetUpdate(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnNavigate(UINavigationData data)
        {
            if (_state != State.Selecting)
            {
                return;
            }

            switch (data.direction)
            {
                case UINavigationDirection.Up:
                    _selectedSkinIndex += 1;
                    break;
                case UINavigationDirection.Down:
                    _selectedSkinIndex -= 1;
                    break;
                case UINavigationDirection.Right:
                    do
                    {
                        _selectedCharacterIndex += 1;
                        _selectedCharacterIndex = MathExtensions.Mod(_selectedCharacterIndex, Bootstrap.instance.data.characters.Count);
                    } while (!matchSettingsManager.IsCharacterAvailable(characterDescriptor));
                    _selectedSkinIndex = 0;
                    break;
                case UINavigationDirection.Left:
                    do
                    {
                        _selectedCharacterIndex -= 1;
                        _selectedCharacterIndex = MathExtensions.Mod(_selectedCharacterIndex, Bootstrap.instance.data.characters.Count);
                    } while (!matchSettingsManager.IsCharacterAvailable(characterDescriptor));
                    _selectedSkinIndex = 0;
                    break;
            }

            while (_selectedSkinIndex < 0)
            {
                _selectedSkinIndex = characterDescriptor.skinDescriptors.Count;
            }

            _selectedSkinIndex %= characterDescriptor.skinDescriptors.Count;

            if(data.direction == UINavigationDirection.Up || data.direction == UINavigationDirection.Down)
            {
                return;
            }

            this.EmitSound(NavigateSoundKey);

            TweenExtensions.SafeComplete(ref _cycleSequence);
            _cycleSequence = DOTween.Sequence();
            _cycleSequence.Append(transform.DOLocalMoveX(transform.position.x + 3f, 0.05f));
            _cycleSequence.Append(transform.DOLocalMoveX(transform.position.x - 3f, 0.05f));
            _cycleSequence.Append(transform.DOLocalMoveX(transform.position.x, 0.05f));
            _cycleSequence.SetLoops(1);

            UpdateCharacterInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnSubmit(UINavigationData data)
        {
            if(_useCPU)
            {
                return;
            }

            switch (_state)
            {
                case State.NotJoining:
                    _state = State.Selecting;
                    _joinButton.gameObject.SetActive(false);
                    _addCPUButton.gameObject.SetActive(false);
                    _characterNameSelector.gameObject.SetActive(true);
                    _characterBackground.color = new Color(0.5f, 0.5f, 0.5f);
                    this.EmitSound(JoinSoundKey);

                    while(!matchSettingsManager.IsCharacterAvailable(characterDescriptor))
                    {
                        GotoNextAvailableCharacter();
                    }

                    UpdateCharacterInfo();
                    break;
                case State.Selecting:
                    _characterBackground.color = Color.white;
                    OnCharacterSelected();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnCancel(UINavigationData data)
        {
            if(_useCPU)
            {
                return;
            }

            switch (_state)
            {
                case State.Selecting:
                    _state = State.NotJoining;
                    _joinButton.gameObject.SetActive(true);
                    _addCPUButton.gameObject.SetActive(true);
                    _characterNameSelector.gameObject.SetActive(false);
                    _characterBackground.color = Color.white;
                    this.EmitSound(CancelSoundKey);
                    UpdateCharacterInfo();
                    break;
                case State.Selected:
                    this.EmitSound(CancelSoundKey);
                    break;
                case State.Ready:
                    _state = State.Selecting;
                    _characterBackground.color = new Color(0.5f, 0.5f, 0.5f);
                    this.EmitSound(CancelSoundKey);
                    OnCharacterUnselected();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            TweenExtensions.SafeKill(ref _leftArrowSequence);
            TweenExtensions.SafeKill(ref _rightArrowSequence);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void ResetSettings()
        {
            _state = State.NotJoining;
            _joinButton.gameObject.SetActive(true);
            _addCPUButton.gameObject.SetActive(true);
            _characterNameSelector.gameObject.SetActive(false);
            _characterBackground.color = Color.white;
            UpdateCharacterInfo();
            OnCharacterUnselected();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="focus"></param>
        public void SetFocus(bool focus)
        {
            if (focus)
            {
                GetComponent<UINavigation>().Focus();
                if (_state == State.Selected || _state == State.Ready)
                {
                    OnCancel(null);
                }
            }
            else
            {
                GetComponent<UINavigation>().Unfocus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GotoNextAvailableCharacter()
        {
            OnNavigate(new UINavigationData(null, null, UINavigationDirection.Right));
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCharacterInfo()
        {
            _characterBackground.sprite = isJoining ? characterSkinLightDescriptor.selectSprite : _joinSprite;
            _characterNameText.text = characterSkinLightDescriptor.name;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnCharacterSelected()
        {
            _joinButton.gameObject.SetActive(false);
            _addCPUButton.gameObject.SetActive(false);
            _characterNameSelector.gameObject.SetActive(true);
            _characterBackground.color = Color.white;
            _confirmAnimation.gameObject.SetActive(true);
            _leftArrow.gameObject.SetActive(false);
            _rightArrow.gameObject.SetActive(false);
            var animator = _confirmAnimation.GetComponent<SpriteAnimator>();
            _state = State.Selected;
            animator.Play(animator.StartAnimation, true);
            animator.onFinish.AddListener(() =>
            {
                _confirmAnimation.gameObject.SetActive(false);
                _readyCaption.gameObject.SetActive(true);
                _state = State.Ready;
            });
            transform.DOShakePosition(0.3f, 10.0f, 50);
            this.EmitSound(CharacterConfirmSoundKey);
            this.StartVibrationCoroutine(controllerIndex, 0.7f, 0.7f, 0.3f);
            matchSettingsManager.OnCharacterSelected(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnCharacterUnselected()
        {
            var animator = _confirmAnimation.GetComponent<SpriteAnimator>();
            animator.Stop();
            _confirmAnimation.gameObject.SetActive(false);
            _readyCaption.gameObject.SetActive(false);
            _leftArrow.gameObject.SetActive(true);
            _rightArrow.gameObject.SetActive(true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int playerIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int controllerIndex
        {
            get
            {
                return _controllerIndex;
            }
            set
            {
                _controllerIndex = value;
                GetComponent<UINavigation>().userIndex = value;

                if (value == UINavigation.InvalidUserIndex)
                {
                    _controllerImage.enabled = false;
                }
                else
                {
                    _controllerImage.enabled = true;
                    _controllerImage.sprite = value == 0 ? _keyboardSprites[playerIndex] : _gamepadSprites[playerIndex];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isJoining => _state != State.NotJoining;

        /// <summary>
        /// 
        /// </summary>
        public bool hasSelected
        {
            get
            {
                return _state == State.Selected || isReady;
            }
            set
            {
                _state = _useCPU ? State.ReadyCPU : State.Ready;
                OnSubmit(null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isReady => _state == State.Ready || _state == State.ReadyCPU;

        /// <summary>
        /// 
        /// </summary>
        public CharacterDescriptor characterDescriptor
        {
            get
            {
                return Bootstrap.instance.data.characters[_selectedCharacterIndex];
            }
            set
            {
                _selectedCharacterIndex = Bootstrap.instance.data.characters.ToList().IndexOf(value);
                _joinButton.gameObject.SetActive(false);
                _addCPUButton.gameObject.SetActive(false);
                _characterNameSelector.gameObject.SetActive(true);
                _leftArrow.gameObject.SetActive(false);
                _rightArrow.gameObject.SetActive(false);

                UpdateCharacterInfo();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CharacterSkinLightDescriptor characterSkinLightDescriptor => characterDescriptor.skinDescriptors[_selectedSkinIndex];

        /// <summary>
        /// 
        /// </summary>
        public CharacterSkinDescriptor characterSkinDescriptor
        {
            get
            {
                if (characterSkinLightDescriptor.skinDescriptor == null)
                {
                    characterSkinLightDescriptor.Load();
                }

                return characterSkinLightDescriptor.skinDescriptor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MatchSettingsManager matchSettingsManager { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool useCPU
        {
            get { return _useCPU; }
            set
            {
                _useCPU = value;
                _joinButton.gameObject.SetActive(!value);
                _addCPUButton.gameObject.SetActive(!value);
                _controllerImage.gameObject.SetActive(!value);
                _removeCPUButton.gameObject.SetActive(value);

                _characterBackground.sprite = value ? _cpuSprite : _joinSprite;
                _state = value ? State.ReadyCPU : State.NotJoining;
                this.EmitSound(_useCPU ? JoinSoundKey : CancelSoundKey);
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _selectedCharacterIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _selectedSkinIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _characterBackground = default;
    
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform _characterNameSelector = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _characterNameText = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UIButton _joinButton = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _confirmAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _readyCaption = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _controllerImage = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite[] _gamepadSprites = new Sprite[GameConstants.MaxPlayerCount];

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite[] _keyboardSprites = new Sprite[GameConstants.MaxPlayerCount];

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform _leftArrow = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform _rightArrow = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _joinSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _addCPUButton = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _removeCPUButton = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _cpuSprite = default;

        private Sequence _cycleSequence = null;
        private int _controllerIndex = 0;

        private Sequence _leftArrowSequence;
        private Sequence _rightArrowSequence;

        private State _state = State.NotJoining;

        private bool _useCPU = false;

        #endregion

        #region Nested

        /// <summary>
        /// 
        /// </summary>
        private enum State
        {
            NotJoining,
            Selecting,
            Selected,
            Ready,
            ReadyCPU,
        }

        #endregion
    }
}