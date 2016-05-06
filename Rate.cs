using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.ServiceModel;

namespace CANDF.RATES.WCF.SERVICE.LIBRARY
{
    /// <summary>
    /// Rate details
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    //[MessageContract(IsWrapped = false)]
    public class Rate
    {
        /// <summary>
        /// Main service
        /// </summary>
        [DataMember]
        public Service MainService { get; set; }

        /// <summary>
        /// Add on service collection
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [XmlElementAttribute(Form = XmlSchemaForm.None, Namespace="")]
        public ServiceCollection AddonServiceCollection { get; set; }

        /// <summary>
        /// Total GST charge for the carrier service
        /// </summary>
        [DataMember]
        public double GST { get; set; }

        /// <summary>
        /// Total discounts
        /// </summary>
        //[DataMember]
        //public double DiscountValue { get; set; }

        /// <summary>
        /// Discount percentage
        /// </summary>
        [DataMember]
        public double DiscountPercentage { get; set; }

        /// <summary>
        /// Total charge for the carrier service
        /// </summary>
        [DataMember]
        public double Charge { get; set; }

        /// <summary>
        /// Carrier ID
        /// </summary>
        [DataMember]
        public Guid CarrierID { get; set; }

        /// <summary>
        /// Carrier Name
        /// </summary>
        [DataMember]
        public string CarrierName { get; set; }

        /// <summary>
        /// Delivery time
        /// </summary>
        [DataMember]
        public string ETAText { get; set; }

        /// <summary>
        /// Estimated time for delivery
        /// </summary>
        [DataMember]
        public DateTime ETAValue { get; set; }

        /// <summary>
        /// Cut off time of the service
        /// </summary>
        [DataMember]
        public string CutOffTime { get; set; }

        /// <summary>
        /// Carrier service related messages
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [XmlElementAttribute(Form = XmlSchemaForm.None, Namespace = "")]
        public List<string> MessageCollection { get; set; }

        #region [IGNORE DATA MEMBER]

        /// <summary>
        /// Markup percentage
        /// </summary>
        [IgnoreDataMember]
        public double MarkupPercentage { get; set; }

        private Carrier carrier;
        /// <summary>
        /// Carrier
        /// </summary>
        [IgnoreDataMember]
        public Carrier Carrier
        {
            get
            {
                return carrier;
            }
            set
            {
                carrier = value;
                if (carrier != null)
                {
                    CarrierName = carrier.Name;
                    CarrierID = carrier.ID;
                }
                else
                {
                    CarrierName = String.Empty;
                    CarrierID = Guid.Empty;
                }
            }
        }

        #endregion

        /// <summary>
        /// string presentation of the rate 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", this.CarrierName, this.MainService.Name, this.Charge);
        }

        /// <summary>
        /// Calculate final charge for the rate
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="markUpPercentage"></param>
        /// <param name="GSTPercentage"></param>
        /// <returns></returns>
        public static double CalculateFinalCharge(Rate rate, double GSTPercentage)
        {
            var totalCharges = rate.MainService.Charge + rate.AddonServiceCollection.TotalCharge;
            var totalMarkup = totalCharges * rate.MarkupPercentage / 100;
            //rate.GST = System.Math.Round(((totalCharges + totalMarkup) * GSTPercentage / 100), 2, MidpointRounding.AwayFromZero);
            //rate.Charge = System.Math.Round((totalCharges + totalMarkup + rate.GST), 2, MidpointRounding.AwayFromZero);            
            var totalGST = ((totalCharges + totalMarkup) * GSTPercentage / 100);
            rate.GST = GetCeilingWith2DecimalPlaces(totalGST);
            rate.Charge = GetCeilingWith2DecimalPlaces((totalCharges + totalMarkup + totalGST));
            return rate.Charge;
        }

        /// <summary>
        /// Get ceiling value with two decimal points
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static double GetCeilingWith2DecimalPlaces(double value)
        {
            /// Use the below code to round up decimal points to ceiling for 2 decimal places
            return System.Math.Ceiling(value * 100) / 100;
        }
    }
}
