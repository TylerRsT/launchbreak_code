using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class Bootstrap
    {
        #region Const

        private const string BootstrapResourcePath = "Bootstrap";

        #endregion

        #region Main

        /// <summary>
        /// 
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Main()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Telemetry.Initialize();

            _instance = new Bootstrap(Resources.Load<BootstrapData>(BootstrapResourcePath));
            SceneManager.activeSceneChanged += OnMapLoaded;
            SceneManager.sceneUnloaded += OnMapUnloaded;

            // For now, we choose to provide default values.
            for(var i = 0; i < GameModeParams.instance.playerParams.Length; ++i)
            {
                GameModeParams.instance.playerParams[i].isPlaying = true;
                GameModeParams.instance.playerParams[i].selectedCharacter = _instance.data.characters[i];
                GameModeParams.instance.playerParams[i].selectedSkin = Resources.Load<CharacterSkinDescriptor>(_instance.data.characters[i].skinDescriptors[0].skinDescriptorPath);
#if UNITY_EDITOR
                GameModeParams.instance.playerParams[i].loadout[0] = _instance.data.defaultConstruct;

                if (SceneManager.GetActiveScene().name.StartsWith(GameConstants.MapPrefixe))
                {
                    GameModeParams.instance.selectedMap = SceneManager.GetActiveScene().name;
                }
#endif // UNITY_EDITOR
            }

#if UNITY_EDITOR
            if (Input.GetJoystickNames().Length < GameConstants.MaxPlayerCount)
            {
                GameModeParams.instance.playerParams[3].controllerIndex = 0;
            }
#else
            Cursor.visible = false;
#endif // UNITY_EDITOR

            GameModeParams.instance.selectedGameModePrefab = _instance.data.gameModes[0].gameModePrefab;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void OnMapLoaded(Scene oldScene, Scene newScene)
        {
            if (newScene.name.StartsWith(GameConstants.MapPrefixe))
            {
                foreach(var playerParams in GameModeParams.instance.playerParams)
                {
                    foreach(var loadoutItem in playerParams.loadout.items)
                    {
                        loadoutItem?.Load();
                    }
                }
                GameObject.Instantiate(GameModeParams.instance.selectedGameModePrefab, Vector3.zero, Quaternion.identity);
            }

            GameplayStatics.InitMap(newScene);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unloadedScene"></param>
        private static void OnMapUnloaded(Scene unloadedScene)
        {
            GameplayStatics.DeinitMap(unloadedScene);
        }

#endregion

#region Singleton

        /// <summary>
        /// 
        /// </summary>
        public static Bootstrap instance => _instance;

        private static Bootstrap _instance = null;

#endregion

#region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private Bootstrap(BootstrapData data)
        {
            this.data = data;
        }

#endregion

#region Properties

        /// <summary>
        /// 
        /// </summary>
        public BootstrapData data { get; }

#endregion
    }
}
