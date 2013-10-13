using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUASTIS
{
    enum UserType { Manager, Employee };
    public class WSUASTIS
    {
        UserType User = UserType.Employee;

        public WSUASTIS()
        {

        }

        /// <summary>
        /// Starts the system
        /// </summary>
        public void Run()
        {
            Console.WriteLine("*****Welcome to WSUASTIS*****");
            Console.WriteLine("Mode: {0}", User.ToString());
        }


    }
}
