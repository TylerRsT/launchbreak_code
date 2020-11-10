using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerParams
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerIndex"></param>
        public PlayerParams(int playerIndex)
        {
            this.playerIndex = playerIndex;
            controllerIndex = playerIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int playerIndex { get; }

        /// <summary>
        /// 
        /// </summary>
        public int controllerIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool isPlaying { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CharacterDescriptor selectedCharacter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CharacterSkinDescriptor selectedSkin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Loadout loadout { get; private set; } = new Loadout();

        /// <summary>
        /// 
        /// </summary>
        public int teamIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float vibrationMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        public int score { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int killCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int deathCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool isAiControlled => isPlaying && controllerIndex < 0;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class GameModeParams
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private GameModeParams()
        {
            for(var i = 0; i < GameConstants.MaxPlayerCount; ++i)
            {
                playerParams[i] = new PlayerParams(i + 1)
                {
                    teamIndex = i + 1,
                };
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static GameModeParams instance => _instance = _instance ?? new GameModeParams();

        /// <summary>
        /// 
        /// </summary>
        public PlayerParams[] playerParams { get; } = new PlayerParams[4];

        /// <summary>
        /// 
        /// </summary>
        public GameObject selectedGameModePrefab { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public string selectedMap { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public bool useRandomMap { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public bool firstGame { get; set; } = false;

        #endregion

        #region Fields

        private static GameModeParams _instance;

        #endregion
    }
}