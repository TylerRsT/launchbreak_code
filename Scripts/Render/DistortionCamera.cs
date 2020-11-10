using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ProjectShovel
{
    /// <summary>
    /// TO BE REMOVED
    /// </summary>
    public class DistortionCamera : MonoBehaviour
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            _buffer = new RenderTexture(Screen.width, Screen.height, 0);
            _buffer.name = "Displacement Texture";
            _buffer.enableRandomWrite = true;
            _buffer.Create();

            var mainCamera = Camera.main;
            var gameObject = new GameObject();
            gameObject.name = "Displacement Camera";
            _camera = gameObject.AddComponent<Camera>();
            _camera.CopyFrom(mainCamera);
            _camera.transform.parent = mainCamera.transform;
            _camera.cullingMask = _mask;
            _camera.backgroundColor = new Color(0.5f, 0.5f, 1.0f);
            _camera.targetTexture = _buffer;

            var settings = _volume.profile.GetSetting<Distortion>();
            settings.distortion.value = _buffer;
        }

        #endregion

        #region Fields

        [SerializeField]
        LayerMask _mask = default;
        [SerializeField]
        PostProcessVolume _volume = default;

        /// <summary>
        /// 
        /// </summary>
        Camera _camera;

        /// <summary>
        /// 
        /// </summary>
        RenderTexture _buffer;

        #endregion
    }
}
