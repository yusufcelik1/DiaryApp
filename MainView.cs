using System;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;


namespace Odev2
{
    public class MainView
    {
        public static void View()
        {
            Console.WriteLine("Welcome!\nPress 1 to log in, press 2 to register, press 3 to forgot password and press enter.");
            string sayi = "";
            ConsoleKeyInfo karakter;
            do
            {

                karakter = Console.ReadKey(true);
                if (karakter.Key != ConsoleKey.Backspace && karakter.Key != ConsoleKey.Enter)
                {
                    sayi += karakter.KeyChar;
                    Console.Write(karakter.KeyChar);
                }
                else if (karakter.Key == ConsoleKey.Backspace && sayi.Length > 0)
                {
                    sayi = sayi.Substring(0, (sayi.Length - 1));
                    Console.Write("\b \b");
                }
            }
            while (karakter.Key != ConsoleKey.Enter || string.IsNullOrWhiteSpace(sayi));
            Console.WriteLine();

            if (!string.IsNullOrWhiteSpace(sayi))
            {
                int selection = Convert.ToInt32(sayi);
                connenctionDB.connection = new SqlConnection(connenctionDB.connectionString);
                connenctionDB.connection.Open();
                switch (selection)
                {
                    case 1:
                        User.Login();
                        break;
                    case 2:
                        User.SignUp();
                        break;
                    case 3:
                        User.ForgotPassword();
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;
                }
                connenctionDB.connection.Close();
            }
        }
        public static void ShowMainMenu(string username)
        {
            Console.Clear();
            Console.WriteLine("1. Write a diary");
            Console.WriteLine("2. Read diary");
            Console.WriteLine("3. List All Diary Entires");
            Console.WriteLine("4. Delete Diary Entry");
            Console.WriteLine("5. Exit");
            Console.Write("Make your selection:");

            double val = 0;
            string sayi = "";

            ConsoleKeyInfo karakter;
            do
            {
                karakter = Console.ReadKey(true);
                if (karakter.Key != ConsoleKey.Backspace)
                {
                    bool kontrol = double.TryParse(karakter.KeyChar.ToString(), out val);
                    if (kontrol)
                    {
                        sayi += karakter.KeyChar;
                        Console.Write(karakter.KeyChar);
                    }
                }
                else

                {
                    if (karakter.Key == ConsoleKey.Backspace && sayi.Length > 0)
                    {
                        sayi = sayi.Substring(0, (sayi.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            while (karakter.Key != ConsoleKey.Enter || string.IsNullOrWhiteSpace(sayi));
            Console.WriteLine();

            int selection = Convert.ToInt32(sayi);
            switch (selection)
            {
                case 1:
                   DiaryProperties.WriteDiaryEntry(username);
                    break;
                case 2:
                    DiaryProperties.ReadDiaryEntry(username);
                    break;
                case 3:
                    DiaryProperties.ListAllDiaryEntries(username);
                    break;
                case 4:
                    DiaryProperties.DeleteDiaryEntry(username);
                    break;
                case 5:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    ShowMainMenu(username);
                    break;
            }
        }
    }
}

