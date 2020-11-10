using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class GameBuilder
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public static void Build(AutomationParameters parameters)
        {
            var buildTarget = (BuildTarget)System.Enum.Parse(typeof(BuildTarget), parameters.Get("targetPlatform"));

            var scenesToBuild = new HashSet<string>(EditorBuildSettings.scenes.Select(x => x.path));
            var additionalScenes = parameters.Get("scenesToBuild").Split(';');

            foreach(var scene in additionalScenes)
            {
                scenesToBuild.Add(scene);
            }

            var buildOptions = BuildOptions.None;
            if(parameters.Has("showBuiltPlayer"))
            {
                buildOptions |= BuildOptions.ShowBuiltPlayer;
            }

            var options = new BuildPlayerOptions
            {
                scenes = scenesToBuild.ToArray(),
                target = buildTarget,
                targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget),
                locationPathName = parameters.Get("outputPath"),
                options = buildOptions,
            };

            BuildPipeline.BuildPlayer(options);
        }

        #endregion
    }
}