using System.Windows;
using System.Windows.Input;

namespace MFlow.Operation.Adapters.Portals
{
    public partial class InputView : Window
    {
        bool _UseValue;
        
        InputView()
        {
            InitializeComponent();
        }

        public static string Show(string title, string caption, string defaultValue)
        {
            var dlg = new InputView
            {
                Owner = Application.Current.MainWindow, 
                Title = title, 
                Caption = { Text = caption }, 
                Input = {Text = defaultValue}
            };

            dlg.ShowDialog();

            return dlg._UseValue ? dlg.Input.Text : string.Empty;
        }

        void InputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            
            _UseValue = true;
            Close();
        }

        void Ok_Click(object sender, RoutedEventArgs e)
        {
            _UseValue = true;
            Close();
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}