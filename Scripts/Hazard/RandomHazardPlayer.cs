using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class RandomHazardPlayer : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _hazardsToTrigger.AddRange(FindObjectsOfType<RandomHazard>());
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {
            if(_hazardsToTrigger.Count == 0)
            {
                return;
            }

            var randomHazard = _hazardsToTrigger[Random.Range(0, _hazardsToTrigger.Count)];
            Play(randomHazard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="interval"></param>
        /// <param name="delay"></param>
        public void Play(int count, float interval, float delay = 0.0f)
        {
            StartCoroutine(PlayCount(count, interval, delay));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        public void Play(int count)
        {
            for(var i = 0; i < count; ++i)
            {
                Play();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hazard"></param>
        public void Play(RandomHazard hazard)
        {
            hazard.Play();
            if(!hazard.canTriggerMultipleTimes)
            {
                _hazardsToTrigger.Remove(hazard);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hazards"></param>
        public void Play(IEnumerable<RandomHazard> hazards)
        {
            foreach(var hazard in hazards)
            {
                Play(hazard);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hazards"></param>
        /// <param name="interval"></param>
        /// <param name="delay"></param>
        public void Play(IEnumerable<RandomHazard> hazards, float interval, float delay = 0.0f)
        {
            StartCoroutine(PlayList(hazards, interval, delay));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="interval"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator PlayCount(int count, float interval, float delay = 0.0f)
        {
            if(delay > 0.0f)
            {
                yield return new WaitForSeconds(delay);
            }

            for(var i = 0; i < count; ++i)
            {
                Play();
                yield return new WaitForSeconds(interval);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hazards"></param>
        /// <param name="interval"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator PlayList(IEnumerable<RandomHazard> hazards, float interval, float delay = 0.0f)
        {
            if (delay > 0.0f)
            {
                yield return new WaitForSeconds(delay);
            }

            foreach(var hazard in hazards)
            {
                Play(hazard);
                yield return new WaitForSeconds(interval);
            }
        }

        #endregion

        #region Fields

        private List<RandomHazard> _hazardsToTrigger = new List<RandomHazard>();

        #endregion
    }
}