using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class BuildInfo : ScriptableObject
    {
        #region Const

        private const string FileName = "BuildInfo";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(_commit))
            {
                ParseVersion();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BuildInfo Load()
        {
            var buildInfo = Resources.Load<BuildInfo>(FileName);
            if(buildInfo == null)
            {
                buildInfo = CreateInstance<BuildInfo>();
            }

            buildInfo.ParseVersion();
            return buildInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static BuildInfo FromVersion(string v)
        {
            var buildInfo = CreateInstance<BuildInfo>();
            buildInfo.version = v;

            return buildInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsGreaterThan(BuildInfo other)
        {
            if (_product != other._product) return false;

            if(major > other.major) return true;
            if(major == other.major)
            {
                if (minor > other.minor) return true;
                if (minor == other.minor)
                {
                    if (update > other.update) return true;
                    if (update == other.update)
                    {
                        if (patch > other.patch) return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ParseVersion()
        {
            // Yes, I could use regex!!
            
            var split = _version.Split('.');

            var foundMajor = split[0];
            while(!char.IsNumber(foundMajor[0]))
            {
                foundMajor = foundMajor.Substring(1);
            }

            _major = int.Parse(foundMajor);
            _minor = int.Parse(split[1]);
            _update = int.Parse(split[2]);

            if(split.Length > 3)
            {
                var subSplit = split[3].Split('-');

                _patch = int.Parse(subSplit[0]);
                _product = subSplit[1];
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string version
        {
            get { return _version; }
            set
            {
                _version = value;
                ParseVersion();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string commit
        {
            get => _commit;
            set => _commit = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int major => _major;

        /// <summary>
        /// 
        /// </summary>
        public int minor => _minor;

        /// <summary>
        /// 
        /// </summary>
        public int update => _update;

        /// <summary>
        /// 
        /// </summary>
        public int patch => _patch;

        /// <summary>
        /// 
        /// </summary>
        public string product => _product;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _version = "v0.0.0.0-local";

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _commit = string.Empty;

        private int _major;
        private int _minor;
        private int _update;
        private int _patch;

        private string _product = string.Empty;

        #endregion
    }
}