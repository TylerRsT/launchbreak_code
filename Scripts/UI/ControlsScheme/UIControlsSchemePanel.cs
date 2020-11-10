using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectShovel
{
    /// <summary>
    ///
    /// </summary>
    public class UIControlsSchemePanel : MonoBehaviour
    {
        #region Messages

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
        private void Update()
        {
            var horizontalAxis = InputHelper.GetAnyUIHorizontalAxis();
            if(horizontalAxis == 0.0f || (horizontalAxis != 0.0f && _lastHorizontalAxis != 0.0f))
            {
                _lastHorizontalAxis = horizontalAxis;
                return;
            }

            _lastHorizontalAxis = horizontalAxis;

            if (canFlip && horizontalAxis != 0.0f)
            {
                SwitchScheme();
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
        public static UIControlsSchemePanel OpenControlsSchemePanel()
        {
            return _controlsSchemePanelInstance = Instantiate(Bootstrap.instance.data.controlsSchemePanelPrefab, GameObject.FindObjectOfType<Canvas>().transform).GetComponent<UIControlsSchemePanel>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SwitchScheme()
        {
            if(_gamepadGroup.activeInHierarchy)
            {
                _gamepadGroup.SetActive(false);
                _keyboardGroup.SetActive(true);
                _paginationImage.sprite = _keyboardPage;
            }
            else
            {
                _gamepadGroup.SetActive(true);
                _keyboardGroup.SetActive(false);
                _paginationImage.sprite = _gamepadPage;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static void CloseControlsSchemePanel()
        {
            Destroy(_controlsSchemePanelInstance.gameObject);
            _controlsSchemePanelInstance = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static bool exists => _controlsSchemePanelInstance != null;

        /// <summary>
        /// 
        /// </summary>
        public bool canFlip { get; set; }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private static UIControlsSchemePanel _controlsSchemePanelInstance;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _paginationImage = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _gamepadPage = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _keyboardPage = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _gamepadGroup = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _keyboardGroup = default;

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

        private Sequence _leftArrowSequence;
        private Sequence _rightArrowSequence;

        private float _lastHorizontalAxis = 0.0f;

        #endregion

    }
}
