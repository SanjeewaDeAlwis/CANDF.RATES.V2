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
    public enum Status
    {
        [EnumMember]
        SUCCESS = 1,
        [EnumMember]
        FAIL = 0
    }

    [Serializable]
    [DataContract(Namespace = "")]
    //[MessageContract(IsWrapped = false)]
    public class BaseResponse
    {
        /// <summary>
        /// Status (Success or Fail)
        /// </summary>
        [DataMember]
        public Status Status { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Detailed message collection
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        //[XmlElementAttribute(Form = XmlSchemaForm.None, Namespace = "")]
        public List<string> DetailedMessageCollection { get; set; }
    }
}