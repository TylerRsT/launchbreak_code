using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class Telemetry
    {
        #region Const

        public const string Preproc = "TELEMETRY";

        public const string DataVersion = "0.1";

#if ENV_PRODUCTION
        public const string ServerEndpoint = "";
#else
        public const string ServerEndpoint = "http://localhost:12344";
#endif // ENV_PRODUCTION

#endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        [Conditional(Preproc)]
        public static void Initialize()
        {
            UnityEngine.Application.wantsToQuit += OnApplicationQuit;
            _session = new TelemetrySession();
            FillWithBasicInfo(session);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        [Conditional(Preproc)]
        public static void NewGame(Scene scene)
        {
            if (scene.name.StartsWith(GameConstants.MapPrefixe))
            {
                session.Incr("gameCount");
                UnityEngine.Debug.Assert(game == null);
                _game = TelemetryGame.NewWithGameModeParams(session);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        [Conditional(Preproc)]
        public static void EndGame(Scene scene)
        {
            if (scene.name.StartsWith(GameConstants.MapPrefixe))
            {
                DisposableHelper.SafeDispose(ref _game);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="telemetryEvent"></param>
        [Conditional(Preproc)]
        public static void FillWithBasicInfo(TelemetryEvent telemetryEvent)
        {
            telemetryEvent.Set("gameVersion", GameConstants.Version);
            telemetryEvent.Set("graphicsCard", SystemInfo.graphicsDeviceName);
            telemetryEvent.Set("graphicsMemory", SystemInfo.graphicsMemorySize);
            telemetryEvent.Set("systemMemory", SystemInfo.systemMemorySize);
            telemetryEvent.Set("operatingSystem", SystemInfo.operatingSystem);
            telemetryEvent.Set("batteryStatus", SystemInfo.batteryStatus.ToString());
            telemetryEvent.Set("datetime", System.DateTime.Now.ToString());
            telemetryEvent.Set("playerID", GetPlayerUniqueIdentifier());

#if PLATFORM_STEAM
            telemetryEvent.Set("platform", "steam");
#elif PLATFORM_ITCH
            telemetryEvent.Set("platform", "itch");
#endif

#if PROJECT_DEMO
            telemetryEvent.Set("demo", 1.0);
#endif // PROJECT_DEMO
        }

        /// <summary>
        /// 
        /// </summary>
        private static bool OnApplicationQuit()
        {
            UnityEngine.Application.wantsToQuit -= OnApplicationQuit;
            DisposableHelper.SafeDispose(ref _game);
            DisposableHelper.SafeDispose(ref _session);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetPlayerUniqueIdentifier()
        {
            if (string.IsNullOrEmpty(_playerID))
            {
#if PLATFORM_STEAM
                _playerID = SteamInterface.GetUserID().ToString();
#else
                _playerID = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault().GetHashCode().ToString();
#endif // PLATFFORM_STEAM
            }

            return _playerID;
        }

#endregion

#region Properties

        /// <summary>
        /// 
        /// </summary>
        public static TelemetrySession session
        {
            get => _session;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static TelemetryGame game
        {
            get => _game;
        }

#endregion

#region Fields

        private static TelemetrySession _session;
        private static TelemetryGame _game;

        private static string _playerID;

#endregion
    }
}