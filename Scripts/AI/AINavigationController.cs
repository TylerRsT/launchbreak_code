using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(AIAimController))]
    public class AINavigationController : MonoBehaviour
    {
        #region Const

        private const float NodeTolerance = 20.0f;

        private const float IntervalBetweenRaycasts = 0.4f;
        private const float ConstructCheckDistance = 125.0f;

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _aimController = GetComponent<AIAimController>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _character = GetComponentInParent<Character>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            if (_navCoroutine == null)
            {
                _character.Orientate(Vector2.zero, Vector2.zero);
            }
            else
            {
                if(_elapsedSinceLastRaycast < IntervalBetweenRaycasts)
                {
                    return;
                }
                var raycastHit = Physics2D.Raycast(_character.transform.position, _character.moveOrientation, ConstructCheckDistance, LayerMask.GetMask("Solid"));
                if(raycastHit.collider != null)
                {
                    if(raycastHit.collider.GetComponent<BreakableConstruct>() != null)
                    {
                        _aimController.currentTarget = raycastHit.collider.GetComponent<DamageComponent>();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            StopNavigation();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            navMesh?.DrawGizmos();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Coroutine NavigateTo(Vector2 target)
        {
            StopNavigation();

            _navCoroutine = StartCoroutine(StartNavigation(target));
            return _navCoroutine;
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopNavigation()
        {
            if (_navCoroutine != null)
            {
                StopCoroutine(_navCoroutine);
                _navCoroutine = null;
            }
            _character.Orientate(Vector2.zero, Vector2.zero);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Coroutine WaitForNavigation()
        {
            return _navCoroutine;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private IEnumerator StartNavigation(Vector2 target)
        {
            var characterNode = GetClosestNode(_character.transform.position);
            var closestNode = GetClosestNode(target);
            if (closestNode == null)
            {
                yield break;
            }

            target = closestNode.worldPosition;
            var nodes = new List<Vector2>(navMesh.FindPath(characterNode, closestNode));

            if (nodes.Count == 0)
            {
                yield break;
            }

            while (nodes.Count > 0)
            {
                var node = nodes[0];

                Vector2 nodeDifference;
                Vector2 targetOrientation;
                do
                {
                    nodeDifference = (node - (Vector2)_character.transform.position);
                    var moveOrientation = nodeDifference.normalized;

                    if (_aimController.hasTarget)
                    {
                        targetOrientation = _aimController.targetOrientation;
                    }
                    else
                    {
                        var targetDifference = (target - (Vector2)_character.transform.position);
                        targetOrientation = targetDifference.normalized;
                    }

                    _character.Orientate(moveOrientation, targetOrientation);

                    yield return 0;

                    nodeDifference.x = Mathf.Abs(nodeDifference.x);
                    nodeDifference.y = Mathf.Abs(nodeDifference.y);
                } while (nodeDifference.x > NodeTolerance || nodeDifference.y > NodeTolerance);

                nodes.RemoveAt(0);
            }

            _character.Orientate(Vector2.zero, Vector2.zero);
            _navCoroutine = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private NavMeshNode GetClosestNode(Vector2 target)
        {
            int xDim;
            int yDim;

            var node = navMesh.nodeContainer.GetNodeAt(target.x, target.y, out xDim, out yDim);
            if(node == null)
            {
                return null;
            }

            var step = 0;
            while(node == null || !node.walkable)
            {
                ++step;

                NavMeshNode closestNode = null;
                float closestDistance = float.MaxValue;

                for(var i = -1; i <= 1; ++i)
                {
                    for(var j = -1; j <= 1; ++j)
                    {
                        if(i == 0 && j == 0)
                        {
                            continue;
                        }

                        if(TryGetNode(xDim + (step * i), yDim + (step * j), out node))
                        {
                            var distance = Vector2.Distance(target, node.worldPosition);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestNode = node;
                            }
                        }
                    }
                }

                if(closestNode != null)
                {
                    node = closestNode;
                    break;
                }
            }

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool TryGetNode(int x, int y, out NavMeshNode node)
        {
            node = null;
            if (x < 0 || y < 0)
            {
                return false;
            }

            if(x >= navMesh.nodeContainer.nodes.Count)
            {
                return false;
            }
            if (y >= navMesh.nodeContainer.nodes[x].Count)
            {
                return false;
            }

            node = navMesh.nodeContainer.GetNodeAt(x, y);
            return node.walkable;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public NavMesh navMesh { get; set; }

        #endregion

        #region Fields

        private AIAimController _aimController;
        private Character _character;

        private Coroutine _navCoroutine;

        private float _elapsedSinceLastRaycast = IntervalBetweenRaycasts;

        #endregion
    }
}