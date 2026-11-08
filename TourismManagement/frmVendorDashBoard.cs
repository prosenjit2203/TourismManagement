using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TourismManagement
{
    public partial class frmVendorDashBoard : Form
    {
        private string vendorId;
        SqlConnection con = new SqlConnection("Data Source=faysal\\sqlexpress01;Initial Catalog=TravelManagement;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
        public frmVendorDashBoard(string id)
        {
            InitializeComponent();
            this.vendorId = id;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadForm(new frmVendorServiceList(vendorId));
        }

        public void ShowName(string id)
        {


            con.Open();
            SqlCommand cmd = new SqlCommand("Select Name, Picture from Vendor_Info Where VendorID='" + id + "'", con);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
             
                label4.Text = reader["Name"].ToString();

                
                if (!(reader["Picture"] is DBNull))
                {
                    byte[] pictureData = (byte[])reader["Picture"];
                    if (pictureData.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream(pictureData))
                        {
                            pictureBox2.Image = Image.FromStream(ms);
                        }
                    }
                }
            }

            reader.Close();
            con.Close();


        }

        public void loadForm(Form form)
        {

            if (this.panel2.Controls.Count > 0)
                this.panel2.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            this.panel2.Controls.Add(form);
            this.panel2.Tag = form;
            form.Show();

           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string id = vendorId;

            if (id.StartsWith("VA"))
            {
                frmVendorFlight fvf= new frmVendorFlight(id);
                fvf.Show();
                this.Close();
                
            }
            else if(id.StartsWith("VH"))
            {
                frmAddHotel fah= new frmAddHotel(id);
                fah.Show();
                this.Close();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmVendorDashBoard_Load(object sender, EventArgs e)
        {
            ShowName(vendorId);
            loadForm(new frmVendorServiceList(vendorId));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new frmUser().Show();
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            new frmAboutUs().Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            new frmHelp().Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            new frmServices().Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadForm(new frmViewRoom_Seat(vendorId));
        }


        private void button4_Click(object sender, EventArgs e)
        {
            loadForm(new frmTravelerUpdateProfile(vendorId, 2));
        }

        private void button13_Click(object sender, EventArgs e)
        {
            loadForm(new frmVendorPayment(vendorId));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            loadForm(new frmUpdateService(vendorId));
        }
    }
}
