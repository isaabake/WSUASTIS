﻿using System;
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
                SaveDatabaseChanges();
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
        /// Starts the system.
        /// </summary>
        public void Run()
        {
            char input = '0';

            //Main Menu
            do
            {
                PrintHeader();
                Console.WriteLine("Main Menu ");
                Console.WriteLine("1) Inventory Search");
                Console.WriteLine("2) Start Transaction");
                Console.WriteLine("3) Edit Inventory");
                Console.WriteLine("4) Print Receipt");
                Console.WriteLine("0) Exit");
                Console.WriteLine();
                Console.Write("Enter Selection: ");

                input = Console.ReadKey(true).KeyChar;
                while (input != '1' && input != '2' && input != '3' && input != '4' && input != '0')
                {
                    input = Console.ReadKey(true).KeyChar;
                }
                Console.WriteLine();

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
                    EditInventory();
                }
                else if (input == '4')
                {
                    PrintReceipt();
                }


            } while (input != '0');
        }

        /// <summary>
        /// Saves all changes to the database
        /// </summary>
        private void SaveDatabaseChanges()
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

        /// <summary>
        /// Prints Console header
        /// </summary>
        private void PrintHeader()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("         WSUASTIS         ");
            Console.ResetColor();
            Console.WriteLine("Mode: {0}", User.ToString());
            Console.WriteLine();
        }

        /// <summary>
        /// Search inventory by name, category, or subcategory
        /// </summary>
        private void InventorySearch()
        {
            string search = string.Empty;
            PrintHeader();
            Console.WriteLine("Inventory Search");
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
                    Console.WriteLine("Title:\t\t{0}", i.Title);
                    Console.WriteLine("Description:\t{0}", i.Description);
                    Console.WriteLine("Category:\t{0}", i.Category);
                    Console.WriteLine("Subcategory:\t{0}", i.Subcategory);
                    Console.WriteLine("Quantity:\t{0}", i.Quantity);
                    Console.WriteLine("Price:\t\t{0}", i.Price);
                    Console.WriteLine("PID:\t\t{0}", i.PID);
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Press any key to return to Main Menu...");
            Console.ReadKey(true);



        }

        /// <summary>
        /// Start a sale or return transaction
        /// </summary>
        private void StartTransaction()
        {
            char input = '0';
            //Inventory Editor
            do
            {
                PrintHeader();
                Console.WriteLine("Transaction Menu");
                Console.WriteLine("1) Sale");
                Console.WriteLine("2) Return");
                Console.WriteLine("0) Exit");
                Console.WriteLine();
                Console.Write("Enter selection: ");

                input = Console.ReadKey(true).KeyChar;
                while (input != '1' && input != '2' && input != '0')
                {
                    input = Console.ReadKey(true).KeyChar;
                }
                Console.WriteLine();

                Transaction newTransaction = new Transaction();
                if (RecordDB.Transactions.Count == 0)
                {
                    newTransaction.TransactionID = 1;
                }
                else
                {
                    newTransaction.TransactionID = RecordDB.Transactions.Max(t => t.TransactionID) + 1; //Generate the next sequential ID
                }

                //Sale transaction
                if (input == '1')
                {
                    newTransaction.Type = TransactionType.Sale;
                    //Get items for transaction
                    string line;
                    do
                    {
                        int PID;
                        Console.Write("Enter Item PID: ");
                        line = Console.ReadLine();
                        while (!int.TryParse(line, out PID))
                        {
                            if (line == "")
                            {
                                break;
                            }
                            Console.Write("Enter a valid PID or blank to finish: ");
                            line = Console.ReadLine();
                        }
                        if (line == "")
                        {
                            break;
                        }

                        //Find item in db
                        InventoryRecord dbitem = RecordDB.Inventory.Find(p => p.PID == PID);
                        if (dbitem == null)
                        {
                            Console.WriteLine("Could not find an item with PID = {0}", PID);
                            continue;
                        }

                        //Get number of said item
                        uint quantity;
                        Console.Write("Quantity: ");
                        while (!uint.TryParse(Console.ReadLine(), out quantity))
                        {
                            Console.Write("Quantity: ");
                        }
                        //Copy the item into the transaction with the specified quantity
                        InventoryRecord item = new InventoryRecord();
                        item.PID = dbitem.PID;
                        item.Price = dbitem.Price;
                        item.Subcategory = dbitem.Subcategory;
                        item.Title = dbitem.Title;
                        item.Category = dbitem.Category;
                        item.Description = dbitem.Description;
                        item.Quantity = quantity;
                        dbitem.Quantity -= quantity;
                                    
                        newTransaction.Items.Add(item);
                    } while (line != "");

                    //Ask for discount
                    Console.Write("Discount? (Member, Student, Employee, FoFree, Custom): ");
                    line = Console.ReadLine();
                    while (line != "Member" && line != "Student" && line != "Employee" && line != "FoFree" && line != "Custom" && line != "")
                    {
                        Console.Write("Discount? (Member, Student, Employee, FoFree, Custom): ");
                        line = Console.ReadLine();
                    }
                    double discount = 0;
                    if (line == "Member")
                    {
                        discount = .05;
                    }
                    else if (line == "Student")
                    {
                        discount = .01;
                    }
                    else if (line == "Employee")
                    {
                        discount = .33;
                    }
                    else if (line == "FoFree")
                    {
                        discount = 1;
                    }
                    else if (line == "Custom")
                    {
                        Console.Write("Please enter manager password: ");
                        if (Console.ReadLine() != "123")
                        {
                            Console.WriteLine("Invalid password...");
                        }
                        Console.Write("Enter discount %: ");
                        line = Console.ReadLine();
                        while (!double.TryParse(line, out discount))
                        {
                            if (line == "")
                            {
                                break;
                            }
                            Console.Write("Enter discount %: ");
                            line = Console.ReadLine();
                        }
                    }

                    //Sum up total
                    foreach (InventoryRecord i in newTransaction.Items)
                    {
                        newTransaction.Amount += (i.Price - i.Price * discount) * i.Quantity;
                    }

                    Console.WriteLine("Total:\t{0}", newTransaction.Amount);
                    RecordDB.Transactions.Add(newTransaction);
                    SaveDatabaseChanges();
                }
                else if (input == '2')
                {
                    newTransaction.Type = TransactionType.Return;

                    string line;
                    int TransactionID;
                    Console.Write("Enter Transaction ID: ");
                    line = Console.ReadLine();
                    while (!int.TryParse(line, out TransactionID))
                    {
                        Console.Write("Enter Transaction ID: ");
                        line = Console.ReadLine();
                    }
                    //Find Transaction in db
                    Transaction OldTransaction = RecordDB.Transactions.Find(t => t.TransactionID == TransactionID);
                    if (OldTransaction == null)
                    {
                        Console.WriteLine("Could not find a Transaction with ID = {0}", TransactionID);
                    }

                    //Get item(s) to return
                    do
                    {
                        int PID;
                        Console.Write("Enter Item PID: ");
                        line = Console.ReadLine();
                        while (!int.TryParse(line, out PID))
                        {
                            if (line == "")
                            {
                                break;
                            }
                            Console.Write("Enter a valid PID: ");
                            line = Console.ReadLine();
                        }
                        if (line == "")
                        {
                            break;
                        }

                        //Find item in transaction
                        InventoryRecord item = OldTransaction.Items.Find(p => p.PID == PID);
                        if (item == null)
                        {
                            Console.WriteLine("Could not find an item with PID = {0}", PID);
                            continue;
                        }

                        //Get number of said item to return
                        uint quantity;
                        Console.Write("Quantity: ");
                        while (!uint.TryParse(Console.ReadLine(), out quantity))
                        {
                            Console.Write("Quantity: ");
                        }

                        //Copy the item into the transaction with the specified quantity
                        InventoryRecord ReturnItem = new InventoryRecord(); //The item to be recorded on the new(return) transaction
                        ReturnItem.PID = item.PID;
                        ReturnItem.Price = item.Price;
                        ReturnItem.Subcategory = item.Subcategory;
                        ReturnItem.Title = item.Title;
                        ReturnItem.Category = item.Category;
                        ReturnItem.Description = item.Description;
                        ReturnItem.Quantity = quantity;

                        newTransaction.Items.Add(ReturnItem);
                        newTransaction.Amount -= ReturnItem.Price * quantity;

                    } while (line != "");

                    Console.WriteLine("Total:\t{0}", newTransaction.Amount);
                    RecordDB.Transactions.Add(newTransaction);
                    SaveDatabaseChanges();
                    
                }

                Console.WriteLine("Press any key to return to Transaction Menu...");
                Console.ReadKey(true);
            } while (input != '0');
        }

        /// <summary>
        /// Prints a Receipt to a file for a transaction
        /// </summary>
        private void PrintReceipt()
        {
            int TransactionID;
            Console.WriteLine();
            Console.Write("Enter TransactionID to print receipt: ");
            while (!int.TryParse(Console.ReadLine(), out TransactionID))
            {
                Console.Write("Please enter a valid TransactionID number: ");
            }
            Transaction trans = RecordDB.Transactions.Find(t => t.TransactionID == TransactionID);  //Find transaction
            if (trans == null)
            {
                Console.WriteLine("Could not find a Transaction with TransactionID = {0}", TransactionID);
            }
            else
            {
                using (StreamWriter receiptFile = new StreamWriter(string.Format("receipt_{0}.txt", TransactionID)))
                {
                    receiptFile.WriteLine("Transaction ID:\t\t{0}", trans.TransactionID);
                    receiptFile.WriteLine("Transaction Type:\t{0}", trans.Type.ToString());
                    receiptFile.WriteLine();
                    receiptFile.WriteLine("Items");
                    receiptFile.WriteLine("-------------------------------------------------");
                    foreach (InventoryRecord item in trans.Items)
                    {
                        receiptFile.WriteLine("Item:\t\t{0}", item.Title);
                        receiptFile.WriteLine("Description:\t{0}", item.Description);
                        receiptFile.WriteLine("Category:\t{0}", item.Category);
                        receiptFile.WriteLine("Subcategory:\t{0}", item.Subcategory);
                        receiptFile.WriteLine("Quantity:\t{0}", item.Quantity);
                        receiptFile.WriteLine("Price:\t\t{0}", item.Price);
                        receiptFile.WriteLine("PID:\t\t{0}", item.PID);
                        receiptFile.WriteLine();
                    }
                    receiptFile.WriteLine("-------------------------------------------------");
                    receiptFile.WriteLine();
                    receiptFile.WriteLine("Total Amount:\t\t{0}", trans.Amount);

                    receiptFile.Close();
                }

                Console.WriteLine();
                Console.WriteLine("Receipt printed: \"receipt_{0}.txt\"", TransactionID);
                Console.WriteLine("Press any key to return to Main Menu...");
                Console.ReadKey(true);

            }
        }

        /// <summary>
        /// Edit or create inventory items. Requires manager elevation.
        /// </summary>
        private void EditInventory()
        {
            Console.Write("Please enter manager password: ");
            if (Console.ReadLine() != "123")
            {
                Console.WriteLine("Invalid password...");
                return;
            }

            User = UserType.Manager;

            char input = '0';
            //Inventory Editor
            do
            {
                PrintHeader();
                Console.WriteLine("Inventory Editor");
                Console.WriteLine("1) Add New Item");
                Console.WriteLine("2) Edit Existing Item");
                Console.WriteLine("0) Exit");
                Console.WriteLine();
                Console.Write("Enter selection: ");

                input = Console.ReadKey(true).KeyChar;
                while (input != '1' && input != '2' && input != '0')
                {
                    input = Console.ReadKey(true).KeyChar;
                }
                Console.WriteLine();

                InventoryRecord item;
                if (input == '1')
                {
                    item = new InventoryRecord();
                    if (RecordDB.Inventory.Count == 0)
                    {
                        item.PID = 1;
                    }
                    else
                    {
                        item.PID = RecordDB.Inventory.Max(p => p.PID) + 1; //Generate the next sequential PID
                    }
                    RecordDB.Inventory.Add(item);
                }
                else if (input == '2')
                {
                    int PID;
                    Console.WriteLine();
                    Console.Write("Enter PID of item to edit: ");
                    while (!int.TryParse(Console.ReadLine(), out PID))
                    {
                        Console.Write("Please enter a valid PID number: ");
                    }
                    item = RecordDB.Inventory.Find(p => p.PID == PID);
                    if (item == null)
                    {
                        Console.WriteLine("Could not find an item with PID = {0}", PID);
                        continue;
                    }
                    else
                    {
                        //Print item
                        Console.WriteLine("Editing item:");
                        Console.WriteLine("Title:\t\t{0}", item.Title);
                        Console.WriteLine("Description:\t{0}", item.Description);
                        Console.WriteLine("Category:\t{0}", item.Category);
                        Console.WriteLine("Subcategory:\t{0}", item.Subcategory);
                        Console.WriteLine("Quantity:\t{0}", item.Quantity);
                        Console.WriteLine("Price:\t\t{0}", item.Price);
                        Console.WriteLine("PID:\t\t{0}", item.PID);
                        Console.WriteLine();
                    }
                }
                else  //'0' case
                {
                    break;
                }

                //Add or Edit both need to enter information
                Console.Write("Title: ");
                item.Title = Console.ReadLine();

                Console.Write("Description: ");
                item.Description = Console.ReadLine();

                Console.Write("Category: ");
                item.Category = Console.ReadLine();

                Console.Write("Subcategory: ");
                item.Subcategory = Console.ReadLine();

                uint quantity = 0;
                Console.Write("Quantity: ");
                while (!uint.TryParse(Console.ReadLine(), out quantity))
                {
                    Console.Write("Please enter a valid Quantity: ");
                }
                item.Quantity = quantity;

                double price = 0;
                Console.Write("Price: ");
                while (!double.TryParse(Console.ReadLine(), out price))
                {
                    Console.Write("Please enter a valid Price: ");
                }
                item.Price = price;

                SaveDatabaseChanges();


            } while (input != '0');

            User = UserType.Employee;

        }

    }
}
