using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CANDF.RATES.WCF.SERVICE.LIBRARY
{

    public partial class Carrier
    {
        /// <summary>
        /// Create a new Carrier object.
        /// </summary>
        /// <param name="ID">Initial value of the Carrier_Id property.</param>
        /// <param name="name">Initial value of the Carrier_Name property.</param>
        /// <param name="carrier_Quality">Initial value of the Carrier_Quality property.</param>
        /// <param name="carrier_Volume">Initial value of the Carrier_Volume property.</param>
        /// <param name="created_By">Initial value of the Created_By property.</param>
        /// <param name="created_On">Initial value of the Created_On property.</param>
        /// <param name="last_Updated_By">Initial value of the Last_Updated_By property.</param>
        /// <param name="last_Updated_On">Initial value of the Last_Updated_On property.</param>
        /// <param name="code">Initial value of the Code property.</param>
        /// <param name="web_Address">Initial value of the Web_Address property.</param>
        /// <param name="acount_Number">Initial value of the Acount_Number property.</param>
        /// <param name="connote_Prefix">Initial value of the Connote_Prefix property.</param>
        /// <param name="login">Initial value of the Login property.</param>
        /// <param name="password">Initial value of the Password property.</param>
        /// <param name="booking_Phone">Initial value of the Booking_Phone property.</param>
        /// <param name="booking_Fax">Initial value of the Booking_Fax property.</param>
        /// <param name="admin_Phone">Initial value of the Admin_Phone property.</param>
        /// <param name="admin_Fax">Initial value of the Admin_Fax property.</param>
        /// <param name="rep_Name">Initial value of the Rep_Name property.</param>
        /// <param name="rep_Phone">Initial value of the Rep_Phone property.</param>
        /// <param name="rep_Email">Initial value of the Rep_Email property.</param>
        /// <param name="remittance_Advice_Email">Initial value of the Remittance_Advice_Email property.</param>
        /// <param name="credit_Request_Email">Initial value of the Credit_Request_Email property.</param>
        /// <param name="additional_Email">Initial value of the Additional_Email property.</param>
        /// <param name="is_Integrate_Ftp">Initial value of the Is_Integrate_Ftp property.</param>
        /// <param name="is_Integrate_WebService">Initial value of the Is_Integrate_WebService property.</param>
        /// <param name="integrate_Ftp_Host">Initial value of the Integrate_Ftp_Host property.</param>
        /// <param name="integrate_Ftp_User">Initial value of the Integrate_Ftp_User property.</param>
        /// <param name="integrate_Ftp_Pwd">Initial value of the Integrate_Ftp_Pwd property.</param>
        /// <param name="integrate_Web_Service_Url">Initial value of the Integrate_Web_Service_Url property.</param>
        /// <param name="integrate_WebService_Token">Initial value of the Integrate_WebService_Token property.</param>
        /// <param name="is_Enable_Ftp">Initial value of the Is_Enable_Ftp property.</param>
        /// <param name="row_Version">Initial value of the Row_Version property.</param>
        /// <param name="skype_Id">Initial value of the Skype_Id property.</param>
        /// <param name="isVoid">Initial value of the IsVoid property.</param>
        /// <param name="booking_Email">Initial value of the Booking_Email property.</param>
        /// <param name="is_Email_Booking">Initial value of the Is_Email_Booking property.</param>
        /// <param name="booking_Emai_From">Initial value of the Booking_Emai_From property.</param>
        /// <param name="receiverSender_Email">Initial value of the ReceiverSender_Email property.</param>
        /// <param name="carrier_Email_CC">Initial value of the Carrier_Email_CC property.</param>
        /// <param name="isActive">Initial value of the Is_Active_Carrier property.</param>
        /// <param name="is_Private_Carrier_Integration">Initial value of the Is_Private_Carrier_Integration property.</param>
        public static Carrier CreateCarrier(Guid ID, String name, String code, Boolean isActive, 
            bool isAllowPOBoxDelivery, bool isAllowDGGoodsDelivery, bool isAllowResidentialPickup, bool isAllowResidentialDelivery)
        {
            Carrier carrier = new Carrier();
            carrier.ID = ID;
            carrier.Name = name;
            carrier.Code = code;
            carrier.IsActive = isActive;
            carrier.IsAllowDGGoodsDelivery = isAllowDGGoodsDelivery;
            carrier.IsAllowPOBoxDelivery = isAllowPOBoxDelivery;
            carrier.IsAllowResidentialDelivery = isAllowResidentialDelivery;
            carrier.IsAllowResidentialPickup = isAllowResidentialPickup;
            return carrier;
        }

        /// <summary>
        /// Carrier ID.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Carrier Name.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Carrier Code.
        /// </summary>
        public String Code { get; set; }

        /// <summary>
        /// Is active carrier.
        /// </summary>
        public Boolean IsActive { get; set; }

        /// <summary>
        /// Is allow PO Box delivery.
        /// </summary>
        public Boolean IsAllowPOBoxDelivery { get; set; }

        /// <summary>
        /// Is allow DG goods delivery.
        /// </summary>
        public Boolean IsAllowDGGoodsDelivery { get; set; }

        /// <summary>
        /// Is allow residential pickup.
        /// </summary>
        public Boolean IsAllowResidentialPickup { get; set; }

        /// <summary>
        /// Is allow residential delivery.
        /// </summary>
        public Boolean IsAllowResidentialDelivery { get; set; }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return base.ToString();
            return String.Format("{0}, {1}", this.Code, this.Name);
        }
    }
}
