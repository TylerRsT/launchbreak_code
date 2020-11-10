using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CameraShakeManager
    {
        #region Singleton

        /// <summary>
        /// 
        /// </summary>
        private CameraShakeManager()
        { }

        /// <summary>
        /// 
        /// </summary>
        public static CameraShakeManager instance
        {
            get => _instance ?? (_instance = new CameraShakeManager());
        }

        private static CameraShakeManager _instance;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            if(ProCamera2DShake.Exists)
            {
                _shakeExtInstance = ProCamera2DShake.Instance;
                _shakeExtInstance.OnShakeCompleted += OnShakeCompleted;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Deinit()
        {
            if (ProCamera2DShake.Exists)
            {
                _shakeExtInstance.OnShakeCompleted -= OnShakeCompleted;
                _shakeExtInstance = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cameraShake"></param>
        /// <param name="multiplier"></param>
        /// <param name="priority"></param>
        public void Shake(ShakePreset cameraShake, float multiplier = 1.0f, int priority = 1)
        {
            if (_shakeExtInstance == null)
            {
                return;
            }

            if (_isShaking)
            {
                if (priority >= _priority)
                {
                    _shakeExtInstance.StopShaking();
                }
                else
                {
                    return;
                }
            }

            _isShaking = true;

            _shakeExtInstance.Shake(
                cameraShake.Duration,
                cameraShake.Strength * multiplier,
                cameraShake.Vibrato,
                cameraShake.Randomness,
                cameraShake.UseRandomInitialAngle ? -1 : cameraShake.InitialAngle,
                cameraShake.Rotation,
                cameraShake.Smoothness,
                cameraShake.IgnoreTimeScale
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="presetName"></param>
        /// <param name="priority"></param>
        public void Shake(string presetName, int priority = 1)
        {
            if (_shakeExtInstance == null)
            {
                return;
            }

            if (_isShaking)
            {
                if (priority >= _priority)
                {
                    _shakeExtInstance.StopShaking();
                }
                else
                {
                    return;
                }
            }

            _isShaking = true;

            _shakeExtInstance.Shake(presetName);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnShakeCompleted()
        {
            _isShaking = false;
        }

        #endregion

        #region Fields

        private ProCamera2DShake _shakeExtInstance;

        private bool _isShaking = false;
        private int _priority = int.MinValue;

        #endregion
    }
}