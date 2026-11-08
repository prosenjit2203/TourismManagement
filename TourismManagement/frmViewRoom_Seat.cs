using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TourismManagement
{
    public partial class frmViewRoom_Seat : Form
    {
        private string vendorID;
        private bool isBinding = false;
        public frmViewRoom_Seat(string id)
        {
            InitializeComponent();
            this.vendorID = id;
        }

        SqlConnection con = new SqlConnection("Data Source=faysal\\sqlexpress01;Initial Catalog=TravelManagement;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");

        private void BindData()
        {
            try
            {
                isBinding = true; 

                DataTable dt = new DataTable();

                if (vendorID.StartsWith("VH", StringComparison.OrdinalIgnoreCase))
                {
                    string query = @"
                SELECT 
                    H.HotelName,
                    R.HotelID, 
                    R.RoomNumber, 
                    R.RoomType, 
                    R.PricePerNight, 
                    R.RoomStatus
                FROM Room_Info R
                INNER JOIN Hotel_Info H ON R.HotelID = H.HotelID
                WHERE H.VendorID = @VendorID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@VendorID", vendorID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }

                    dataGridView1.DataSource = dt;

                    if (dt.Rows.Count > 0)
                    {
                        
                        dataGridView1.Columns["HotelID"].Visible = false;

                        
                        if (!dataGridView1.Columns.Contains("BookColumn"))
                        {
                            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                            chk.Name = "BookColumn";
                            chk.HeaderText = "Booked?";
                            dataGridView1.Columns.Add(chk);
                        }

                        ApplyGridDesign();
                    }
                }
                else if (vendorID.StartsWith("VA", StringComparison.OrdinalIgnoreCase))
                {
                    string query = @"
                SELECT 
                    S.FlightID,
                    S.SeatNumber,
                    S.IsBooked,
                    S.BookedBy,
                    S.BookingDate
                FROM Seat_Info S
                INNER JOIN Flight_Info F ON S.FlightID = F.FlightID
                WHERE F.VendorID = @VendorID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@VendorID", vendorID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }

                    dataGridView1.DataSource = dt;

                    if (dt.Rows.Count > 0)
                    {
                        if (!dataGridView1.Columns.Contains("BookColumn"))
                        {
                            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                            chk.Name = "BookColumn";
                            chk.HeaderText = "Booked?";
                            dataGridView1.Columns.Add(chk);
                        }

                        ApplyGridDesign_Flight();
                    }
                }
                else
                {
                    dataGridView1.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                isBinding = false; 
            }
        }


        private void ApplyGridDesign()
        {
            dataGridView1.RowTemplate.Height = 30;
            dataGridView1.ScrollBars = ScrollBars.Both;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.AutoResizeColumns();

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSteelBlue; 
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.EnableHeadersVisualStyles = false;

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkSlateBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;

            dataGridView1.Columns["PricePerNight"].DefaultCellStyle.Format = "N0";
            dataGridView1.Columns["PricePerNight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["RoomStatus"].Value != null &&
                    row.Cells["RoomStatus"].Value.ToString().Trim().Equals("Booked", StringComparison.OrdinalIgnoreCase))
                {
                    row.Cells["BookColumn"].Value = true;
                    row.DefaultCellStyle.BackColor = Color.MistyRose;   
                }
                else
                {
                    row.Cells["BookColumn"].Value = false;
                    row.DefaultCellStyle.BackColor = Color.Honeydew;
                }
            }
        }

        private void ApplyGridDesign_Flight()
        {
            dataGridView1.RowTemplate.Height = 30;
            dataGridView1.ScrollBars = ScrollBars.Both;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.AutoResizeColumns();

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSteelBlue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.EnableHeadersVisualStyles = false;

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkSlateBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool isBooked = Convert.ToBoolean(row.Cells["IsBooked"].Value);

                row.Cells["BookColumn"].Value = isBooked;

                if (isBooked)
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                else
                    row.DefaultCellStyle.BackColor = Color.Honeydew;
            }
        }

        private void SendCancellationEmail(string toEmail, string travelerName, string bookingId,
                                         string flightOrHotelId, string seatOrRoom, string totalBill, bool isFlight)
        {
            string subject = "Booking Cancellation Notice";
            string type = isFlight ? "Flight" : "Hotel";
            string body = $@"
Hello {travelerName},

Your {type} booking has been cancelled by the owner.

Booking ID : {bookingId}
{(isFlight ? "Flight ID" : "Hotel ID")} : {flightOrHotelId}
{(isFlight ? "Seat Number" : "Room Number")} : {seatOrRoom}
Total Bill : {totalBill}

You will receive your refund within 72 hours. 
You may book another {type.ToLower()} from our system.

Regards, 
Team Brainstomers
";

            try
            {
                using (MailMessage mail = new MailMessage("teambrainstomers05@gmail.com", toEmail))
                {
                    mail.Subject = subject;
                    mail.Body = body;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("teambrainstomers05@gmail.com", "bgeo elqz itqw wyvs");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                MessageBox.Show("Mail Sent " );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message);
            }
        }


        private bool isUpdating = false;
        private void dataGridView1_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
         {
            if (isBinding || isUpdating) return;
            if (e.RowIndex < 0 || dataGridView1.Columns[e.ColumnIndex].Name != "BookColumn") return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            isUpdating = true;

            try
            {
                if (vendorID.StartsWith("VH"))
                {
                    string hotelId = row.Cells["HotelID"].Value?.ToString();
                    string roomNumber = row.Cells["RoomNumber"].Value?.ToString();
                    if (string.IsNullOrEmpty(hotelId) || string.IsNullOrEmpty(roomNumber)) return;

                    bool isBooked = Convert.ToBoolean(row.Cells["BookColumn"].Value);
                    string newStatus = isBooked ? "Booked" : "Available";

                    if (con.State != ConnectionState.Open) con.Open();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                       
                        string selectRoom = "SELECT RoomStatus FROM Room_Info WHERE HotelID=@HotelID AND RoomNumber=@RoomNumber";
                        using (SqlCommand selCmd = new SqlCommand(selectRoom, con, tran))
                        {
                            selCmd.Parameters.AddWithValue("@HotelID", hotelId);
                            selCmd.Parameters.AddWithValue("@RoomNumber", roomNumber);
                            object dbVal = selCmd.ExecuteScalar();
                            string dbStatus = dbVal == null || dbVal == DBNull.Value ? null : dbVal.ToString();

                            if (string.Equals(dbStatus, newStatus, StringComparison.OrdinalIgnoreCase))
                            {
                                tran.Commit();
                            }
                            else
                            {
                                string updateRoom = "UPDATE Room_Info SET RoomStatus=@Status WHERE HotelID=@HotelID AND RoomNumber=@RoomNumber";
                                using (SqlCommand uCmd = new SqlCommand(updateRoom, con, tran))
                                {
                                    uCmd.Parameters.AddWithValue("@Status", newStatus);
                                    uCmd.Parameters.AddWithValue("@HotelID", hotelId);
                                    uCmd.Parameters.AddWithValue("@RoomNumber", roomNumber);
                                    uCmd.ExecuteNonQuery();
                                }

                                string updateAvailable = isBooked
                                    ? "UPDATE Hotel_Info SET AvailableRooms = CASE WHEN AvailableRooms > 0 THEN AvailableRooms - 1 ELSE 0 END WHERE HotelID=@HotelID"
                                    : "UPDATE Hotel_Info SET AvailableRooms = AvailableRooms + 1 WHERE HotelID=@HotelID";

                                using (SqlCommand aCmd = new SqlCommand(updateAvailable, con, tran))
                                {
                                    aCmd.Parameters.AddWithValue("@HotelID", hotelId);
                                    aCmd.ExecuteNonQuery();
                                }

                               
                                if (!isBooked)
                                {
                                    string bookingId = null;
                                    string travelerName = null;
                                    string travelerEmail = null;
                                    string totalBill = null;
                                    string allRooms = null;

                                   
                                    string selectBooking = @"
SELECT TOP 1 BookingID, TravelerID, NetPayable, RoomNumbers
FROM HotelBooking_Info
WHERE HotelID=@HotelID AND RoomNumbers LIKE '%' + @RoomNumber + '%'
ORDER BY BookingDate DESC";

                                    using (SqlCommand bCmd = new SqlCommand(selectBooking, con, tran))
                                    {
                                        bCmd.Parameters.AddWithValue("@HotelID", hotelId);
                                        bCmd.Parameters.AddWithValue("@RoomNumber", roomNumber);

                                        using (SqlDataReader reader = bCmd.ExecuteReader())
                                        {
                                            if (reader.Read())
                                            {
                                                bookingId = reader["BookingID"].ToString();
                                                totalBill = reader["NetPayable"].ToString();
                                                allRooms = reader["RoomNumbers"].ToString();
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(bookingId))
                                    {
                                        var rooms = allRooms.Split(',').Select(r => r.Trim()).ToList();

                                        string roomParams = string.Join(",", rooms.Select((r, i) => $"@r{i}"));
                                        string checkRooms = $"SELECT COUNT(*) FROM Room_Info WHERE HotelID=@HotelID AND RoomNumber IN ({roomParams}) AND RoomStatus='Booked'";

                                        using (SqlCommand checkCmd = new SqlCommand(checkRooms, con, tran))
                                        {
                                            checkCmd.Parameters.AddWithValue("@HotelID", hotelId);
                                            for (int i = 0; i < rooms.Count; i++)
                                                checkCmd.Parameters.AddWithValue($"@r{i}", rooms[i]);

                                            int bookedCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                                            if (bookedCount == 0) 
                                            {
                                                
                                                string tQuery = "SELECT Name, Email FROM Traveler_Info WHERE TravelerID = (SELECT TravelerID FROM HotelBooking_Info WHERE BookingID=@BookingID)";
                                                using (SqlCommand tCmd = new SqlCommand(tQuery, con, tran))
                                                {
                                                    tCmd.Parameters.AddWithValue("@BookingID", bookingId);
                                                    using (SqlDataReader tReader = tCmd.ExecuteReader())
                                                    {
                                                        if (tReader.Read())
                                                        {
                                                            travelerName = tReader["Name"].ToString();
                                                            travelerEmail = tReader["Email"].ToString();
                                                        }
                                                    }
                                                }

                                             
                                                if (!string.IsNullOrEmpty(travelerEmail))
                                                {
                                                    SendCancellationEmail(travelerEmail, travelerName, bookingId, hotelId, allRooms, totalBill, false);

                                                   
                                                    string cancelBooking = "UPDATE HotelBooking_Info SET BookingStatus='Cancelled' WHERE BookingID=@BookingID";
                                                    using (SqlCommand dCmd = new SqlCommand(cancelBooking, con, tran))
                                                    {
                                                        dCmd.Parameters.AddWithValue("@BookingID", bookingId);
                                                        dCmd.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                tran.Commit();
                            }
                        }
                    }

                    con.Close();
                    row.Cells["RoomStatus"].Value = newStatus;
                    row.DefaultCellStyle.BackColor = isBooked ? Color.MistyRose : Color.Honeydew;
                }
                else if (vendorID.StartsWith("VA"))
                {
                    string flightId = row.Cells["FlightID"].Value?.ToString();
                    string seatNumber = row.Cells["SeatNumber"].Value?.ToString();
                    if (string.IsNullOrEmpty(flightId) || string.IsNullOrEmpty(seatNumber)) return;

                    bool isBooked = Convert.ToBoolean(row.Cells["BookColumn"].Value);

                    if (con.State != ConnectionState.Open) con.Open();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        string selectSeat = "SELECT IsBooked FROM Seat_Info WHERE FlightID=@FlightID AND SeatNumber=@SeatNumber";
                        using (SqlCommand selCmd = new SqlCommand(selectSeat, con, tran))
                        {
                            selCmd.Parameters.AddWithValue("@FlightID", flightId);
                            selCmd.Parameters.AddWithValue("@SeatNumber", seatNumber);
                            object dbVal = selCmd.ExecuteScalar();
                            bool dbIsBooked = dbVal != null && dbVal != DBNull.Value && Convert.ToBoolean(dbVal);

                            if (dbIsBooked == isBooked)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                string updateSeat = @"
                                UPDATE Seat_Info
                                SET IsBooked = @IsBooked,
                                    BookedBy = @BookedBy,
                                    BookingDate = @BookingDate
                                WHERE FlightID = @FlightID AND SeatNumber = @SeatNumber";

                                using (SqlCommand uCmd = new SqlCommand(updateSeat, con, tran))
                                {
                                    uCmd.Parameters.AddWithValue("@IsBooked", isBooked);
                                    uCmd.Parameters.AddWithValue("@FlightID", flightId);
                                    uCmd.Parameters.AddWithValue("@SeatNumber", seatNumber);
                                    uCmd.Parameters.AddWithValue("@BookedBy", isBooked ? (object)vendorID : DBNull.Value);
                                    uCmd.Parameters.AddWithValue("@BookingDate", isBooked ? (object)DateTime.Now : DBNull.Value);
                                    uCmd.ExecuteNonQuery();
                                }

                                string updateFlight = isBooked
                                    ? "UPDATE Flight_Info SET Seat = CASE WHEN Seat > 0 THEN Seat - 1 ELSE 0 END WHERE FlightID=@FlightID"
                                    : "UPDATE Flight_Info SET Seat = Seat + 1 WHERE FlightID=@FlightID";

                                using (SqlCommand fCmd = new SqlCommand(updateFlight, con, tran))
                                {
                                    fCmd.Parameters.AddWithValue("@FlightID", flightId);
                                    fCmd.ExecuteNonQuery();
                                }


                                if (!isBooked) // owner unticked a seat
                                {
                                    string bookingId = null;
                                    string travelerName = null;
                                    string travelerEmail = null;
                                    string totalBill = null;
                                    string allSeats = null;

                                    // 1️⃣ Get booking info
                                    string selectBooking = @"
        SELECT TOP 1 BookingID, TotalBill, TravelerID, SeatNumber
        FROM Booking_Info
        WHERE ServiceID=@FlightID AND SeatNumber LIKE '%' + @SeatNumber + '%' 
        ORDER BY BookingDate DESC";

                                    using (SqlCommand cmd = new SqlCommand(selectBooking, con, tran))
                                    {
                                        cmd.Parameters.AddWithValue("@FlightID", flightId);
                                        cmd.Parameters.AddWithValue("@SeatNumber", seatNumber);

                                        using (SqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            if (reader.Read())
                                            {
                                                bookingId = reader["BookingID"].ToString();
                                                travelerEmail = null; // will get later
                                                travelerName = null;
                                                totalBill = reader["TotalBill"].ToString();
                                                allSeats = reader["SeatNumber"].ToString(); // e.g., "S1,S2,S3"
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(bookingId))
                                    {
                                        // 2️⃣ Split seat numbers
                                        var seats = allSeats.Split(',').Select(s => s.Trim()).ToList();

                                        // 3️⃣ Check if all seats are unticked in Seat_Info
                                        string seatParams = string.Join(",", seats.Select((s, i) => $"@s{i}"));
                                        string checkSeats = $"SELECT COUNT(*) FROM Seat_Info WHERE FlightID=@FlightID AND SeatNumber IN ({seatParams}) AND IsBooked=1";

                                        using (SqlCommand checkCmd = new SqlCommand(checkSeats, con, tran))
                                        {
                                            checkCmd.Parameters.AddWithValue("@FlightID", flightId);
                                            for (int i = 0; i < seats.Count; i++)
                                                checkCmd.Parameters.AddWithValue($"@s{i}", seats[i]);

                                            int bookedCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                                            if (bookedCount == 0) // all seats unticked
                                            {
                                                // 4️⃣ Get traveler info
                                                string travelerQuery = "SELECT Name, Email FROM Traveler_Info WHERE TravelerID = (SELECT TravelerID FROM Booking_Info WHERE BookingID=@BookingID)";
                                                using (SqlCommand tCmd = new SqlCommand(travelerQuery, con, tran))
                                                {
                                                    tCmd.Parameters.AddWithValue("@BookingID", bookingId);
                                                    using (SqlDataReader tReader = tCmd.ExecuteReader())
                                                    {
                                                        if (tReader.Read())
                                                        {
                                                            travelerName = tReader["Name"].ToString();
                                                            travelerEmail = tReader["Email"].ToString();
                                                        }
                                                    }
                                                }

                                                // 5️⃣ Send email
                                                if (!string.IsNullOrEmpty(travelerEmail))
                                                {
                                                    SendCancellationEmail(travelerEmail, travelerName, bookingId, flightId, allSeats, totalBill, true);

                                                    // 6️⃣ Delete booking
                                                    string cancelBooking = "UPDATE Booking_Info SET BookingStatus='Cancelled' WHERE BookingID=@BookingID";
                                                    using (SqlCommand dCmd = new SqlCommand(cancelBooking, con, tran))
                                                    {
                                                        dCmd.Parameters.AddWithValue("@BookingID", bookingId);
                                                        dCmd.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                tran.Commit();
                            }
                        }
                    }

                    con.Close();

                    if (dataGridView1.Columns.Contains("IsBooked"))
                        row.Cells["IsBooked"].Value = isBooked;

                    row.DefaultCellStyle.BackColor = isBooked ? Color.MistyRose : Color.Honeydew;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating booking: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { if (con.State == ConnectionState.Open) con.Close(); } catch { }
            }
            finally
            {
                isUpdating = false;
            }
        }



        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void frmViewRoom_Seat_Load(object sender, EventArgs e)
        {
            BindData();
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged_1;
           
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
        }
    }
}
