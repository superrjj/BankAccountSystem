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

namespace BankAccountSystem
{
    public partial class HomePage : Form
    {
        
        SqlConnection connect = new SqlConnection(@"Data Source=MSI\SQLEXPRESS;Initial Catalog=BankDatabase;Integrated Security=True;Encrypt=False");
        private string fullName;

        // Constructor that accepts the fullName
        public HomePage(string fullName)
        {
            InitializeComponent();
            this.fullName = fullName;
        }

        private void lblUsername_Click(object sender, EventArgs e)
        {

        }

        private void HomePage_Load(object sender, EventArgs e)
        {
            lblUsername.Text = $"Hi, {fullName}!";
        }
    }
}
