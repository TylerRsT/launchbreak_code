using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class Turret : ShooterBehaviour
    {
        #region Const

        private const string WeaponChildName = "Weapon";
        private const string MuzzleChildName = "Muzzle";

        private const string SolidLayerName = "Solid";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override string damageProviderName => "Turret";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _circleCollider = GetComponent<CircleCollider2D>();

            _weapon = transform.Find(WeaponChildName).GetComponent<WeaponEquippable>();
            _muzzle = _weapon.transform.Find(MuzzleChildName);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            UpdateInternal();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckPotentialTargets(collision);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerStay2D(Collider2D collision)
        {
            CheckPotentialTargets(collision);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            var shooter = collision.GetComponent<ShooterBehaviour>();
            if (shooter != null)
            {
                _potentialTargets.Remove(shooter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void OnConstructed(ConstructLoadoutItem item)
        {
            var instigator = GetComponent<Construct>().instigator;
            if(instigator != null)
            {
                spawner = instigator.spawner;
            }

            GetComponent<GameplayTeam>().teamIndex = spawner.teamIndex;
            _circleCollider.isTrigger = true;
            _circleCollider.enabled = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void UpdateInternal()
        {
            if(_currentTarget != null && CanFireTarget(_currentTarget))
            {
                FireTarget(_currentTarget);
                return;
            }

            ShooterBehaviour closestTarget = null;
            var closestDistance = float.MaxValue;

            var targetsToRemove = new List<ShooterBehaviour>();
            foreach(var potentialTarget in _potentialTargets)
            {
                var vec1 = transform.position;
                var vec2 = potentialTarget.transform.position;
                Vector2 difference = vec2 - vec1;
                float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
                var diffRadians = Mathf.Deg2Rad * Vector2.Angle(Vector2.right, difference) * sign;

                var raycastHit = Physics2D.Raycast(_muzzle.position, new Vector2(Mathf.Cos(diffRadians), Mathf.Sin(diffRadians)), _circleCollider.radius, LayerMask.GetMask(SolidLayerName));

                if(raycastHit.collider == null)
                {
                    continue;
                }

                var shooter = raycastHit.collider.GetComponent<ShooterBehaviour>();

                if(shooter == potentialTarget)
                {
                    if (raycastHit.distance < closestDistance)
                    {
                        closestDistance = raycastHit.distance;
                        closestTarget = shooter;
                    }
                }
                else
                {
                    targetsToRemove.Add(potentialTarget);
                }
            }

            foreach(var targetToRemove in targetsToRemove)
            {
                _potentialTargets.Remove(targetToRemove);
            }

            if(closestTarget)
            {
                _currentTarget = closestTarget;
                FireTarget(closestTarget);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool CanFireTarget(ShooterBehaviour target)
        {
            var vec1 = transform.position;
            var vec2 = target.transform.position;
            Vector2 difference = vec2 - vec1;
            float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
            var diffRadians = Mathf.Deg2Rad * Vector2.Angle(Vector2.right, difference) * sign;

            var raycastHit = Physics2D.Raycast(_muzzle.position, new Vector2(Mathf.Cos(diffRadians), Mathf.Sin(diffRadians)), _circleCollider.radius, LayerMask.GetMask(SolidLayerName));
            var character = raycastHit.collider.GetComponent<Character>();

            return character == target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        private void FireTarget(ShooterBehaviour target)
        {
            var vec1 = transform.position;
            var vec2 = target.transform.position;
            Vector2 difference = vec2 - vec1;
            float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
            var diffRadians = Mathf.Deg2Rad * Vector2.Angle(Vector2.right, difference) * sign;
            targetOrientation = new Vector2(Mathf.Cos(diffRadians), Mathf.Sin(diffRadians));

            _weapon.transform.localRotation = Quaternion.Euler(targetOrientation);
            //_weapon.SetTargetOrientation(targetOrientation);
            _weapon.Trigger();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckPotentialTargets(Collider2D collision)
        {
            if(this.IsInConstruction())
            {
                return;
            }

            var shooter = collision.GetComponent<ShooterBehaviour>();
            if (shooter == null)
            {
                return;
            }

            if (spawner != null && shooter.GetComponent<GameplayTeam>().teamIndex == spawner.teamIndex)
            {
                return;
            }

            _potentialTargets.Add(shooter);
        }

        #endregion

        #region Fields

        private CircleCollider2D _circleCollider;

        private WeaponEquippable _weapon;
        private Transform _muzzle;

        private HashSet<ShooterBehaviour> _potentialTargets = new HashSet<ShooterBehaviour>();
        private ShooterBehaviour _currentTarget = null;

        #endregion
    }
}