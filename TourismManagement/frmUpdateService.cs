using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TourismManagement
{
    public partial class frmUpdateService : Form
    {

        string connectionString = @"Data Source=faysal\sqlexpress01;Initial Catalog=TravelManagement;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        string selectedImagePath = "";
        private string vendorId ;
       
        public frmUpdateService(string id)
        {
            InitializeComponent();
            groupBox1.Visible = false;
            button3.Visible = false;
            this.vendorId = id;
            label5.Visible = false;
        }

        private void LoadVendorData()
        {
            if (vendorId.StartsWith("VA")) 
            {
                groupBox1.Visible = false;
                LoadFlightData(vendorId);
            }
            else if (vendorId.StartsWith("VH")) 
            {
                groupBox1.Visible = true;
                LoadHotelData(vendorId);
            }
            else
            {
                MessageBox.Show("Invalid Vendor ID!");
            }
        }

        private void LoadFlightData(string vendorId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT FlightID, FromCity, ToCity, FlightDate, Time, Fare,Seat FROM Flight_Info WHERE VendorID=@VendorID";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@VendorID", vendorId);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void LoadHotelData(string vendorId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT h.HotelID, h.HotelName, h.City, h.Country,
               r.PricePerNight, h.AvailableRooms, h.TotalRooms, r.RoomType
        FROM Hotel_Info h
        INNER JOIN Room_Info r ON h.HotelID = r.HotelID
        WHERE h.VendorID = @VendorID";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@VendorID", vendorId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
           string currentID = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(currentID))
            {
                MessageBox.Show("Enter FlightID or HotelID to search!");
                return;
            }

            if (vendorId.StartsWith("VA"))
            {
                SearchFlight(currentID);
            }
            else if (vendorId.StartsWith("VH"))
            {
                SearchHotel(currentID);
            }
        }
        private void SearchFlight(string flightId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT FlightID, FromCity, ToCity, FlightDate, Time, Fare, Seat, Picture
            FROM Flight_Info
            WHERE FlightID = @FlightID AND VendorID = @VendorID";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@FlightID", flightId);
                da.SelectCommand.Parameters.AddWithValue("@VendorID", vendorId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Flight found!");
                    dataGridView1.DataSource = dt;

                    
                    if (dt.Rows[0]["Picture"] != DBNull.Value)
                    {
                        byte[] imgData = (byte[])dt.Rows[0]["Picture"];
                        using (MemoryStream ms = new MemoryStream(imgData))
                        {
                            pictureBox1.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pictureBox1.Image = null;
                    }
                    label5.Text = flightId;
                    button3.Visible = true;
                }
                else
                {
                    MessageBox.Show("No flight found with this ID.");
                    pictureBox1.Image = null;
                    button3.Visible = false;
                }
            }
        }


        private void SearchHotel(string hotelId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT h.HotelID, h.HotelName, h.City, h.Country,
               r.PricePerNight, r.RoomType, h.AvailableRooms, h.TotalRooms, h.Picture
        FROM Hotel_Info h
        INNER JOIN Room_Info r ON h.HotelID = r.HotelID
        WHERE h.HotelID = @HotelID AND h.VendorID = @VendorID";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@HotelID", hotelId);
                da.SelectCommand.Parameters.AddWithValue("@VendorID", vendorId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Hotel found!");
                    dataGridView1.DataSource = dt;

                    
                    if (dt.Rows[0]["Picture"] != DBNull.Value)
                    {
                        byte[] imgData = (byte[])dt.Rows[0]["Picture"];
                        using (MemoryStream ms = new MemoryStream(imgData))
                        {
                            pictureBox1.Image = Image.FromStream(ms);
                        }
                    }
                    label5.Text=hotelId;
                    button3.Visible = true;
                }
                else
                {
                    MessageBox.Show("No hotel found with this ID.");
                    pictureBox1.Image = null; 
                    button3.Visible = false;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                selectedImagePath = ofd.FileName;
                pictureBox1.Image = Image.FromFile(selectedImagePath);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string currentID = label5.Text;
            if (string.IsNullOrEmpty(currentID))
            {
                MessageBox.Show("Search first before saving!");
                return;
            }

            if (vendorId.StartsWith("VA"))
            {
                UpdateFlight(currentID);
            }
            else if (vendorId.StartsWith("VH"))
            {
                UpdateHotel(currentID);
            }
        }

        private void UpdateFlight(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Please search a flight first!");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string selectQuery = "SELECT Seat, Fare FROM Flight_Info WHERE FlightID=@FlightID AND VendorID=@VendorID";
                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                selectCmd.Parameters.AddWithValue("@FlightID", id);
                selectCmd.Parameters.AddWithValue("@VendorID", vendorId);

                int currentSeats = 0;
                decimal currentFare = 0;
                using (SqlDataReader reader = selectCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        currentSeats = Convert.ToInt32(reader["Seat"]);
                        currentFare = Convert.ToDecimal(reader["Fare"]);
                    }
                }

               
                int addSeats = 0;
                if (!string.IsNullOrEmpty(textBox2.Text) && int.TryParse(textBox2.Text, out addSeats))
                {
                    string lastSeatQuery = @"
                SELECT TOP 1 SeatNumber
                FROM Seat_Info
                WHERE FlightID = @FlightID
                ORDER BY CAST(SUBSTRING(SeatNumber, 2, LEN(SeatNumber)) AS INT) DESC";
                    SqlCommand lastSeatCmd = new SqlCommand(lastSeatQuery, con);
                    lastSeatCmd.Parameters.AddWithValue("@FlightID", id);

                    object lastSeatObj = lastSeatCmd.ExecuteScalar();
                    int lastSeatNumber = 0;
                    if (lastSeatObj != null)
                    {
                        string lastSeatStr = lastSeatObj.ToString();
                        lastSeatNumber = int.Parse(lastSeatStr.Substring(1)); 
                    }

                    for (int i = 1; i <= addSeats; i++)
                    {
                        string newSeat = "S" + (lastSeatNumber + i);
                        string insertSeatQuery = "INSERT INTO Seat_Info (FlightID, SeatNumber) VALUES (@FlightID, @SeatNumber)";
                        using (SqlCommand insertCmd = new SqlCommand(insertSeatQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("@FlightID", id);
                            insertCmd.Parameters.AddWithValue("@SeatNumber", newSeat);
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    currentSeats += addSeats;
                }

               
                decimal fare = currentFare;
                if (!string.IsNullOrEmpty(textBox3.Text) && decimal.TryParse(textBox3.Text, out decimal parsedFare))
                {
                    if(parsedFare>0)
                    {
                        fare = parsedFare;
                    }
                    
                }

               
                string updateQuery = "UPDATE Flight_Info SET Fare=@Fare, Seat=@Seats, Picture=@Picture WHERE FlightID=@FlightID AND VendorID=@VendorID";
                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@FlightID", id);
                    cmd.Parameters.AddWithValue("@VendorID", vendorId);
                    cmd.Parameters.AddWithValue("@Fare", fare);
                    cmd.Parameters.AddWithValue("@Seats", currentSeats);

                  
                    byte[] imgData = null;
                    try
                    {
                        if (!string.IsNullOrEmpty(selectedImagePath))
                            imgData = File.ReadAllBytes(selectedImagePath);
                        else if (pictureBox1.Image != null)
                            imgData = GetCurrentImage();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    cmd.Parameters.AddWithValue("@Picture", (object)imgData ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }

                con.Close();
                MessageBox.Show($"Flight updated! Total seats: {currentSeats}");
                LoadFlightData(vendorId);
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
        private void UpdateHotel(string id)
        {
            int addRooms = 0;
            decimal fare = 0;

            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Please Search a Hotel!!");
                return;
            }

            
            int.TryParse(textBox2.Text, out addRooms);
            decimal.TryParse(textBox3.Text, out fare);

            string category = "";
            string prefix = "";

            
            if (addRooms > 0 || fare > 0)
            {
                if (radioButton1.Checked)
                {
                    category = "Single";
                    prefix = "RS";
                }
                else if (radioButton2.Checked)
                {
                    category = "Double";
                    prefix = "RD";
                }
                else if (radioButton3.Checked)
                {
                    category = "Tripple";
                    prefix = "RT";
                }
                else
                {
                    MessageBox.Show("Please select a room category!");
                    return;
                }
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string selectQuery = "SELECT TotalRooms, AvailableRooms FROM Hotel_Info WHERE HotelID=@HotelID";
                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                selectCmd.Parameters.AddWithValue("@HotelID", id);

                int currentTotal = 0, currentAvailable = 0;
                using (SqlDataReader reader = selectCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        currentTotal = Convert.ToInt32(reader["TotalRooms"]);
                        currentAvailable = Convert.ToInt32(reader["AvailableRooms"]);
                    }
                }

               
                if (addRooms > 0)
                {
                    string lastRoomQuery = @"
    SELECT TOP 1 RoomNumber
    FROM Room_Info
    WHERE HotelID = @HotelID AND RoomType = @Category
    ORDER BY CAST(SUBSTRING(RoomNumber, 3, LEN(RoomNumber)) AS INT) DESC";
                    SqlCommand lastRoomCmd = new SqlCommand(lastRoomQuery, con);
                    lastRoomCmd.Parameters.AddWithValue("@HotelID", id);
                    lastRoomCmd.Parameters.AddWithValue("@Category", category);

                    object lastRoomObj = lastRoomCmd.ExecuteScalar();
                    int lastRoomNumber = 0;
                    if (lastRoomObj != null)
                    {
                        string lastRoomStr = lastRoomObj.ToString();
                        lastRoomNumber = int.Parse(lastRoomStr.Substring(2));
                    }

                    for (int i = 1; i <= addRooms; i++)
                    {
                        string newRoom = prefix + (lastRoomNumber + i);
                        string insertRoomQuery = "INSERT INTO Room_Info (HotelID, RoomNumber, RoomType, RoomStatus, PricePerNight) VALUES (@HotelID, @RoomNumber, @Category, 'Available', @Fare)";
                        SqlCommand insertCmd = new SqlCommand(insertRoomQuery, con);
                        insertCmd.Parameters.AddWithValue("@HotelID", id);
                        insertCmd.Parameters.AddWithValue("@RoomNumber", newRoom);
                        insertCmd.Parameters.AddWithValue("@Category", category);
                        insertCmd.Parameters.AddWithValue("@Fare", fare);
                        insertCmd.ExecuteNonQuery();
                    }

                    currentTotal += addRooms;
                    currentAvailable += addRooms;
                }

            
                if (fare > 0 && !string.IsNullOrEmpty(category))
                {
                    string updateFareQuery = "UPDATE Room_Info SET PricePerNight=@Fare WHERE HotelID=@HotelID AND RoomType=@Category";
                    SqlCommand updateFareCmd = new SqlCommand(updateFareQuery, con);
                    updateFareCmd.Parameters.AddWithValue("@HotelID", id);
                    updateFareCmd.Parameters.AddWithValue("@Category", category);
                    updateFareCmd.Parameters.AddWithValue("@Fare", fare);
                    updateFareCmd.ExecuteNonQuery();
                }

              
                string updateQuery = "UPDATE Hotel_Info SET TotalRooms=@TotalRooms, AvailableRooms=@AvailableRooms, Picture=@Picture WHERE HotelID=@HotelID";

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

                SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                updateCmd.Parameters.AddWithValue("@HotelID", id);
                updateCmd.Parameters.AddWithValue("@TotalRooms", currentTotal);
                updateCmd.Parameters.AddWithValue("@AvailableRooms", currentAvailable);

                if (imgData != null)
                    updateCmd.Parameters.AddWithValue("@Picture", imgData);
                else
                    updateCmd.Parameters.AddWithValue("@Picture", DBNull.Value);

                updateCmd.ExecuteNonQuery();

                con.Close();

                MessageBox.Show($"Hotel updated! Total: {currentTotal}, Available: {currentAvailable}");

                LoadHotelData(vendorId);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string currentID = label5.Text;
            if (string.IsNullOrEmpty(currentID))
            {
                MessageBox.Show("Search first before deleting!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this Service?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "";

                if (vendorId.StartsWith("VA"))
                    query = "DELETE FROM Flight_Info WHERE FlightID=@ID";
                else if (vendorId.StartsWith("VH"))
                    query = "DELETE FROM Hotel_Info WHERE HotelID=@ID";
                else
                {
                    MessageBox.Show("Invalid ID.");
                    return;
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ID", currentID);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Deleted successfully!");
                    if (vendorId.StartsWith("VA"))
                        LoadFlightData(vendorId);
                    else if (vendorId.StartsWith("VH"))
                        LoadHotelData(vendorId);
                    pictureBox1.Image = null;
                    button3.Visible = false;
                }
            }
        }

        private void frmUpdateService_Load(object sender, EventArgs e)
        {

            LoadVendorData();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Enter Your Service ID")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Enter Your Service ID";
                textBox1.ForeColor = Color.Gray;

            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "000")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.Text = "000";
                textBox2.ForeColor = Color.Gray;

            }
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "00000")
            {
                textBox3.Text = "";
                textBox3.ForeColor = Color.Black;
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                textBox3.Text = "00000";
                textBox3.ForeColor = Color.Gray;

            }
        }
    }
}
