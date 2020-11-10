using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class VictoryScreen : MonoBehaviour
    {
        #region Const

        private const string VictorySoundKey = "Victory";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            StartCoroutine(StartVictoryScreen());
            _continueCaption.SetActive(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void WaitForDisplayRematchPanel()
        {
            if (_endGamePanelInstance == null)
            {
                _continueCaption.SetActive(true);
                StartCoroutine(DisplayRematchPanelCoroutine());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartVictoryScreen()
        {
            yield return new WaitForSeconds(0.7f);

            var players = GameModeParams.instance.playerParams.Where(x => x.isPlaying).ToArray();

            var orderedPlayers = players.OrderByDescending(x => x.score).ThenByDescending(x => x.killCount - x.deathCount).ToArray();
            var podium = new List<PlayerParams>[]
            {
                new List<PlayerParams>(),
                new List<PlayerParams>(),
                new List<PlayerParams>(),
            };

            var currentIndex = 0;
            for (var i = 0; i < orderedPlayers.Length; ++i)
            {
                if (i > 0 && currentIndex < podium.Length - 1)
                {
                    if ((orderedPlayers[i].score < orderedPlayers[i - 1].score) ||
                        (orderedPlayers[i].killCount - orderedPlayers[i].deathCount < orderedPlayers[i - 1].killCount - orderedPlayers[i - 1].deathCount))
                    {
                        ++currentIndex;
                    }
                }

                podium[currentIndex].Add(orderedPlayers[i]);
            }

            foreach(var player in orderedPlayers)
            {
                var playerStatsPanel = Instantiate(_playerStatsPanelPrefab, new Vector2(500, -8), Quaternion.identity, FindObjectOfType<Canvas>().transform)
                    .GetComponent<PlayerStats>();

                _playerStatsPanels.Add(playerStatsPanel);
                playerStatsPanel.playerID = player.playerIndex - 1;

                var backgroundSprite = playerStatsPanel.playerThirdPlaceSprite;
                if(podium[0].Contains(player))
                {
                    backgroundSprite = playerStatsPanel.playerFirstPlaceSprite;
                    _goldPlayers.Add(playerStatsPanel);
                }
                else if(podium[1].Contains(player))
                {
                    backgroundSprite = playerStatsPanel.playerSecondPlaceSprite;
                    _silverPlayers.Add(playerStatsPanel);
                }

                playerStatsPanel.GetComponent<Image>().sprite = backgroundSprite;
            }

            Sequence sequence = DOTween.Sequence();

            for(var i = 0; i < _playerStatsPanels.Count; i++)
            {
                if (i == 0)
                {
                    _currentPos = new Vector2(transform.position.x - _panelAdjustValue * (players.Length - 1), 0);
                }
                else
                {
                    _currentPos = new Vector2(_currentPos.x + _panelSpacing + _panelWidth, 0);
                }

                sequence.Append(_playerStatsPanels[i].transform.DOMoveX(_currentPos.x, 0.3f));
            }

            yield return sequence.WaitForCompletion();

            yield return new WaitForSeconds(0.3f);

            sequence = DOTween.Sequence();

            foreach (var goldPlayer in _goldPlayers)
            {
                sequence.Append(goldPlayer.transform.DOMoveY(4f, 0.3f));
            }
            foreach (var silverPlayer in _silverPlayers)
            {
                sequence.Append(silverPlayer.transform.DOMoveY(1f, 0.3f));
            }

            yield return sequence.WaitForCompletion();

            _continueCaption.SetActive(true);

            yield return DisplayRematchPanelCoroutine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator DisplayRematchPanelCoroutine()
        {
            yield return 0;

            yield return new WaitUntil(() => Input.anyKeyDown);

            var canvasTransform = FindObjectOfType<Canvas>().transform;

            yield return 0;

            _endGamePanelInstance = Instantiate(Bootstrap.instance.data.rematchPanelPrefab, Vector3.zero, Quaternion.identity, canvasTransform).GetComponent<UINavigation>();
            _endGamePanelInstance?.InvalidateAxes();
            _endGamePanelInstance?.Focus();

            _continueCaption.SetActive(false);
            _endGamePanelInstance = null;

            yield break;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public GameObject continueCaption => _continueCaption;

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _playerStatsPanelPrefab = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _continueCaption = default;

        private List<PlayerStats> _goldPlayers = new List<PlayerStats>();
        private List<PlayerStats> _silverPlayers = new List<PlayerStats>();

        private Vector2 _currentPos = new Vector2(0,0);
        private float _panelSpacing = 16f;
        private float _panelWidth = 128f;
        private float _panelAdjustValue = 72f;
        private List<PlayerStats> _playerStatsPanels = new List<PlayerStats>();
        private UINavigation _endGamePanelInstance;

        #endregion
    }
}
