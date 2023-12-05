
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;

namespace BetterPlates {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Bite : UdonSharpBehaviour {
        public string uniqueName;
        public IPlatesController controller;
        [UdonSynced(UdonSyncMode.None)] private string _attachedPlate = "";
        [UdonSynced(UdonSyncMode.None)] private bool visible = true;
        public Plate origin;
        public Plate attachedPlate {
            get {
                return controller.GetPlate(_attachedPlate);
            }
            set {
                _attachedPlate = PlatesController.GetPlateName(value);
            }
        }
        public Renderer[] renderers = null;
        public Collider[] colliders = null;
        public Plate[] childPlates = { };
        public bool occupying {
            get {
                if (childPlates.Length == 0) return visible;
                for(int i=0;i<childPlates.Length;i++) {
                    Bite b = childPlates[i].attachedBite;
                    if (b != null && b.occupying) return true;
                }
                return false;
            }
        }
        public void Start()
        {
            uniqueName = transform.name;
            Transform t = transform.parent;
            while (t != null) {
                uniqueName = t.name + "/" + uniqueName;
                t = t.parent;
            }
            if (controller == null) Debug.LogError("Controller not set");
            if(origin != null) {
                if (Networking.IsOwner(gameObject)) SendCustomEventDelayedFrames(nameof(Reset), 1);
            }
            controller.RegisterBite(this);
        }
        public void Reattach(Plate to)
        {
            if (!Networking.IsOwner(gameObject)) return;
            if(attachedPlate != null) {
                bool match = false;
                foreach(int t in to.takeFrom) {
                    if(t == attachedPlate.plateType) {
                        match = true;
                        break;
                    }
                }
                Debug.Log($"{to} {(match ? "matches" : "doesn't match")}");
                if (!match) return;
            } else {
                Debug.Log($"{to}({to.plateType}) accepted due to unplaced bite");
            }
            attachedPlate = to;
            Reparent();
            RequestSerialization();
        }
        public override void OnDeserialization()
        {
            base.OnDeserialization();
            Reparent();
            SetVisibility();
        }
        private void Reparent()
        {
            Plate plate = attachedPlate ?? origin;
            transform.SetParent(plate == null ? null  : plate.attachPoint ?? plate.transform, false);
        }
        private void SetVisibility()
        {
            if(renderers != null) foreach(Renderer r in renderers) {
                r.enabled = visible;
            }
            if(colliders != null) foreach(Collider c in colliders) {
                c.enabled = visible;
            }
        }
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if (player == Networking.LocalPlayer) controller.TriggerReattachStep2();
        }
        public void Reset()
        {
            if (!Networking.IsOwner(gameObject)) {
                Debug.Log($"Relaying to owner: Reset");
                SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(Reset));
            } else {
                Debug.Log($"Resetting");
                attachedPlate = origin;
                visible = true;
                Reparent();
                SetVisibility();
                RequestSerialization();
            }
        }
        public void Consume()
        {
            if (!Networking.IsOwner(gameObject)) {
                Debug.Log($"Relaying to owner: Consume");
                SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(Consume));
            } else {
                Debug.Log($"Consuming");
                visible = false;
                _attachedPlate = "";
                Reparent();
                SetVisibility();
                RequestSerialization();
            }
        }

        [ContextMenu("Childrenderers")]
        private void childrenderers() 
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        [ContextMenu("Childcolliders")]
        private void childcolliders() 
        {
            colliders = GetComponentsInChildren<Collider>();
        }

        [ContextMenu("Childall")]
        private void childall()
        {
            childrenderers();
            childcolliders();
        }

        [ContextMenu("Plateparent")]
        private void plateparent() 
        {
            for(Transform t = transform.parent; t != null; t = t.parent) {
                Plate p = t.GetComponent<Plate>();
                if(p != null) {
                    origin = p;
                    break;
                }
            }
        }
        [ContextMenu("Current Child Plates")]
        private void CurrentChildPlates()
        {
            childPlates = GetComponentsInChildren<Plate>();
        }
    }
}