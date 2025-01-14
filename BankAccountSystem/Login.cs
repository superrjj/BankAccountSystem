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
    public partial class Login : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=MSI\SQLEXPRESS;Initial Catalog=BankDatabase;Integrated Security=True;Encrypt=False");
        private NewAccount newAcc;
        
        public Login(NewAccount newAcc)
        {
            InitializeComponent();
            this.newAcc = newAcc;
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void picNewAccount_Click(object sender, EventArgs e)
        {
            NewAccount newAccount = new NewAccount(this);
            newAccount.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string usernameOrEmail = txtUsername.Text;
            string password = txtPassword.Text;

            // Validate input
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
            {
                MessageBox.Show("Please enter your username or email.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Determine if the input is an email or a username
            bool isEmail = IsValidEmail(usernameOrEmail);

            try
            {
                connect.Open();

                // Query to check username/email and password
                string fullName = null;
                string getFullNameQuery = isEmail
                    ? "SELECT Name FROM Account WHERE Email = @Email AND Password = @Password"
                    : "SELECT Name FROM Account WHERE Username = @Username AND Password = @Password";

                SqlCommand nameCmd = new SqlCommand(getFullNameQuery, connect);
                if (isEmail)
                {
                    nameCmd.Parameters.AddWithValue("@Email", usernameOrEmail);
                }
                else
                {
                    nameCmd.Parameters.AddWithValue("@Username", usernameOrEmail);
                }
                nameCmd.Parameters.AddWithValue("@Password", password);

                object result = nameCmd.ExecuteScalar();
                if (result != null)
                {
                    fullName = result.ToString();
                }

                if (fullName != null)
                {
                    HomePage mainForm = new HomePage(fullName);
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username/email or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }

        // Helper method to validate email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }

        }
    }
}
