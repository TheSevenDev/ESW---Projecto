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
            //DisplayVersion();
        }

        /*
        private void DisplayVersion()
        {
            var version = string.Format("assembly: {0}", ((System.Reflection.AssemblyFileVersionAttribute)(System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute),false)[0])).Version);

            if (System.DeploymentApplicationDeployment.IsNetworkDeployed)
            {

               System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
               version = string.Format("ClickOnce: {0}", ad.CurrentVersion.ToString());

            }

             Title = version;
        }
        */

     }

}
   