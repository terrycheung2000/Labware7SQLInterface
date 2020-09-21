using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;
using Window = System.Windows.Window;

namespace LIMS_Invoice_grabber {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class invoiceGrabber : Window {
        public static loadInput loadbar;
        public static BackgroundWorker backgroundWorker1;
        static DataTable[] tables = new DataTable[6];
        public static SqlConnection con;
        static SqlCommand[] commands = new SqlCommand[6];
        public static bool searched = false;
        // initialization of the main window, creation of server selection window.
        public invoiceGrabber() {
            InitializeComponent();
            this.Show();
            serverSelect select = new serverSelect();
            select.ShowDialog();
            this.Title += select.databaseInfo();
            initCommands();
        }
        // getting all queries
        private void initCommands() {
            try {
                commands[0] = new SqlCommand(File.ReadAllText(Directory.GetCurrentDirectory() + "\\SQL\\invoicegrab.sql"), con);
                commands[1] = new SqlCommand(File.ReadAllText(Directory.GetCurrentDirectory() + "\\SQL\\invoicepdf.sql"), con);
                commands[2] = new SqlCommand(File.ReadAllText(Directory.GetCurrentDirectory() + "\\SQL\\COAgrab.sql"), con);
                commands[3] = new SqlCommand(File.ReadAllText(Directory.GetCurrentDirectory() + "\\SQL\\missinginvoice.sql"), con);
                commands[4] = new SqlCommand(File.ReadAllText(Directory.GetCurrentDirectory() + "\\SQL\\testsinprogress.sql"), con);
                commands[5] = new SqlCommand(File.ReadAllText(Directory.GetCurrentDirectory() + "\\SQL\\newCustomers.sql"), con);
            } catch (DirectoryNotFoundException dE) {
                System.Windows.MessageBox.Show("SQL Command not found. Please check your directory for SQL folder containing all SQL queries.", dE.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception error) {
                System.Windows.MessageBox.Show(error.Message, error.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // obsolete, queries have moved to flat files ***MISSING NEW CUSTIOMERS STORED FUNCTION***
        //private void initCommands() {
        //    commands[0] = new SqlCommand("SELECT * FROM invoicegrab(@accountnumber, @invoicenumber, @samplenum, @lot, @invoicefrom, @invoiceto)", con);
        //    commands[1] = new SqlCommand("SELECT * FROM invoicepdf(@accountnumber, @invoicenumber, @invoicefrom, @invoiceto)", con);
        //    commands[2] = new SqlCommand("SELECT * FROM COAgrab(@accountnumber, @sentFrom, @sentTo, @samplenum, @lot)", con);
        //    commands[3] = new SqlCommand("SELECT * FROM missinginvoice(@accountnumber, @samplenum, @lot, @changedfrom, @changedto)", con);
        //    commands[4] = new SqlCommand("SELECT * FROM testsInProgress(@accountnumber, @samplenum, @lot, @changedfrom, @changedto)", con);
        //}

        //click functionality of the main search button
        private void search_Click(object sender, RoutedEventArgs e) {
            // only continue when at least 1 input is entered
            if (customer.Text == "" && invoiceNumber.Text == "" && arlot.Text == "" && from.SelectedDate == null && to.SelectedDate == null) {
                System.Windows.Forms.MessageBox.Show("Please enter a field", "Attention", (System.Windows.Forms.MessageBoxButtons)MessageBoxButton.OK, (System.Windows.Forms.MessageBoxIcon)MessageBoxImage.Warning);
                return;
            }
            // create and generate sql queries
            initCommands();
            generateCommands();
            // clear all previous tables 
            foreach (DataTable table in tables) {
                if (table != null) {
                    table.Clear();
                }
            }
            //query visible table
            queryCurrentTable();
        }
        // enteres search filters to sql queries
        private void generateCommands() {
            //initializing sql parameters
            int parse;
            object accounts = DBNull.Value;
            object arOrLot = DBNull.Value;
            object fromDate = DBNull.Value;
            object toDate = DBNull.Value;
            // setting account numbers from customer name
            if (customer.Text != "") {
                if (int.TryParse(customer.Text, out parse)) {
                    accounts = customer.Text.Trim();
                } else if (customer.Text.Contains(",")) {
                    accounts = customer.Text.Trim();
                } else {
                    accounts = getAccountNumbers();
                }
            }
            // setting invoice numbers
            if (invoiceNumber.Text != "") {
                commands[0].Parameters.Add("@invoicenumber", SqlDbType.VarChar, -1).Value = invoiceNumber.Text.Trim();
                commands[1].Parameters.Add("@invoicenumber", SqlDbType.VarChar, -1).Value = invoiceNumber.Text.Trim();
            } else {
                commands[0].Parameters.Add("@invoicenumber", SqlDbType.VarChar, -1).Value = DBNull.Value;
                commands[1].Parameters.Add("@invoicenumber", SqlDbType.VarChar, -1).Value = DBNull.Value;
            }
            // setting ar number and lot grouped together
            if (arlot.Text != "") {
                if (int.TryParse(arlot.Text, out parse)) {
                    arOrLot = arlot.Text.Trim().Replace("'", "''");
                }
            }
            // setting date ranges
            if (from.SelectedDate != null) {
                fromDate = from.SelectedDate;
            }
            if (to.SelectedDate != null) {
                toDate = to.SelectedDate;
            } else if (from.SelectedDate != null) {
                toDate = DateTime.MaxValue;
            }
            // ordering queries
            if ((bool)sortInvoice.IsChecked) {
                commands[0].CommandText += "order by INVOICE_NUMBER, INVOICED_ON, Z_PRODUCT_LOT OPTION(OPTIMIZE FOR UNKNOWN)";
            }
            if ((bool)sortItem.IsChecked) {
                commands[0].CommandText += "order by ACCOUNT_NUMBER, COMPANY_NAME, BILLING_ITEM_DESC OPTION(OPTIMIZE FOR UNKNOWN)";
            }
            commands[1].CommandText += "order by INVOICE_NUMBER, INVOICED_ON OPTION(OPTIMIZE FOR UNKNOWN)";
            commands[2].CommandText += "order by DATE_SENT desc, COMPANY_NAME OPTION(OPTIMIZE FOR UNKNOWN)";
            commands[3].CommandText += "order by SAMPLE_NUMBER OPTION(OPTIMIZE FOR UNKNOWN)";
            commands[4].CommandText += "order by ANALYSIS_TYPE, DUE_DATE desc, REPORTED_NAME OPTION(OPTIMIZE FOR UNKNOWN)";
            commands[5].CommandText += " OPTION(OPTIMIZE FOR UNKNOWN)";
            searched = true;
            // adding parameters to all queries
            foreach (SqlCommand com in commands) {
                com.CommandTimeout = 0;
                com.Parameters.Add("@accountnumber", SqlDbType.VarChar, -1).Value = accounts;
                com.Parameters.Add("@samplenum", SqlDbType.VarChar,-1).Value = arOrLot;
                com.Parameters.Add("@lot", SqlDbType.VarChar, -1).Value = arOrLot;
                com.Parameters.Add("@fromdate", SqlDbType.DateTime, -1).Value = fromDate;
                com.Parameters.Add("@todate", SqlDbType.DateTime, -1).Value = toDate;
            }
        }
        // queries visible table
        private void queryCurrentTable() {
            // initialization of variables and backgroundworker
            int selected = tabs.SelectedIndex;
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerSupportsCancellation = true;
            List<object> args = new List<object>();
            args.Add(con);
            args.Add(commands[selected]);
            args.Add(selected);
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            // run the background worker 
            backgroundWorker1.RunWorkerAsync(args);
            // start the loading bar
            loadbar = new loadInput();
            loadbar.Activate();
            while (backgroundWorker1.IsBusy) {
                DialogResult result = loadbar.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.Cancel) {
                    backgroundWorker1.CancelAsync();
                }
                System.Threading.Thread.Sleep(50);
            }
            backgroundWorker1.Dispose();
            // attaching tables to datagrid
            try {
                switch (selected) {
                    case 0:
                        queryResult.ItemsSource = tables[selected].DefaultView;
                        break;
                    case 1:
                        invoiceGrid.ItemsSource = tables[selected].DefaultView;
                        break;
                    case 2:
                        coaGrid.ItemsSource = tables[selected].DefaultView;
                        break;
                    case 3:
                        missingGrid.ItemsSource = tables[selected].DefaultView;
                        break;
                    case 4:
                        testGrid.ItemsSource = tables[selected].DefaultView;
                        break;
                    case 5:
                        if ((bool)Filter.IsChecked) {
                            tables[selected].DefaultView.RowFilter = "T_BAD_DEBT = 'T'";
                        }
                        customersGrid.ItemsSource = tables[selected].DefaultView;
                        break;
                }
                searched = true;
            } catch (Exception e) {
            }
        }
        // saves all present invoices to zip file
        private void saveAll_Click(object sender, RoutedEventArgs e) {
            using (System.Windows.Forms.SaveFileDialog saveInvoices = new System.Windows.Forms.SaveFileDialog()) {
                saveInvoices.Filter = "Zip File (*.zip)|*.zip";
                saveInvoices.Title = "Save to";
                // opens the excel file chosen 
                if (saveInvoices.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    if (saveInvoices.FileName != "") {
                        invoiceGrab.grab(tables[tabs.SelectedIndex], saveInvoices.FileName);
                    }
                }
            }
        }
        // datagrid row numbers 
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e) {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        // selection changed on tab list
        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // if a query has been searched, then query the visible table
            if (searched == false) {
                return;
            } else {
                if (tables[tabs.SelectedIndex] == null || tables[tabs.SelectedIndex].Rows.Count == 0) {
                    queryCurrentTable();
                }
            }
        }
        // backroungworker functionality runs the sql query
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            List<object> args = e.Argument as List<object>;
            SqlConnection con = (SqlConnection)args[0];
            SqlCommand command = (SqlCommand)args[1];
            int selected = (int)args[2];
            try {
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                // run query for visible table
                switch (selected) {
                    case 0:
                        tables[selected] = new DataTable("Tests");
                        tables[selected].Load(reader);
                        cleanDescription(ref tables[selected]);
                        break;
                    case 1:
                        // using invoice numbers from first page
                        if (tables[0] != null) {
                            if (tables[0].Rows.Count > 0) {
                                List<string> invoices = tables[0].AsEnumerable().Select(s => s.Field<int>("INVOICE_NUMBER").ToString()).Distinct().ToList();
                                command.Parameters["@invoicenumber"].Value = getInvoiceNumbers(invoices);
                                reader.Close();
                                reader = command.ExecuteReader();
                            }
                        }
                        tables[selected] = new DataTable("Invoices");
                        tables[selected].Load(reader);
                        break;
                    case 2:
                        tables[selected] = new DataTable("COAs");
                        tables[selected].Load(reader);
                        break;
                    case 3:
                        tables[selected] = new DataTable("Missing Invoices");
                        tables[selected].Load(reader);
                        break;
                    case 4:
                        tables[selected] = new DataTable("Tests In Progress");
                        tables[selected].Load(reader);
                        break;
                    case 5:
                        tables[selected] = new DataTable("Customers");
                        tables[selected].Load(reader);
                        break;
                }
                reader.Close();
                con.Close();
            // catching connection issues with query
            } catch (Exception error) {
                System.Windows.MessageBox.Show(error.Message, error.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                if (con.State == ConnectionState.Open) {
                    con.Close();
                }
            }
        }
        // when the background worker is complated or canceled
        private static void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            loadbar.Close();
        }
        // cleans description of table
        private void cleanDescription(ref DataTable table) {
            foreach (DataRow row in table.Rows) {
                if (row["BILLING_ITEM_DESC"].ToString().Contains("[")) {
                    row["BILLING_ITEM_DESC"] = row["BILLING_ITEM_DESC"].ToString().Substring(0, row["BILLING_ITEM_DESC"].ToString().IndexOf("[")).Trim();
                }
            }
        }
        // setting invoice numebers to csv
        private string getInvoiceNumbers(List<string> entries) {
            string invoices = "";
            if (entries.Count > 0) {
                for (int i = 0; i < entries.Count - 1; i++) {
                    invoices += entries[i] + ",";
                }
                invoices += entries[entries.Count - 1];
            }
            return invoices;
        }
        // getting account numbers of given customer name as csv
        private string getAccountNumbers() {
            string accountString = "";
            ArrayList accounts = new ArrayList();
            con.Open();
            SqlCommand getaccount = new SqlCommand("select ACCOUNT_NUMBER from ACCOUNT inner join CUSTOMER on CUSTOMER.NAME = ACCOUNT.CUSTOMER where ACCOUNT.CUSTOMER like '%" + customer.Text.ToString().Trim().Replace(' ', '_').Replace("'", "''") + "%' or CUSTOMER.COMPANY_NAME like '%" + customer.Text.ToString().Trim().Replace("'", "''") + "%'", con);
            using (SqlDataReader reader = getaccount.ExecuteReader()) {
                while (reader.Read()) {
                    accounts.Add(reader.GetValue(0));
                }
            }
            con.Close();
            if (accounts.Count > 0) {
                for (int i = 0; i < accounts.Count - 1; i++) {
                    accountString += accounts[i].ToString() + ",";
                }
                accountString += accounts[accounts.Count - 1].ToString();
            }
            return accountString;
        }
        // functionality of radio button sort by invoice number
        private void sortInvoice_Checked(object sender, RoutedEventArgs e) {
            if (this.IsInitialized == true) {
                if (searched == true) {
                    search_Click(sender, e);
                }
                revenueGenBtn.Visibility = Visibility.Hidden;
            }
        }
        // functionality of radio button sort by invoice item
        private void sortItem_Checked(object sender, RoutedEventArgs e) {
            if (searched == true) {
                search_Click(sender, e);
            }
            revenueGenBtn.Visibility = Visibility.Visible;
        }
        // functionality of generate revenue button; creates instance of revenue report window
        private void revenueGenBtn_Click(object sender, RoutedEventArgs e) {
            if (searched == true) {
                List<string> info = getContactString();
                if (info != null) {
                    Window report = new revenueReport(info[0], info[1], info[2], info[3], testPivot(commands[0]), productPivot(commands[0]));
                    report.Show();
                }
            }
        }
        // gets contact information of first account number found
        private List<string> getContactString() {
            List<string> info = new List<string>();
            string account = commands[0].Parameters["@accountnumber"].Value.ToString().Split(',')[0];
            if (account == "") {
                System.Windows.MessageBox.Show("Please enter a company name", "Error", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Error);
                return null;
            }
            con.Open();
            SqlCommand getCompany = new SqlCommand("select CUSTOMER.COMPANY_NAME, CUSTOMER.CONTACT, CUSTOMER.PHONE_NUM, CUSTOMER.Z_RECEIPT_DIST  from ACCOUNT inner join CUSTOMER on CUSTOMER.NAME = ACCOUNT.CUSTOMER where ACCOUNT_NUMBER = '" + account + "' ", con);
            SqlDataReader reader = getCompany.ExecuteReader();
            reader.Read();
            for (int i = 0; i < reader.FieldCount; i++) {
                info.Add(reader.GetValue(i).ToString());
            }
            con.Close();
            return info;
        }
        // runs sql query for test pitvoted table
        private DataTable testPivot(SqlCommand sql) {
            bool dated = true;
            DataTable pivot = new DataTable();
            sql.CommandText = File.ReadAllText(Directory.GetCurrentDirectory()+"\\SQL\\pivotinvoicetests.sql") + " OPTION(OPTIMIZE FOR UNKNOWN)";
            if (sql.Parameters["@fromdate"].Value == DBNull.Value || sql.Parameters["@todate"].Value == DBNull.Value) {
                dated = false;
            }
            con.Open();
            SqlDataReader reader = sql.ExecuteReader();
            pivot.Load(reader);
            if (dated == false) {
                pivot = new DataView(pivot).ToTable(false, new string[] { "TESTS", "TOTAL" });
            }
            con.Close();
            return pivot;
        }
        // runs sql query for product pitvoted table
        private DataTable productPivot(SqlCommand sql) {
            bool dated = true;
            DataTable pivot = new DataTable();
            sql.CommandText = File.ReadAllText(Directory.GetCurrentDirectory() + "\\SQL\\pivotinvoiceproducts.sql") + " OPTION(OPTIMIZE FOR UNKNOWN)";
            if (from.Text == "" || to.Text == "") {
                dated = false;
            }
            con.Open();
            SqlDataReader reader = sql.ExecuteReader();
            pivot.Load(reader);
            if (dated == false) {
                pivot = new DataView(pivot).ToTable(false, new string[] { "PRODUCTS", "TOTAL" });
            }
            con.Close();
            return pivot;
        }
        // functionality of invoice item detail right click
        private void invoiceItemDetail_Click(object sender, RoutedEventArgs e) {
            // if the selected item exists then create instance of invoice item detail window
            if (queryResult.SelectedIndex != -1) {
                if (tables[0].Rows[queryResult.SelectedIndex].ItemArray != null) {
                    InvoiceItemDetailView detail = new InvoiceItemDetailView(tables[0].Rows[queryResult.SelectedIndex].ItemArray);
                    detail.Show();
                }
            }
        }
        // functionality of save selection to zip 
        private void saveSelection_Click(object sender, RoutedEventArgs e) {
            if (invoiceGrid.SelectedItems.Count != 0) {
                using (System.Windows.Forms.SaveFileDialog saveInvoices = new System.Windows.Forms.SaveFileDialog()) {
                    saveInvoices.Filter = "Zip File (*.zip)|*.zip";
                    saveInvoices.Title = "Save Invoices to"; 
                    if (saveInvoices.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                        // adds existing files to zip folder
                        if (saveInvoices.FileName != "") {
                            DataTable selected = tables[tabs.SelectedIndex].Clone();
                            foreach (DataRowView selectedItem in invoiceGrid.SelectedItems) {
                                selected.Rows.Add(selectedItem.Row.ItemArray);
                            }
                            // grab selected file
                            invoiceGrab.grab(selected, saveInvoices.FileName);
                        }
                    }
                }
            }
        }
        // functionality of get sample submission form right click; opens sample submission form of selected sample
        private void sampleSubmission_Click(object sender, RoutedEventArgs e) {
            string numPages;
            // if the selected item exists then go to sample submission folder
            if (queryResult.SelectedIndex != -1) {
                if (tables[0].Rows[queryResult.SelectedIndex].ItemArray != null) {
                    // gets file with the closest creation time
                    FileInfo[] files = new DirectoryInfo("\\\\caserver.calabs.local\\Reports\\Sample submission form\\SUBMISSION FORM\\LIMS").GetFiles("*.pdf").OrderBy(f => f.CreationTime).ToArray();
                    foreach (FileInfo f in files) {
                        Directory.GetLastWriteTime(f.FullName);
                        // opens to estimated page 
                        if (Directory.GetLastWriteTime(f.FullName) > Convert.ToDateTime(tables[0].Rows[queryResult.SelectedIndex]["RECD_DATE"].ToString())) {
                            Process process = new Process();
                            numPages = "/A \"page=" + numberOfPages(f, tables[0].Rows[queryResult.SelectedIndex]["SAMPLE_NUMBER"].ToString()) + "\"";
                            process.StartInfo.Arguments = numPages + " \"" + f.FullName + "\"";
                            process.StartInfo.FileName = "AcroRd32.exe";
                            process.Start();
                            break;
                        }
                    }
                }
            }
        }
        // getting number of pages in a pdf file
        private string numberOfPages(FileInfo filename, string ar) {
            MatchCollection matches;
            int page;
            using (StreamReader sr = new StreamReader(File.OpenRead(filename.FullName))) {
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                matches = regex.Matches(sr.ReadToEnd());
            }
            page = matches.Count - (int.Parse(ar) - int.Parse(filename.Name.Split('-')[0]));
            return page.ToString();
        }
        // sends single table to a csv file
        private void tableToCSV(DataTable table, string path) {
            int row = 0;
            StreamWriter writer = null;
            try {
                writer = new StreamWriter(path, false);
            } catch (Exception error) {
                System.Windows.MessageBox.Show(error.Message, error.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }

            for (int i = 0; i < table.Columns.Count; i++) {
                writer.Write("\"" + table.Columns[i].ColumnName + "\"" + ",");
            }
            writer.WriteLine();

            while (row < table.Rows.Count) {
                for (int i = 0; i < table.Columns.Count; i++) {
                    writer.Write("\"" + table.Rows[row][i].ToString() + "\"" + ",");
                }
                writer.WriteLine();
                row++;
            }
            writer.Close();
        }
        // functionality of print button for single tables
        private void printButton_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult response = MessageBoxResult.Yes;
            using (System.Windows.Forms.SaveFileDialog saveReport = new System.Windows.Forms.SaveFileDialog()) {
                saveReport.Filter = "CSV (*.csv)|*.csv";
                saveReport.Title = "Save Report to";
                // sends the current visible table to csv
                if (tables[tabs.SelectedIndex] != null) {
                    // warns of blank table
                    if (tables[tabs.SelectedIndex].Rows.Count < 1) {
                        response = System.Windows.MessageBox.Show("There is nothing to save. Continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    }
                    if (response == MessageBoxResult.Yes) {
                        if (saveReport.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                            if (saveReport.FileName != "") {
                                tableToCSV(tables[tabs.SelectedIndex], saveReport.FileName);
                            }
                        }
                    }
                }
            }
        }
        // functionality of save invoice as pdf 
        private void getInvoicePDF_Click(object sender, RoutedEventArgs e) {
            if (invoiceGrid.SelectedItems.Count != 0) {
                using (System.Windows.Forms.FolderBrowserDialog saveInvoices = new System.Windows.Forms.FolderBrowserDialog()) {
                    if (saveInvoices.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                        if (saveInvoices.SelectedPath != "") {
                            DataTable selected = tables[1].Clone();
                            foreach (DataRowView selectedItem in invoiceGrid.SelectedItems) {
                                selected.Rows.Add(selectedItem.Row.ItemArray);
                            }
                            // grabs the pdf of the chosen file
                            invoiceGrab.grabPDF(selected, saveInvoices.SelectedPath);
                        }
                    }
                }
            }
        }
        // functionality of save coa as pdf 
        private void getCOAPDF_Click(object sender, RoutedEventArgs e) {
            if (coaGrid.SelectedItems.Count != 0) {
                using (System.Windows.Forms.FolderBrowserDialog saveCOA = new System.Windows.Forms.FolderBrowserDialog()) {
                    if (saveCOA.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                        if (saveCOA.SelectedPath != "") {
                            DataTable selected = tables[2].Clone();
                            foreach (DataRowView selectedItem in coaGrid.SelectedItems) {
                                selected.Rows.Add(selectedItem.Row.ItemArray);
                            }
                            // grabs the pdf of the chosen file
                            invoiceGrab.grabPDF(selected, saveCOA.SelectedPath);
                        }
                    }
                }
            }
        }
        // functionality of admin add dropdown
        private void functionAdd_Click(object sender, RoutedEventArgs e) {
            functionAdder adder = new functionAdder();
            // gets the permissions of the user
            if (adder.getPermissions(con)) {
                // gets the list of sql files
                string[] files = adder.getFiles();
                if (files != null) {
                    // writes the functions if they exist
                    if (files.Count() > 0) {
                        adder.writeFunctions(con, files);
                    } else {
                        System.Windows.MessageBox.Show("No *.sql files were found in selected folder.", "Warning".ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            } 
        }
        // functionality of admin change connection dropdown
        private void changeConnection_Click(object sender, RoutedEventArgs e) {
            // creates new instance of server select
            serverSelect select = new serverSelect();
            select.ShowDialog();
            // updates title of window
            this.Title = "LIMS Invoice Grabber" + select.databaseInfo();
        }
        // functionality of the filter by bad debt checkbox
        private void Filter_Click(object sender, RoutedEventArgs e) {
            if (tables[5] != null) {
                // filters table by bad debt if checked, reverts if unchecked
                if ((bool)Filter.IsChecked) {
                    tables[5].DefaultView.RowFilter = "T_BAD_DEBT = 'T'";
                } else {
                    tables[5].DefaultView.RowFilter = string.Empty;
                }
            }
        }
    }
}
