
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace BetterPlates {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class EndPlateConsumer : UdonSharpBehaviour {
        public Plate plate;
        public override void OnPickupUseDown()
        {
            Debug.Log("pickupuse");
            base.OnPickupUseDown();
            Bite bite = plate.attachedBite;
            if (bite != null) bite.Consume();
            else Debug.Log($"{PlatesController.GetPlateName(plate)} has no attached Bite");
        }
    }
}