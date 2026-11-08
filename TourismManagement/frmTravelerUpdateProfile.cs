using System;
using System.Collections;
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
using System.Xml.Linq;

namespace TourismManagement
{
    public partial class frmTravelerUpdateProfile : Form
    {

        string selectedImagePath = "";
        private string ID;
        private int role;
        SqlConnection con = new SqlConnection("Data Source=faysal\\sqlexpress01;Initial Catalog=TravelManagement;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
        public frmTravelerUpdateProfile(string id,int r)
        {
            InitializeComponent();
            this.ID = id;
            this.role = r;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void LoadTravelerDetails()
        {
            string query = "";
            string paramName = "";

            if (role == 1)
            {
                query = "SELECT AdminID, Name, Email, Nid_PassportNumber, PhoneNumber, Gender, Password, Picture FROM Admin_Info WHERE AdminID=@ID";
                paramName = "@ID";
            }
            else if (role == 2)
            {
                query = "SELECT VendorID, Name, Email, Nid_PassportNumber, PhoneNumber, Gender, Password, Picture FROM Vendor_Info WHERE VendorID=@ID";
                paramName = "@ID";
            }
            else if (role == 3)
            {
                query = "SELECT TravelerID, Name, Email, Nid_PassportNumber, PhoneNumber, Gender, Password, Picture FROM Traveler_Info WHERE TravelerID=@ID";
                paramName = "@ID";
            }
            else
            {
                MessageBox.Show("Invalid role.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue(paramName, ID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            label3.Text = reader[0].ToString();
                            label4.Text = reader["Name"].ToString();
                            label5.Text = reader["Email"].ToString();
                            label6.Text = reader["Nid_PassportNumber"].ToString();
                            label8.Text = reader["Password"].ToString();
                            textBox1.Text = reader["PhoneNumber"].ToString();
                            label7.Text = reader["Gender"].ToString();

                            if (reader["Picture"] != DBNull.Value)
                            {
                                byte[] imgData = (byte[])reader["Picture"];
                                using (MemoryStream ms = new MemoryStream(imgData))
                                {
                                    pictureBox1.Image = Image.FromStream(ms);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                selectedImagePath = ofd.FileName;

                
                pictureBox1.Image = new Bitmap(selectedImagePath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string phone = textBox1.Text?.Trim();

            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Phone number cannot be empty!", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            string query = "";
            if (role == 1)
            {
                query = "UPDATE Admin_Info SET PhoneNumber=@Phone, Picture=@Picture WHERE AdminID=@ID";
            }
            else if (role == 2)
            {
                query = "UPDATE Vendor_Info SET PhoneNumber=@Phone, Picture=@Picture WHERE VendorID=@ID";
            }
            else if (role == 3)
            {
                query = "UPDATE Traveler_Info SET PhoneNumber=@Phone, Picture=@Picture WHERE TravelerID=@ID";
            }
            else
            {
                MessageBox.Show("Invalid role.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            byte[] imgData = null;
            try
            {
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    imgData = File.ReadAllBytes(selectedImagePath);
                }
                else if (pictureBox1.Image != null)
                {
                    imgData = GetCurrentImage(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading image: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
               
                if (con.State != ConnectionState.Open)
                    con.Open();

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@ID", ID);
                    cmd.Parameters.AddWithValue("@Picture", (object)imgData ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }

                gifMassageBox.Show("Profile updated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating profile: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private byte[] GetCurrentImage()
        {
            if (pictureBox1.Image == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bmp = new Bitmap(pictureBox1.Image))
                {
                   
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }
                return ms.ToArray();
            }
        }

        private void frmTravelerUpdateProfile_Load(object sender, EventArgs e)
        {
            LoadTravelerDetails();
        }
    }
}
