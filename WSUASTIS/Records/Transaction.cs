using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUASTIS
{
    enum Type { Sale, Return }

    class Transaction
    {
		public Type Type;
        public uint TransactionID;
        public IList<InventoryRecord> Items;
        public int Amount;
    }
}
