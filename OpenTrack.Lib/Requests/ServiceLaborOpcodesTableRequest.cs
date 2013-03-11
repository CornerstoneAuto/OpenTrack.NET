﻿using System;
using System.Xml.Linq;

namespace OpenTrack.Requests
{
    public class ServiceLaborOpcodesTableRequest : IRequest<ServiceLaborOpcodesTable>
    {
        public ServiceLaborOpcodesTableRequest(String EnterpriseCode, String DealerCode, String ServerName)
            : base(EnterpriseCode, DealerCode, ServerName)
        {
        }

        internal override XElement XML
        {
            get
            {
                return new XElement("ServiceLaborOpcodesTableRequest", this.Dealer);
            }
        }
    }
}