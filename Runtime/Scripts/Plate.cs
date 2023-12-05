using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BetterPlates {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Plate : UdonSharpBehaviour {
        public string uniqueName;
        public Transform attachPoint;
        public IPlatesController controller;
        public int plateType = 1;
        public int[] takeFrom = {
            0,
        };
        public bool occupied {
            get {
                return controller.IsOccupied(this);
            }
        }
        public Bite attachedBite {
            get {
                return controller.GetAttachedBite(this);
            }
        }
        private Bite trg = null;
        public void Start()
        {
            uniqueName = transform.name;
            Transform t = transform.parent;
            while (t != null) {
                uniqueName = t.name + "/" + uniqueName;
                t = t.parent;
            }
            if (controller == null) Debug.LogError("Controller not set");
            controller.RegisterPlate(this);
        }
        public void OnTriggerEnter(Collider other)
        {
            Bite bite = controller.GetBiteFor(other);
            if (bite == null) return;
            if (!Networking.IsOwner(gameObject) && !Networking.IsOwner(bite.attachedPlate.gameObject)) return;
            if (occupied) return;
            if (Networking.IsOwner(bite.gameObject)) bite.Reattach(this);
            else {
                Networking.SetOwner(Networking.LocalPlayer, bite.gameObject);
                trg = bite;
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (trg == null) return;
            Bite bite = controller.GetBiteFor(other);
            if (bite != null && trg == bite) trg = null;
        }
        public void ReattachStep2()
        {
            if(trg != null && Networking.IsOwner(trg.gameObject)) {
                trg.Reattach(this);
                trg = null;
            }
        }

        [ContextMenu("SelfAttach")]
        private void selfattach() {
            attachPoint = transform;
        }
    }
}