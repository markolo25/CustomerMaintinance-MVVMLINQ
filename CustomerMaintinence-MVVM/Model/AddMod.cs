using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerMaintinence_MVVM.Model
{
    /// <summary>
    /// Message Class that is responsible for controlling the Child Window
    /// </summary>
    public class AddMod
    {
        public Boolean isMod { get; set; }
        public int CustomerID { get; set; }

        /// <summary>
        /// Creates an AddMod Message Object
        /// </summary>
        /// <param name="isMod"></param>
        /// <param name="CustomerID"></param>
        public AddMod(Boolean isMod, int CustomerID)
        {
            this.isMod = isMod;
            this.CustomerID = CustomerID;
        }
    }
}
