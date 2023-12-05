
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace BetterPlates {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RemotePlatesController : IPlatesController {
        public IPlatesController remote;

        public override Bite GetAttachedBite(Plate plate)
        {
            return remote.GetAttachedBite(plate);
        }

        public override Bite GetBite(string name)
        {
            return remote.GetBite(name);
        }

        public override Bite GetBiteFor(Collider collider)
        {
            return remote.GetBiteFor(collider);
        }

        public override Plate GetPlate(string name)
        {
            return remote.GetPlate(name);
        }

        public override bool IsOccupied(Plate plate)
        {
            return remote.IsOccupied(plate);
        }

        public override void RegisterBite(Bite bite)
        {
            remote.RegisterBite(bite);
        }

        public override void RegisterPlate(Plate plate)
        {
            remote.RegisterPlate(plate);
        }

        public override void TriggerReattachStep2()
        {
            remote.TriggerReattachStep2();
        }
    }
}