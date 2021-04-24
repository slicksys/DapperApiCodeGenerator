using System.Windows.Controls;
using System.Windows.Input;

namespace CodeGenerator
{
    /// <summary>
    /// Interaction logic for DAL.xaml
    /// </summary>
    public partial class DalView : UserControl
    {
        public DalView()
        {
            InitializeComponent();
        }

        public void GenerateAllButton1_OnClick()
        {
            doit();
        }

        public Button GenerateAllButton1
        {
            get { return GenerateAllButton; }
            set { GenerateAllButton = value; }
        }

        private void doit()
        {
            var x =this.ProcedureNames.SelectedItems;
        }

        private void ProcedureNames_KeyDown(System.Object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.A))
            {
                foreach (StoredProcedure obj in ProcedureNames.ItemsSource)
                {
                    obj.IsChecked = !obj.IsChecked;
                }
            }
        }
    }
}
