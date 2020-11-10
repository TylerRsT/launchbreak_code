using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class DelayedWall : MonoBehaviour, ITrapElement
    {
        #region ITrapElement

        /// <summary>
        /// 
        /// </summary>
        bool ITrapElement.Trigger()
        {
            if(GetComponent<ConstructionValidator>() != null)
            {
                return false;
            }

            var colliders = new List<Collider2D>();
            _boxCollider.GetContacts(new ContactFilter2D
            {
                useTriggers = true,
            }, colliders);

            var constructionRequest = new ConstructionSpaceRequest(null, colliders);
            foreach (var collider in colliders)
            {
                collider.gameObject.SendMessage(ConstructionSpace.OnConstructionValidationMethodName, constructionRequest, SendMessageOptions.DontRequireReceiver);
                if (!constructionRequest.accepted)
                {
                    return false;
                }
            }

            Instantiate(_wallPrefab, transform.position, transform.rotation).GetComponent<Construct>().supplyCost = GetComponent<Construct>().supplyCost;
            Destroy(gameObject);
            return true;
        }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            var instigator = GetComponent<Construct>().instigator;
            if(instigator != null)
            {
                instigator.spawner.traps.Add(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            if(!_hasTriggered && collision.GetComponent<Character>() != null)
            {
                _hasTriggered = ((ITrapElement)this).Trigger();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void OnConstructed(ConstructLoadoutItem item)
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            var instigator = GetComponent<Construct>().instigator;
            if(instigator != null)
            {
                instigator.spawner.traps.Remove(this);
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _wallPrefab = default;

        private BoxCollider2D _boxCollider;

        private bool _hasTriggered = false;

        #endregion
    }
}