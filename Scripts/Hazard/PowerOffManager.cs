using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PowerOffManager : MonoBehaviour
    {
        #region Const

        private const string PowerOffSoundKey = "PowerOff";
        private const string PowerOnSoundKey = "PowerOn";

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        public void PowerOff(int level)
        {
            if(isPoweringOff)
            {
                return;
            }

            var odds = _oddsByLevel.Count <= level ? _oddsByLevel[_oddsByLevel.Count - 1] : _oddsByLevel[level];
            var random = Random.Range(0.0f, 1.0f);

            if (random <= odds)
            {
                StartCoroutine(DoPowerOff());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator DoPowerOff()
        {
            isPoweringOff = true;

            var announcementDuration = Random.Range(_startDelayMin, _startDelayMax);
            var announcementElapsed = 0.0f;

            var isOn = true;
            var intensityMultiplier = Random.Range(_intensityMultiplierMin, _intensityMultiplierMax);

            while (announcementElapsed < announcementDuration)
            {
                foreach(var light in _lights)
                {
                    light.intensity = isOn ? light.intensity * intensityMultiplier : light.intensity / intensityMultiplier;
                }

                yield return new WaitForSeconds(_announcementInterval);

                isOn = !isOn;
                announcementElapsed += _announcementInterval;
            }

            foreach (var light in _lights)
            {
                light.intensity = isOn ? light.intensity : light.intensity / intensityMultiplier;
            }

            yield return new WaitForSeconds(Random.Range(_startDelayMin, _startDelayMax));

            var transitionDuration = Random.Range(_transitionDurationMin, _transitionDurationMax);

            var tweens = new List<Tween>();
            foreach(var light in _lights)
            {
                tweens.Add(DOTween.To(() => light.intensity, (value) => light.intensity = value, light.intensity * intensityMultiplier, transitionDuration));
            }
            this.EmitSound(PowerOffSoundKey, false);

            while(tweens.Any(x => x.IsActive()))
            {
                yield return 0;
            }

            tweens.Clear();
            yield return new WaitForSeconds(Random.Range(_noPowerDurationMin, _noPowerDurationMax));

            foreach (var light in _lights)
            {
                tweens.Add(DOTween.To(() => light.intensity, (value) => light.intensity = value, light.intensity / intensityMultiplier, transitionDuration));
            }
            this.EmitSound(PowerOnSoundKey, false);

            while (tweens.Any(x => x.IsActive()))
            {
                yield return 0;
            }

            tweens.Clear();
            isPoweringOff = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool isPoweringOff { get; private set; }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _startDelayMin = 0.5f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _startDelayMax = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _announcementInterval = 0.2f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _intensityMultiplierMin = 0.5f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _intensityMultiplierMax = 0.6f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _transitionDurationMin = 0.5f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _transitionDurationMax = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _noPowerDurationMin = 4.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _noPowerDurationMax = 10.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<LightSource> _lights = new List<LightSource>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<float> _oddsByLevel = new List<float>();

        #endregion
    }
}