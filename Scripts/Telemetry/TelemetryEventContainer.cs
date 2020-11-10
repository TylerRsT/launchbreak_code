using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class TelemetryEventContainer : TelemetryTimedEvent
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        public TelemetryEventContainer(string eventName)
            : base(eventName)
        {

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
            if(events.Count > 0)
            {
                var jsonArray = new JSONArray();
                foreach(var @event in events)
                {
                    jsonArray.Add(@event.Serialize());
                }

                jsonObject[nameof(events)] = jsonArray;
            }

            return jsonObject;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public List<TelemetryEvent> events { get; } = new List<TelemetryEvent>();

        #endregion
    }
}