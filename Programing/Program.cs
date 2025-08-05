using System;
using System.Collections.Generic;

namespace Programing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<BankAccount> accounts = new List<BankAccount>();  // List of account 
            int nextId = 1;                                        // ID for the next account 
            BankAccount currentAccount = null;                     // current account 

            while (true)
            {
                Console.Clear();

                if (currentAccount != null)
                {
                    Console.WriteLine($"Current account selected: ID {currentAccount.Id}");
                }
                else
                {
                    Console.WriteLine("No account selected.");
                }


                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Create new account");
                Console.WriteLine("2. View all accounts");
                Console.WriteLine("3. Select account by ID");
                Console.WriteLine("4. Deposit");
                Console.WriteLine("5. Withdraw");
                Console.WriteLine("6. View balance");
                Console.WriteLine("7. Exit");


                Console.Write("Choose an option: ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid input. Try again.");

                    Console.ReadLine();
                    continue;

                    
                }

                if (choice == 1)
                {
                    Console.Write("Enter account name: ");
                    string name = Console.ReadLine();

                    BankAccount newAccount = new BankAccount(nextId++, name);
                    accounts.Add(newAccount);
                    currentAccount = newAccount;

                    Console.WriteLine($" Account '{newAccount.Name}' created with ID: {newAccount.Id}");

                    Console.ReadLine();

                }
                else if (choice == 2)
                {
                    Console.WriteLine("Accounts:");
                    foreach (var acc in accounts)
                    {
                        Console.WriteLine($"ID: {acc.Id},{acc.Name} , Balance: {acc.GetBalance()}");
                    }

                    Console.ReadLine();
                }
                else if (choice == 3)
                {
                    Console.Write("Enter account ID to select: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        var found = accounts.Find(a => a.Id == id);
                        if (found != null)
                        {
                            currentAccount = found;
                            Console.WriteLine($"Account ID {id} selected.");
                        }
                        else
                        {
                            Console.WriteLine("Account not found.");
                        }
                    }
                }
                else if (choice == 4)
                {
                    if (currentAccount == null)
                    {
                        Console.WriteLine("No account selected.");
                        continue;
                    }

                    Console.Write("Enter deposit amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal amount))
                    {
                        currentAccount.Deposit(amount);
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount.");
                    }
                    Console.ReadLine();
                }
                else if (choice == 5)
                {
                    if (currentAccount == null)
                    {
                        Console.WriteLine("No account selected.");
                        continue;
                    }

                    Console.Write("Enter withdraw amount: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal amount))
                    {
                        currentAccount.Withdraw(amount);
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount.");
                    }
                    Console.ReadLine();
                }
                else if (choice == 6)
                {
                    if (currentAccount == null)
                    {
                        Console.WriteLine("No account selected.");
                    }
                    else
                    {
                        Console.WriteLine($"Current balance: {currentAccount.GetBalance()}");
                    }
                    Console.ReadLine();
                }
                else if (choice == 7)
                {
                    Console.WriteLine("Exiting...");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }
            }
        }
    }
}
