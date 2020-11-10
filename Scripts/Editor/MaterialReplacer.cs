using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class MaterialReplacer
    {
        #region Methods

        [MenuItem("Shovel/Replace All URP Materials")]
        private static void ReplaceAllURPMaterials()
        {
            const string spriteLitDefaultAssetPath = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Lit-Default.mat";
            const string spriteUnlitDefaultAssetPath = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat";

            const string urpNamespace = "UnityEngine.Experimental.Rendering.Universal";

            var spriteLitMaterial = AssetDatabase.LoadAssetAtPath<Material>(spriteLitDefaultAssetPath);
            var spriteUnlitMaterial = AssetDatabase.LoadAssetAtPath<Material>(spriteUnlitDefaultAssetPath);

            var prefabChangedCount = 0;
            var sceneChangedCount = 0;

            var spriteDefaultMaterial = Resources.Load<BootstrapData>("Bootstrap").spriteDefaultMaterial;

            var allPrefabs = AssetDatabase.FindAssets("t:Prefab");
            foreach (var prefabID in allPrefabs)
            {
                var asset = AssetDatabase.GUIDToAssetPath(prefabID);

                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(asset);
                var renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
                var hasChanged = false;
                foreach (var renderer in renderers)
                {
                    if (renderer.sharedMaterial == spriteLitMaterial || renderer.sharedMaterial == spriteUnlitMaterial)
                    {
                        renderer.sharedMaterial = spriteDefaultMaterial;
                        hasChanged = true;
                    }
                }

                var components = gameObject.GetComponents<Component>();
                foreach(var component in components)
                {
                    if (component.GetType().Namespace.StartsWith(urpNamespace))
                    {
                        Component.DestroyImmediate(component, true);
                        hasChanged = true;
                    }
                }

                if (hasChanged)
                {
                    ++prefabChangedCount;
                    EditorUtility.SetDirty(gameObject);
                    PrefabUtility.SavePrefabAsset(gameObject);
                }
            }

            var allScenes = AssetDatabase.FindAssets("t:Scene");
            foreach(var sceneID in allScenes)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneID);
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var rootObjects = scene.GetRootGameObjects();
                var hasChanged = false;

                foreach(var rootObject in rootObjects)
                {
                    var renderers = rootObject.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var renderer in renderers)
                    {
                        if (renderer.sharedMaterial == spriteLitMaterial || renderer.sharedMaterial == spriteUnlitMaterial)
                        {
                            renderer.sharedMaterial = spriteDefaultMaterial;
                            hasChanged = true;
                        }
                    }

                    var components = rootObject.GetComponents<Component>();
                    foreach (var component in components)
                    {
                        if (component == null || string.IsNullOrEmpty(component.GetType().Namespace))
                        {
                            continue;
                        }
                        if (component.GetType().Namespace.StartsWith(urpNamespace))
                        {
                            Component.DestroyImmediate(component, true);
                            hasChanged = true;
                        }
                    }
                }

                if (hasChanged)
                {
                    ++sceneChangedCount;
                    EditorSceneManager.MarkAllScenesDirty();
                    EditorSceneManager.SaveOpenScenes();
                }
            }

            Debug.Log($"Changed {prefabChangedCount} prefabs.");
            Debug.Log($"Changed {sceneChangedCount} scenes.");
        }

        #endregion
    }
}