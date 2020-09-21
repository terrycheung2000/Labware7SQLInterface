using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Forms;

namespace LIMS_Invoice_grabber {
    // static class for grabing functionality
    class invoiceGrab {
        public static loadInput loadbar;
        public static BackgroundWorker backgroundWorker2;
        // grab function to take invoices
        public static void grab(DataTable invoiceT, string dest) {
            // initialize and sets backgroundworker
            backgroundWorker2 = new BackgroundWorker();
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerReportsProgress = true;
            List<object> args = new List<object>();
            backgroundWorker2.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker2.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            backgroundWorker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            args.Add(invoiceT);
            args.Add(dest);
            // run the background worker 
            backgroundWorker2.RunWorkerAsync(args);
            loadbar = new loadInput();
            loadbar.Activate();
            while (backgroundWorker2.IsBusy) {
                DialogResult result = loadbar.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.Cancel) {
                    backgroundWorker2.CancelAsync();
                }
                System.Threading.Thread.Sleep(50);
            }
            // end the background worker
            backgroundWorker2.Dispose();
        }
        // backgroundworker functionality; visits existing filepaths and saves to pdf
        private static void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            string path;
            double percent = 0.0;
            List<object> args = (List<object>)e.Argument;
            DataTable table = (DataTable)args[0];
            string dest = (string)args[1];
            if (table != null) {
                using (FileStream zipStream = new FileStream(dest, FileMode.Create)) {
                    // creates zip folder from given path
                    using (ZipArchive zipFile = new ZipArchive(zipStream, ZipArchiveMode.Create)) {
                        // iterates through all rows of the table 
                        foreach (DataRow row in table.Rows) {
                            // changes T_PDF_FILE to caserver.calabs.local
                            path = row["T_PDF_FILE"].ToString();
                            path = path.Replace("\\\\caserver\\", "\\\\caserver.calabs.local\\");
                            path = path.Replace("\\\\192.168.1.5\\", "\\\\caserver.calabs.local\\");
                            path = path.Replace("\\\\192.168.1.40\\", "\\\\caserver.calabs.local\\");
                            // if file exists then add entry to zip folder else prompt user
                            if (System.IO.File.Exists(path)) {
                                FileInfo info = new FileInfo(path);
                                zipFile.CreateEntryFromFile(info.FullName, info.Name);
                            } else {
                                string error = "Invoice #" + row["INVOICE_NUMBER"].ToString() + " could not be found continue?";
                                DialogResult result = System.Windows.Forms.MessageBox.Show(error, "Warning", (MessageBoxButtons)MessageBoxButton.OKCancel, (MessageBoxIcon)MessageBoxImage.Exclamation);
                                if (result == DialogResult.Cancel) {
                                    return;
                                }
                            }
                            // report the completion percent
                            percent += 100.0 / table.Rows.Count;
                            backgroundWorker2.ReportProgress((int)percent);
                        }
                    }
                }
            }
        }
        // called when progress changed
        private static void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            // update the loading bar
            loadbar.progressBar1.Value = e.ProgressPercentage;
        }
        // when the background worker is completed or canceled
        private static void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            // close the loading bar
            loadbar.Close();
        }
        // gets invoices in pdf format unzipped
        public static void grabPDF(DataTable invoiceT, string dest) {
            // initializing and setting backgroundworker 
            backgroundWorker2 = new BackgroundWorker();
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerReportsProgress = true;
            List<object> args = new List<object>();
            backgroundWorker2.DoWork += new DoWorkEventHandler(backgroundWorker1_DoPDFWork);
            backgroundWorker2.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            backgroundWorker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            args.Add(invoiceT);
            args.Add(dest);
            // run the background worker 
            backgroundWorker2.RunWorkerAsync(args);
            loadbar = new loadInput();
            loadbar.Activate();
            while (backgroundWorker2.IsBusy) {
                DialogResult result = loadbar.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.Cancel) {
                    backgroundWorker2.CancelAsync();
                }
                System.Threading.Thread.Sleep(50);
            }
            // end the background worker
            backgroundWorker2.Dispose();
        }
        // alternate functionality of background worker; copies all given invoices and saves as pdf
        private static void backgroundWorker1_DoPDFWork(object sender, DoWorkEventArgs e) {
            string path;
            double percent = 0.0;
            List<object> args = (List<object>)e.Argument;
            DataTable table = (DataTable)args[0];
            string dest = (string)args[1];
            if (table != null) {
                foreach (DataRow row in table.Rows) {
                    // changes T_PDF_FILE to caserver.calabs.local
                    path = row["T_PDF_FILE"].ToString();
                    path = path.Replace("\\\\caserver\\", "\\\\caserver.calabs.local\\");
                    path = path.Replace("\\\\192.168.1.5\\", "\\\\caserver.calabs.local\\");
                    path = path.Replace("\\\\192.168.1.40\\", "\\\\caserver.calabs.local\\");
                    // if file exists then add entry to zip folder else prompt user
                    if (System.IO.File.Exists(path)) {
                        FileInfo info = new FileInfo(path);
                        File.Copy(path, dest + "\\" + info.Name);
                    } else {
                        string error = "Invoice #" + row["INVOICE_NUMBER"].ToString() + " could not be found continue?";
                        DialogResult result = System.Windows.Forms.MessageBox.Show(error, "Warning", (MessageBoxButtons)MessageBoxButton.OKCancel, (MessageBoxIcon)MessageBoxImage.Exclamation);
                        if (result == DialogResult.Cancel) {
                            return;
                        }
                    }
                    // reports completion percentage
                    percent += 100.0 / table.Rows.Count;
                    backgroundWorker2.ReportProgress((int)percent);
                }
            }
        }
    }
}
