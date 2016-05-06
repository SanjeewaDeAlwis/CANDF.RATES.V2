using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CANDF.RATES.BLL
{
    public enum ServiceTypeID
    {
        Local = 1,
        Country = 2,
        Interstate = 3,
        International = 4,
        ANY = 5,
        Hourly = 6,
        Prepaid = 7,


    }

    /// <summary>
    /// Primary rate types
    /// </summary>
    public enum PrimaryRateTypes
    {
        ZONE_TO_ZONE = 1,
        TIME = 5,
        DISTANCE = 3,
        PERCENTAGE = 10,
        ENTER = 9
    }

    /// <summary>
    /// Secondary rate type
    /// </summary>
    public enum SecondaryRateTypes
    {
        FLAT = 11,
        COMPLEX = -999 /// TODO: Need to set the exact value from DB
    }

    /// <summary>
    /// Calculation Basic ID
    /// </summary>
    public enum CalculationBasis
    {
        RANGE_MULTIPER = 2,
        RANGE_ACCUMULATOR = 1,
        RANGE_Total = 3
    }

    /// <summary>
    /// Customer Types
    /// </summary>
    public enum CustomerTypes
    {
        Cash = 1,
        PAYG = 2,
        Prepaid = 3,
        InvoicedDirectDebit = 4,
        InvoicedAccountholders = 5
    }

    /// <summary>
    /// Carrier code collection in string format
    /// </summary>
    public static class CarrierCodes
    {
        public static string ANY = "[ANY]";
        public static string AaE = "AaE";
        public static string AlliedExpress = "AlliedExpress";
        public static string AlphaMail = "AlphaMail";
        public static string CFAP = "CFAP";
        public static string CP = "CP";
        public static string CPHWS = "CPHWS";
        public static string DHL = "DHL";
        public static string DHT = "DHT";
        public static string DSTM = "DSTM";
        public static string DYN_ENV = "DYN_ENV";
        public static string DYN_JINDEX = "DYN_JINDEX";
        public static string DYN_STYLETEX = "DYN_STYLETEX";
        public static string DYNAMIC = "DYNAMIC";
        public static string DYNAMIC2 = "DYNAMIC2";
        public static string DYNAMICDIS = "DYNAMICDIS";
        public static string Eparcel = "Eparcel";
        public static string EparcelFM = "EparcelFM";
        public static string EparcelRS = "EparcelRS";
        public static string Fastway = "Fastway";
        public static string GC = "GC";
        public static string HTE = "HTE";
        public static string Northline = "Northline";
        public static string SHAWS = "SHAWS";
        public static string Swift = "Swift";
        public static string TasFreight = "TasFreight";
        public static string TNT = "TNT";
        public static string TNT_INT = "TNT_INT";
        public static string TNTDGSS = "TNTDGSS";
        public static string TOLL = "TOLL";
        public static string TOLLIPEC = "TOLLIPEC";
        public static string TOLLIPECCOI = "TOLLIPECCOI";
        public static string TOLLIPECCON = "TOLLIPECCON";
        public static string TOWER = "TOWER";
        public static string TTL = "TTL";
        public static string VELLEX = "VELLEX";
        public static string WAFG = "WAFG";
    }

    public class Constants
    {
    }
}
