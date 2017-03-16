using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTrack.ServiceAPI;

namespace OpenTrack.Requests
{
    public class DeleteRepairOrderLinesRequest: IRequest
    {
        public DeleteRepairOrderLinesRequest(string EnterpriseCode, string CompanyNumber) : base(EnterpriseCode, CompanyNumber)
        {
            LineItems = new List<UpdateLineItem>();
        }

        public DeleteRepairOrderLinesRequest(string EnterpriseCode, string CompanyNumber, string ServerName) : base(EnterpriseCode, CompanyNumber, ServerName)
        {
            LineItems = new List<UpdateLineItem>();
        }
        
        public string RepairOrderNumber { get; set; }

        public IList<UpdateLineItem> LineItems { get; set; }

        internal virtual ServiceAPI.DeleteRepairOrderLinesRequest Request
        {
            get { return new ServiceAPI.DeleteRepairOrderLinesRequest
            {
               RepairOrderNumber = this.RepairOrderNumber,
               LineItems = this.LineItems == null ? new UpdateLineItem[0] : this.LineItems.ToArray()
            }; }
        }
    }
}
