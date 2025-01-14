using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
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

        // Public method to expose txtEmailAdd for Login Form
        public string GetEmail()
        {
            return txtEmailAdd.Text;
        }

        private void picBack_Click(object sender, EventArgs e)
        {
            Login login = new Login(this);
            login.Show();
            this.Hide();
        }

        private bool IsValidUsername(string username)
        {
            // Regular expression to validate usernames with letters, numbers, underscores, dashes, and periods
            var regex = new Regex(@"^[a-zA-Z0-9_.-]+$");
            return regex.IsMatch(username);
        }

        private bool IsValidName(string name)
        {
            // Regular expression to validate full names with letters, spaces, and hyphens
            var regex = new Regex(@"^[a-zA-Z\s-]+$");
            return regex.IsMatch(name);
        }

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

        private string GenerateAccountNumber()
        {
            Random random = new Random();
            return random.Next(1000000000, 2000000000).ToString(); // Generates a unique 10-digit number
        }

        private bool IsValidAccountNumber(string accountNumber)
        {
            var regex = new Regex(@"^\d{10}$"); // Matches exactly 10 numeric digits
            return regex.IsMatch(accountNumber);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtName.Text;
            string username = txtUsername.Text;
            string email = txtEmailAdd.Text;
            string password = txtPassword.Text;
            string accountNumber = GenerateAccountNumber(); // Generate the 10-digit account number

            // Validate inputs
            if (!IsValidUsername(username))
            {
                MessageBox.Show("Username can only contain letters, numbers, underscores (_), dashes (-), and periods (.)", "Invalid Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidName(fullName))
            {
                MessageBox.Show("Fullname can only contain letters", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("Passwords do not match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                connect.Open();

                // Check if username or account number already exists
                string checkQuery = "SELECT COUNT(*) FROM Account WHERE Username = @Username OR AccountNumber = @AccountNumber";
                SqlCommand checkCmd = new SqlCommand(checkQuery, connect);
                checkCmd.Parameters.AddWithValue("@Username", username);
                checkCmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                int existingCount = (int)checkCmd.ExecuteScalar();

                if (existingCount > 0)
                {
                    MessageBox.Show("Username or Account Number already exists. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Insert new user along with the account number
                    string insertQuery = @"
                        INSERT INTO Account (Name, Username, Email, Password, AccountNumber) 
                        VALUES (@Name, @Username, @Email, @Password, @AccountNumber)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, connect);
                    insertCmd.Parameters.AddWithValue("@Name", fullName);
                    insertCmd.Parameters.AddWithValue("@Username", username);
                    insertCmd.Parameters.AddWithValue("@Email", email);
                    insertCmd.Parameters.AddWithValue("@Password", password);
                    insertCmd.Parameters.AddWithValue("@AccountNumber", accountNumber);

                    insertCmd.ExecuteNonQuery();

                    MessageBox.Show($"Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Login login = new Login(this);
                    login.Show();
                    this.Hide();
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
    }
}
