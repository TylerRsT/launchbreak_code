using CreativeSpore.SuperTilemapEditor;
using DG.Tweening;
using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PickupResponse
    {
        #region Constructors

        public PickupResponse(Pickable pickable, Character character, bool autoPickup)
        {
            this.pickable = pickable;
            this.character = character;
            this.autoPickup = autoPickup;
        }

        #endregion

        #region Methods

        public void Accept()
        {
            accepted = true;
        }

        #endregion

        #region Properties

        public Pickable pickable { get; }
        public Character character { get; }
        public bool autoPickup { get; }
        public bool accepted { get; private set; } = false;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class Pickable : MonoBehaviour
    {
        #region Const

        public static readonly Color whiteOutlineColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public static readonly Color blackOutlineColor = new Color(0.125f, 0.122f, 0.149f, 1.0f);

        private const string OnPickupMethodName = "OnPickup";

        private const string DefaultLayerName = "Default";
        private const string PickBlockerLayerName = "PickBlocker";
        private const string PickableJumpLayerName = "PickableJump";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteAnimator = GetComponent<SpriteAnimator>();
            _circleCollider = GetComponent<CircleCollider2D>();
            _defaultColliderRadius = _circleCollider.radius;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Start()
        {
            _availablePickables.Add(this);

            if (doFloatingAnimAtStart)
            {
                DoFloatingAnim();
            }

            UpdateMaterial();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Update()
        {
            if(_charactersStepping.RemoveWhere(x => !(x.gameplayStateHandler is CharacterGameplayState_Default)) > 0)
            {
                UpdateMaterial();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDestroy()
        {
            _isDestroyed = true;
            KillFloatingAnim();

            _availablePickables.Remove(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            OnCollision(collision);

            var character = collision.GetComponent<Character>();
            if (character != null && character.gameplayStateHandler is CharacterGameplayState_Default)
            {
                _charactersStepping.Add(character);
            }

            UpdateMaterial();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            var character = collision.GetComponent<Character>();
            if (character != null)
            {
                _charactersStepping.Remove(character);
            }
            UpdateMaterial();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        protected virtual void OnPickup(PickupResponse response)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="autoPickup"></param>
        public void SendPickup(Character character, bool autoPickup)
        {
            if(_isPickedUp || !_isPickable)
            {
                return;
            }

            var pickupResponse = new PickupResponse(this, character, autoPickup);
            SendMessage(OnPickupMethodName, pickupResponse, SendMessageOptions.DontRequireReceiver);
            _isPickedUp = pickupResponse.accepted;

            if(_isPickedUp)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool CanBePickedUpBy(Character character)
        {
            var raycastHit = Physics2D.Raycast(transform.position, (character.transform.position - transform.position).normalized,
                Vector2.Distance(transform.position, character.transform.position), LayerMask.GetMask(PickBlockerLayerName));

            return raycastHit.collider == null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DoFloatingAnim()
        {
            KillFloatingAnim();
            if (!_isDestroyed)
            {
                _floatingSequence = DOTween.Sequence();
                _floatingSequence.Append(transform.DOLocalMoveY(transform.localPosition.y + 5.0f, 0.5f));
                _floatingSequence.Append(transform.DOLocalMoveY(transform.localPosition.y, 0.5f));
                _floatingSequence.SetLoops(-1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void KillFloatingAnim()
        {
            if (_floatingSequence != null && _floatingSequence.IsActive())
            {
                _floatingSequence.Kill();
            }
            _floatingSequence = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minDistance"></param>
        /// <param name="maxDistance"></param>
        /// <param name="callback"></param>
        public void DoRandomJumpAnim(float minDistance, float maxDistance, System.Action callback = null)
        {
            doFloatingAnimAtStart = false;
            System.Func<float> nextRandom = () =>
            {
                var range = Random.Range(minDistance, maxDistance);
                return Random.Range(0, 2) == 0 ? range : range * -1.0f;
            };
            DoJumpAnim(transform.position + new Vector3(nextRandom() * MathExtensions.RandOne(), nextRandom() * MathExtensions.RandOne()), () =>
            {
                DoFloatingAnim();
                callback?.Invoke();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endValue"></param>
        public void DoJumpAnim(Vector2 endValue, System.Action additionalCallback = null)
        {
            StartCoroutine(JumpCoroutine(endValue, additionalCallback));
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void UpdateMaterial()
        {
            var outlineColor = _charactersStepping.Count > 0 ? whiteOutlineColor : blackOutlineColor;
            _spriteRenderer.material.SetColor("_DesiredColor", outlineColor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endValue"></param>
        /// <param name="additionalCallback"></param>
        /// <returns></returns>
        private IEnumerator JumpCoroutine(Vector2 endValue, System.Action additionalCallback = null)
        {
            var body = GetComponent<Rigidbody2D>();
            if (body != null)
            {
                yield break;
            }

            body = gameObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;

            _circleCollider.isTrigger = false;
            _circleCollider.radius = _jumpingColliderRadius;

            body.velocity = (endValue - (Vector2)transform.position) / 0.2f * 2.0f;
            yield return new WaitForSeconds(0.1f);

            body.velocity = (endValue - (Vector2)transform.position) / 0.2f * 0.5f;
            yield return new WaitForSeconds(0.1f);

            _circleCollider.radius = _defaultColliderRadius;
            Destroy(body);
            gameObject.layer = LayerMask.NameToLayer(DefaultLayerName);
            _circleCollider.isTrigger = true;
            additionalCallback?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollision(Collider2D collision)
        {
            var character = collision.GetComponent<Character>();
            if (character != null && _autoPickup && !_isPickedUp)
            {
                SendPickup(character, true);
            }

            /*if (_jumpTween != null && collision.GetComponent<Character>() == null && collision.GetComponent<Pickable>() == null)
            {
                var tween = _jumpTween;
                var onComplete = _jumpTween.onComplete;
                if (_jumpTween != null && _jumpTween.IsActive())
                {
                    _jumpTween.Kill();
                }
                onComplete();
            }*/
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static IReadOnlyCollection<Pickable> availablePickables => _availablePickables;

        /// <summary>
        /// 
        /// </summary>
        public bool isPickable
        {
            get => _isPickable;
            set => _isPickable = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool autoPickup => _autoPickup;

        /// <summary>
        /// 
        /// </summary>
        public int priority
        {
            get => _priority;
            set => _priority = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool doFloatingAnimAtStart
        {
            get => _doFloatingAnimAtStart;
            set => _doFloatingAnimAtStart = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isPickedUp => _isPickedUp;

        /// <summary>
        /// 
        /// </summary>
        protected IReadOnlyCollection<Character> charactersStepping => _charactersStepping;

        #endregion

        #region Fields

        private static HashSet<Pickable> _availablePickables = new HashSet<Pickable>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _isPickable = true;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _priority = 1;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _autoPickup = false;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _jumpingColliderRadius = 3.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _doFloatingAnimAtStart = true;

        private Sequence _floatingSequence;

        private SpriteRenderer _spriteRenderer;
        private SpriteAnimator _spriteAnimator;
        private CircleCollider2D _circleCollider;
        private float _defaultColliderRadius;

        private HashSet<Character> _charactersStepping = new HashSet<Character>();

        private bool _isPickedUp = false;
        private bool _isDestroyed = false;

        #endregion
    }
}