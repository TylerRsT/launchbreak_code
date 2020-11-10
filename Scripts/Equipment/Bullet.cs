using CreativeSpore.SuperTilemapEditor;
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
    public class BulletResponse
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bullet"></param>
        /// <param name="collider"></param>
        public BulletResponse(Bullet bullet, Collider2D collider)
        {
            this.bullet = bullet;
            this.collider = collider;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Bullet bullet { get; }

        /// <summary>
        /// 
        /// </summary>
        public Collider2D collider { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool received { get; set; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(GameplayTeam))]
    public class Bullet : MonoBehaviour
    {
        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bullet"></param>
        public static implicit operator DamageInfo(Bullet bullet)
        {
            return new DamageInfo
            {
                provider = bullet.instigator,
                damages = bullet.damages,
            };
        }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Reset()
        {
            var collider = GetComponent<CapsuleCollider2D>();
            collider.isTrigger = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteAnimator = GetComponent<SpriteAnimator>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            gameplayTeam = GetComponent<GameplayTeam>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            float radians = Mathf.Deg2Rad * Vector2.SignedAngle(Vector2.right, orientation);
            _rigidbody.velocity = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * speed;

            _untriggeredContacts.RemoveAll(x => x == null);

            foreach(var contact in _untriggeredContacts.OrderBy(x =>
            {
                if(x.GetComponent<Character>() != null)
                {
                    return -1;
                }
                else if(x.GetComponent<Construct>() != null)
                {
                    return 0;
                }
                return 1;
            }))
            {
                if(OnTriggerCollide(contact))
                {
                    _untriggeredContacts.Clear();
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //OnTriggerCollide(collision);
            _untriggeredContacts.Add(collision);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            //OnTriggerCollide(collision);
            _untriggeredContacts.Add(collision);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void OnReceivingBullet(BulletResponse response)
        {
            if (isDashReflected)
            {
                response.received = true;
                ++_damages;
            }
            else
            {
                response.received = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private bool OnTriggerCollide(Collider2D collision)
        {
            _untriggeredContacts.Remove(collision);

            if (_beingDestroyed)
            {
                return true;
            }

            var shooter = (instigator as WeaponEquippable)?.shooter;
            if(shooter == null)
            {
                shooter = instigator as ShooterBehaviour;
            }

            if(shooter != null)
            {
                if(collision.GetComponent<ShooterBehaviour>() == shooter)
                {
                    return false;
                }
            }

            var bulletReaction = collision.GetComponent<BulletReaction>();
            var tilemapChunk = collision.GetComponent<TilemapChunk>();
            if(bulletReaction == null && tilemapChunk == null)
            {
                return false;
            }

            var received = false;

            if (bulletReaction != null)
            {
                var bulletResponse = new BulletResponse(this, collision);
                bulletReaction.SendBulletReceive(bulletResponse);

                received = bulletResponse.received;
            }
            if(tilemapChunk != null)
            {
                received = true;
            }

            if (received)
            {
                _beingDestroyed = true;
                if(_hitAnimation)
                {
                    GameplayStatics.SpawnFireAndForgetAnimation(_hitAnimation, transform.position, Quaternion.identity)
                        .AddComponent<TileObjectRenderer>().additive = 12;
                }
                Destroy(gameObject);
            }

            return received;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int damages
        {
            get => _damages;
            set => _damages = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public DamageType damageType
        {
            get => _damageType;
            set => _damageType = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public float speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool useWeaponRotation => _useWeaponRotation;

        /// <summary>
        /// 
        /// </summary>
        public bool isDashReflected { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 orientation { get; set; } = Vector2.zero;

        public IDamageProvider instigator { get; set; }

        public GameplayTeam gameplayTeam { get; private set; }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _damages = 1;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private DamageType _damageType = DamageType.Normal;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _speed = 2000.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _useWeaponRotation = false;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteAnimation _hitAnimation = default;

        private Rigidbody2D _rigidbody;
        private SpriteAnimator _spriteAnimator;
        private CapsuleCollider2D _capsuleCollider;

        private bool _beingDestroyed = false;

        private List<Collider2D> _untriggeredContacts = new List<Collider2D>();

        #endregion
    }
}