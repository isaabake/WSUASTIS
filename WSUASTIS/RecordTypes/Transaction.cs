using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUASTIS.RecordTypes
{
    public enum TransactionType { Sale, Return }

    public class Transaction
    {
		public TransactionType Type;
        public uint TransactionID;
        public List<InventoryRecord> Items;
        public double Amount;
    }
}
