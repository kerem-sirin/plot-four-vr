using PlotFourVR.Models;
using System;

namespace PlotFourVR.Events
{
    /// <summary>
    /// Handles interaction events for nodes in the game.
    /// </summary>
    public class InteractionEvents
    {
        public event Action<Node> NodeHoverEntered;
        public void InvokeNodeHoverEntered(Node node)
        {
            NodeHoverEntered?.Invoke(node);
        }

        public event Action<Node> NodeHoverExited;
        public void InvokeNodeHoverExited(Node node)
        {
            NodeHoverExited?.Invoke(node);
        }

        public event Action<Node> NodeInteracted;
        public void InvokeNodeInteracted(Node node)
        {
            NodeInteracted?.Invoke(node);
        }

        public event Action<Node> NodeTypeChanged;
        public void InvokeNodeTypeChanged(Node node)
        {
            NodeTypeChanged?.Invoke(node);
        }

        public event Action<Node> WinningNodeDetected;
        public void InvokeWinningNodeDetected(Node node)
        {
            WinningNodeDetected?.Invoke(node);
        }
    }
}