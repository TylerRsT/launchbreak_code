using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class NavMesh
    {
        #region Const

        private const float RayInterval = GameConstants.HalfTileSize;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        static NavMesh()
        {
            AStarSharp.Node.NODE_SIZE = 1;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Generate()
        {
            var additiveX = GameConstants.SceneWidth / 2.0f / RayInterval;
            var additiveY = GameConstants.SceneHeight / 2.0f / RayInterval;

            var colCount = Mathf.CeilToInt(GameConstants.SceneWidth / RayInterval);
            var rowCount = Mathf.CeilToInt(GameConstants.SceneHeight / RayInterval);

            _nodeContainer = new NavMeshNodeContainer(
                new Vector2Int(colCount, rowCount),
                new Vector2(additiveX, additiveY),
                RayInterval
            );

            var circleCastMask = LayerMask.GetMask("Solid");
            var astarNodes = new List<List<AStarSharp.Node>>();

            for (var i = 0; i < colCount; ++i)
            {
                var row = new List<AStarSharp.Node>();

                for (var j = 0; j < rowCount; ++j)
                {
                    var vec = new System.Numerics.Vector2(i, j);
                    var node = _nodeContainer.GetNodeAt(i, j);
                    var walkable = true;
                    var raycastHit = Physics2D.CircleCast(node.worldPosition, 13.0f, Vector2.zero, 0.0f, circleCastMask);

                    walkable = raycastHit.collider == null;
                    if (!walkable)
                    {
                        if (raycastHit.collider.GetComponent<Construct>() != null)
                        {
                            walkable = true;
                        }
                    }

                    var navMeshHelpers = GameObject.FindObjectsOfType<NavMeshHelper>()
                        .Where(x => x.bounds.Contains(node.worldPosition));
                    if (navMeshHelpers.Any())
                    {
                        walkable = navMeshHelpers.All(x => x.walkable);
                    }

                    node.walkable = walkable;

                    row.Add(node.node);
                }

                astarNodes.Add(row);
            }

            _astar = new AStarSharp.Astar(astarNodes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public IReadOnlyList<Vector2> FindPath(Vector2 position, Vector2 target)
        {
            const float additiveX = GameConstants.SceneWidth / 2.0f / RayInterval;
            const float additiveY = GameConstants.SceneHeight / 2.0f / RayInterval;

            var path = new List<Vector2>();

            if (_astar == null)
            {
                return path;
            }

            var nodeStack = _astar.FindPath(
                new System.Numerics.Vector2(position.x / RayInterval + additiveX, position.y / RayInterval + additiveY),
                new System.Numerics.Vector2(target.x / RayInterval + additiveX, target.y / RayInterval + additiveY));

            if (nodeStack == null)
            {
                return path;
            }

            foreach (var node in nodeStack)
            {
                path.Add(new Vector2((node.Position.X - additiveX) * RayInterval, (node.Position.Y - additiveY) * RayInterval));
            }

            path.Add(target);
            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="targetNode"></param>
        /// <returns></returns>
        public IReadOnlyList<Vector2> FindPath(NavMeshNode startNode, NavMeshNode targetNode)
        {
            const float additiveX = GameConstants.SceneWidth / 2.0f / RayInterval;
            const float additiveY = GameConstants.SceneHeight / 2.0f / RayInterval;

            var path = new List<Vector2>();

            if (_astar == null)
            {
                return path;
            }

            var nodeStack = _astar.FindPath(
                new System.Numerics.Vector2(startNode.gridPosition.x, startNode.gridPosition.y),
                new System.Numerics.Vector2(targetNode.gridPosition.x, targetNode.gridPosition.y));

            if (nodeStack == null)
            {
                return path;
            }

            foreach (var node in nodeStack)
            {
                path.Add(new Vector2((node.Position.X - additiveX) * RayInterval, (node.Position.Y - additiveY) * RayInterval));
            }

            path.Add(targetNode.worldPosition);
            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DrawGizmos()
        {
            if (_nodeContainer == null)
            {
                return;
            }

            for (var i = 0; i < _nodeContainer.gridSize.x; ++i)
            {
                for (var j = 0; j < _nodeContainer.gridSize.y; ++j)
                {
                    var node = _nodeContainer.GetNodeAt(i, j);
                    Gizmos.color = node.walkable ? Color.green : Color.red;
                    Gizmos.DrawSphere(node.worldPosition, RayInterval / 4.0f);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public NavMeshNodeContainer nodeContainer => _nodeContainer;

        #endregion

        #region Fields

        private AStarSharp.Astar _astar;
        private NavMeshNodeContainer _nodeContainer;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class NavMeshNode
    {
        public AStarSharp.Node node;

        public Vector2 worldPosition;
        public Vector2Int gridPosition;

        public bool walkable
        {
            get { return _walkable; }
            set
            {
                _walkable = value;
                node.Walkable = value;
            }
        }

        private bool _walkable;
    }

    /// <summary>
    /// 
    /// </summary>
    public class NavMeshNodeContainer
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridSize"></param>
        /// <param name="additive"></param>
        /// <param name="interval"></param>
        public NavMeshNodeContainer(Vector2Int gridSize, Vector2 additive, float interval)
        {
            _nodes = new List<List<NavMeshNode>>(gridSize.x);
            for (var i = 0; i < gridSize.x; ++i)
            {
                var row = new List<NavMeshNode>(gridSize.y);
                for (var j = 0; j < gridSize.y; ++j)
                {
                    var node = new NavMeshNode
                    {
                        node = new AStarSharp.Node(new System.Numerics.Vector2(i, j), true),

                        worldPosition = new Vector2((i - additive.x) * interval, (j - additive.y) * interval),
                        gridPosition = new Vector2Int(i, j),

                        walkable = true,
                    };

                    row.Add(node);
                }

                _nodes.Add(row);
            }

            this.gridSize = gridSize;
            _additive = additive;
            _interval = interval;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public NavMeshNode GetNodeAt(int x, int y)
        {
            return _nodes[x][y];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public NavMeshNode GetNodeAt(float x, float y)
        {
            return _nodes[(int)(x / _interval + _additive.x)][(int)(y / _interval + _additive.y)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xDim"></param>
        /// <param name="yDim"></param>
        /// <returns></returns>
        public NavMeshNode GetNodeAt(float x, float y, out int xDim, out int yDim)
        {
            xDim = (int)(x / _interval + _additive.x);
            yDim = (int)(y / _interval + _additive.y);

            if (xDim < 0 || yDim < 0)
            {
                return null;
            }

            if (xDim >= _nodes.Count || yDim >= _nodes[xDim].Count)
            {
                return null;
            }

            return _nodes[xDim][yDim];
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Vector2Int gridSize { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<List<NavMeshNode>> nodes => _nodes;

        #endregion

        #region Fields

        private List<List<NavMeshNode>> _nodes;

        private Vector2 _additive;
        private float _interval;

        #endregion
    }
}