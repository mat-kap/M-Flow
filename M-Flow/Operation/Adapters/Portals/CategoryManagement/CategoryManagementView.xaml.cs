using System;
using System.Windows;
using System.Windows.Input;

namespace MFlow.Operation.Adapters.Portals.CategoryManagement
{
    public partial class CategoryManagementView : Window
    {
        public CategoryManagementView()
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