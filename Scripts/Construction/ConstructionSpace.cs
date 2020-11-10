using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class ConstructionSpaceRequest
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constructionSpace"></param>
        /// <param name="contactingColliders"></param>
        public ConstructionSpaceRequest(ConstructionSpace constructionSpace, IReadOnlyList<Collider2D> contactingColliders)
        {
            this.constructionSpace = constructionSpace;
            this.contactingColliders = contactingColliders;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Decline()
        {
            accepted = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ConstructionSpace constructionSpace { get; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<Collider2D> contactingColliders { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool accepted { get; private set; } = true;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(SpriteAnimator))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class ConstructionSpace : MonoBehaviour
    {
        #region Const

        public const string OnConstructionSpaceRequestedMethodName = "OnConstructionSpaceRequested";
        public const string OnConstructionValidationMethodName = "OnConstructionValidation";
        private const int ConstructionSpaceDefaultSortingOrder = -3;
        private const int ConstructionBlueprintAdditiveSortingOrder = 30;

        private static Color _blueprintColor = new Color(0.1765f, 0.4588f, 0.702f, 0.0f);

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Reset()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = ConstructionSpaceDefaultSortingOrder;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _boxCollider.isTrigger = true;

            _spriteAnimator = GetComponent<SpriteAnimator>();

            _spriteAnimator.Play(_validAnimation);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            //InstantiateBlueprint(_breakableWallPrefab);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        private void OnConstructionSpaceRequested(ConstructionSpaceRequest request)
        {
            if (request.constructionSpace == null || request.constructionSpace.transform.position == transform.position)
            {
                request.Decline();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="construct"></param>
        /// <param name="instigator"></param>
        public void InstantiateBlueprint(ConstructLoadoutItem construct, Character instigator)
        {
            Debug.Assert(construct != null);
            Debug.Assert(construct.gameObjectPrefab != null);

            if(_blueprint != null)
            {
                Destroy(_blueprint);
            }

            _blueprint = Instantiate(construct.gameObjectPrefab, transform.position, transform.rotation, transform);
            var colliders = _blueprint.GetComponentsInChildren<Collider2D>();
            foreach(var collider in colliders)
            {
                collider.isTrigger = true;
            }
            _blueprint.GetComponent<Construct>().instigator = instigator;

            var blueprintObjectRenderer = _blueprint.GetComponent<TileObjectRenderer>();
            if (blueprintObjectRenderer != null)
            {
                blueprintObjectRenderer.isChild = true;
                blueprintObjectRenderer.additive = ConstructionBlueprintAdditiveSortingOrder;
            }

            var spriteRenderers = new List<SpriteRenderer>();
            spriteRenderers.Add(_blueprint.GetComponent<SpriteRenderer>());
            spriteRenderers.AddRange(_blueprint.GetComponentsInChildren<SpriteRenderer>());

            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = _blueprintColor;
            }

            _blueprint.AddComponent<ConstructionValidator>().construct = construct;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool canBePlaced
        {
            get { return _canBePlaced; }
            set
            {
                _canBePlaced = value;
                _spriteAnimator.Play(value ? _validAnimation : _invalidAnimation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isPlaced
        {
            get { return _isPlaced; }
            set
            {
                _isPlaced = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BoxCollider2D boxCollider => _boxCollider;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _validAnimation = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _invalidAnimation = default;

        private bool _isPlaced = false;
        private bool _canBePlaced = false;

        private GameObject _blueprint;

        private BoxCollider2D _boxCollider;
        private SpriteAnimator _spriteAnimator;

        #endregion
    }
}