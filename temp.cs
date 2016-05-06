using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CANDF.RATES.DAL
{
    public class temp
    {
        public temp()
        {
            DBContext context = new DBContext();
            var query = (from CarrierAddOnServices in context.Carrier_Addon_Service
                         join CarrierService1 in context.Carrier_Service on CarrierAddOnServices.Carrier_Service_Id equals CarrierService1.Carrier_Service_ID
                         join CarrierService2 in context.Carrier_Service on CarrierAddOnServices.Carrier_Addon_Service_Id equals CarrierService2.Carrier_Service_ID
                         join Svc in context.ServicesDB on CarrierService2.Mapping_CF_Service_Id equals Svc.Service_ID
                         join CarrierSvcPrtType in context.Carrier_Service_Parent_Type on CarrierService2.Carrier_Service_ID equals CarrierSvcPrtType.Carrier_Service_ID

                         into t
                         from rt in t.DefaultIfEmpty()
                         orderby CarrierService2.Carrier_Service_ID

                         //where CarrierService1.Carrier_Id equals @carrierID && CarrierService1.Mapping_CF_Service_Id equals @carrierServiceId
                         select
                             new
                             {
                                 CarrierService2.Mapping_CF_Service_Id,
                                 Svc.Service_Code,
                                 Svc.Service_Description,
                                 Carrier_Service_Parent_Type = (int?)rt.Carrier_Service_ID
                             }).ToList();


            /*
            int palletID = RateDAO.GetCarrierService(carrierRate).Pallet_ID;// CachedDAO.CarrierServiceCollection.Where(o => o.Carrier_Id == carrierRate.Carrier_Id && o.Carrier_Service_ID == carrierRate.CF_service_ID).FirstOrDefault().Pallet_ID;
            decimal PalletMeasuringQty = 0;
            if (palletID != 0)
            {
                List<Pallet> Pallet = RateDAO.GetPalletByPalletID(palletID);

                decimal PackageLength = 0;
                decimal PackageHeight = 0;
                decimal PackageWidth = 0;

                decimal PalletVolume = 0;
                decimal PalletCubic = 0;

                if (Pallet != null && Pallet.Count > 0)
                {
                    if (request.PackageCollection.Count > 0)
                    {
                        foreach (var item in request.PackageCollection)
                        {
                            PackageLength = Math.Ceiling(Convert.ToDecimal(item.Length) / (Convert.ToDecimal(Pallet[0].Length) * 100));
                            PackageHeight = Math.Ceiling(Convert.ToDecimal(item.Height) / (Convert.ToDecimal(Pallet[0].Height) * 100));
                            PackageWidth = Math.Ceiling(Convert.ToDecimal(item.Height) / (Convert.ToDecimal(Pallet[0].Width) * 100));

                            PalletVolume = PackageLength * PackageHeight * PackageWidth;
                            PalletCubic = Math.Ceiling(Convert.ToDecimal(item.Weight) / Convert.ToDecimal(Pallet[0].Weight));

                            PalletMeasuringQty += PalletVolume > PalletCubic ? PalletVolume : PalletCubic;
                        }
                    }
                }
            }
            measuringQuantity = Convert.ToDouble(PalletMeasuringQty);
            break;

            */


        }
    }
}
