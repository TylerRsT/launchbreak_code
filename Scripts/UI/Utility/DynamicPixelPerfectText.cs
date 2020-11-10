using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DynamicPixelPerfectText : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            _textMesh = GetComponent<TextMeshProUGUI>();
            _transform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (_textPreviousLength != _textMesh.text.Length)
            {
                if (_textMesh.text.Length % 2 == 0)
                {
                    _transform.anchoredPosition = new Vector2(0.0f, _transform.anchoredPosition.y);
                    _textPreviousLength = _textMesh.text.Length;
                }
                else
                {
                    _transform.anchoredPosition = new Vector2(0.5f, _transform.anchoredPosition.y);
                    _textPreviousLength = _textMesh.text.Length;
                }
            }
        }

        #endregion

        #region Fields

        private TextMeshProUGUI _textMesh;
        private RectTransform _transform;
        private int _textPreviousLength;

        #endregion
    }
}
