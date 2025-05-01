using System;
using UnityEngine;

namespace PlotFourVR
{
    public class NodeColliderWrapper : MonoBehaviour
    {
        public event Action ColliderInteracted;

        public void TriggerCollisionInteraction()
        {
            ColliderInteracted?.Invoke();
        }
    }
}