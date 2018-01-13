using CIMOB_IPS_BackOffice.Models;
using CIMOB_IPS_BackOffice.Windows;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Xaml;

namespace CIMOB_IPS_BackOffice
{
    /// <summary>
    /// Interaction logic for ProgramTypesWindow.xaml
    /// </summary>
    public partial class ProgramTypesWindow : Window
    {
        string connstr = Utility.GetConnectionString();

        private ProgramType SelectedProgramType { get; set; }
        private List<ProgramType> ProgramTypeList { get; set; }

        public ProgramTypesWindow()
        {
            ProgramTypeList = new List<ProgramType>();

            InitializeComponent();
        }

        private void LbProgramTypes_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshProgramTypes();
        }

        private void RefreshProgramTypes()
        {
            LbProgramTypes.Items.Clear();
            ProgramTypeList.Clear();

            SqlConnection scnConn = new SqlConnection(connstr);

            string strQuery = "SELECT id_program_type, name, description FROM Program_Type";

            SqlCommand scmCommand = new SqlCommand(strQuery, scnConn);

            try
            {
                scnConn.Open();

                SqlDataReader dtrReader = scmCommand.ExecuteReader();

                while (dtrReader.Read())
                {
                    ProgramTypeList.Add(new ProgramType
                    {
                        ProgramTypeId = dtrReader.GetInt64(0),
                        Name = dtrReader.GetString(1),
                        Description = dtrReader.GetString(2)
                    });
                }

                foreach (ProgramType pt in ProgramTypeList)
                {
                    LbProgramTypes.Items.Add(pt);
                }

                SelectedProgramType = ProgramTypeList.First();
                UpdateFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                scnConn.Close();
            }
        }

        private void UpdateFields()
        {
            TxtName.Text = SelectedProgramType.Name;
            TxtDescription.Text = SelectedProgramType.Description;

            LbProgramTypes.SelectedItem = SelectedProgramType;
        }

        private void LbProgramTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LbProgramTypes.Items.CurrentItem != null)
            {
                SelectedProgramType = LbProgramTypes.Items.CurrentItem as ProgramType;
                UpdateFields();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ManageProgramTypeWindow manageProgramTypeWindow = new ManageProgramTypeWindow(null);
            if (manageProgramTypeWindow.ShowDialog() == true)
            {
                ProgramType newProgramType = manageProgramTypeWindow.ProgramType;

                AddProgramType(newProgramType);
                SelectedProgramType = newProgramType;
                UpdateFields();
            }
        }

        private void LbProgramTypes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ManageProgramTypeWindow manageProgramTypeWindow = new ManageProgramTypeWindow(SelectedProgramType);

            if (manageProgramTypeWindow.ShowDialog() == true)
            {
                EditProgramType(SelectedProgramType);
                LbProgramTypes.Items.Refresh(); 
                UpdateFields();
            }
        }

        private void AddProgramType(ProgramType programType)
        {
            SqlConnection scnConn = new SqlConnection(connstr);

            string strQuery = "INSERT INTO Program_Type VALUES(@Name, @Description, null)";

            SqlCommand scmCommand = new SqlCommand(strQuery, scnConn);
            scmCommand.Parameters.AddWithValue("@Name", programType.Name);
            scmCommand.Parameters.AddWithValue("@Description", programType.Description);

            try
            {
                scnConn.Open();
                scmCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                scnConn.Close();
                RefreshProgramTypes();
            }
        }

        private void EditProgramType(ProgramType programType)
        {
            SqlConnection scnConn = new SqlConnection(connstr);

            string strQuery = "UPDATE Program_Type SET Name = @Name, Description = @Description WHERE id_program_type = @id";

            SqlCommand scmCommand = new SqlCommand(strQuery, scnConn);
            scmCommand.Parameters.AddWithValue("@id", programType.ProgramTypeId);
            scmCommand.Parameters.AddWithValue("@Name", programType.Name);
            scmCommand.Parameters.AddWithValue("@Description", programType.Description);

            try
            {
                scnConn.Open();
                scmCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                scnConn.Close();
                RefreshProgramTypes();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Tem a certeza que pretende remover?", "Confirmação de Remoção", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SqlConnection scnConn = new SqlConnection(connstr);

                string strQuery = "DELETE FROM Program_Type WHERE id_program_type = @id";

                SqlCommand scmCommand = new SqlCommand(strQuery, scnConn);
                scmCommand.Parameters.AddWithValue("@id", SelectedProgramType.ProgramTypeId);

                try
                {
                    scnConn.Open();
                    scmCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    scnConn.Close();
                    RefreshProgramTypes();
                }
            }
        }
    }
}
