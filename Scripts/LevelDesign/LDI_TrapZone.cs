using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class LDI_TrapZone : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();

            _topCollider.enabled = false;
            _bottomCollider.enabled = false;
            _rightCollider.enabled = false;
            _leftCollider.enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
#if UNITY_EDITOR
        private void OnValidate()
        {
            _topCollider.size = new Vector2(_width * _tileSize, 1);
            _topCollider.offset = new Vector2(0, _height * _tileSize / 2);
            _bottomCollider.size = new Vector2(_width * _tileSize, 1);
            _bottomCollider.offset = new Vector2(0, -_height * _tileSize / 2);
            _rightCollider.size = new Vector2(1, _height * _tileSize);
            _rightCollider.offset = new Vector2(_width * _tileSize / 2, 0);
            _leftCollider.size = new Vector2(1, _height * _tileSize);
            _leftCollider.offset = new Vector2(-_width * _tileSize / 2, 0);

            var particleSystem = GetComponent<ParticleSystem>();

            var scale = new Vector3(_width * _tileSize, _height * _tileSize, 1);
            var emission = particleSystem.emission;

            emission.rateOverTime = new ParticleSystem.MinMaxCurve(100 * (_width + _height));

            SerializedObject serializedObject = new SerializedObject(particleSystem);
            serializedObject.FindProperty("ShapeModule.m_Scale").vector3Value = scale;
            serializedObject.ApplyModifiedProperties();
        }
#endif // UNITY_EDITOR

        /// <summary>
        /// 
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Assets/Art/Editor/Gizmos/SPR_LDI_Gizmo.png");
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public IEnumerator BuildUp()
        {
            var tileAnimators = GetComponentsInChildren<SpriteAnimator>();
            foreach (var item in tileAnimators)
            {
                item.Play(item.animations[0], loopType: LoopType.repeat);
            }

            var manager = FindObjectOfType<LDI_TrapZoneManager>();

            yield return new WaitForSeconds(manager.switchDelay - 2);

            foreach (var item in tileAnimators)
            {
                item.animations[0].FPS = 10;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Activate()
        {
            _topCollider.enabled = true;
            _bottomCollider.enabled = true;
            _rightCollider.enabled = true;
            _leftCollider.enabled = true;

            var tileAnimators = GetComponentsInChildren<SpriteAnimator>();
            foreach (var item in tileAnimators)
            {
                item.Stop();
                item.SpriteRenderer.sprite = _buildUpAnimation.Frames[1];
            }

            _particleSystem.Play();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Deactivate()
        {
            _topCollider.enabled = false;
            _bottomCollider.enabled = false;
            _rightCollider.enabled = false;
            _leftCollider.enabled = false;

            var tileAnimators = GetComponentsInChildren<SpriteAnimator>();
            foreach (var item in tileAnimators)
            {
                item.Stop();
                item.SpriteRenderer.sprite = _buildUpAnimation.Frames[0];

                item.animations[0].FPS = 3;
            }

            _particleSystem.Stop();
            _particleSystem.Clear();
        }

#if UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        public void GenerateTiles()
        {
            var tileAnimators = GetComponentsInChildren<SpriteAnimator>();
            foreach(var item in tileAnimators)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(item.gameObject);
                };
            }

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    var obj = new GameObject("TrapZoneTile");
                    var animator = obj.AddComponent<SpriteAnimator>();
                    animator.animations = new List<SpriteAnimation>();
                    animator.animations.Add(_buildUpAnimation);
                    var renderer = obj.GetComponent<SpriteRenderer>();
                    renderer.sortingLayerID = SortingLayerHelper.GetSortingLayerValue(StaticSortingLayer.AboveFloor);
                    obj.GetComponent<SpriteRenderer>().sprite = _tileSprite;
                    obj.transform.localPosition = new Vector2(transform.localPosition.x + (_width > 1 ? ((_tileSize * i) - (_tileSize / 2 * _width) + _tileSize / 2) : 0), 
                        transform.localPosition.y + (_height > 1 ? ((_tileSize * j) - (_tileSize / 2 * _height) + _tileSize / 2) : 0));
                    obj.transform.SetParent(this.gameObject.transform);
                    obj.layer = LayerMask.NameToLayer("Floor");
                }
            }
        }
#endif // UNITY_EDITOR

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _buildUpAnimation = default;

#if UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _tileSprite = default;
#endif // UNITY_EDITOR

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private BoxCollider2D _topCollider = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private BoxCollider2D _bottomCollider = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private BoxCollider2D _rightCollider = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private BoxCollider2D _leftCollider = default;

#if UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [Range(1, 100)]
        private int _width = 1;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [Range(1,100)]
        private int _height = 1;

        private float _tileSize = GameConstants.TileSize;
#endif // UNITY_EDITOR

        private ParticleSystem _particleSystem;

        #endregion
    }
}
