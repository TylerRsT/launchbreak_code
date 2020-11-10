using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class RedeemRepulser : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _circleCollider = GetComponent<CircleCollider2D>();
            _circleCollider.radius = 0.0f;
            _circleCollider.isTrigger = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            _circleCollider.radius += _radiusSpeed * GameplayStatics.gameFixedDeltaTime;

            if(_circleCollider.radius >= _radiusMax)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Vector2.Distance(transform.position, collision.transform.position) < _circleCollider.radius - _effectMaxDistance)
            {
                return;
            }

            foreach (var type in _typesToDestroy)
            {
                if(collision.GetComponent(type) != null)
                {
                    Destroy(collision.gameObject);
                    return;
                }
            }

            var character = collision.GetComponent<Character>();
            character?.Push(character.transform.position - transform.position, _characterPushPower, _characterPushDuration);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, _circleCollider.radius);
        }

        #endregion

        #region Fields

        private static Type[] _typesToDestroy = new Type[]
        {
            typeof(Bullet),
            typeof(GrenadeBehaviour),
        };

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _radiusSpeed = 1000.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _radiusMax = 640.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _effectMaxDistance = 72.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _characterPushPower = 5.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _characterPushDuration = 0.2f;

        private CircleCollider2D _circleCollider;

        #endregion
    }
}