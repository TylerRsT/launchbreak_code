using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _characterNameText.text = GameModeParams.instance.playerParams[_playerID].selectedCharacter.name;
            _characterImage.sprite = GameModeParams.instance.playerParams[_playerID].selectedSkin.headSprite;
            _keyCountText.text = ($"Keys: " + GameModeParams.instance.playerParams[_playerID].score.ToString());
            _killCountText.text = ($"Kills: " + GameModeParams.instance.playerParams[_playerID].killCount.ToString());
            _deathCountText.text = ($"Deaths: " + GameModeParams.instance.playerParams[_playerID].deathCount.ToString());
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int playerID { get => _playerID; set => _playerID = value; }

        /// <summary>
        /// 
        /// </summary>
        public Sprite playerFirstPlaceSprite { get => _playerFirstPlaceSprite; set => _playerFirstPlaceSprite = value; }

        /// <summary>
        /// 
        /// </summary>
        public Sprite playerSecondPlaceSprite { get => _playerSecondPlaceSprite; set => _playerSecondPlaceSprite = value; }

        /// <summary>
        /// 
        /// </summary>
        public Sprite playerThirdPlaceSprite { get => _playerThirdPlaceSprite; set => _playerThirdPlaceSprite = value; }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _playerFirstPlaceSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _playerSecondPlaceSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Sprite _playerThirdPlaceSprite = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _characterNameText = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Image _characterImage = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _keyCountText = default;


        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _killCountText = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _deathCountText = default;

        private int _playerID;

        #endregion
    }
}
