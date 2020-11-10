using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class ItchBuildInfoManager : MonoBehaviour
    {
        #region Const

        private const string OnNewerVersionFoundMethodName = "OnNewerVersionFound";
        private const string ItchUrl = "https://badrezgames.itch.io/launch-break";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            StartCoroutine(CheckLatestBuild());
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckLatestBuild()
        {
            const string versionText = "<span class=\"version_name\">Version ";
            const string endVersionEntry = "</span>";

            var request = UnityWebRequest.Get(ItchUrl);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();
            while (!request.downloadHandler.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                yield break;
            }

            string htmlContent = request.downloadHandler.text;

            var split = htmlContent.Split(new string[] { versionText }, System.StringSplitOptions.RemoveEmptyEntries);
            split = split.Skip(1).ToArray();

            var foundVersions = new List<string>();
            foreach(var entry in split)
            {
                var version = entry.Split(new string[] { endVersionEntry }, System.StringSplitOptions.RemoveEmptyEntries)
                    .First();
                foundVersions.Add(version);
            }

            var currentBuildInfo = BuildInfo.Load();

            foreach(var version in foundVersions)
            {
                var buildInfo = BuildInfo.FromVersion(version);
                if(currentBuildInfo.product != buildInfo.product)
                {
                    continue;
                }

                if(buildInfo.IsGreaterThan(currentBuildInfo))
                {
                    SendMessage(OnNewerVersionFoundMethodName, buildInfo, SendMessageOptions.DontRequireReceiver);
                    Debug.Log($"Newer version found on Itch : {buildInfo.version}");
                    yield break;
                }
            }

            Debug.Log("No new build found on Itch.");
        }

        #endregion
    }
}