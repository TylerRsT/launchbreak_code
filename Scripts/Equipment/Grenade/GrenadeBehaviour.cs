using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using Elendow.SpritedowAnimator;
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
    [HideInInspector]
    public class GrenadeBehaviour : MonoBehaviour
    {
        #region Const

        private const string PickBlockerLayerName = "PickBlocker";
        private const string ExplosionSoundKey = "Explosion";

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        static GrenadeBehaviour()
        {
            var type = typeof(GrenadeBehaviour);
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            _fieldList = type.GetFields(bindingFlags);
        }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            StartCoroutine(Flash());
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            _elapsedTime += GameplayStatics.gameFixedDeltaTime;

            var interval = 0.0f;
            if(_elapsedTime >= 0.0f && _elapsedTime < explosionDelay / 4.0f)
            {
                interval = 0.3f;
            }
            else if(_elapsedTime >= explosionDelay / 4.0f && _elapsedTime < explosionDelay * 2.0f / 4.0f)
            {
                interval = 0.2f;
            }
            else if(_elapsedTime >= explosionDelay * 2.0f / 4.0f && _elapsedTime < explosionDelay * 3.0f / 4.0f)
            {
                interval = 0.1f;
            }
            else
            {
                interval = 0.05f;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = _elapsedTime % (interval * 2.0f) < interval ? onSprite : offSprite;
            }

            if(!hasExploded && _elapsedTime >= explosionDelay)
            {
                Explode();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            _rollDirection = Vector2.Reflect(_rollDirection, collision.contacts[0].normal);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void CopyFrom(GrenadeBehaviour other)
        {
            foreach(var fieldInfo in _fieldList)
            {
                fieldInfo.SetValue(this, fieldInfo.GetValue(other));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="throwInfo"></param>
        /// <param name="multiplier"></param>
        public void RollOnTheFloor(Vector2 direction, GrenadeThrowInfo throwInfo, float multiplier)
        {
            StartCoroutine(RollCoroutine(direction, throwInfo, multiplier));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="throwInfo"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        private IEnumerator RollCoroutine(Vector2 direction, GrenadeThrowInfo throwInfo, float multiplier)
        {
            _rollDirection = direction;

            var rigidbody = GetComponent<Rigidbody2D>();

            var elapsed = 0.0f;
            var oldPosition = throwInfo.rollCurve.Evaluate(elapsed);
            while (elapsed < throwInfo.rollDuration)
            {
                yield return new WaitForFixedUpdate();
                elapsed += GameplayStatics.gameFixedDeltaTime;
                var rollPosition = throwInfo.rollCurve.Evaluate(elapsed / throwInfo.rollDuration);

                var diff = (rollPosition - oldPosition) * throwInfo.rollMoveSpeed;
                float radians = Mathf.Deg2Rad * Vector2.SignedAngle(Vector2.right, _rollDirection);
                rigidbody.velocity = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * diff * multiplier;
                var rotation = diff * throwInfo.rollRotSpeed;

                if (_rollDirection.x >= 0.0f)
                {
                    rotation *= -1.0f;
                }

                rotation += _subRenderer.transform.localRotation.z;
                rotation %= 360.0f;
                var throwPosition = throwInfo.throwCurve.Evaluate(elapsed / throwInfo.rollDuration);
                _subRenderer.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation * multiplier);
                _subRenderer.transform.localPosition = new Vector2(0.0f, throwPosition * throwInfo.throwMultiplier) * multiplier;
            }

            rigidbody.velocity = Vector2.zero;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Explode()
        {
            var localRaycasts = new RaycastHit2D[16];
            var damageables = new HashSet<IDamageable>();

            var grenadeExplosion = new GameObject().AddComponent<GrenadeExplosion>();
            grenadeExplosion.transform.position = transform.position;
            grenadeExplosion.explosionAnimation = explosionAnimation;
            grenadeExplosion.frequency = explosionFrequency;

            grenadeExplosion.explosions.Add(new List<Vector2> { transform.position });

            this.EmitSound(ExplosionSoundKey);

            for (var x = -1.0f; x <= 1.0f; x += 0.5f)
            {
                for(var y = -1.0f; y <= 1.0f; y += 0.5f)
                {
                    if(x == 0.0f && y == 0.0f)
                    {
                        continue;
                    }
                    var localExplosions = ExplosionRaycast(new Vector2(x, y).normalized, localRaycasts, damageables);
                    while (grenadeExplosion.explosions.Count < localExplosions.Count + 1)
                    {
                        grenadeExplosion.explosions.Add(new List<Vector2>());
                    }
                    for (var i = 0; i < localExplosions.Count; ++i)
                    {
                        grenadeExplosion.explosions[i + 1].Add(localExplosions[i]);
                    }
                }
            }

            Character instigator = null;

            if (equippable != null)
            {
                instigator = equippable.character;
            }

            foreach (var damageable in damageables)
            {
                var character = damageable as Character;
                var isDodging = character != null && (character.GetComponent<InvincilityBuff>() || character.gameplayState == CharacterGameplayState.Dash);
                if (character != null && !isDodging)
                {
                    KnockbackCharacter(character);

                    // The following was when we had disposable grenades as weapons.
                    /*if (character == instigator)
                    {
                        character.ClearWeapon(equippable, false);
                    }*/
                }
                if (!isDodging)
                {
                    damageable.TakeDamages(new DamageInfo
                    {
                        provider = equippable,
                        damages = damages,
                        damageType = damageType,
                    });
                }
            }

            if(GetComponent<GrenadeWeaponEquippable>() == null)
            {
                Destroy(gameObject);
            }

            CameraShakeManager.instance.Shake(explosionShake);

            hasExploded = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<Vector2> ExplosionRaycast(Vector2 direction, RaycastHit2D[] tempArray, HashSet<IDamageable> damageables)
        {
            var pickBlockerLayer = LayerMask.NameToLayer(PickBlockerLayerName);
            var count = Physics2D.RaycastNonAlloc(transform.position, direction, tempArray, radius);
            var explosionPositions = new List<Vector2>();
            var maxDistance = radius;

            for (var i = 0; i < count; ++i)
            {
                var raycastHit = tempArray[i];

                var damageComponent = raycastHit.collider.GetComponent<DamageComponent>();
                if (damageComponent != null)
                {
                    damageables.Add(damageComponent);
                }

                if (raycastHit.collider.gameObject.layer == pickBlockerLayer/* || raycastHit.collider.GetComponent<Construct>() != null*/)
                {
                    maxDistance = raycastHit.distance;
                    break;
                }
            }

            for(var i = explosionDistance; i < maxDistance; i += explosionDistance)
            {
                explosionPositions.Add((Vector2)transform.position + direction * i);
            }

            return explosionPositions;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        private void KnockbackCharacter(Character character)
        {
            Vector2 diff = transform.position - character.transform.position;
            diff.Normalize();
            diff *= -1.0f;

            character.Push(diff, knockbackCurve, knockback,
                character.resources.hasArmor ? knockbackDuration / 2.0f : knockbackDuration);
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator Flash()
        {
            yield return new WaitUntil(() => _elapsedTime / explosionDelay >= 0.5f);

            var remainingTime = explosionDelay - _elapsedTime;

            HitFlash hitFlash;
            Transform transform;
            SpriteRenderer renderer;

            if (subRenderer != null)
            {
                renderer = subRenderer;
                hitFlash = renderer.GetComponent<HitFlash>();
                transform = renderer.transform;
            }
            else
            {
                renderer = GetComponent<SpriteRenderer>();
                hitFlash = GetComponent<HitFlash>();
                transform = this.transform;
            }

            if(hitFlash == null)
            {
                yield break;
            }

            hitFlash.Flash(1.0f, remainingTime, 0.1f);

            yield return new WaitUntil(() => _elapsedTime / explosionDelay >= 0.75f);
            
            var flashMaterial = hitFlash.flashMaterial;
            Destroy(hitFlash);

            remainingTime = explosionDelay - _elapsedTime;
            renderer.material = flashMaterial;
            transform.DOScale(2.0f, remainingTime);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public SpriteRenderer subRenderer => _subRenderer;

        /// <summary>
        /// 
        /// </summary>
        public float explosionDelay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float radius { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float knockback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float knockbackDuration { get; set; } = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        public AnimationCurve knockbackCurve { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int damages { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DamageType damageType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Sprite offSprite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Sprite onSprite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SpriteAnimation explosionAnimation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int explosionFrequency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float explosionDistance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShakePreset explosionShake { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WeaponEquippable equippable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool hasExploded { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SpriteRenderer spriteRenderer { get; set; }

        #endregion

        #region Fields

        private static FieldInfo[] _fieldList;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SpriteRenderer _subRenderer = default;

        private Vector2 _rollDirection;

        private float _elapsedTime = 0.0f;

        #endregion
    }
}