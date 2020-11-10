using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class SupplyPickable : Pickable
    {
        #region Const

        private const string PickupSoundKey = "Pickup";

        #endregion

        #region Messages

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        protected override void OnPickup(PickupResponse response)
        {
            base.OnPickup(response);

            if(response.character.GetComponent<NoSupplyPickupBuff>() != null)
            {
                return;
            }

            if(response.character.resources.supply >= response.character.resources.maxSupply)
            {
                response.character.resources.SupplyAnim(CharacterResources.SupplyAnimReason.Enough, true);
                return;
            }
            
            this.EmitSound($"{PickupSoundKey}{++response.character.resources.supply}");

            Telemetry.game.Incr("supply_pickup");

            response.Accept();
        }

        #endregion
    }
}