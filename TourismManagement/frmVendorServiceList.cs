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
    public partial class frmVendorServiceList : Form
    {
        private string vendorId;
        SqlConnection con = new SqlConnection("Data Source=faysal\\sqlexpress01;Initial Catalog=TravelManagement;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
        public frmVendorServiceList(string id)
        {
            InitializeComponent();
            this.vendorId = id;
        }

        private void frmVendorServiceList_Load(object sender, EventArgs e)
        {
            string vendorID = vendorId; 
            string query = "";

            if (vendorID.StartsWith("VA"))
            {
                query = @"SELECT FlightID, FromCity, ToCity, FlightDate, Time, Seat, Fare
                  FROM Flight_Info
                  WHERE VendorID = @VendorID";
            }
            else if (vendorID.StartsWith("VH"))
            {
                query = @"SELECT HotelID, HotelName, Country, City, TotalRooms, AvailableRooms
                  FROM Hotel_Info
                  WHERE VendorID = @VendorID";
            }
            else
            {
                MessageBox.Show("Invalid Vendor ID.");
                return;
            }

            try
            {
                
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@VendorID", vendorID);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);


                    dataGridView1.DataSource = dt;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    dataGridView1.AutoResizeColumns();

                    dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AllowUserToDeleteRows = false;
                    dataGridView1.RowHeadersVisible = false;

                   
                    dataGridView1.ScrollBars = ScrollBars.Both;

                    dataGridView1.BackgroundColor = Color.Honeydew;
                    dataGridView1.DefaultCellStyle.BackColor = Color.Honeydew;
                    dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.MintCream;
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightGreen;
                    dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

                    dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
