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
    [Serializable]
    [DataContract(Namespace = "")]
    //[MessageContract(IsWrapped = false)]
    public class RateResponse : BaseResponse
    {
        /// <summary>
        /// Rate collection
        /// </summary>
        [DataMember]
        [XmlElementAttribute(Form = XmlSchemaForm.None, Namespace = "")]
        public List<Rate> RateCollection { get; set; }
    }
}
