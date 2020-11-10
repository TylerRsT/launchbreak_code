using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public enum UINavigationDirection
    {
        Up,
        Right,
        Down,
        Left,
    }

    /// <summary>
    /// 
    /// </summary>
    public class UINavigationData
    {
        public UINavigationData(UINavigable navigable, UINavigation navigationHandler, string actionName = "")
        {
            this.navigable = navigable;
            this.navigationHandler = navigationHandler;
            this.actionName = actionName;
        }

        public UINavigationData(UINavigable navigable, UINavigation navigationHandler, UINavigationDirection direction)
        {
            this.navigable = navigable;
            this.navigationHandler = navigationHandler;
            this.direction = direction;
        }

        public UINavigable navigable { get; set; }

        public UINavigation navigationHandler { get; set; }

        public string actionName { get; set; }

        public UINavigationDirection direction { get; set; }

        public int controllerIndex { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DisplayName("UI Navigation")]
    public class UINavigation : MonoBehaviour
    {
        #region Const

        public const int InvalidUserIndex = -2;

        private const string OnGotFocusMethodName = "OnGotFocus";
        private const string OnLostFocusMethodName = "OnLostFocus";
        private const string OnSubmitMethodName = "OnSubmit";
        private const string OnCancelMethodName = "OnCancel";
        private const string OnNavigateMethodName = "OnNavigate";

        private const string InputHorizontalAxisName = "UI_Horizontal";
        private const string InputVerticalAxisName = "UI_Vertical";
        private const string InputSubmitName = "UI_Submit";
        private const string InputCancelName = "UI_Cancel";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            if (_currentNavigable == null)
            {
                if (!_autoFocus)
                {
                    enabled = false;
                }
                else
                {
                    Focus();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LateUpdate()
        {
            if(userIndex == InvalidUserIndex)
            {
                return;
            }

            var axisValues = new Vector2(GetHorizontalAxis(), GetVerticalAxis());

            if (_lastAxisValues.x == 0.0f)
            {
                if (axisValues.x < 0.0f)
                {
                    SendNavigation(UINavigationDirection.Left);
                }
                else if (axisValues.x > 0.0f)
                {
                    SendNavigation(UINavigationDirection.Right);
                }
            }

            if (_lastAxisValues.y == 0.0f)
            {
                if (axisValues.y < 0.0f)
                {
                    SendNavigation(UINavigationDirection.Down);
                }
                else if (axisValues.y > 0.0f)
                {
                    SendNavigation(UINavigationDirection.Up);
                }
            }

            var actionName = string.Empty;
            int controllerIndex;

            if (GetSubmitState(out controllerIndex))
            {
                var navigationData = new UINavigationData(_currentNavigable, this, _currentNavigable?.submitActionName);
                navigationData.controllerIndex = controllerIndex;
                SendMessage(OnSubmitMethodName, navigationData, SendMessageOptions.DontRequireReceiver);
                if (_currentNavigable?.gameObject != gameObject)
                {
                    _currentNavigable?.SendMessage(OnSubmitMethodName, navigationData, SendMessageOptions.DontRequireReceiver);
                }
            }
            else if (GetCancelState(out controllerIndex))
            {
                var navigationData = new UINavigationData(_currentNavigable, this, _currentNavigable?.cancelActionName);
                navigationData.controllerIndex = controllerIndex;
                SendMessage(OnCancelMethodName, navigationData, SendMessageOptions.DontRequireReceiver);
                if (_currentNavigable?.gameObject != gameObject)
                {
                    _currentNavigable?.SendMessage(OnCancelMethodName, navigationData, SendMessageOptions.DontRequireReceiver);
                }
            }

            _lastAxisValues = axisValues;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            Unfocus();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void InvalidateAxes()
        {
            _lastAxisValues = Vector2.one;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Focus()
        {
            enabled = true;
            if(_currentNavigable != null)
            {
                Focus(_currentNavigable);
            }
            else
            {
                Focus(_defaultNavigable);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unfocus(bool loseFocusOnCurrentElement = false)
        {
            if(loseFocusOnCurrentElement)
            {
                Focus(null);
            }
            enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigable"></param>
        public void Focus(UINavigable navigable)
        {
            _currentNavigable?.gameObject.SendMessage(OnLostFocusMethodName, _currentNavigable, SendMessageOptions.DontRequireReceiver);
            _currentNavigable = navigable;
            _currentNavigable?.gameObject.SendMessage(OnGotFocusMethodName, _currentNavigable, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axes"></param>
        public void SetLastAxes(Vector2 axes)
        {
            _lastAxisValues = axes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        private void TryMoveFocus(UINavigationDirection direction)
        {
            if(_currentNavigable != null)
            {
                UINavigable nextNavigable = null;
                switch(direction)
                {
                    case UINavigationDirection.Up: nextNavigable = _currentNavigable.up; break;
                    case UINavigationDirection.Right: nextNavigable = _currentNavigable.right; break;
                    case UINavigationDirection.Down: nextNavigable = _currentNavigable.down; break;
                    case UINavigationDirection.Left: nextNavigable = _currentNavigable.left; break;
                }

                if(nextNavigable != null)
                {
                    Focus(nextNavigable);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private float GetHorizontalAxis()
        {
            if(userIndex == -1)
            {
                return InputHelper.GetAnyUIHorizontalAxis();
            }
            return InputHelper.GetUIHorizontalAxis(userIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private float GetVerticalAxis()
        {
            if (userIndex == -1)
            {
                return InputHelper.GetAnyUIVerticalAxis();
            }
            return InputHelper.GetUIVerticalAxis(userIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool GetSubmitState(out int index)
        {
            if(userIndex == -1)
            {
                return InputHelper.AnyButtonDown(InputSubmitName, out index);
            }
            index = userIndex;
            return _playerController.GetButtonDown(InputSubmitName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool GetCancelState(out int index)
        {
            if (userIndex == -1)
            {
                return InputHelper.AnyButtonDown(InputCancelName, out index);
            }
            index = userIndex;
            return _playerController.GetButtonDown(InputCancelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        private void SendNavigation(UINavigationDirection direction)
        {
            var data = new UINavigationData(_currentNavigable, this, direction);
            SendMessage(OnNavigateMethodName, data, SendMessageOptions.DontRequireReceiver);
            TryMoveFocus(direction);
            if (gameObject != _currentNavigable?.gameObject)
            {
                _currentNavigable?.SendMessage(OnNavigateMethodName, data, SendMessageOptions.DontRequireReceiver);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int userIndex
        {
            get
            {
                return _userIndex;
            }
            set
            {
                _userIndex = value;
                if(value >= 0)
                {
                    _playerController = Rewired.ReInput.players.Players[value];
                }
                else
                {
                    _playerController = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool hasFocus => _currentNavigable != null;

        /// <summary>
        /// 
        /// </summary>
        public UINavigable defaultNavigable => _defaultNavigable;

        /// <summary>
        /// 
        /// </summary>
        protected UINavigable currentNavigable => _currentNavigable;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UINavigable _defaultNavigable = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _autoFocus = false;

        private UINavigable _currentNavigable = null;

        private Vector2 _lastAxisValues = Vector2.zero;

        private int _userIndex = -1;
        private Rewired.Player _playerController;

        #endregion
    }
}