using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class ConstructionValidator : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider2D>();

            var bulletReaction = GetComponent<BulletReaction>();
            if(bulletReaction != null)
            {
                _acceptBullets = bulletReaction.acceptBullets;
                bulletReaction.acceptBullets = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            var constructionSpace = GetComponentInParent<ConstructionSpace>();
            if(constructionSpace == null || !constructionSpace.isPlaced)
            {
                return;
            }

            var contactingColliders = new List<Collider2D>();
            foreach (var contactingCollider in _colliders)
            {
                var currentContactingColliders = new List<Collider2D>();
                contactingCollider.GetContacts(new ContactFilter2D
                {
                    useTriggers = true,
                }, currentContactingColliders);
                contactingColliders.AddRange(currentContactingColliders);
            }

            var constructionRequest = new ConstructionSpaceRequest(constructionSpace, contactingColliders);
            foreach (var contactingCollider in contactingColliders)
            {
                contactingCollider.gameObject.SendMessage(ConstructionSpace.OnConstructionValidationMethodName, constructionRequest, SendMessageOptions.DontRequireReceiver);
                if (!constructionRequest.accepted)
                {
                    return;
                }
            }
            
            transform.SetParent(constructionSpace.transform.parent);

            var spriteRenderers = new List<SpriteRenderer>();
            spriteRenderers.Add(GetComponent<SpriteRenderer>());
            spriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>());

            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = Color.white;
            }

            foreach(var contactingCollider in _colliders)
            {
                contactingCollider.isTrigger = false;
            }

            var objectRenderer = GetComponent<TileObjectRenderer>();
            if (objectRenderer != null)
            {
                objectRenderer.isChild = false;
                objectRenderer.additive = 0;
            }

            Destroy(constructionSpace.gameObject);
            Destroy(this);

            var bulletReaction = GetComponent<BulletReaction>();
            if(bulletReaction != null)
            {
                bulletReaction.acceptBullets = _acceptBullets;
            }

            SendMessage(Construct.OnConstructedMethodName, construct, SendMessageOptions.DontRequireReceiver);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ConstructLoadoutItem construct { get; set; }

        #endregion

        #region Fields

        private Collider2D[] _colliders;

        private bool _acceptBullets;

        #endregion
    }
}