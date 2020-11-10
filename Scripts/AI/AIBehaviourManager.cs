using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(AINavigationController))]
    public class AIBehaviourManager : MonoBehaviour
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        static AIBehaviourManager()
        {
            _behaviourTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.GetCustomAttributes(true).OfType<AIBehaviourAttribute>().Any())
                .Where(x => typeof(AIBehaviour).IsAssignableFrom(x))
                .Where(x => !x.IsAbstract)
                .ToArray();
        }

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _character = GetComponentInParent<Character>();
            foreach (var behaviourType in _behaviourTypes)
            {
                if (gameObject.GetComponent(behaviourType) == null)
                {
                    gameObject.AddComponent(behaviourType);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            if (GameMode.instance.isGameOver)
            {
                Destroy(gameObject);
                return;
            }

            _currentTask?.FixedUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            StopTask();
            StopAllCoroutines();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="score"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public T SetTask<T>(T task, float score = 0.0f, bool force = false)
            where T : AITask
        {
            if(GameMode.instance.isGameOver)
            {
                return null;
            }

            if (_currentTask == null || force || _currentTask.score < score)
            {
                StopTask();
                _currentTask = task;

                task.Initialize(_character, this, score);
                _taskCoroutine = StartCoroutine(PerformTask(task));

                return task;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="score"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public T SetTask<T>(float score = 0.0f, bool force = false)
            where T : AITask, new()
        {
            return SetTask(new T(), score, force);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        public void StopTask(AITask task)
        {
            if(_currentTask == task && _currentTask != null)
            {
                StopTask();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopTask()
        {
            if (_taskCoroutine != null)
            {
                StopCoroutine(_taskCoroutine);
                _taskCoroutine = null;
                _currentTask = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private IEnumerator PerformTask(AITask task)
        {
            yield return task.OnHandleController();
            StopTask();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool isTasked => _currentTask != null;

        #endregion

        #region Fields

        private static System.Type[] _behaviourTypes;

        private AITask _currentTask;
        private Coroutine _taskCoroutine;

        private Character _character;

        #endregion
    }
}