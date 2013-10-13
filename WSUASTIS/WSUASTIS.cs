using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WSUASTIS.RecordTypes;

namespace WSUASTIS
{
    enum UserType { Manager, Employee };
    public class WSUASTIS
    {
        UserType User = UserType.Employee;
        RecordDatabase RecordDB;

        /// <summary>
        /// Constructor for system
        /// </summary>
        public WSUASTIS()
        {
            #region Initialize Records
            if (!File.Exists(@"RecordDatabase.xml")) //If RecordDatabase does not exist
            {
                RecordDB = new RecordDatabase();
                RecordDB.Inventory = new List<InventoryRecord>();
                RecordDB.Transactions = new List<Transaction>();
                SaveRecords();
            }
            #endregion
            try
            {
                //Load RecordDatabase
                XmlSerializer RecordDatabaseReader = new XmlSerializer(typeof(RecordDatabase));
                StreamReader RecordDatabaseFile = new StreamReader(@"RecordDatabase.xml");
                RecordDB = (RecordDatabase)RecordDatabaseReader.Deserialize(RecordDatabaseFile);
                RecordDatabaseFile.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Fatal Error: Could not read inventory file. Exiting...");
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Starts the system
        /// </summary>
        public void Run()
        {
            char input = '0';

            //Main Menu
            do
            {
                PrintHeader();
                Console.WriteLine("Main Menu: ");
                Console.WriteLine("1) Inventory search");
                Console.WriteLine("2) Start transaction");
                Console.WriteLine("3) Edit inventory");
                Console.WriteLine("0) Exit");
                Console.WriteLine();
                Console.Write("Enter selection: ");

                input = Console.ReadKey(true).KeyChar;
                while (input != '1' && input != '2' && input != '3' && input != '0')
                {
                    input = Console.ReadKey(true).KeyChar;
                }

                if (input == '1')
                {
                    InventorySearch();
                }
                else if (input == '2')
                {
                    StartTransaction();
                }
                else if (input == '3')
                {
                    Console.WriteLine();
                    Console.Write("Please enter manager password: ");
                    if (Console.ReadLine() == "123")
                    {
                        EditInventory();
                    }
                    else
                    {
                        Console.WriteLine("Invalid password...");
                    }
                }


            } while (input != '0');
        }

        private void SaveRecords()
        {
            try
            {
                //Save Record Database
                XmlSerializer RecordDatabaseWriter = new XmlSerializer(typeof(RecordDatabase));
                StreamWriter RecordDatabaseFile = new StreamWriter(@"RecordDatabase.xml");
                RecordDatabaseWriter.Serialize(RecordDatabaseFile, RecordDB);
                RecordDatabaseFile.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Fatal Error: Could not save RecordDatabase file. Exiting...");
                Environment.Exit(-1);
            }
        }

        private void PrintHeader()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("         WSUASTIS         ");
            Console.ResetColor();
            Console.WriteLine("Mode: {0}", User.ToString());
            Console.WriteLine();
        }

        private void InventorySearch()
        {
            string search = string.Empty;
            PrintHeader();
            Console.WriteLine("Inventory Search ");
            Console.Write("Enter item's name, category, or subcategory: ");
            search = Console.ReadLine();
            Console.WriteLine();

            IEnumerable<InventoryRecord> match = RecordDB.Inventory.Where(p => p.Title.Contains(search) || p.Category.Contains(search) || p.Subcategory.Contains(search));
            
            if (match.Count() == 0)
            {
                Console.WriteLine("Nothing matched your search...");
                Console.ReadKey(true);
            }
            else
            {
                Console.WriteLine("Results:\n");
                foreach (InventoryRecord i in match)
                {
                    Console.WriteLine("Title:\t{0}", i.Title);
                    Console.WriteLine("Description:\t{0}", i.Description);
                    Console.WriteLine("Category:\t{0}", i.Category);
                    Console.WriteLine("Subcategory:\t{0}", i.Subcategory);
                    Console.WriteLine("Quantity:\t{0}", i.Quantity);
                    Console.WriteLine("Price:\t{0}", i.Price);
                    Console.WriteLine("PID:\t{0}", i.PID);
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Press any key to return to Main Menu...");
            Console.ReadKey(true);



        }

        private void StartTransaction()
        {
        }

        private void EditInventory()
        {
            PrintHeader();
            Console.WriteLine("Inventory Editor ");
            
        }

    }
}
