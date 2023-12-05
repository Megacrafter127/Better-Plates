
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Data;
using VRC.Udon.Common;
using VRC.SDK3.Rendering;
using VRC.SDK3;
using VRC.SDK3.Image;
using VRC.Economy;
using VRC.SDK3.Platform;
using VRC.SDK3.StringLoading;
using VRC.SDK3.Components.Video;

namespace BetterPlates {
    public abstract class IPlatesController : UdonSharpBehaviour {
        public abstract void RegisterPlate(Plate plate);
        public abstract void RegisterBite(Bite bite);
        public abstract Plate GetPlate(string name);
        public static string GetPlateName(Plate plate)
        {
            if (plate == null) return null;
            return plate.uniqueName;
        }
        public abstract Bite GetBite(string name);
        public static string GetBiteName(Bite bite)
        {
            if (bite == null) return null;
            return bite.uniqueName;
        }
        public abstract bool IsOccupied(Plate plate);
        public abstract Bite GetAttachedBite(Plate plate);
        public abstract Bite GetBiteFor(Collider collider);
        public abstract void TriggerReattachStep2();
    }
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlatesController : IPlatesController {
        private DataDictionary plates = new DataDictionary();
        private DataDictionary bites = new DataDictionary();
        private DataDictionary colliders = new DataDictionary();
        public override void RegisterPlate(Plate plate)
        {
            string plateName = GetPlateName(plate);
            Debug.Log($"Registering {plate} as {plateName}");
            plates.Add(new DataToken(plateName), new DataToken(plate));
        }
        public override void RegisterBite(Bite bite)
        {
            string biteName = GetBiteName(bite);
            Debug.Log($"Registering {bite} as {biteName}");
            bites.Add(new DataToken(biteName), new DataToken(bite));
            foreach(Collider c in bite.colliders) {
                colliders.Add(new DataToken(c), new DataToken(bite));
            }
        }
        public override Plate GetPlate(string name)
        {
            if (name == null || name.Equals("")) return null;
            DataToken tok = plates[new DataToken(name)];
            if (tok.TokenType == TokenType.Error) Debug.LogWarning($"Unknown plate: {name}\n{tok.Error}");
            else return (Plate)tok.Reference;
            return null;
        }
        public override Bite GetBite(string name)
        {
            if (name == null || name.Equals("")) return null;
            DataToken tok = bites[new DataToken(name)];
            if (tok.TokenType == TokenType.Error) Debug.LogWarning($"Unknown bite: {name}\n{tok.Error}");
            else return (Bite)tok.Reference;
            return null;
        }
        public override bool IsOccupied(Plate plate)
        {
            DataList l = bites.GetValues();
            for(int i=0;i<l.Count;i++) {
                DataToken tok = l[i];
                if (tok.TokenType == TokenType.Error) Debug.LogError($"bad index: {i}\n{tok.Error}");
                Bite b = (Bite)tok.Reference;
                if (plate == b.attachedPlate && b.occupying) return true;
            }
            return false;
        }
        public override Bite GetAttachedBite(Plate plate)
        {
            DataList l = bites.GetValues();
            for (int i = 0; i < l.Count; i++) {
                DataToken tok = l[i];
                if (tok.TokenType == TokenType.Error) Debug.LogError($"bad index: {i}\n{tok.Error}");
                Bite b = (Bite)tok.Reference;
                if (plate == b.attachedPlate && b.occupying) return b;
            }
            return null;
        }
        public override Bite GetBiteFor(Collider collider)
        {
            if (collider == null) return null;
            DataToken tok = colliders[new DataToken(collider)];
            if (tok.TokenType == TokenType.Error) return null;
            else return (Bite)tok.Reference;
        }
        public override void TriggerReattachStep2()
        {
            DataList l = plates.GetValues();
            for (int i = 0; i < l.Count; i++) {
                DataToken tok = l[i];
                if (tok.TokenType == TokenType.Error) Debug.LogError($"bad index: {i}\n{tok.Error}");
                Plate plate = (Plate)tok.Reference;
                if (Networking.IsOwner(plate.gameObject)) plate.ReattachStep2();
            }
        }
    }
}