using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class TelemetryEvent
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        public TelemetryEvent(string eventName)
        {
            name = eventName;
            startTime = Time.realtimeSinceStartup;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public float startTime { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [Conditional(Telemetry.Preproc)]
        public void Set(string name, string value)
        {
            ReplaceName(ref name);
            _stringValues[name] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [Conditional(Telemetry.Preproc)]
        public void Set(string name, double value)
        {
            ReplaceName(ref name);
            _numberValues[name] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [Conditional(Telemetry.Preproc)]
        public void Incr(string name, double value = 1.0)
        {
            ReplaceName(ref name);

            double originalValue;
            if (_numberValues.TryGetValue(name, out originalValue))
            {
                value += originalValue;
            }

            _numberValues[name] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal virtual JSONObject Serialize()
        {
            var jsonObject = new JSONObject();
            jsonObject[nameof(name)] = name;
            jsonObject[nameof(startTime)] = startTime;

            foreach(var numberValue in _numberValues)
            {
                jsonObject[numberValue.Key] = numberValue.Value;
            }

            foreach(var stringValue in _stringValues)
            {
                jsonObject[stringValue.Key] = stringValue.Value;
            }

            return jsonObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        private void ReplaceName(ref string name)
        {
            name = name.Replace('.', '_');
            name = name.Replace(' ', '_');
        }

        #endregion

        #region Fields

        private Dictionary<string, double> _numberValues = new Dictionary<string, double>();

        private Dictionary<string, string> _stringValues = new Dictionary<string, string>();

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class TelemetryTimedEvent : TelemetryEvent, System.IDisposable
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        public TelemetryTimedEvent(string eventName)
            : base(eventName)
        { }

        #endregion

        #region Destructor

        /// <summary>
        /// 
        /// </summary>
        ~TelemetryTimedEvent()
        {
            Dispose(false);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            endTime = Time.realtimeSinceStartup;
            Dispose(true);
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
            jsonObject[nameof(endTime)] = endTime;

            return jsonObject;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float endTime { get; private set; }

        #endregion
    }
}