using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerLoadoutSettings : MonoBehaviour
    {
        #region Const

        private const string CancelInputName = "UI_Cancel";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            /*_loadoutSettings = GetComponentInChildren<LoadoutSettings>();
            _loadoutSettings.playerLoadoutSettings = this;
            _loadoutSettings.playerIndex = playerIndex;*/

            _armorySettings = GetComponentInChildren<ArmorySettings>();
            _armorySettings.playerLoadoutSettings = this;
            _armorySettings.playerIndex = playerIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (isReady && Input.GetButtonDown($"{CancelInputName}{playerIndex}"))
            {
                SetFocus(true);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skinLightDescriptor"></param>
        public void BeforeFocus(CharacterSkinLightDescriptor skinLightDescriptor)
        {
            this.skinLightDescriptor = skinLightDescriptor;

            //_loadoutSettings.transform.localScale = Vector3.one;
            _armorySettings.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="focus"></param>
        public void SetFocus(bool focus)
        {
            isReady = false;
            _readyCaption.gameObject.SetActive(false);
            if (focus)
            {
                GoToArmory(_loadoutItemPresenter);
                SetState(State.Armory);
            }
            else
            {
                SetCurrentStateActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPresenter"></param>
        public void ValidateItem(LoadoutItemPresenter targetPresenter)
        {
            _armorySettings.SetFocus(false);
            _readyCaption.gameObject.SetActive(true);
            isReady = true;
            /*Transition(State.LoadoutOverview);
            _loadoutSettings.ValidateItem(targetPresenter);*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPresenter"></param>
        public void GoToArmory(LoadoutItemPresenter targetPresenter)
        {
            _armorySettings.targetPresenter = targetPresenter;
            _armorySettings.presenterType = targetPresenter.acceptedTypes;
            _armorySettings.BuildItems();

            //Transition(State.Armory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextState"></param>
        private void Transition(State nextState)
        {
            GetCurrentStateTransform().DOScaleX(0.0f, 0.1f)
                .onComplete += (() =>
                {
                    SetState(nextState);
                    GetCurrentStateTransform().DOScaleX(1.0f, 0.1f);
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Transform GetCurrentStateTransform()
        {
            switch (_state)
            {
                /*case State.LoadoutOverview:
                    return _loadoutSettings.transform;*/
                case State.Armory:
                    return _armorySettings.transform;
                default:
                    throw new System.Exception($"No transform associated with state '{_state}'");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextState"></param>
        private void SetState(State nextState)
        {
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
            switch (_state)
            {
                case State.LoadoutOverview:
                    //_loadoutSettings.SetFocus(active);
                    break;
                case State.Armory:
                    _armorySettings.SetFocus(active);
                    break;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int playerIndex
        {
            get => _playerIndex;
            set => _playerIndex = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isReady { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool isJoining => gameObject.activeSelf;

        /// <summary>
        /// 
        /// </summary>
        public LoadoutItem[] loadout => new LoadoutItem[1] { _loadoutItemPresenter.item }; //_loadoutSettings.loadout;

        /// <summary>
        /// 
        /// </summary>
        public CharacterSkinLightDescriptor skinLightDescriptor
        {
            get { return _skinLightDescriptor; }
            set
            {
                _skinLightDescriptor = value;
                if(value != null)
                {
                    _bannerImage.sprite = value.bannerSprite;
                    _characterNameText.text = value.name;
                }
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _playerIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _bannerImage = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _characterNameText = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _readyCaption = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private LoadoutItemPresenter _loadoutItemPresenter = default;

        private CharacterSkinLightDescriptor _skinLightDescriptor;

        //private LoadoutSettings _loadoutSettings;
        private ArmorySettings _armorySettings;

        private State _state = State.Armory;

        #endregion

        #region Nested

        /// <summary>
        /// 
        /// </summary>
        public enum State
        {
            LoadoutOverview,
            Armory,
            LoadoutItemDescriptor,

            Transition,
        }

        #endregion
    }
}