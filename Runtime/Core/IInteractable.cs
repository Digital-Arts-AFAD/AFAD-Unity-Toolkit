using UnityEngine;

namespace DigitalArtsAFAD
{
    public interface IInteractable
    {
        public string GetMessage();
        public void Activate();
        public void Deactivate();
        public bool IsActive();
        public void Use(PlayerBase caller);

        public float GetOverrideDistance();

        public Vector3 GetPosition();

        public Sprite GetCursor();
    }
}