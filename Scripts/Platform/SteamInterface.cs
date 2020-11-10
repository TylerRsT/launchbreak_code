using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#if PLATFORM_STEAM

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class SteamInterface
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ulong GetUserID()
        {
            TryInitialize();
            if (_initialized)
            {
                return SteamUser.GetSteamID().m_SteamID;
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            TryInitialize();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void TryInitialize()
        {
            if (!_initialized && SteamAPI.Init())
            {
                _initialized = true;
                Debug.Log($"Steam ID : {GetUserID()}");
                Application.quitting += SteamAPI.Shutdown;
            }
        }

        #endregion

        #region Fields

        private static bool _initialized;

        #endregion
    }
}

#endif // PLATFORM_STEAM