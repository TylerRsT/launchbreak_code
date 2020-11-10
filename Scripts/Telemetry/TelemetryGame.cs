using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class TelemetryGame : TelemetryEventContainer
    {
        #region Const

        private const string GameDataServiceName = "gamedata";

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public TelemetryGame(TelemetrySession session)
            : base("Game")
        {
            _session = session;
        }

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal override JSONObject Serialize()
        {
            var jsonObject = base.Serialize();
            jsonObject["sessionGuid"] = _session.guid.ToString();
            jsonObject["gameID"] = _gameID;
            jsonObject["guid"] = _guid.ToString();

            return jsonObject;
        }

#if !UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(disposing)
            {
                var jsonObj = new JSONObject();
                jsonObj["version"] = Telemetry.DataVersion;
                jsonObj["content"] = Serialize();

                var jsonStr = jsonObj.ToString();
                var serviceUrl = $"{Telemetry.ServerEndpoint}/{GameDataServiceName}";
                var request = UnityWebRequest.Put(serviceUrl, jsonStr);
                request.SetRequestHeader("Content-Type", "application/json");
                request.SendWebRequest();
            }
        }
#endif // UNITY_EDITOR

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public static TelemetryGame NewWithGameModeParams(TelemetrySession session)
        {
            var telemetryGame = new TelemetryGame(session);

            telemetryGame.Set("map", GameModeParams.instance.selectedMap);
            telemetryGame.Set("mode", GameModeParams.instance.selectedGameModePrefab.name);
            telemetryGame.Set("playerCount", GameModeParams.instance.playerParams.Count(x => x.isPlaying));
            telemetryGame.Set("rematch", GameModeParams.instance.firstGame ? 0 : 1);

            for(var i = 0; i < GameModeParams.instance.playerParams.Length; ++i)
            {
                var playerParams = GameModeParams.instance.playerParams[i];
                if(!playerParams.isPlaying)
                {
                    continue;
                }

                telemetryGame.Set($"player{i}Char", playerParams.selectedSkin.name);
                telemetryGame.Set($"player{i}Controller", playerParams.controllerIndex);
            }

            Telemetry.FillWithBasicInfo(telemetryGame);

            return telemetryGame;
        }

        #endregion

        #region Fields

        private static uint _gameCounter = 0;

        private TelemetrySession _session;
        private uint _gameID = _gameCounter++;
        private System.Guid _guid = System.Guid.NewGuid();

        #endregion
    }
}