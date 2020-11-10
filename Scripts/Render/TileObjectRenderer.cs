using CreativeSpore.SuperTilemapEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public enum TileObjectType
    {
        Static,
        Dynamic,
    }

    /// <summary>
    /// 
    /// </summary>
    public class TileObjectRenderer : TileObjectBehaviour
    {
        #region Const

        private const string LightLayerFormat = "LightLayer{0}";
        private const string SortingLayersFieldName = "m_ApplyToSortingLayers";

        private static readonly StaticSortingLayer[] _floorSortingLayerValues = new StaticSortingLayer[]
        {
            StaticSortingLayer.Floor,
            StaticSortingLayer.AboveFloor,
            StaticSortingLayer.Spawner,
            StaticSortingLayer.KeySlotTop,
            StaticSortingLayer.KeyAtSpawner,
            StaticSortingLayer.KeySlotBottom,
            StaticSortingLayer.AboveSpawner,
        };

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        static TileObjectRenderer()
        {
            //_shadowCastersSortingLayersField = typeof(ShadowCaster2D).GetField(SortingLayersFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            //_lightSortingLayersField = typeof(Light2D).GetField(SortingLayersFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void OnTilePrefabCreation(TilemapChunk.OnTilePrefabCreationData data)
        {
            base.OnTilePrefabCreation(data);

            OnValidate();
        }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Awake()
        {
            _renderer = GetComponent<Renderer>();
            //_shadowCaster = GetComponent<ShadowCaster2D>();
            //_light = GetComponent<Light2D>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Start()
        {
            if(_type == TileObjectType.Static)
            {
                enabled = false;
            }

            ApplyOrder();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Update()
        {
            ApplyOrder();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnValidate()
        {
            _renderer = GetComponent<Renderer>();
            //_shadowCaster = GetComponent<ShadowCaster2D>();
            //_light = GetComponent<Light2D>();

            ApplyOrder();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        protected virtual void ApplyOrder()
        {
            var lightLayerNumber = ComputeLightLayerNumber();
            if (_renderer != null)
            {
                _renderer.sortingLayerID = SortingLayerHelper.GetLightSortingLayerValue(lightLayerNumber);
                _renderer.sortingOrder = _additive;
            }

            /*if (_shadowCaster != null || _light != null)
            {
                var layers = new int[lightLayerNumber - GameConstants.MinLightLayer + _floorSortingLayerValues.Length];

                for(var i = 0; i < _floorSortingLayerValues.Length; ++i)
                {
                    layers[i] = SortingLayerHelper.GetSortingLayerValue(_floorSortingLayerValues[i]);
                }

                var x = _floorSortingLayerValues.Length;
                for (var i = lightLayerNumber - 1; i >= GameConstants.MinLightLayer; --i)
                {
                    layers[x++] = SortingLayerHelper.GetLightSortingLayerValue(i);
                }

                if (_shadowCaster != null)
                {
                    _shadowCastersSortingLayersField.SetValue(_shadowCaster, layers);
                }
                if(_light != null)
                {
                    _lightSortingLayersField.SetValue(_light, layers);
                }
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual int ComputeLightLayerNumber()
        {
            // TODO check why transform.parent can not be null here for weapons.
            var parent = transform.parent;
            if (_isChild && parent != null)
            {
                for(var i = 1; i < _childLevel; ++i)
                {
                    parent = parent.parent;
                }

                if (parent == null)
                {
                    return 0;
                }

                var parentTileObjectRenderer = parent.GetComponent<TileObjectRenderer>();
                if(parentTileObjectRenderer == null)
                {
                    return 0;
                }
                return parentTileObjectRenderer.ComputeLightLayerNumber();
            }

            var reference = GetReferenceHeight();
            float y = (GameConstants.TileSize * (Mathf.Floor(reference / GameConstants.TileSize))) + GameConstants.TileSize;
            y *= -1.0f;
            y /= GameConstants.TileSize;

            return Mathf.FloorToInt(y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual float GetReferenceHeight()
        {
            if(!_useCollidersBounds)
            {
                return transform.position.y;
            }

            return GetComponents<Collider2D>().Min(x => x.bounds.min.y);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public TileObjectType type
        {
            get { return _type; }
            set
            {
                _type = value;
                enabled = value == TileObjectType.Dynamic;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isChild
        {
            get { return _isChild; }
            set { _isChild = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int childLevel
        {
            get { return _childLevel; }
            set { _childLevel = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool useCollidersBounds
        {
            get { return _useCollidersBounds; }
            set { _useCollidersBounds = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int additive
        {
            get { return _additive; }
            set { _additive = value; }
        }

        #endregion

        #region Fields

        //private static FieldInfo _shadowCastersSortingLayersField;
        //private static FieldInfo _lightSortingLayersField;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TileObjectType _type = TileObjectType.Dynamic;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _isChild = false;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _childLevel = 1;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _useCollidersBounds = false;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _additive = 0;

        private Renderer _renderer;
        //private ShadowCaster2D _shadowCaster;
        //private Light2D _light;

        #endregion
    }
}