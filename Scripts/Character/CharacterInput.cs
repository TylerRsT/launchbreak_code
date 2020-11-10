using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Character))]
    public class CharacterInput : MonoBehaviour
    {
        #region Const

        private const int KeyboardUserIndex = 0;

        private const string HorizontalMoveInputName = "Gameplay_HorizontalMove";
        private const string VerticalMoveInputName = "Gameplay_VerticalMove";

        private const string LeftMoveInputName = "Gameplay_LeftMove";
        private const string RightMoveInputName = "Gameplay_RightMove";
        private const string UpMoveInputName = "Gameplay_UpMove";
        private const string DownMoveInputName = "Gameplay_DownMove";

        private const string HorizontalTargetInputName = "Gameplay_HorizontalTarget";
        private const string VerticalTargetInputName = "Gameplay_VerticalTarget";

        private const string FireInputName = "Gameplay_Fire";
        private const string ConstructionStateInputName = "Gameplay_ConstructionState";
        private const string DashInputName = "Gameplay_Dash";
        private const string PickInputName = "Gameplay_Pick";
        private const string DropInputName = "Gameplay_Drop";
        private const string SwitchInputName = "Gameplay_Switch";
        private const string ActivateInputName = "Gameplay_Activate";

        private const float IntervalBetweenFires = 0.15f;
        private const float ThrowInputDelay = 0.5f;

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _character = GetComponent<Character>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            //_playerController = Rewired.ReInput.players.GetPlayer(_userIndex);
            _playerController = Rewired.ReInput.players.Players[_userIndex];
            if(_userIndex == KeyboardUserIndex)
            {
                _originalMousePosition = Input.mousePosition;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if(inputLocked)
            {
                return;
            }

            if (_playerController.GetButtonDown(InputName(DashInputName)))
            {
                _character.Action(CharacterAction.Dash);
            }
            else if (_playerController.GetButtonDown(InputName(DropInputName)))
            {
                _character.Action(CharacterAction.Drop);
            }
            else if (_playerController.GetButtonDown(InputName(PickInputName)))
            {
                _character.Action(CharacterAction.Pick);
            }
            else if (_playerController.GetButtonUp(InputName(SwitchInputName)) && !_hasThrown)
            {
                _character.Action(CharacterAction.Switch);
            }
            else if (_playerController.GetButtonDown(InputName(ActivateInputName)))
            {
                _character.Action(CharacterAction.Activate);
            }
            else if (_playerController.GetButton(InputName(SwitchInputName)))
            {
                _switchHeldTime += GameplayStatics.gameDeltaTime;
                if(_switchHeldTime >= ThrowInputDelay)
                {
                    _hasThrown = true;
                    _character.Action(CharacterAction.Throw);
                }
            }
            else
            {
                _switchHeldTime = 0.0f;
                _hasThrown = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            var wasFiring = _isFiring;
            _isFiring = _playerController.GetAxis(InputName(FireInputName)) != 0.0f;

            if (_currentVibration.isValid)
            {
                _currentVibration.elapsed += GameplayStatics.gameFixedDeltaTime;
                if (_currentVibration.elapsed >= _currentVibration.duration)
                {
                    _currentVibration.isValid = false;
                    InputHelper.StopVibration(_userIndex);
                }
            }

            if (inputLocked)
            {
                _character.Orientate(Vector2.zero, Vector2.zero);
                _character.HandleConstructionMode(0.0f);
                return;
            }

            var horizontalMove = InputHelper.GetGameplayHorizontalAxis(userIndex);
            var verticalMove = InputHelper.GetGameplayVerticalAxis(userIndex);

            var horizontalTarget = 0.0f;
            var verticalTarget = 0.0f;

            if(userIndex != KeyboardUserIndex)
            {
                horizontalTarget = _playerController.GetAxis(InputName(HorizontalTargetInputName));
                verticalTarget = _playerController.GetAxis(InputName(VerticalTargetInputName));
            }

            _character.Orientate(new Vector2(horizontalMove, verticalMove), new Vector2(horizontalTarget, verticalTarget));
            _character.HandleConstructionMode(_playerController.GetAxis(InputName(ConstructionStateInputName)));

            _elapsedSinceLastFire += GameplayStatics.gameFixedDeltaTime;

            if (_elapsedSinceLastFire >= IntervalBetweenFires && _isFiring)
            {
                _character.Action(CharacterAction.Fire);
                _elapsedSinceLastFire = 0.0f;
            }
            else if(wasFiring && !_isFiring)
            {
                _character.Action(CharacterAction.Release);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            _currentVibration.isValid = false;
            InputHelper.StopVibration(_userIndex);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Default vibration values.
        /// </summary>
        public void Vibrate()
        {
            Vibrate(0.75f, 0.75f, 0.2f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="duration"></param>
        public void Vibrate(float left, float right, float duration)
        {
            if(_userIndex <= 0)
            {
                return;
            }

            if (vibrationMultiplier == 0.0f)
            {
                return;
            }

            _currentVibration = new VibrationInfo
            {
                isValid = true,

                left = left * vibrationMultiplier,
                right = right * vibrationMultiplier,
                elapsed = 0.0f,
                duration = duration,
            };

            InputHelper.SetVibration(_userIndex, _currentVibration.left, _currentVibration.right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        private string InputName(string baseName)
        {
            return baseName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int userIndex
        {
            get { return _userIndex; }
            set { _userIndex = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public float vibrationMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        public bool inputLocked
        {
            get => _inputLocked;
            set => _inputLocked = value;
        }

        private bool _inputLocked = false;

        /// <summary>
        /// 
        /// </summary>
        public bool isFiring => _isFiring;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _userIndex;

        private Character _character;

        private float _elapsedSinceLastFire = IntervalBetweenFires;

        private VibrationInfo _currentVibration;

        private float _switchHeldTime = 0.0f;
        private bool _hasThrown = false;

        private bool _isFiring = false;

        private Vector3 _originalMousePosition;

        private Rewired.Player _playerController;

        #endregion

        #region Nested

        /// <summary>
        /// 
        /// </summary>
        private struct VibrationInfo
        {
            public bool isValid;

            public float left;
            public float right;
            public float duration;
            public float elapsed;
        }

        #endregion
    }
}