using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CANDF.RATES.WCF.SERVICE.LIBRARY
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class Service : BaseLibrary
    {
        #region [IGNORE DATA MEMBER]

        /// <summary>
        /// CF Service ID
        /// </summary>
        [IgnoreDataMember]
        public int CFServiceID { get; set; }

        /// <summary>
        /// Service charge
        /// </summary>
        [IgnoreDataMember]
        public double Charge { get; set; }

        /// <summary>
        /// Carrier ID
        /// </summary>
        [IgnoreDataMember]
        public Guid CarrierID { get; set; }

        /// <summary>
        /// Measuring quantity
        /// </summary>
        [IgnoreDataMember]
        public decimal MeasuringQuantity { get; set; }

        /// <summary>
        /// Complex rate ID
        /// </summary>
        [IgnoreDataMember]
        public int ComplexRateID { get; set; }

        /// <summary>
        /// Carrier service parent type
        /// </summary>
        [IgnoreDataMember]
        public int ParentTypeID { get; set; }

        /// <summary>
        /// Carrier service sorting order ID
        /// </summary>
        //[IgnoreDataMember]
        //public int SortOrderID { get; set; }

        #endregion

        public override string ToString()
        {
            //return base.ToString();
            return String.Format("{0}, {1}, {2}", this.CFServiceID, this.Name, this.Charge);
        }
    }

    /// <summary>
    /// Service collection
    /// </summary>
    [Serializable]
    //[DataContract]
    public class ServiceCollection : List<Service>
    {
        private double totalCharge = 0;
        /// <summary>
        /// Total service charges
        /// </summary>
        [IgnoreDataMember]
        public double TotalCharge
        {
            get
            {
                if (totalCharge == 0)
                {
                    totalCharge = this.Sum(o => o.Charge);
                }

                return totalCharge;
            }
        }
    }
}
