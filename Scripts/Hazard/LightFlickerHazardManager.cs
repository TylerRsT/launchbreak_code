using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class LightFlickerHazardManager : HazardManager
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void OnGameModeStep()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameMode"></param>
        /// <returns></returns>
        public override bool TryInitialize(GameMode gameMode)
        {
            foreach(var lightController in _lightControllers)
            {
                SetupLightFlicker(lightController);
            }

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lightController"></param>
        private void SetupLightFlicker(LightController lightController)
        {
            lightController.StartCoroutine(LightFlickerCoroutine(lightController));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lightController"></param>
        /// <returns></returns>
        private IEnumerator LightFlickerCoroutine(LightController lightController)
        {
            yield return new WaitForSeconds(Random.Range(_startDelayMin, _startDelayMax));

            var currentIntensity = lightController.intensity;
            var count = Random.Range(_flickerCountMin, _flickerCountMax + 1);
            for (var i = 0; i < count; ++i)
            {
                lightController.Flicker(Random.Range(_intervalMin, _intervalMax), 0.0f);

                yield return new WaitForSeconds(Random.Range(_flickerDurationMin, _flickerDurationMax));

                lightController.StopFlickering();
                yield return lightController.WaitForFlicker();
            }

            lightController.intensity = currentIntensity;
            SetupLightFlicker(lightController);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _startDelayMin = 20.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _startDelayMax = 60.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _intervalMin = 0.05f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _intervalMax = 0.2f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _flickerDurationMin = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _flickerDurationMax = 2.5f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _flickerCountMin = 1;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _flickerCountMax = 3;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<LightController> _lightControllers = new List<LightController>();

        #endregion
    }
}