using Com.LuisPedroFonseca.ProCamera2D;
using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class GameplayStatics
    {
        #region Const

        private const float ScatterMinDistance = 20.0f;
        private const float ScatterMaxDistance = 40.0f;

        private const float WeaponMinDistance = 15.0f;
        private const float WeaponMaxDistance = 30.0f;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        public static void InitMap(Scene scene)
        {
            Telemetry.NewGame(scene);

            state = GameplayState.GameRunning;
            CameraShakeManager.instance.Init();

            var transition = GameObject.FindObjectOfType<ProCamera2DTransitionsFX>();
            if (ProCamera2D.Exists)
            {
                ProCamera2D.Instance.IgnoreTimeScale = true;
            }

            if(Debug.isDebugBuild)
            {
                ShowDebugger();
            }

            transition?.TransitionEnter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        public static void DeinitMap(Scene scene)
        {
            CameraShakeManager.instance.Deinit();

            DG.Tweening.DOTween.Clear();

            Telemetry.EndGame(scene);
            AudioExtensions.ClearGameplayLoopings();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ScatterSupply(Transform origin, int count)
        {
            var supplyPickablePrefab = Bootstrap.instance.data.supplyPickablePrefab;
            Debug.Assert(supplyPickablePrefab != null);
            var position = origin.position;

            for (var i = 0; i < count; ++i)
            {
                var supplyPickable = GameObject.Instantiate(supplyPickablePrefab, position, Quaternion.identity).GetComponent<Pickable>();
                supplyPickable.doFloatingAnimAtStart = false;
                supplyPickable.DoRandomJumpAnim(ScatterMinDistance, ScatterMaxDistance, supplyPickable.DoFloatingAnim);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weaponDescriptor"></param>
        /// <param name="position"></param>
        public static void SpawnWeaponPickable(WeaponDescriptor weaponDescriptor, Vector2 position)
        {
            var weaponPickablePrefab = Bootstrap.instance.data.weaponPickablePrefab;
            var weaponPickable = GameObject.Instantiate(weaponPickablePrefab, position, Quaternion.identity).GetComponent<WeaponPickable>();
            weaponPickable.Initialize(weaponDescriptor);
            weaponPickable.DoRandomJumpAnim(WeaponMinDistance, WeaponMaxDistance, () =>
            {
                weaponPickable.isPickable = true;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteAnimation"></param>
        /// <param name="transform"></param>
        /// <param name="childOfTransform"></param>
        /// <returns></returns>
        public static GameObject SpawnFireAndForgetAnimation(SpriteAnimation spriteAnimation, Vector2 position, Quaternion rotation, Transform parent = null)
        {
            var newGameObject = new GameObject();
            newGameObject.transform.position = position;
            newGameObject.transform.rotation = rotation;

            if (parent != null)
            {
                newGameObject.transform.SetParent(parent);
            }

            newGameObject.AddComponent<FireAndForgetAnimation>().spriteAnimation = spriteAnimation;

            return newGameObject;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PauseGame()
        {
            _stateBeforePause = _state;
            state = GameplayState.GamePaused;

            if (_pausePanel == null)
            {
                var canvas = GameObject.FindObjectOfType<Canvas>();
                _pausePanel = GameObject.Instantiate(Bootstrap.instance.data.pausePanelPrefab, Vector2.zero, Quaternion.identity, canvas.transform).GetComponent<UIPausePanel>();
                _pausePanel.GetComponent<UINavigation>().InvalidateAxes();
            }

            foreach (var input in GameObject.FindObjectsOfType<CharacterInput>())
            {
                input.inputLocked = true;
            }

            InputHelper.StopAllVibrations();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UnpauseGame()
        {
            if (_pausePanel != null)
            {
                _pausePanel.Close();
                _pausePanel = null;
            }
            state = _stateBeforePause;

            foreach (var input in GameObject.FindObjectsOfType<CharacterInput>())
            {
                if (!GameMode.instance.isGameOver)
                {
                    input.inputLocked = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void TogglePause()
        {
            if (_pausePanel != null)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneName"></param>
        public static void TransitionToScene(string sceneName)
        {
            var transition = GameObject.FindObjectOfType<ProCamera2DTransitionsFX>();
            ProCamera2D.Instance.IgnoreTimeScale = true;

            if (transition == null)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                return;
            }

            System.Action onTransitionEnded = null;
            onTransitionEnded = () =>
            {
                transition.OnTransitionExitEnded -= onTransitionEnded;
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            };

            transition.OnTransitionExitEnded += onTransitionEnded;
            transition.TransitionExit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldState"></param>
        private static void OnStateChanged(GameplayState oldState)
        {
            switch (_state)
            {
                case GameplayState.GameRunning:
                    Time.timeScale = 1.0f;
                    break;
                case GameplayState.Cutscene:
                    Time.timeScale = 0.0f;
                    break;
                case GameplayState.GamePaused:
                    TweenExtensions.SetCutscenesActive(false);
                    AnimatorExtensions.SetCutscenesPlaying(false);
                    AudioExtensions.SetGameplayLoopingPlaying(false);
                    Time.timeScale = 0.0f;
                    break;
            }

            if(oldState == GameplayState.GamePaused && oldState != _state)
            {
                TweenExtensions.SetCutscenesActive(true);
                AnimatorExtensions.SetCutscenesPlaying(true);
                AudioExtensions.SetGameplayLoopingPlaying(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ShowDebugger()
        {
            var canvas = GameObject.FindObjectOfType<Canvas>();
            if(canvas != null)
            {
                var debuggerObject = new GameObject("Debugger", typeof(Debugger));
                var transform = debuggerObject.GetComponent<RectTransform>();

                transform.localPosition = new Vector2(-230.0f, 160.0f);
                transform.sizeDelta = new Vector2(150.0f, 32.0f);

                transform.SetParent(canvas.transform);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static GameplayState state
        {
            get { return _state; }
            set
            {
                var oldState = _state;
                _state = value;
                OnStateChanged(oldState);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static float gameDeltaTime => Time.deltaTime;

        /// <summary>
        /// 
        /// </summary>
        public static float gameFixedDeltaTime => Time.fixedDeltaTime;

        /// <summary>
        /// 
        /// </summary>
        public static float cutsceneDeltaTime
        {
            get
            {
                if(_state == GameplayState.GamePaused)
                {
                    return 0.0f;
                }
                return Time.unscaledDeltaTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static float cutsceneFixedDeltaTime
        {
            get
            {
                if (_state == GameplayState.GamePaused)
                {
                    return 0.0f;
                }
                return Time.fixedUnscaledDeltaTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static float pauseDeltaTime => Time.unscaledDeltaTime;

        /// <summary>
        /// 
        /// </summary>
        public static float pauseFixedDeltaTime => Time.fixedUnscaledDeltaTime;

        #endregion

        #region Fields

        private static GameplayState _state = GameplayState.GameRunning;
        private static GameplayState _stateBeforePause = _state;

        private static UIPausePanel _pausePanel;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum GameplayState
    {
        GameRunning,
        Cutscene,
        GamePaused,
    }
}