using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CharacterMarketingZoom : MonoBehaviour
    {
        #region Const

        private const float ZoomPower = 2.0f;

        private const float PositionMaxX = 158.0f;
        private const float PositionMaxYPositive = 85.0f;
        private const float PositionMaxYNegative = -89.0f;

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _camera = Camera.main;
            _proCamera = _camera.GetComponent<ProCamera2D>();
            _pixelPerfectProCamera = _camera.GetComponent<ProCamera2DPixelPerfect>();
            _pixelPerfectCamera = _camera.GetComponent<PixelPerfectCamera>();

            _cameraOriginalPosition = _camera.transform.position;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            foreach(var lootTextComponent in FindObjectsOfType<LootTextComponent>())
            {
                Destroy(lootTextComponent.gameObject);
            }

            LootTextComponent.canSpawn = false;

            _character = GetComponentInParent<Character>();

            _originalRenderScale = LightRenderer.RenderScale;
            LightRenderer.RenderScale = 1;
            _pixelPerfectCamera.enabled = false;
            _pixelPerfectProCamera.enabled = false;
            _proCamera.enabled = false;

            _camera.orthographicSize /= ZoomPower;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            var newPosition = _character.transform.position;
            newPosition.z = _cameraOriginalPosition.z;

            newPosition.x = Mathf.Clamp(newPosition.x, -PositionMaxX, PositionMaxX);
            newPosition.y = Mathf.Clamp(newPosition.y, PositionMaxYNegative, PositionMaxYPositive);

            _camera.transform.position = newPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            LightRenderer.RenderScale = _originalRenderScale;

            if (_pixelPerfectCamera != null)
            {
                _pixelPerfectCamera.enabled = true;
            }

            if (_pixelPerfectProCamera != null)
            {
                _pixelPerfectProCamera.enabled = true;
            }

            if (_proCamera != null)
            {
                _proCamera.enabled = true;
            }

            if (_camera != null)
            {
                _camera.orthographicSize *= ZoomPower;
                _camera.transform.position = _cameraOriginalPosition;
            }

            LootTextComponent.canSpawn = true;
        }

        #endregion

        #region Fields

        private Camera _camera;
        private ProCamera2D _proCamera;
        private ProCamera2DPixelPerfect _pixelPerfectProCamera;
        private PixelPerfectCamera _pixelPerfectCamera;

        private float _originalRenderScale = 1.0f;
        private Vector3 _cameraOriginalPosition;

        private Character _character;

        #endregion
    }
}