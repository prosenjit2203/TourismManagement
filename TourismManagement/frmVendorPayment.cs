using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TourismManagement
{
    public partial class frmVendorPayment : Form
    {
        private string vendorID;
        public frmVendorPayment(string id)
        {
            InitializeComponent();
            this.vendorID = id;
        }

        SqlConnection con = new SqlConnection("Data Source=faysal\\sqlexpress01;Initial Catalog=TravelManagement;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
        private void LoadVendorPayments(string vendorId)
        {
            try
            {
                string query = "";

                if (vendorId.StartsWith("VA")) 
                {
                    query = @"
                SELECT 
                    P.PaymentID,
                    P.WalletID,
                    P.BookingID,
                    P.TotalBill,
                    P.VendorAmount,
                    P.TotalBalance,
                    P.PaymentDate
                FROM Vendor_Payment P
                INNER JOIN Booking_Info B ON P.BookingID = B.BookingID
                INNER JOIN Flight_Info F ON B.ServiceID = F.FlightID
                WHERE F.VendorID = @VendorID
                ORDER BY P.PaymentDate DESC;";
                }
                else if (vendorId.StartsWith("VH"))
                {
                    query = @"
                SELECT 
                    P.PaymentID,
                    P.WalletID,
                    P.BookingID,
                    P.TotalBill,
                    P.VendorAmount,
                    P.TotalBalance,
                    P.PaymentDate
                FROM Vendor_Payment P
                INNER JOIN HotelBooking_Info HB ON P.BookingID = HB.BookingID
                INNER JOIN Hotel_Info H ON HB.HotelID = H.HotelID
                WHERE H.VendorID = @VendorID
                ORDER BY P.PaymentDate DESC;";
                }

                if (!string.IsNullOrEmpty(query))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@VendorID", vendorId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;
                    DesignPaymentGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vendor payments: " + ex.Message);
            }
        }

        private void DesignPaymentGrid()
        {
            if (dataGridView1.Columns.Count == 0) return;

          
            dataGridView1.RowTemplate.Height = 30;

           
            dataGridView1.ScrollBars = ScrollBars.Both;


            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.AutoResizeColumns();

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.RowHeadersVisible = false;



            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSteelBlue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.EnableHeadersVisualStyles = false;

           
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkSlateBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

           
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;

            
            if (dataGridView1.Columns.Contains("TotalBill"))
                dataGridView1.Columns["TotalBill"].DefaultCellStyle.Format = "N2";
            if (dataGridView1.Columns.Contains("VendorAmount"))
                dataGridView1.Columns["VendorAmount"].DefaultCellStyle.Format = "N2";
            if (dataGridView1.Columns.Contains("TotalBalance"))
                dataGridView1.Columns["TotalBalance"].DefaultCellStyle.Format = "N2";

            
            if (dataGridView1.Columns.Contains("PaymentDate"))
            {
                dataGridView1.Columns["PaymentDate"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
                dataGridView1.Columns["PaymentDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void frmVendorPayment_Load(object sender, EventArgs e)
        {
            LoadVendorPayments(vendorID);
            DesignPaymentGrid();
        }
    }
}
