using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace CANDF.RATES.WCF.SERVICE.LIBRARY
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class BaseLibrary
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        [DataMember]
        //[XmlElementAttribute(Form = XmlSchemaForm.None, Namespace = "")]
        public string Code { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        //[XmlElementAttribute(Form = XmlSchemaForm.None, Namespace = "")]
        public string Description { get; set; }

        public override string ToString()
        {
            //return base.ToString();
            return String.Format("{0}, {1}", this.Code, this.Name);
        }
    }
}
