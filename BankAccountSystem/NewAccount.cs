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
    public partial class NewAccount : Form
    {

        private Login loginForm;
        SqlConnection connect = new SqlConnection(@"Data Source=MSI\SQLEXPRESS;Initial Catalog=BankDatabase;Integrated Security=True;Encrypt=False");
        public NewAccount(Login loginForm)
        {
            InitializeComponent();
            this.loginForm = loginForm;
        }

        private void picBack_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }

        private bool IsValidUsername(string username)
        {
            // Regular expression to match a username with letters, numbers, underscores, dashes, and periods
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_.-]+$");
            return regex.IsMatch(username);
        }

        private bool IsValidName(string name)
        {
            // Regular expression to match a name with letters, spaces, and hyphens
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z\s-]+$");
            return regex.IsMatch(name);
        }

        // Method to validate email
        bool IsValidEmail(string email)
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

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtName.Text;
            string username = txtUsername.Text;
            string email = txtEmailAdd.Text;
            string password = txtPassword.Text;

           // Validate the username
            if (!IsValidUsername(username))
            {
                MessageBox.Show("Username can only contain letters, numbers, underscores (_), dashes (-), and periods (.)", "Invalid Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidUsername(username))
            {
                MessageBox.Show("Fullname can only contain letters", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate the email
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate password match
            if (txtPassword.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("Password does not match", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
         

            try
            {
                connect.Open();

                // Check if the username already exists
                string checkQuery = "SELECT COUNT(*) FROM UserAccount WHERE username = @username";
                SqlCommand checkCmd = new SqlCommand(checkQuery, connect);
                checkCmd.Parameters.AddWithValue("@username", username);
                int userCount = (int)checkCmd.ExecuteScalar();

                if (userCount > 0)
                {
                    MessageBox.Show("Username already exists, please choose another one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Insert the new user and retrieve the userId
                    string insertQuery = @"
                    INSERT INTO UserAccount (Name, Username, Email, Password) 
                    VALUES (@Name, @Username, @Email, @Password);
                    SELECT SCOPE_IDENTITY();";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, connect);
                    insertCmd.Parameters.AddWithValue("@Name", fullName);
                    insertCmd.Parameters.AddWithValue("@Username", username);
                    insertCmd.Parameters.AddWithValue("@Email", email);
                    insertCmd.Parameters.AddWithValue("@Password", password);

                    // Execute the query and get the new userId
                    object userIdObj = insertCmd.ExecuteScalar();
                    int newUserId = Convert.ToInt32(userIdObj);

                    MessageBox.Show($"Registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Login login = new Login();
                    login.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connect.Close();
            }

           
          

        }
    }
}
