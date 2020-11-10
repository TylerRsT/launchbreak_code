using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(LDI_TrapZone))]
    [CanEditMultipleObjects]
    public class LDI_TrapZoneEditor : Editor
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate Tiles"))
            {
                var trapZone = (LDI_TrapZone)target;
                trapZone.GenerateTiles();
            }
        }

        #endregion
    }
}
