using System.Linq;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [AIBehaviour]
    public class AIBehaviour_Patrol : AIBehaviour
    {
        #region Const

        private const float NoTaskTimeout = 1.0f;

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            _walkableNodes = navigationController.navMesh.nodeContainer.nodes.SelectMany(x => x.Where(y => y.walkable)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (!behaviourManager.isTasked)
            {
                _elapsedSinceNoTask += GameplayStatics.gameDeltaTime;
                if(_elapsedSinceNoTask >= NoTaskTimeout)
                {
                    var patrol = new AITask_Patrol();
                    patrol.walkableNodes = _walkableNodes;
                    behaviourManager.SetTask(patrol);
                }
            }
            else
            {
                _elapsedSinceNoTask = 0.0f;
            }
        }

        #endregion

        #region Fields

        private float _elapsedSinceNoTask;
        private NavMeshNode[] _walkableNodes;

        #endregion
    }
}