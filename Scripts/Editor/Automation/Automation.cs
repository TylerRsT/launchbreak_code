using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class Automation
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private static void RunAutomation()
        {
            var commandLineArgs = System.Environment.GetCommandLineArgs();
            var properties = new Dictionary<string, string>();

            for(var i = 0; i < commandLineArgs.Length; ++i)
            {
                if(commandLineArgs[i].StartsWith("-"))
                {
                    if(i < commandLineArgs.Length - 1)
                    {
                        if(commandLineArgs[i + 1].StartsWith("-"))
                        {
                            properties[commandLineArgs[i].TrimStart('-')] = string.Empty;
                        }
                        else
                        {
                            properties[commandLineArgs[i].TrimStart('-')] = commandLineArgs[i + 1];
                            ++i;
                        }
                    }

                    properties[commandLineArgs[i].TrimStart('-')] = string.Empty;
                }
            }

            parameters = new AutomationParameters(properties);

            string automationMethod;
            if (parameters.TryGet(nameof(automationMethod), out automationMethod))
            {
                var split = automationMethod.Split('.');
                var typeName = string.Join(".", split.Take(split.Length - 1).ToArray());
                var methodName = split[split.Length - 1];

                Debug.Log($"Trying to find method '{methodName}' in type '{typeName}'.");

                var type = System.Type.GetType(typeName);
                var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                var methodParameters = method.GetParameters();
                if(methodParameters.Length == 0)
                {
                    method.Invoke(null, null);
                }
                else if(methodParameters.Length == 1 && methodParameters[0].ParameterType == typeof(AutomationParameters))
                {
                    method.Invoke(null, new object[] { parameters });
                }
                else
                {
                    Debug.LogError($"Could not invoke '{automationMethod}' due to unrecognized signature.");
                }
            }
            else
            {
                Debug.LogWarning("No Automation Method to execute...");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static AutomationParameters parameters { get; private set; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class AutomationParameters
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        public AutomationParameters(IReadOnlyDictionary<string, string> properties)
        {
            _properties = properties;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Has(string key)
        {
            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string Get(string key, string defaultValue = "")
        {
            string value;
            if(TryGet(key, out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(string key, out string value)
        {
            return _properties.TryGetValue(key, out value);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private IReadOnlyDictionary<string, string> _properties;

        #endregion
    }
}