using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class GameConstants
    {
        #region Const

        public const string Version = "0.1.0.0";

        public const float TileSize = 24.0f;
        public const float HalfTileSize = TileSize / 2.0f;

        public const float SceneWidth = 640.0f;
        public const float SceneHeight = 360.0f;

        public const int MaxPlayerCount = 4;

        // Layers
        public const int MaxLightLayer = 7;
        public const int MinLightLayer = MaxLightLayer * -1;

        // Scenes
        public const string OptimalSetupScreenScene = "SCN_OptimalSetupScreen";
        public const string TitleScreenScene = "SCN_TitleScreen";
        public const string MatchSettingsScene = "SCN_MatchSettings";
        public const string SupportScene = "SCN_SupportScreen";
        public const string VictoryScene = "SCN_VictoryScreen";
        public const string MapPrefixe = "MAP_";

        // Camera Tags
        public const string GameplayCameraTag = "MainCamera";
        public const string UICameraTag = "UICamera";

        #endregion
    }
}