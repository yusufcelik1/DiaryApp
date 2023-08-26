using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Odev2
{
    public class User
    {
        public static void Login()
        {
            Console.Clear();
            Console.WriteLine("LOGIN SCREEN");
            Console.WriteLine("------------");
            Console.Write("Enter Email: ");
            string UserEmail = Console.ReadLine();
            if (IsValidEmailAddress(UserEmail))
            {
                Console.Write("Enter password: ");
                string password = ReadPassword();
                string hashedPassword = HashPassword(password); // Hash the provided password

                // Prepare an SQL command to retrieve the hashed password from the database for the given username
                SqlCommand selectCommand = new SqlCommand("SELECT UserPassword, UserName FROM UserInformation WHERE UserEmail = @UserEmail",connenctionDB.connection);
                selectCommand.Parameters.AddWithValue("@UserEmail", UserEmail);
                SqlDataReader reader = selectCommand.ExecuteReader();

                if (reader.Read())
                {
                    string storedHashedPassword = reader["UserPassword"] as string;
                    string username = reader["UserName"] as string;

                    if (storedHashedPassword != null && storedHashedPassword.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Login successful.");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadKey();
                        reader.Close();
                        MainView.ShowMainMenu(username);
                    }
                    else
                    {
                        Console.WriteLine("Login failed.");
                        Console.ReadKey();
                        reader.Close();
                        Login();
                    }
                }
                else
                {
                    Console.WriteLine("User not found.");
                    Console.ReadKey();
                    reader.Close();
                    Console.Clear();
                    Login();
                }
            }
            else
            {
                Console.WriteLine("This is not a valid email address! Please try again");
                Console.ReadKey();
                Login();
            }
        }
        public static void SignUp()
        {
            Console.Clear();
            string Password1, Password2;
            Console.WriteLine("Register");
            Console.WriteLine("--------");
            Console.Write("User Name: ");
            string userName = Console.ReadLine();
            Console.Write("User Surname: ");
            string userSurname = Console.ReadLine();
            Console.Write("User Email: ");
            string userEmail = Console.ReadLine();
            SqlCommand checkEmailCommand = new SqlCommand("SELECT COUNT(*) FROM UserInformation WHERE UserEmail = @UserEmail", connenctionDB.connection);
            checkEmailCommand.Parameters.AddWithValue("@UserEmail", userEmail);
            int emailCount = Convert.ToInt32(checkEmailCommand.ExecuteScalar());

            if (emailCount > 0)
            {
                Console.WriteLine("This email address is already registered. Please use a different email.");
                Console.ReadKey();
                Console.Clear();
                SignUp();
                return; // Exit the function to prevent further execution
            }
            Console.WriteLine("User Date of Birth");
            Console.Write("Enter Day:");
            int birthDay = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Month:");
            int birthMonth = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Year:");
            int birthYear = Convert.ToInt32(Console.ReadLine());

            DateTime enteredDate = new DateTime(birthYear, birthMonth, birthDay);
            do
            {
                Console.Write("Password: ");
                Password1 = ReadPassword();
                Console.Write("Confirm Password: "); // Prompt the user to confirm the password
                Password2 = ReadPassword();

                if (Password1 != Password2)
                {
                    Console.WriteLine("Password did not match! Please try again.");
                }
            } while (Password1 != Password2);
            DateTime registrationDate = DateTime.Now;
            string hashedUserName = HashPassword(userName);
            string hashedUserSurname = HashPassword(userSurname);
            string hashedUserEmail = HashPassword(userEmail);
            string hashedEnteredDate = HashPassword(enteredDate.ToString());
            string hashedRegistrationDate = HashPassword(registrationDate.ToString());
            string hashedPassword = HashPassword(Password2); // Hash the confirmed password

            Console.Write("Security Answer\n");
            Console.Write("What is your favorite animal? : ");
            string securityAnswer = Console.ReadLine();
            string hashedSecurityAnswer = HashPassword(securityAnswer);
            string query = "INSERT INTO [UserInformation] (UserName, UserSurname, UserEmail, UserDateOfBirth,UserRegistrationDate, UserPassword, SecurityAnswer) VALUES (@UserName, @UserSurname, @UserEmail, @UserDateOfBirth,@UserRegistrationDate, @UserPassword, @SecurityAnswer)";

            // Create a SqlCommand with parameters to insert user data into the database
            using (SqlCommand command = new SqlCommand(query,connenctionDB.connection))
            {
                command.Parameters.AddWithValue("@UserName", hashedUserName);
                command.Parameters.AddWithValue("@UserSurname", hashedUserSurname);
                command.Parameters.AddWithValue("@UserEmail", hashedUserEmail);
                command.Parameters.AddWithValue("@UserDateOfBirth", hashedEnteredDate);
                command.Parameters.AddWithValue("@UserRegistrationDate", hashedRegistrationDate);
                command.Parameters.AddWithValue("@UserPassword", hashedPassword);
                command.Parameters.AddWithValue("@SecurityAnswer", hashedSecurityAnswer);
                command.ExecuteNonQuery(); // Execute the query to insert data into the database
            }

            Console.Write("Registration is Success\n");
            Console.Write("Press enter to continue");
            Console.ReadKey();
            Console.Clear();
            Login();
        }
        static string HashPassword(string Password)
        {
            using (SHA256 sha256Hash = SHA256.Create()) // Create a SHA-256 hashing algorithm instance
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Password)); // Convert the password to bytes and compute the hash

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Convert the hash bytes to a hexadecimal string representation
                }
                return builder.ToString(); // Return the hashed password as a string
            }
        }
        static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true); // Read a key from the console without displaying it
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(); // Print a newline when Enter is pressed to maintain console formatting
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1); // Remove the last character when Backspace is pressed
                    Console.Write("\b \b"); // Clear the character from the console
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar); // Append the typed character to the password
                    Console.Write("*"); // Display an asterisk to mask the typed character
                }
            }
            return password.ToString(); // Return the securely read password as a string
        }

       public static void ForgotPassword()
        {
            Console.Write("Enter your email: ");
            string userEmail = Console.ReadLine();

            SqlCommand selectCommand = new SqlCommand("SELECT SecurityAnswer, UserName FROM UserInformation WHERE UserEmail = @UserEmail", connenctionDB.connection);
            selectCommand.Parameters.AddWithValue("@UserEmail", userEmail);
            SqlDataReader reader = selectCommand.ExecuteReader();

            if (reader.Read())
            {
                string storedHashedSecurityAnswer = reader["SecurityAnswer"] as string;
                string username = reader["UserName"] as string;

                Console.Write("Answer to security question: ");
                string securityAnswer = Console.ReadLine();
                string hashedSecurityAnswer = HashPassword(securityAnswer);

                if (storedHashedSecurityAnswer != null && storedHashedSecurityAnswer.Equals(hashedSecurityAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    reader.Close();
                    ResetPassword(username);
                }
                else
                {
                    Console.WriteLine("Security answer is incorrect.");
                    Console.ReadKey();
                    reader.Close();
                    ForgotPassword();
                }
            }
            else
            {
                Console.WriteLine("User not found.");
                Console.ReadKey();
                reader.Close();
            }
        }
        static void ResetPassword(string username)
        {
            Console.Write("Enter a new password: ");
            string newPassword = ReadPassword();
            Console.Write("Confirm new password: ");
            string confirmNewPassword = ReadPassword();

            if (newPassword == confirmNewPassword)
            {
                string hashedPassword = HashPassword(newPassword);

                SqlCommand updateCommand = new SqlCommand("UPDATE UserInformation SET UserPassword = @UserPassword WHERE UserName = @UserName", connenctionDB.connection);
                updateCommand.Parameters.AddWithValue("@UserPassword", hashedPassword);
                updateCommand.Parameters.AddWithValue("@UserName", username);
                updateCommand.ExecuteNonQuery();

                Console.WriteLine("Password reset successful.");
                Console.WriteLine("Press enter to continue");
                Console.ReadKey();
                Login();
            }
            else
            {
                Console.WriteLine("Passwords do not match. Password reset failed.");
                Console.ReadKey();
                ForgotPassword();
            }
        }
        static bool IsValidEmailAddress(string emailAddress)
        {
            try
            {
                var mailAddress = new MailAddress(emailAddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
