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
    public partial class PrepareBill : Form
    {
        public PrepareBill()
        {
            InitializeComponent();
        }
        
        private void label5_Click(object sender, EventArgs e)
        {

        }
        public decimal showAmount;
        public string TId;
        public string BId;
        public void SetBillDetails(string travelerName, string flightID, string bookingID, List<string> seats, decimal totalBill, decimal vat, decimal discount, decimal netPayable,DateTime date,string id)
                            
        {
            label9.Text = travelerName;
            label10.Text = flightID;
            label17.Text = bookingID;
            label11.Text = string.Join(",", seats);
            label6.Text = totalBill.ToString("F2") + " BDT";
            label8.Text = vat.ToString("F2") + " BDT";
            label13.Text = discount > 0 ? discount.ToString("F2") + " BDT" : "0.00 BDT";
            label15.Text = netPayable.ToString("F2") + " BDT";
            label18.Text = $"{date: dd-MMM-yyyy}";

            showAmount = netPayable;
            TId = id;
            BId = bookingID;
        }
        public void SetHotelBillDetails(string travelerName, string hotelID, string bookingID,
                                DateTime checkIn, DateTime checkOut, int roomsBooked, int days,
                                decimal pricePerNight, string roomType,
                                decimal totalBill, decimal vat, decimal discount, decimal netPayable,string id)

        {
            label9.Text = travelerName;                      
            label10.Text = hotelID;                          
            label17.Text = bookingID;                        
            label11.Text = $"Rooms: {roomsBooked}, Days: {days}, Type: {roomType}";
            label6.Text = totalBill.ToString("F2") + " BDT"; 
            label8.Text = vat.ToString("F2") + " BDT";       
            label13.Text = discount > 0 ? discount.ToString("F2") + " BDT" : "0.00 BDT"; 
            label15.Text = netPayable.ToString("F2") + " BDT"; 
            label18.Text = $"Check-in: {checkIn:dd-MMM-yyyy} Check-out: {checkOut:dd-MMM-yyyy}";

            showAmount= netPayable;
            TId = id;
            BId = bookingID;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            gifMassageBox.Show2("Loading.......");
            frmPaymentMethod fpm = new frmPaymentMethod(showAmount,TId,BId);
            fpm.Show();
           
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
