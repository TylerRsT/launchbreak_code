using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SelectorNavigationHandler : MonoBehaviour
    {
        #region Messages

        /// <summary>
        ///
        /// </summary>
        protected virtual void Awake()
        {
            _text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            _leftArrow = transform.Find("LeftArrow");
            _rightArrow = transform.Find("RightArrow");
            _optionIndex = InitializeValue(_optionValues);
            DisplayUpdate();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        private void OnNavigate(UINavigationData data)
        {
            if(SelectorValueUpdate(data))
            {
                DisplayUpdate();
                OnSelectedValueChanged(_optionValues[_optionIndex]);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnSelectedValueChanged(string value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual int InitializeValue(string[] values)
        {
            return _optionIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private bool SelectorValueUpdate(UINavigationData data)
        {
            var nextOptionIndex = 0;
            switch (data.direction)
            {
                case UINavigationDirection.Right:
                    nextOptionIndex = 1;
                    break;

                case UINavigationDirection.Left:
                    nextOptionIndex = -1;
                    break;
                default:
                    return false;
            }

            var currentOptionIndex = _optionIndex;
            _optionIndex += nextOptionIndex;
            _optionIndex = Mathf.Clamp(_optionIndex, 0, _optionValues.Length - 1);

            return currentOptionIndex != _optionIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisplayUpdate()
        {
            if (_optionIndex == 0)
            {
                _leftArrow.gameObject.SetActive(false);
            }
            else
            {
                _leftArrow.gameObject.SetActive(true);
            }

            if (_optionIndex == _optionValues.Length - 1)
            {
                _rightArrow.gameObject.SetActive(false);
            }
            else
            {
                _rightArrow.gameObject.SetActive(true);
            }

            _text.text = _optionTexts[_optionIndex];
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int optionIndex
        {
            get { return _optionIndex; }
            set
            {
                if(value == _optionIndex)
                {
                    return;
                }
                _optionIndex = value;
                DisplayUpdate();
                OnSelectedValueChanged(_optionValues[value]);
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string[] _optionValues = new string[]
        {

        };

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string[] _optionTexts = new string[]
        {

        };

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _optionIndex = 0;

        private TextMeshProUGUI _text;
        private Transform _leftArrow;
        private Transform _rightArrow;

        #endregion
    }
}
