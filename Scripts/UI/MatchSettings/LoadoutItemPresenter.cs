using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [System.Flags]
    public enum LoadoutItemPresenterType
    {
        None = 0,
        Construct = 1 << 0,
        Ability = 1 << 2,

        Both = Construct | Ability,
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoadoutItemPresenter : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _selfImage = GetComponent<Image>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            OnItemChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigable"></param>
        private void OnGotFocus(UINavigable navigable)
        {
            _hasFocus = true;
            OnItemChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigable"></param>
        private void OnLostFocus(UINavigable navigable)
        {
            _hasFocus = false;
            OnItemChanged();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void OnItemChanged()
        {
            if(_selfImage == null)
            {
                _selfImage = GetComponent<Image>();
            }

            if(_item == null)
            {
                _selfImage.sprite = _hasFocus ? _constructBackgroundOn : _constructBackgroundOff;
                _itemImage.sprite = null;
                _itemImage.color = new Color();
            }

            switch(_item)
            {
                case ConstructLoadoutItem constructItem:
                    _selfImage.sprite = _hasFocus ? _constructBackgroundOn : _constructBackgroundOff;
                    _itemImage.sprite = constructItem.loadoutIcon;
                    _itemImage.color = Color.white;
                    break;
                case AbilityLoadoutItem abilityItem:
                    _selfImage.sprite = _hasFocus ? _abilityBackgroundOn : _abilityBackgroundOff;
                    _itemImage.sprite = abilityItem.loadoutIcon;
                    _itemImage.color = Color.white;
                    break;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int slotIndex => _slotIndex;

        /// <summary>
        /// 
        /// </summary>
        public LoadoutItemPresenterType acceptedTypes => _acceptedTypes;

        /// <summary>
        /// 
        /// </summary>
        public LoadoutItem item
        {
            get { return _item; }
            set
            {
                if(_item != value)
                {
                    _item = value;
                    OnItemChanged();
                }
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _slotIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private LoadoutItem _item = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _itemImage = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private LoadoutItemPresenterType _acceptedTypes = LoadoutItemPresenterType.Both;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _constructBackgroundOff = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _constructBackgroundOn = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _abilityBackgroundOff = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _abilityBackgroundOn = default;

        private bool _hasFocus;

        private Image _selfImage;

        #endregion
    }
}