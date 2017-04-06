using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerMaintinence_MVVM.Model
{
    public class AddMod
    {
        public Boolean isMod { get; set; }
        public int CustomerID { get; set; }

        public AddMod(Boolean isMod, int CustomerID)
        {
            this.isMod = isMod;
            this.CustomerID = CustomerID;
        }
    }
}
