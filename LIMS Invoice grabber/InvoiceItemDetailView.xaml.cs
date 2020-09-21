using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for InvoiceItemDetailView.xaml
    /// </summary>
    public partial class InvoiceItemDetailView : Window {
        public InvoiceItemDetailView() {
            InitializeComponent();
        }

        public InvoiceItemDetailView(object[] array) {
            InitializeComponent();
            sampleNumber.Text = array[0].ToString();
            zProductLot.Text = array[1].ToString();
            description.Text = array[2].ToString();
            invoiceItemNo.Text = array[3].ToString();
            unitPrice.Text = array[4].ToString();
            costItemNo.Text = array[5].ToString();
            quantity.Text = array[6].ToString();
            totalPrice.Text = array[7].ToString();
            itemTotal.Text = array[8].ToString();
            invoiceNumberBox.Text = array[9].ToString();
            billingItemDesc.Text = array[10].ToString();
            invoicedOn.Text = array[11].ToString();
            companyName.Text = array[12].ToString();
            accountNumber.Text = array[13].ToString();
            phoneNum.Text = array[14].ToString();
            zReceiptDist.Text = array[15].ToString();
            contact.Text = array[16].ToString();
            tPoNumber.Text = array[17].ToString();
            zRush.Text = array[18].ToString();
            recdDate.Text = array[19].ToString();
            loginDate.Text = array[20].ToString();
        }
    }
}
