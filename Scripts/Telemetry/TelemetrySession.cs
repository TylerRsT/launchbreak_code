using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class TelemetrySession : TelemetryEventContainer
    {
        #region Const

        private const string SessionDataServiceName = "sessiondata";

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public TelemetrySession()
            : base("Session")
        { }

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal override JSONObject Serialize()
        {
            var jsonObject = base.Serialize();
            jsonObject[nameof(guid)] = guid.ToString();

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

            if (disposing)
            {
                var jsonObj = new JSONObject();
                jsonObj["version"] = Telemetry.DataVersion;
                jsonObj["content"] = Serialize();

                var jsonStr = jsonObj.ToString();
                var serviceUrl = $"{Telemetry.ServerEndpoint}/{SessionDataServiceName}";
                var request = UnityWebRequest.Put(serviceUrl, jsonStr);
                request.SetRequestHeader("Content-Type", "application/json");
                request.SendWebRequest();
            }
        }
#endif // UNITY_EDITOR

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public System.Guid guid { get; } = System.Guid.NewGuid();

        #endregion
    }
}