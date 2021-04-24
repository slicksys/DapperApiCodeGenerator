using System.Windows.Forms;
using MahApps.Metro.Controls;

namespace CodeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            //ProcedureLiCommand = new RelayCommand(delegate { CurrentView = _procView; });
            //SqlCommand = new RelayCommand(param => DisplayMessage());
            InitializeComponent();
            var s = System.Windows.Forms.Screen.AllScreens[1];

            System.Drawing.Rectangle r = s.WorkingArea;
            this.Top = r.Top;
            this.Left = r.Left;


           
        }
    }
}

