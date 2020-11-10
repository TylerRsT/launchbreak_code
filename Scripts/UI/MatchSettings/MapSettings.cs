using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class MapSettings : MonoBehaviour
    {
        #region Const

        private const string MapInfoFolder = "Content/Maps";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            CreateMapIcons();
            SelectMap(_selectedMapThumbIndex);
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
                    IncrMap(+1);
                    break;
                case UINavigationDirection.Left:
                    IncrMap(-1);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnSubmit(UINavigationData data)
        {
            GameModeParams.instance.useRandomMap = _currentMapThumb.isRandom;
            if (_currentMapThumb.isRandom)
            {
                var random = Random.Range(1, _mapThumbsCount);
                SelectMap(random);
            }

            selectedMap = _currentMapThumb.mapInfo;
            SetFocus(false);
            transform.DOShakePosition(0.3f, 10.0f, 50);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="focus"></param>
        public void SetFocus(bool focus)
        {
            var navigation = GetComponent<UINavigation>();
            if (focus)
            {
                selectedMap = null;
                navigation.Focus();
            }
            else
            {
                navigation.Unfocus();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void CreateMapIcons()
        {
            var sceneCount = SceneManager.sceneCountInBuildSettings;
            var allMapInfo = Resources.LoadAll<MapInfo>(MapInfoFolder);

            for(var i = 0; i < sceneCount; ++i)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if(!sceneName.StartsWith(GameConstants.MapPrefixe))
                {
                    continue;
                }

                var mapInfo = allMapInfo.FirstOrDefault(x => x.sceneName == sceneName);

                var mapIcon = Instantiate(_mapThumbPrefab, _mapThumbsGrid).GetComponent<MapThumb>();
                mapIcon.mapInfo = mapInfo ?? MapInfo.New(sceneName.Substring(GameConstants.MapPrefixe.Length), sceneName);
                ++_mapThumbsCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void IncrMap(int value)
        {
            SelectMap(_selectedMapThumbIndex + value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void SelectMap(int index)
        {
            while(index < 0)
            {
                index += _mapThumbsCount;
            }
            index %= _mapThumbsCount;

            _selectedMapThumbIndex = index;
            _currentMapThumb = _mapThumbsGrid.GetChild(index).GetComponent<MapThumb>();
            var localPosition = _cursorArrow.transform.localPosition;
            _cursorArrow.SetParent(_currentMapThumb.transform);
            _cursorArrow.transform.localPosition = localPosition;

            UpdateMapInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateMapInfo()
        {
            var mapInfo = _currentMapThumb.mapInfo;
            if (mapInfo.mapBackground != null)
            {
                _mapBackground.sprite = mapInfo.mapBackground;
            }
            _mapNameText.text = mapInfo.name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public MapInfo selectedMap { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool isReady => selectedMap != null;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform _mapThumbsGrid = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _mapBackground = default;

        /// <summary>
        /// 
        /// </summary>
        /*[SerializeField]
        private Image _mapThumbnail = default;*/

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _mapNameText = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Transform _cursorArrow = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _mapThumbPrefab = default;

        private int _selectedMapThumbIndex = 0;

        private int _mapThumbsCount = 1;

        private MapThumb _currentMapThumb;

        #endregion
    }
}