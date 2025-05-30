using PlotFourVR.Controllers;
using System;

namespace PlotFourVR.Events
{
    /// <summary>
    /// Handles events related to game settings, such as grid size and opponent type.
    /// </summary>
    public class SettingEvents
    {
        public event Action<int> GridWidthChanged;
        public void InvokeGridWidthChanged(int newWidth)
        {
            GridWidthChanged?.Invoke(newWidth);
        }

        public event Action<int> GridHeightChanged;
        public void InvokeGridHeightChanged(int newHeight)
        {
            GridHeightChanged?.Invoke(newHeight);
        }

        public event Action<int> WinLengthChanged;
        public void InvokeWinLengthChanged(int newWinLength)
        {
            WinLengthChanged?.Invoke(newWinLength);
        }

        public event Action<OpponentType> OpponentTypeChanged;
        public void InvokeOpponentTypeChanged(OpponentType newOpponentType)
        {
            OpponentTypeChanged?.Invoke(newOpponentType);
        }
    }
}