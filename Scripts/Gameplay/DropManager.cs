using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class DropManager : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if(!_isInitialized)
            {
                return;
            }

            _elapsed += GameplayStatics.gameDeltaTime;
            if(_elapsed >= _interval)
            {
                _elapsed = 0.0f;
                Drop();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spawners"></param>
        public void Initialize(CharacterSpawner[] spawners)
        {
            _characterSpawners = spawners;
            _isInitialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerUpToFree"></param>
        public void Free(PowerUpId powerUpToFree)
        {
            var powerUpIndex = _droppedLoots.FindIndex(0, _droppedLoots.Count, x =>
            {
                var powerUpDescriptor = x as PowerUpDescriptor;
                return powerUpDescriptor != null && powerUpDescriptor.powerUpId == powerUpToFree;
            });

            if(powerUpIndex != -1)
            {
                _droppedLoots.RemoveAt(powerUpIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerUpsToFree"></param>
        public void Free(IReadOnlyList<PowerUpId> powerUpsToFree)
        {
            _droppedLoots.RemoveAll(x =>
            {
                var powerUpDescriptor = x as PowerUpDescriptor;
                return powerUpDescriptor != null && powerUpsToFree.Contains(powerUpDescriptor.powerUpId);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void Drop()
        {
            var usableDropBeacons = new List<DropBeacon>(_dropBeacons.Where(x => x.isFreeToDrop));
            if(usableDropBeacons.Count == 0)
            {
                return;
            }

            var random = Random.Range(0, usableDropBeacons.Count);
            var selectedDropBeacon = usableDropBeacons[random];

            var nextLoot = GetNextLoot();
            if(nextLoot == null)
            {
                return;
            }

            selectedDropBeacon.Spawn(nextLoot);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private LootableDescriptor GetNextLoot()
        {
            var lootableItems = _lootDescriptor.lootableItems.ToList();

            lootableItems.RemoveAll(x => _droppedLoots.Contains(x.descriptor));
            if(lootableItems.Count == 0)
            {
                return null;
            }

            var totalOdds = lootableItems.Sum(x => x.odds);
            Debug.Assert(totalOdds > 0.0f);

            var random = Random.Range(0.0f, totalOdds);
            var selectedLoot = lootableItems[0];

            var accumulatedOdds = 0.0f;

            foreach (var lootableItem in lootableItems)
            {
                if (accumulatedOdds >= random)
                {
                    break;
                }
                accumulatedOdds += lootableItem.odds;
                selectedLoot = lootableItem;
            }

            _droppedLoots.Add(selectedLoot.descriptor);

            return selectedLoot.descriptor;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<DropBeacon> _dropBeacons = new List<DropBeacon>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _interval = 30.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _elapsed = 0.0f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private DropBeaconLootDescriptor _lootDescriptor = default;

        private bool _isInitialized = false;

        private CharacterSpawner[] _characterSpawners;
        private List<LootableDescriptor> _droppedLoots = new List<LootableDescriptor>();

        #endregion
    }
}