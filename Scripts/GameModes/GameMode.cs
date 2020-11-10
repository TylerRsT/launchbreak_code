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
    [RequireComponent(typeof(CharacterTransformSorter))]
    public abstract class GameMode : ControllersInfoProvider
    {
        #region Const

        private const string UISubmitInputName = "UI_Submit";
        private const string UIStartInputName = "UI_Start";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override bool hasKeyboardOnly => _joystickNames == null || _joystickNames.All(x => string.IsNullOrEmpty(x));

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Awake()
        {
            Debug.Assert(instance == null);
            instance = this;

            characterTransformSorter = GetComponent<CharacterTransformSorter>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Start()
        {
            _joystickNames = Input.GetJoystickNames();

            characterSpawners.AddRange(GameObject.FindObjectsOfType<CharacterSpawner>());
            Debug.Assert(characterSpawners.Count > 0);

            var hazardManagers = FindObjectsOfType<HazardManager>();
            foreach(var hazardManager in hazardManagers)
            {
                if(!hazardManager.TryInitialize(this))
                {
                    Destroy(hazardManager);
                }
            }

            this.hazardManagers.AddRange(hazardManagers);

            AIBehaviour.easyMode = GameModeParams.instance.playerParams.Any(x => x.isPlaying && !x.isAiControlled);
            InitGame();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (hasGameStarted && !_isGameOver && InputHelper.AnyButtonDown(UIStartInputName))
            {
                GameplayStatics.TogglePause();
            }

            if (!isGameOver && GameplayStatics.state == GameplayState.GamePaused)
            {
                Telemetry.game.Incr("timeInPause", GameplayStatics.pauseDeltaTime);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDestroy()
        {
            instance = null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static GameMode GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>() where T : GameMode
        {
            return instance as T;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void RedeemKey(Character character)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public virtual void OnCharacterSpawned(Character character)
        { }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void InitGame();

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Step();

        /// <summary>
        /// 
        /// </summary>
        protected void TriggerHazardSteps()
        {
            foreach(var hazardManager in hazardManagers)
            {
                hazardManager.OnGameModeStep();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IEnumerator ShowControlsScheme()
        {
            if (GameModeParams.instance.firstGame && GameModeParams.instance.playerParams.Any(x => x.isPlaying && !x.isAiControlled))
            {
                Time.timeScale = 0.0f;
                var panel = UIControlsSchemePanel.OpenControlsSchemePanel();

                var panelTransform = panel.transform;
                panelTransform.position = Vector3.right * 1024.0f;

                yield return new WaitForSecondsRealtime(0.5f);
                panelTransform.DOMoveX(0.0f, 1.0f).SetUpdate(true);

                yield return new WaitUntil(() => panelTransform.position.x == 0.0f);
                panel.canFlip = true;
                yield return new WaitUntil(() => InputHelper.AnyButtonDown(UISubmitInputName));

                panelTransform.DOMoveX(-1024.0f, 1.0f).SetUpdate(true)
                    .onComplete += (() =>
                    {
                        UIControlsSchemePanel.CloseControlsSchemePanel();
                    });

                GameModeParams.instance.firstGame = false;
                Time.timeScale = 1.0f;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static GameMode instance { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CharacterTransformSorter characterTransformSorter { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public abstract int highestScore { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool autoRespawn => _autoRespawn;

        /// <summary>
        /// 
        /// </summary>
        public float respawnDelay => _respawnDelay;

        /// <summary>
        /// 
        /// </summary>
        public bool isGameOver
        {
            get { return _isGameOver; }
            protected set
            {
                _isGameOver = value;
                if(value)
                {
                    GameplayStatics.UnpauseGame();
                }

                Telemetry.game.Set("finishedGame", 1.0);
                Telemetry.game.Set("endGameTime", Time.realtimeSinceStartup);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<CharacterSpawner> characterSpawners { get; } = new List<CharacterSpawner>();

        /// <summary>
        /// 
        /// </summary>
        protected List<HazardManager> hazardManagers { get; } = new List<HazardManager>();

        /// <summary>
        /// 
        /// </summary>
        protected bool hasGameStarted { get; set; }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _autoRespawn = true;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _respawnDelay = 3.0f;

        private bool _isGameOver;

        private List<HazardManager> _hazardManagers = new List<HazardManager>();

        private string[] _joystickNames = null;

        #endregion
    }
}