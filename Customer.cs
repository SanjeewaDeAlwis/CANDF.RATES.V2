using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CANDF.RATES.WCF.SERVICE.LIBRARY
{
    public class Customer
    {
        /// <summary>
        /// Code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Schedule ID
        /// </summary>
        public Guid ScheduleID { get; set; }

        /// <summary>
        /// Type (Ex: Cash = 1, PAYG = 2, Prepaid = 3, Account = 5)
        /// </summary>
        public int Type { get; set; }
    }
}
