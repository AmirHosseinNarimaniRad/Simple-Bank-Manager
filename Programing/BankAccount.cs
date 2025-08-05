using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programing
{
    public class BankAccount
    {
        private decimal Balance ;
        public int Id { get; set; }
        public string Name { get; }


        public BankAccount(int id , string name ) 
        { 
            Balance = 0 ;

            Id = id ;

            Name = name;
        }



        public void Deposit(decimal Amount)
        {
            if (Amount > 0)
            {
                Balance += Amount;
                Console.WriteLine("deposit successful");
            }
            else
            {
                Console.WriteLine("Amount must be greater than zero.");
            }
        }


        public void Withdraw(decimal Amount)
        {

            if (Amount < 0)
            {
                Console.WriteLine("Enter a valid amount! ");
            }
            else if (Balance < Amount)
            {
                Console.WriteLine("Insufficient funds ");
            }
            else
            {
                Balance -= Amount;
                Console.WriteLine("Withdraw successful. ");
            }
        }


        public decimal GetBalance()
        {
            return Balance;
        }
        
    }
}
