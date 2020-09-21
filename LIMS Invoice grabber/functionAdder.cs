using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace LIMS_Invoice_grabber {
    // sql function adding
    [Obsolete("functions have moved to flat file queries hosted in ./SQL")]
    class functionAdder {
        // functionAdder default constructor 
        public functionAdder() {

        }
        // gets user permissions 
        public bool getPermissions(SqlConnection con) {
            SqlCommand command = new SqlCommand("select * from fn_my_permissions(null, 'DATABASE') where permission_name = 'CREATE FUNCTION'", con);
            con.Open();
            SqlDataReader reader = command.ExecuteReader();
            // notify the user if they can modify database
            if (!reader.HasRows) {
                System.Windows.MessageBox.Show("You do not have permissions to modify the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                con.Close();
                return false;
            } else {
                con.Close();
                return true;
            }
        }
        // gets list of sql files
        public string[] getFiles() {
            string[] files = null;
            FolderBrowserDialog browser = new FolderBrowserDialog();
            if (browser.ShowDialog() == DialogResult.OK) {
                files = Directory.GetFiles(browser.SelectedPath, "*.sql", SearchOption.TopDirectoryOnly);
            }
            return files;
        }
        // writes the functions to the sql server
        public void writeFunctions(SqlConnection con, string[]files) {
            con.Open();
            using (SqlTransaction trans = con.BeginTransaction()) {
                try {
                    // read file into sql command and run
                    using (SqlCommand command = new SqlCommand("", con, trans)) {
                        foreach (string filePath in files) {
                            string script = File.ReadAllText(filePath);
                            command.CommandText = script;
                            try {
                                command.ExecuteNonQuery();
                            } catch (Exception error) {
                                MessageBoxResult result = System.Windows.MessageBox.Show(error.Message + "\nContinue?", error.GetType().ToString(), MessageBoxButton.YesNo, MessageBoxImage.Error);
                                if (result == MessageBoxResult.No) {
                                    trans.Rollback();
                                    con.Close();
                                    System.Windows.MessageBox.Show("Transaction has been rolled back. No changes were made to the database.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }
                            }
                        }
                    }
                    trans.Commit();
                    con.Close();
                // catch if no sql transction was made and rollback 
                } catch (System.InvalidOperationException e) { 
                    if (e.Message == "This SqlTransaction has completed; it is no longer usable.") {
                        System.Windows.MessageBox.Show("No SqlTransaction has been made; No changes were made to the database.", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        con.Close();
                    } else {
                        System.Windows.MessageBox.Show(e.Message, e.GetType().ToString(), MessageBoxButton.OKCancel, MessageBoxImage.Error);
                        trans.Rollback();
                        con.Close();
                    }
                } catch (Exception errors) {
                    System.Windows.MessageBox.Show(errors.Message, errors.GetType().ToString(), MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    trans.Rollback();
                    con.Close();
                }
            }
            System.Windows.MessageBox.Show("Update Sucessful. Database functions added.", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
