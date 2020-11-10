using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class SpeedBuff : CharacterBuff
    {
        #region Override

        protected override void Awake()
        {
            base.Awake();

            _characterSpriteRenderer = character.GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            character.speedMultipliers.Add(_speedMultiplier);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if(character.isMoving && Time.frameCount % _hollowFrequency == 0)
            {
                CreateHollow();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            character.speedMultipliers.Remove(_speedMultiplier);

            foreach(var tween in _tweens.ToArray())
            {
                if(tween != null && tween.IsActive())
                {
                    tween.Complete();
                }
            }

            _tweens.Clear();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void CreateHollow()
        {
            var hollow = new GameObject().AddComponent<SpriteRenderer>();
            hollow.transform.position = character.transform.position;
            hollow.transform.rotation = character.transform.rotation;

            hollow.sortingLayerID = SortingLayerHelper.GetSortingLayerValue(StaticSortingLayer.AboveSpawner); //_characterSpriteRenderer.sortingLayerID;
            hollow.sortingOrder = 20;
            hollow.flipX = _characterSpriteRenderer.flipX;
            hollow.color = _characterSpriteRenderer.color.ChangeOpacity(0.5f);
            hollow.sprite = _characterSpriteRenderer.sprite;
            var tween = hollow.DOFade(0.0f, 0.3f);
            tween.onComplete += () =>
            {
                Destroy(hollow.gameObject);
                _tweens.Remove(tween);
            };
            _tweens.Add(tween);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float speedMultiplier
        {
            get => _speedMultiplier;
            set => _speedMultiplier = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int hollowFrequency
        {
            get => _hollowFrequency;
            set => _hollowFrequency = value;
        }

        #endregion

        #region Fields
        
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _speedMultiplier = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _hollowFrequency = 3;

        private SpriteRenderer _characterSpriteRenderer;

        private List<Tween> _tweens = new List<Tween>();

        #endregion
    }
}