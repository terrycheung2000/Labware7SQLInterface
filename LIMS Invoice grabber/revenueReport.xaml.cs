using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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
using DataTable = System.Data.DataTable;
using Excel = Microsoft.Office.Interop.Excel;
using Window = System.Windows.Window;

namespace LIMS_Invoice_grabber {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class revenueReport : Window {
        // revenuereport default constructor
        public revenueReport() {
            InitializeComponent();
        }
        // alternate constructor of revenue report, sets the given information to the report
        public revenueReport(string companyName, string contactName, string phoneNumber, string email, DataTable testTable, DataTable productsTable) {
            InitializeComponent();
            companyNameBox.Text = companyName;
            contactNameBox.Text = contactName;
            phoneNumberBox.Text = phoneNumber;
            emailbox.Text = email;
            // sums the revenue
            grandTotalBox.Text = string.Format("{0:C2}",testTable.Compute("SUM(TOTAL)", ""));
            if (testTable.Columns.Contains("PREV_TOTAL")) {
                grandTotalPrevBox.Text = string.Format("{0:C2}", testTable.Compute("SUM(PREV_TOTAL)", ""));
            }
            testsGrid.ItemsSource = testTable.DefaultView;
            productsGrid.ItemsSource = productsTable.DefaultView;
        }
        // functionality of save to excel button
        private void toExel_Click(object sender, RoutedEventArgs e) {
            using (System.Windows.Forms.SaveFileDialog saveReport = new System.Windows.Forms.SaveFileDialog()) {
                saveReport.Filter = "CSV (*.csv)|*.csv";
                saveReport.Title = "Save Report to";
                if (saveReport.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    if (saveReport.FileName != "") {
                        // sends the report to csv
                        reportToCSV(((DataView)testsGrid.ItemsSource).ToTable(), ((DataView)productsGrid.ItemsSource).ToTable(), saveReport.FileName);
                    }
                }
            }
        }
        // compiles all data on revenue report to csv
        private void reportToCSV(DataTable table1, DataTable table2, string path) {
            int row = 0;
            StreamWriter writer = null;
            // create streamwriter, checking for existin file path
            try {
                writer = new StreamWriter(path, false);
            } catch (Exception error) {
                MessageBox.Show(error.Message, error.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // sets up report with csv format if path exists
            if (writer != null) {
                writer.WriteLine("Company:," + companyNameBox.Text);
                writer.WriteLine("Contact:," + contactNameBox.Text);
                writer.WriteLine("Phone:," + phoneNumberBox.Text);
                writer.WriteLine("Email:," + emailbox.Text);
                writer.WriteLine("Total Revenue:,\"" + grandTotalBox.Text + "\"");
                writer.WriteLine("Total Revenue Previous:,\"" + grandTotalPrevBox.Text + "\"");
                writer.WriteLine();

                // iterates through headers
                for (int i = 0; i < table1.Columns.Count + table2.Columns.Count + 1; i++) {
                    if (i == table1.Columns.Count) {
                        writer.Write(",");
                    } else if (i < table1.Columns.Count) {
                        writer.Write("\"" + table1.Columns[i].ColumnName + "\"" + ",");
                    } else if (i > table1.Columns.Count) {
                        writer.Write("\"" + table2.Columns[i- table1.Columns.Count - 1].ColumnName + "\"" + ",");
                    }
                }
                writer.WriteLine();
                // iterates through table rows
                while (row < table1.Rows.Count || row < table2.Rows.Count) {
                    for (int i = 0; i < table1.Columns.Count + table2.Columns.Count + 1; i++) {
                        if (row >= table1.Rows.Count && i < table1.Columns.Count) {
                            writer.Write(",");
                        } else if (row >= table2.Rows.Count && i > table1.Columns.Count) {
                            writer.Write(",");
                        } else if (i == table1.Columns.Count) {
                            writer.Write(",");
                        } else if (i < table1.Columns.Count) {
                            writer.Write("\"" + table1.Rows[row][i].ToString() + "\"" + ",");
                        } else if (i > table1.Columns.Count) {
                            writer.Write("\"" + table2.Rows[row][i - table1.Columns.Count - 1].ToString() + "\"" + ",");
                        }
                    }
                    writer.WriteLine();
                    row++;
                }
                writer.Close();
            }
        }
    }
}
