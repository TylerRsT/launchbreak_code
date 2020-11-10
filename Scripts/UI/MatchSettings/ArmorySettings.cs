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
    public class ArmorySettings : MonoBehaviour
    {
        #region Const

        private const int MaxItemsToDisplay = 8;
        private const int ColumnCount = 2;

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _arrowUpSequence = DOTween.Sequence();
            _arrowUpSequence.Append(_arrowUp.DOLocalMoveY(_arrowUp.localPosition.y + 2.0f, 0.2f));
            _arrowUpSequence.Append(_arrowUp.DOLocalMoveY(_arrowUp.localPosition.y, 0.2f));
            _arrowUpSequence.SetLoops(-1);

            _arrowDownSequence = DOTween.Sequence();
            _arrowDownSequence.Append(_arrowDown.DOLocalMoveY(_arrowDown.localPosition.y - 2.0f, 0.2f));
            _arrowDownSequence.Append(_arrowDown.DOLocalMoveY(_arrowDown.localPosition.y, 0.2f));
            _arrowDownSequence.SetLoops(-1);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            _arrowUpSequence?.Kill();
            _arrowUpSequence = null;

            _arrowDownSequence?.Kill();
            _arrowDownSequence = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnNavigate(UINavigationData data)
        {
            switch(data.direction)
            {
                case UINavigationDirection.Right:
                case UINavigationDirection.Left:
                    if(_currentItemPresenterIndex % ColumnCount == 0)
                    {
                        if(_currentItemPresenterIndex == _loadoutItemPresenters.Count - 1)
                        {
                            break;
                        }
                        _currentItemPresenterIndex += 1;
                    }
                    else
                    {
                        _currentItemPresenterIndex -= 1;
                    }
                    break;
                case UINavigationDirection.Down:
                    _currentItemPresenterIndex += 2;
                    if(_currentItemPresenterIndex % ColumnCount == ColumnCount - 1)
                    {
                        if(_currentItemPresenterIndex == _loadoutItemPresenters.Count)
                        {
                            _currentItemPresenterIndex = _loadoutItemPresenters.Count - 1;
                        }
                        else
                        {
                            _currentItemPresenterIndex %= _loadoutItemPresenters.Count;
                        }
                    }
                    else
                    {
                        if (_currentItemPresenterIndex == _loadoutItemPresenters.Count + 1)
                        {
                            _currentItemPresenterIndex = 0;
                        }
                        _currentItemPresenterIndex %= _loadoutItemPresenters.Count;
                    }
                    break;
                case UINavigationDirection.Up:
                    _currentItemPresenterIndex -= 2;
                    if(_currentItemPresenterIndex < 0)
                    {
                        if(_loadoutItemPresenters.Count % ColumnCount == 0)
                        {
                            _currentItemPresenterIndex += _loadoutItemPresenters.Count;
                        }
                        else
                        {
                            _currentItemPresenterIndex = _loadoutItemPresenters.Count - 1;
                        }
                    }
                    break;
            }

            FocusItemPresenter(_currentItemPresenterIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnSubmit(UINavigationData data)
        {
            var selectedItemPresenter = data.navigable.GetComponent<LoadoutItemPresenter>();
            targetPresenter.item = selectedItemPresenter?.item;
            playerLoadoutSettings.ValidateItem(targetPresenter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /*private void OnCancel(UINavigationData data)
        {
            playerLoadoutSettings.ValidateItem(targetPresenter);
        }*/

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        public void SetFocus(bool active)
        {
            if(active)
            {
                _currentItemPresenterIndex = 0;
                GetComponent<UINavigation>().Focus();
                Show();
            }
            else
            {
                GetComponent<UINavigation>().Unfocus(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void BuildItems()
        {
            for(var i = _itemsGrid.transform.childCount - 1; i >= 0; --i)
            {
                Destroy(_itemsGrid.transform.GetChild(i).gameObject);
            }

            _loadoutItemPresenters.Clear();

            if(presenterType.HasFlag(LoadoutItemPresenterType.Construct))
            {
                foreach(var constructItem in Bootstrap.instance.data.constructs)
                {
                    var presenter = Instantiate(_itemPresenterPrefab, _itemsGrid.transform).GetComponent<LoadoutItemPresenter>();
                    presenter.item = constructItem;
                    presenter.gameObject.SetActive(false);
                    _loadoutItemPresenters.Add(presenter);
                }
            }

            if (presenterType.HasFlag(LoadoutItemPresenterType.Ability))
            {
                foreach (var abilityItem in Bootstrap.instance.data.abilities)
                {
                    var presenter = Instantiate(_itemPresenterPrefab, _itemsGrid.transform).GetComponent<LoadoutItemPresenter>();
                    presenter.item = abilityItem;
                    presenter.gameObject.SetActive(false);
                    _loadoutItemPresenters.Add(presenter);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Show()
        {
            var count = Mathf.Min(MaxItemsToDisplay, _loadoutItemPresenters.Count);
            for(var i = 0; i < count; ++i)
            {                
                _loadoutItemPresenters[i].gameObject.SetActive(true);

                if (i == 0)
                {
                    _arrowUp.gameObject.SetActive(false);
                }
                if (i == _loadoutItemPresenters.Count - 1)
                {
                    _arrowDown.gameObject.SetActive(false);
                }
            }

            _firstEnabledIndex = 0;
            _lastEnabledIndex = count - 1;

            if(targetPresenter.item != null)
            {
                var index = 0;
                for (var i = 0; i < _loadoutItemPresenters.Count; ++i)
                {
                    if (_loadoutItemPresenters[i].item == targetPresenter.item)
                    {
                        index = i;
                        break;
                    }
                }
                _currentItemPresenterIndex = index;
            }
            FocusItemPresenter(_currentItemPresenterIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void FocusItemPresenter(int index)
        {
            var navigation = GetComponent<UINavigation>();
            navigation.Focus(null);
            CheckEnabledItemsFor(index);

            var itemPresenter = _loadoutItemPresenters[index];
            navigation.Focus(itemPresenter.GetComponent<UINavigable>());
            _itemNameText.text = itemPresenter.item.name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void CheckEnabledItemsFor(int index)
        {
            if (index < _firstEnabledIndex)
            {
                while (index < _firstEnabledIndex)
                {
                    _firstEnabledIndex -= ColumnCount;
                }
                for (var i = _firstEnabledIndex; i <= _lastEnabledIndex; ++i)
                {
                    if (i < _loadoutItemPresenters.Count)
                    {
                        _loadoutItemPresenters[i].gameObject.SetActive(i - _firstEnabledIndex < MaxItemsToDisplay);
                    }
                }
                _lastEnabledIndex = _firstEnabledIndex + MaxItemsToDisplay - 1;
            }
            else if(index > _lastEnabledIndex)
            {
                while(index > _lastEnabledIndex)
                {
                    _lastEnabledIndex += ColumnCount;
                }
                for(var i = _firstEnabledIndex; i <= _lastEnabledIndex; ++i)
                {
                    if(i < _loadoutItemPresenters.Count)
                    {
                        _loadoutItemPresenters[i].gameObject.SetActive(_lastEnabledIndex - i < MaxItemsToDisplay);
                    }
                }
                _firstEnabledIndex = _lastEnabledIndex - MaxItemsToDisplay + 1;
            }

            _arrowUp.gameObject.SetActive(_firstEnabledIndex > 0);
            _arrowDown.gameObject.SetActive(_lastEnabledIndex <= _loadoutItemPresenters.Count - ColumnCount);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public PlayerLoadoutSettings playerLoadoutSettings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int playerIndex
        {
            set => GetComponent<UINavigation>().userIndex = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public LoadoutItemPresenter targetPresenter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LoadoutItemPresenterType presenterType { get; set; } = LoadoutItemPresenterType.Both;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GridLayoutGroup _itemsGrid = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform _arrowUp = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform _arrowDown = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _itemNameText = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _itemPresenterPrefab = default;

        private List<LoadoutItemPresenter> _loadoutItemPresenters = new List<LoadoutItemPresenter>();
        private int _currentItemPresenterIndex = 0;

        private Sequence _arrowUpSequence;
        private Sequence _arrowDownSequence;

        private int _firstEnabledIndex;
        private int _lastEnabledIndex;

        #endregion
    }
}
 