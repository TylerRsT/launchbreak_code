using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Elendow.SpritedowAnimator;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Debugger : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _text.fontSize = 32.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LateUpdate()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            float fps = 1.0f / _deltaTime;
            _text.text = "FPS: " + fps.ToString("#.0");
        }

        #endregion

        #region Fields

        private TextMeshProUGUI _text;

        private float _deltaTime = 0.0f;

        #endregion
    }
}
