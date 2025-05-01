using System;
using UnityEngine;

namespace PlotFourVR
{
    /// <summary>
    /// Wrapper class for the collider to handle mouse interactions.
    /// </summary>
    public class ColliderWrapper : MonoBehaviour
    {
        public event Action ColliderInteracted;

        public void TriggerCollisionInteraction()
        {
            ColliderInteracted?.Invoke();
        }
    }
}