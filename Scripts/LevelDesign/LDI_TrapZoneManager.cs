using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class LDI_TrapZoneManager : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            StartCoroutine(Activate());
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public IEnumerator Activate()
        {
            yield return new WaitForSeconds(_switchPrimerDelay);

            if(_toggle)
            {
                foreach(var item in _trapZonesGroup1)
                {
                    StartCoroutine(item.BuildUp());
                }
            }
            else
            {
                foreach (var item in _trapZonesGroup2)
                {
                    StartCoroutine(item.BuildUp());
                }
            }

            yield return new WaitForSeconds(_switchDelay);

            if (_toggle)
            {
                foreach (var item in _trapZonesGroup1)
                {
                    item.Activate();
                }

                foreach (var item in _trapZonesGroup2)
                {
                    item.Deactivate();
                }
            }
            else
            {
                foreach (var item in _trapZonesGroup1)
                {
                    item.Deactivate();
                }

                foreach (var item in _trapZonesGroup2)
                {
                    item.Activate();
                }
            }

            _toggle = !_toggle;

            StartCoroutine(Activate());
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float switchDelay => _switchDelay;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [Range(4, 100)]
        private float _switchDelay = 4f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [Range(1, 100)]
        private float _switchPrimerDelay = 1f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<LDI_TrapZone> _trapZonesGroup1 = new List<LDI_TrapZone>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<LDI_TrapZone> _trapZonesGroup2 = new List<LDI_TrapZone>();

        private bool _toggle = false;

        #endregion
    }
}
