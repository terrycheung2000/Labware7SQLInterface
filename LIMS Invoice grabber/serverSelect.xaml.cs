using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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

namespace LIMS_Invoice_grabber {
    /// <summary>
    /// Interaction logic for serverSelect.xaml
    /// </summary>
    public partial class serverSelect : Window {
        // serverselect default constructor initializes the window
        public serverSelect() {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            serverNameBox.ItemsSource = connectionNames();
            if (connectionNames().Contains("calqa02 - LabWare-7-072419")) {
                serverNameBox.SelectedItem = "calqa02 - LabWare-7-072419";
            }
        }
        // functionality of cancel button
        private void cancelBtn_Click(object sender, RoutedEventArgs e) {
            Owner.Close();
        }
        // functionality of connect button
        private void dbConnect_Click(object sender, RoutedEventArgs e) {
            // sets connection string with given username and password
            string connectionString = ConfigurationManager.ConnectionStrings[serverNameBox.Text.Trim()].ToString();
            connectionString += ";User ID=" + usernameBox.Text;
            connectionString += ";Password=" + passwordBox.Password;
            invoiceGrabber.con = new SqlConnection(connectionString);
            // test connection to sql server
            try {
                invoiceGrabber.con.Open();
                invoiceGrabber.con.Close();
            } catch (Exception exception) {
                MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            invoiceGrabber.searched = false;
            this.Close();
        }
        // gets the restore date and database name of connected server
        public string databaseInfo() {
            string info = "";
            if (invoiceGrabber.con != null) {
                SqlCommand command = new SqlCommand("select top 1 restore_date from msdb.dbo.restorehistory where destination_database_name = '" + invoiceGrabber.con.Database.ToString() + "' order by restore_date desc", invoiceGrabber.con);
                invoiceGrabber.con.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                info += " - Logged into " + invoiceGrabber.con.DataSource + " - " + invoiceGrabber.con.Database;
                info += " as " + invoiceGrabber.con.ConnectionString.Substring(invoiceGrabber.con.ConnectionString.IndexOf("User ID=") + 8).Trim(';');
                info += " : Last Restored " + reader[0].ToString();
                invoiceGrabber.con.Close();
            }
            return info;
        }
        // gets all connection strings from config
        private List<string> connectionNames() {
            List<string> names = new List<string>();
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;
            foreach(ConnectionStringSettings settings in connections) {
                names.Add(settings.Name);
            }
            return names;
        }
    }


}
