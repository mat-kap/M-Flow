using System;
using System.Windows;
using System.Windows.Input;

namespace MFlow.Operation.Adapters.Portals.DayPlanning
{
    public partial class DayPlanningView : Window
    {
        public DayPlanningView()
        {
            InitializeComponent();
        }
        
        public event Action OnReturnPressed;

        void NameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            
            OnReturnPressed?.Invoke();
        }
    }
}