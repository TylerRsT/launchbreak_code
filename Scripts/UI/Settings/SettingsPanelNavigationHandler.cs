using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsPanelNavigationHandler : MonoBehaviour
    {
        #region Constants

        private const string PrevInputName = "UI_Prev";
        private const string NextInputName = "UI_Next";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            foreach(var tab in GetComponentsInChildren<SettingsTabUINavigation>())
            {
                tab.panelNavigationHandler = this;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            SettingsTabCycle();
            if(isNavigatingTabs && InputHelper.GetAnyUIVerticalAxis() < 0.0f)
            {
                isNavigatingTabs = false;
                _tabs[_currentOptionIndex].Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        private void OnCancel(UINavigationData action)
        {
            CloseSettingsPanel();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public static void OpenSettingsPanel(System.Action closeCallback = null)
        {
            if (_settingsPanelInstance != null)
            {
                return;
            }

            _settingsPanelInstance = Instantiate(Bootstrap.instance.data.settingsPanelPrefab, GameObject.FindObjectOfType<Canvas>().transform)
                .GetComponent<SettingsPanelNavigationHandler>();
            _settingsPanelInstance._closeCallback = closeCallback;
            _settingsPanelInstance.FocusChanged();
        }

        /// <summary>
        ///
        /// </summary>
        public static void CloseSettingsPanel()
        {
            if(_settingsPanelInstance == null)
            {
                return;
            }

            _settingsPanelInstance._closeCallback?.Invoke();
            InputHelper.StopAllVibrations();
            Destroy(_settingsPanelInstance.gameObject);
            _settingsPanelInstance = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SettingsTabCycle()
        {
            if (InputHelper.AnyAxis(PrevInputName) != 0.0f || (isNavigatingTabs && InputHelper.GetAnyUIHorizontalAxis() < 0.0f))
            {
                if (!_isPrevPressed)
                {
                    _currentOptionIndex -= 1;
                    _currentOptionIndex = MathExtensions.Mod(_currentOptionIndex, _optionIndex);
                    FocusChanged();
                }
                _isPrevPressed = true;
            }
            else
            {
                _isPrevPressed = false;
            }

            if (InputHelper.AnyAxis(NextInputName) != 0.0f || (isNavigatingTabs && InputHelper.GetAnyUIHorizontalAxis() > 0.0f))
            {
                if (!_isNextPressed)
                {
                    _currentOptionIndex += 1;
                    _currentOptionIndex = MathExtensions.Mod(_currentOptionIndex, _optionIndex);
                    FocusChanged();
                }
                _isNextPressed = true;
            }
            else
            {
                _isNextPressed = false;
            }

            GameObject selectedPanel = null;
            _tabsToUnfocus.Clear();


            switch (_optionTexts[_currentOptionIndex])
            {
                case "Gameplay":
                    _tabsToUnfocus.Add(_controlsPanel.GetComponent<SettingsTabUINavigation>());
                    _tabsToUnfocus.Add(_videoPanel.GetComponent<SettingsTabUINavigation>());
                    _tabsToUnfocus.Add(_audioPanel.GetComponent<SettingsTabUINavigation>());

                    _gameplayPanel.SetActive(true);
                    _controlsPanel.SetActive(false);
                    _videoPanel.SetActive(false);
                    _audioPanel.SetActive(false);

                    selectedPanel = _gameplayPanel;
                    break;

                case "Controls":
                    _tabsToUnfocus.Add(_gameplayPanel.GetComponent<SettingsTabUINavigation>());
                    _tabsToUnfocus.Add(_videoPanel.GetComponent<SettingsTabUINavigation>());
                    _tabsToUnfocus.Add(_audioPanel.GetComponent<SettingsTabUINavigation>());

                    _controlsPanel.SetActive(true);

                    selectedPanel = _controlsPanel;
                    break;

                case "Video":
                    _tabsToUnfocus.Add(_gameplayPanel.GetComponent<SettingsTabUINavigation>());
                    _tabsToUnfocus.Add(_controlsPanel.GetComponent<SettingsTabUINavigation>());
                    _tabsToUnfocus.Add(_audioPanel.GetComponent<SettingsTabUINavigation>());

                    _videoPanel.SetActive(true);

                    selectedPanel = _videoPanel;
                    break;

                case "Audio":
                    _tabsToUnfocus.Add(_gameplayPanel.GetComponent<SettingsTabUINavigation>());
                    _tabsToUnfocus.Add(_controlsPanel.GetComponent<SettingsTabUINavigation>());
                    _tabsToUnfocus.Add(_videoPanel.GetComponent<SettingsTabUINavigation>());

                    _audioPanel.SetActive(true);

                    selectedPanel = _audioPanel;
                    break;
            }

            foreach(var tabToUnfocus in _tabsToUnfocus)
            {
                if(tabToUnfocus != null)
                {
                    tabToUnfocus.Unfocus(true);
                    tabToUnfocus.gameObject.SetActive(false);
                }
            }

            var navigation = selectedPanel?.GetComponent<SettingsTabUINavigation>();
            if (navigation != null)
            {
                navigation.panelNavigationHandler = this;
                if (!isNavigatingTabs && !navigation.hasFocus)
                {
                    navigation.SetLastAxes(Vector2.one);
                    navigation.Focus();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void FocusChanged()
        {
            for (int i = 0; i < _optionIndex; i++)
            {
                if (i == _currentOptionIndex)
                {
                    _tabs[i].Focus();
                }
                else
                {
                    _tabs[i].Unfocus();
                }
            }

            InputHelper.StopAllVibrations();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool isNavigatingTabs { get; set; }

        #endregion

        #region Fields

        private static SettingsPanelNavigationHandler _settingsPanelInstance;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string[] _optionTexts = new string[]
        {
            "Gameplay",
            "Controls",
            "Video",
            "Audio",
        };

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private UITab[] _tabs = new UITab[]
        {
            default,
            default,
            default,
            default,
        };

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _optionIndex = 3;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _gameplayPanel = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _controlsPanel = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _videoPanel = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _audioPanel = default;

        private bool _isNextPressed = false;
        private bool _isPrevPressed = false;
        private int _currentOptionIndex = 0;
        private System.Action _closeCallback;

        private List<SettingsTabUINavigation> _tabsToUnfocus = new List<SettingsTabUINavigation>(); // Hot patch, would need refactor.

        #endregion
    }
}
