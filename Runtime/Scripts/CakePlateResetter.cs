
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;

namespace BetterPlates {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CakePlateResetter : UdonSharpBehaviour {
        public Bite[] bites;
        public override void OnPickupUseDown()
        {
            base.OnPickupUseDown();
            Debug.Log($"Resetting");
            foreach (Bite b in bites) {
                b.Reset();
            }
        }

        [ContextMenu("All Children")]
        private void allchildren() {
            bites = GetComponentsInChildren<Bite>();
        }
    }
}