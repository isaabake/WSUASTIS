using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUASTIS.RecordTypes
{
    public enum Type { Sale, Return }

    public class Transaction
    {
		public Type Type;
        public uint TransactionID;
        public List<InventoryRecord> Items;
        public int Amount;
    }
}
