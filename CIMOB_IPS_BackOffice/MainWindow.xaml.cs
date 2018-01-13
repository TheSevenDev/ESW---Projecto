using CIMOB_IPS_BackOffice.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CIMOB_IPS_BackOffice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnProgramType_Click(object sender, RoutedEventArgs e)
        {
            ProgramTypesWindow programTypesDialog = new ProgramTypesWindow();
            programTypesDialog.Show();
        }

        private void BtnTechnicians_Click(object sender, RoutedEventArgs e)
        {
            TechniciansWindow techniciansDialog = new TechniciansWindow();
            techniciansDialog.Show();
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpDialog = new HelpWindow();
            helpDialog.Show();
        }
    }
}
