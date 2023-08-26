using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odev2
{
    public class DiaryProperties
    {
        public static void WriteDiaryEntry(string username)
        {
            Console.WriteLine("Write Diary Entry");
            Console.WriteLine("-----------------");
            Console.Write("Date (GG.AA.YYYY): ");
            string date = Console.ReadLine();
            if (connenctionDB.IsDateValid(date))
            {
                Console.Write("Enter your text: ");
                string content = Console.ReadLine();


                string encryptedContent = connenctionDB.EncryptContent(content);

                string userDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string userFolder = Path.Combine(userDocumentsFolder, "diary", username);
                string filePath = Path.Combine(userFolder, $"{date}.txt");

                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder);
                }
                File.WriteAllText(filePath, encryptedContent);
                Console.WriteLine("diary successfully saved");
                Console.WriteLine("Press enter to continue");
                Console.ReadKey();
               MainView.ShowMainMenu(username);
            }
            else
            {
                Console.WriteLine("Incorrect date type!");
                MainView.ShowMainMenu(username);
            }
        }
        public static void ReadDiaryEntry(string username)
        {
            Console.WriteLine("Read Diary Entery");
            Console.WriteLine("-----------------");
            Console.Write("Date (GG.AA.YYYY): ");
            string date = Console.ReadLine();
            if (connenctionDB.IsDateValid(date))
            {
                string userDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string userFolder = Path.Combine(userDocumentsFolder, "diary", username);
                string filePath = Path.Combine(userFolder, $"{date}.txt");

                if (File.Exists(filePath))
                {
                    string encryptedContent = File.ReadAllText(filePath);
                    string decryptedContent = connenctionDB.DecryptContent(encryptedContent);

                    Console.WriteLine("Your Dairy");
                    Console.WriteLine(decryptedContent);
                    Console.WriteLine("Press enter to continue");
                    Console.ReadKey();
                   MainView.ShowMainMenu(username);
                }
                else
                {
                    Console.WriteLine("No diary found for the specified date.");
                }
            }
            else
            {
                Console.WriteLine("Incorrect date type!");
                MainView.ShowMainMenu(username);
            }

        }
        public static void ListAllDiaryEntries(string username)
        {
            Console.WriteLine("List All Diary Entries");
            Console.WriteLine("----------------------");
            string userDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string userFolder = Path.Combine(userDocumentsFolder, "diary", username);

            if (Directory.Exists(userFolder))
            {
                Console.WriteLine("List of Diary Entries:");

                string[] entryFiles = Directory.GetFiles(userFolder, "*.txt");

                if (entryFiles.Length > 0)
                {
                    foreach (string entryFile in entryFiles)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(entryFile);
                        Console.WriteLine($"- {fileName}");
                    }
                }
                else
                {
                    Console.WriteLine("No diary entries found.");
                }
            }
            else
            {
                Console.WriteLine("User folder not found.");
            }

            Console.WriteLine("Press enter to continue");
            Console.ReadKey();
            MainView.ShowMainMenu(username);
        }
        public static void DeleteDiaryEntry(string username)
        {
            Console.WriteLine("Delete Diary Entry");
            Console.WriteLine("------------------");
            Console.Write("Enter Date (GG.AA.YYYY) of the entry to delete: ");
            string date = Console.ReadLine();
            if (connenctionDB.IsDateValid(date))
            {

                string userDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string userFolder = Path.Combine(userDocumentsFolder, "diary", username);
                string filePath = Path.Combine(userFolder, $"{date}.txt");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine("Diary entry deleted successfully.");
                }
                else
                {
                    Console.WriteLine("No diary entry found for the specified date.");
                    DeleteDiaryEntry(username);
                }

                Console.WriteLine("Press enter to continue");
                Console.ReadKey();
                MainView.ShowMainMenu(username);
            }
            else
            {
                Console.WriteLine("Incorrect date type!");
                MainView.ShowMainMenu(username);
            }

        }
    }
}
