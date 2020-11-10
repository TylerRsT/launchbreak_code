using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class CameraScoreZoom : MonoBehaviour
    {
        #region Const

        private const float PositionMaxX = 158.0f;
        private const float PositionMaxYPositive = 85.0f;
        private const float PositionMaxYNegative = -89.0f;

        private const float TransitionDuration = 0.2f;
        private const int ZoomPower = 2;

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

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPosition"></param>
        public void ZoomIn(Vector2 targetPosition)
        {
            StartCoroutine(ZoomInInternal(targetPosition));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPosition"></param>
        public void ZoomOut(Vector2 targetPosition)
        {
            StartCoroutine(ZoomOutInternal(targetPosition));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        private IEnumerator ZoomInInternal(Vector2 targetPosition)
        {
            var targetPositionVec3 = new Vector3();
            targetPositionVec3.x = Mathf.Clamp(targetPosition.x, -PositionMaxX, PositionMaxX);
            targetPositionVec3.y = Mathf.Clamp(targetPosition.y, PositionMaxYNegative, PositionMaxYPositive);
            targetPositionVec3.z = _cameraOriginalPosition.z;

            _originalRenderScale = LightRenderer.RenderScale;
            LightRenderer.RenderScale = 1;
            _pixelPerfectCamera.enabled = false;
            _pixelPerfectProCamera.enabled = false;
            _proCamera.enabled = false;

            var tweens = new List<Tween>();
            tweens.Add(_camera.transform.DOMove(targetPositionVec3, TransitionDuration, true).SetAsCutscene());
            tweens.Add(_camera.DOOrthoSize(_camera.orthographicSize / ZoomPower, TransitionDuration).SetAsCutscene().SetOptions(true));

            foreach(var tween in tweens)
            {
                if (tween.IsActive())
                {
                    yield return tween.WaitForCompletion();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        private IEnumerator ZoomOutInternal(Vector2 targetPosition)
        {
            FindObjectOfType<RippleEffect>().Emit(_camera.WorldToViewportPoint(targetPosition));

            var tweens = new List<Tween>();
            tweens.Add(_camera.transform.DOMove(_cameraOriginalPosition, TransitionDuration * 2.0f, true).SetAsCutscene());
            tweens.Add(_camera.DOOrthoSize(_camera.orthographicSize * ZoomPower, TransitionDuration * 2.0f).SetAsCutscene().SetOptions(true));

            foreach (var tween in tweens)
            {
                if (tween.IsActive())
                {
                    yield return tween.WaitForCompletion();
                }
            }

            LightRenderer.RenderScale = _originalRenderScale;
            _pixelPerfectCamera.enabled = true;
            _pixelPerfectProCamera.enabled = true;
            _proCamera.enabled = true;
        }

        #endregion

        #region Fields

        private Camera _camera;
        private ProCamera2D _proCamera;
        private ProCamera2DPixelPerfect _pixelPerfectProCamera;
        private PixelPerfectCamera _pixelPerfectCamera;

        private float _originalRenderScale = 1.0f;
        private Vector3 _cameraOriginalPosition;

        #endregion
    }
}