using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CANDF.RATES.WCF.SERVICE.LIBRARY;

namespace CANDF.RATES.WEB.SERVICES
{
    public class Services
    {
        /// <summary>
        /// Get distance
        /// </summary>
        public static double GetDistance(RateRequest request)
        {
            DistanceService.ServiceClient service = new DistanceService.ServiceClient();
            try
            {
                double distanceValue = 0;
                if (request.SenderCountry.ID == 9 && request.ReceiverCountry.ID == 9)
                {
                    string distance = service.getAustraliaDistanceByPostCodeandLocation(request.SenderPostCode, request.SenderSuburb.Name, request.ReceiverPostCode, request.ReceiverSuburb.Name).Distance;
                    double.TryParse(distance, out distanceValue);
                }
                return distanceValue;
            }
            catch //(Exception ex)
            {
                throw;
            }
            finally
            {
                service.Close();
            }
        }
    }
}
