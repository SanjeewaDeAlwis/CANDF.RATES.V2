using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CANDF.RATES.WCF.SERVICE.LIBRARY
{
    /// <summary>
    /// Rate request
    /// </summary>
    [Serializable]
    [DataContract]
    public class RateRequest
    {
        /// <summary>
        /// Sender country name
        /// </summary>
        [DataMember(IsRequired = true)]
        public string SenderCountryName { get; set; }

        /// <summary>
        /// Receiver country name
        /// </summary>
        [DataMember(IsRequired = true)]
        public string ReceiverCountryName { get; set; }

        /// <summary>
        /// Sender suburb name
        /// </summary>
        [DataMember(IsRequired = true)]
        public string SenderSuburbName { get; set; }

        /// <summary>
        /// Receiver suburb name
        /// </summary>
        [DataMember(IsRequired = true)]
        public string ReceiverSuburbName { get; set; }

        /// <summary>
        /// Sender post code
        /// </summary>
        [DataMember(IsRequired = true)]
        public string SenderPostCode { get; set; }

        /// <summary>
        /// Receiver post code
        /// </summary>
        [DataMember(IsRequired = true)]
        public string ReceiverPostCode { get; set; }

        /// <summary>
        /// Customer ID
        /// </summary>
        [DataMember]
        public Guid CustomerID { get; set; }

        /// <summary>
        /// Delivery method ID (Ex: )
        /// TODO: Need to give some examples in the comment
        /// </summary>
        [DataMember]
        public int DeliveryMethodID { get; set; }  

        /// <summary>
        /// Schedule ID (Optional and will override default schedule attached by customer ID)
        /// </summary>
        //[DataMember]
        //public string ScheduleID { get; set; }

        /// <summary>
        /// Type of goods ID (Ex: Any = 11, Box = 2, Pallet = 7)
        /// </summary>
        [DataMember]
        public int PackageType { get; set; }   

        /// <summary>
        /// Package detail collection
        /// </summary>
        [DataMember(IsRequired = true)]
        public PackageCollection PackageCollection { get; set; }

        /// <summary>
        /// Carrier ID
        /// </summary>
        [DataMember]
        public Guid CarrierID { get; set; }

        /// <summary>
        /// Service type ID (Local)
        /// </summary>
        [IgnoreDataMember]
        public int ServiceTypeID { get; set; }

        /// <summary>
        /// Is PO Box delivery required
        /// </summary>
        [DataMember]
        public bool IsPOBoxDeliveryRequired { get; set; }

        /// <summary>
        /// Is dangerous goods delivery required
        /// </summary>
        [DataMember]
        public bool IsDangerousGoodsDeliveryRequired { get; set; }

        /// <summary>
        /// Is residential pickup required
        /// </summary>
        [DataMember]
        public bool IsResidentialPickupRequired { get; set; }

        /// <summary>
        /// Is residential delivery required
        /// </summary>
        [DataMember]
        public bool IsResidentialDeliveryRequired { get; set; }

        /// <summary>
        /// Promo code
        /// </summary>
        [DataMember]
        public string PromoCode { get; set; }

        /// <summary>
        /// Pickup date time
        /// </summary>
        [DataMember]
        public DateTime ShipmentDateTime { get; set; }

        /// <summary>
        /// Is add-on service details required
        /// </summary>
        [DataMember]
        public bool IsAddonServiceDetailsRequired { get; set; }

        #region [IGNORED DATAMEMBERS]

        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Is promotion applied
        /// </summary>
        [IgnoreDataMember]
        public bool IsPromotionApplied { get; set; }

        /// <summary>
        /// Customer schedule ID
        /// </summary>
        [IgnoreDataMember]
        public Guid CustomerScheduleIDActived { get; set; }

        /// <summary>
        /// Sender country
        /// </summary>
        [IgnoreDataMember]
        public Country SenderCountry { get; set; }

        /// <summary>
        /// Receiver country
        /// </summary>
        [IgnoreDataMember]
        public Country ReceiverCountry { get; set; }

        /// <summary>
        /// Sender state
        /// </summary>
        [IgnoreDataMember]
        public State SenderState { get; set; }

        /// <summary>
        /// Receiver state
        /// </summary>
        [IgnoreDataMember]
        public State ReceiverState { get; set; }

        /// <summary>
        /// Sender suburb
        /// </summary>
        [IgnoreDataMember]
        public Suburb SenderSuburb { get; set; }

        /// <summary>
        /// Receiver suburb
        /// </summary>
        [IgnoreDataMember]
        public Suburb ReceiverSuburb { get; set; }

        /// <summary>
        /// Sender suburb
        /// </summary>
        [IgnoreDataMember]
        public string SenderZoneIDCollection { get; set; }

        /// <summary>
        /// Receiver suburb
        /// </summary>
        [IgnoreDataMember]
        public string ReceiverZoneIDCollection { get; set; }

        /// <summary>
        /// GST Percentage
        /// </summary>
        [IgnoreDataMember]
        public double GSTPercentage { get; set; }

        /// <summary>
        /// Customer
        /// </summary>
        //[IgnoreDataMember]
        //public Customer Customer { get; set; }

        #endregion
    }

    /// <summary>
    /// Package
    /// </summary>
    [Serializable]
    [DataContract]
    public class Package
    {
        /// <summary>
        /// Length in centimeters
        /// </summary>
        [DataMember(IsRequired = true)]
        public double Length { set; get; }

        /// <summary>
        /// Width in centimeters
        /// </summary>
        [DataMember(IsRequired = true)]
        public double Width { set; get; }

        /// <summary>
        /// Height in centimeters
        /// </summary>
        [DataMember(IsRequired = true)]
        public double Height { set; get; }

        /// <summary>
        /// Weight in kilograms
        /// </summary>
        [DataMember(IsRequired = true)]
        public double Weight { set; get; }

        /// <summary>
        /// Description of package
        /// </summary>
        [DataMember]
        public string Description { set; get; }

        #region [IGNORE DATA MEMBER]

        private double volume = 0;
        /// <summary>
        /// Volume
        /// </summary>
        [IgnoreDataMember]
        public double Volume
        {
            get
            {
                if (volume == 0)
                {
                    volume = (Length * Width * Height);
                }

                return volume;
            }
        }

        private double cubicVolume;
        /// <summary>
        /// Cubic volume
        /// </summary>
        [IgnoreDataMember]
        public double CubicVolume
        {
            get
            {
                if (cubicVolume == 0)
                {
                    cubicVolume = Volume / 1000000;
                }

                return cubicVolume;
            }
        }

        #endregion
    }

    /// <summary>
    /// Package collection
    /// </summary>
    [Serializable]
    //[DataContract]
    public class PackageCollection : List<Package>
    {
        #region [IGNORE DATA MEMBER]

        /// <summary>
        /// Max length
        /// </summary>
        [IgnoreDataMember]
        public double MaxLength { set; get; }

        /// <summary>
        /// Max width
        /// </summary>
        [IgnoreDataMember]
        public double MaxWidth { set; get; }

        /// <summary>
        /// Max height
        /// </summary>
        [IgnoreDataMember]
        public double MaxHeight { set; get; }

        /// <summary>
        /// Max weight
        /// </summary>
        [IgnoreDataMember]
        public double MaxWeight { set; get; }

        /// <summary>
        /// Max volume
        /// </summary>
        [IgnoreDataMember]
        public double MaxVolumn { set; get; }

        /// <summary>
        /// Max dimension
        /// </summary>
        [IgnoreDataMember]
        public double MaxDimension { get; set; }

        /// <summary>
        /// Total weight
        /// </summary>
        [IgnoreDataMember]
        public double TotalWeight { set; get; }

        /// <summary>
        /// Total volume
        /// </summary>
        [IgnoreDataMember]
        public double TotalVolume { set; get; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void InitializeProperties()
        {
            MaxLength = this.Max(o => o.Length);
            MaxWidth = this.Max(o => o.Width);
            MaxHeight = this.Max(o => o.Height);
            MaxWeight = this.Max(o => o.Weight);
            MaxVolumn = this.Max(o => o.Volume);

            /// Get maximum dimension converted to meters
            MaxDimension = (MaxLength > MaxWidth) ? ((MaxLength > MaxHeight) ? MaxLength : MaxHeight) : ((MaxWidth > MaxHeight) ? MaxWidth : MaxHeight);

            //var sumLength = request.PackageCollection.Sum(o => o.Length);
            //var sumWidth = request.PackageCollection.Sum(o => o.Width);
            //var sumHeight = request.PackageCollection.Sum(o => o.Height);
            TotalWeight = this.Sum(o => o.Weight);
            TotalVolume = this.Sum(o => o.Volume);
        }
    }
}
