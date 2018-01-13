using CIMOB_IPS_BackOffice.Models;
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
using System.Windows.Shapes;

namespace CIMOB_IPS_BackOffice.Windows
{
    /// <summary>
    /// Interaction logic for AddProgramTypeWindow.xaml
    /// </summary>
    public partial class ManageProgramTypeWindow : Window
    {
        public ProgramType ProgramType { get; set; }

        public ManageProgramTypeWindow(ProgramType programType)
        {
            InitializeComponent();

            this.ProgramType = programType;

            if (ProgramType != null)
            {
                LblTitle.Content = "Editar";
                UpdateFields();
            }
            else
            {
                LblTitle.Content = "Adicionar";
                ProgramType = new ProgramType();
            }
        }

        private void UpdateFields()
        {
            TxtName.Text = ProgramType.Name;
            TxtDesc.Text = ProgramType.Description;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(TxtName.Text.Length != 0 && TxtName.Text.Length != 0)
            {
                ProgramType.Name = TxtName.Text;
                ProgramType.Description = TxtDesc.Text;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Por favor preencha os campos todos.", "Erro");
            }
        }
    }
}
