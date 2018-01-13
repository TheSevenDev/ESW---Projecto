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
    /// Interaction logic for ManageHelp.xaml
    /// </summary>
    public partial class ManageHelp : Window
    {
        public Help Help { get; set; }

        public ManageHelp(Help help)
        {
            InitializeComponent();

            Help = help;

            if (Help != null)
            {
                UpdateFields();
            }
            else
            {
                Help = new Help();
            }

            InitEvents();
        }

        private void UpdateFields()
        {
            TxtController.Text = Help.Controller;
            TxtAction.Text = Help.Action;
        }

        void InitEvents()
        {
            Editor.DocumentReady += new RoutedEventHandler(Editor_DocumentReady);
            GetHtmlButton.Click += new RoutedEventHandler(GetHtmlButton_Click);
        }

        void Editor_DocumentReady(object sender, RoutedEventArgs e)
        {
            if (Help != null)
                Editor.ContentHtml = Help.HTML;
            else
                Editor.ContentHtml = "<p>Insira aqui o texto pretendido</p>";
        }

        void GetHtmlButton_Click(object sender, RoutedEventArgs e)
        {
            if (TxtAction.Text.Length != 0 && TxtController.Text.Length != 0)
            {
                Help.Controller = TxtController.Text;
                Help.Action = TxtAction.Text;
                Help.HTML = Editor.ContentHtml;
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
