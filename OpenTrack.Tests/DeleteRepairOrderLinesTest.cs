using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTrack.Requests;
using OpenTrack.ServiceAPI;
using Xunit;
using DeleteRepairOrderLinesRequest = OpenTrack.Requests.DeleteRepairOrderLinesRequest;

namespace OpenTrack.Tests
{
    /// <summary>
    /// In order to run these tests you should have a Repair Order and a ServiceLineNumber that exists in the DMS already.
    /// </summary>
    public class DeleteRepairOrderLinesTest
    {

        [Fact(Skip = "This should not be automated since it will actually update a Repair Order.")]
        public void Test_Delete_RepairOrder_Line()
        {
            var api = Credentials.GetAPI();
            var result = api.DeleteRepairOrderLines(
                new DeleteRepairOrderLinesRequest(Credentials.EnterpriseCode, Credentials.DealerNumber)
                {
                    RepairOrderNumber = "6041150",
                    LineItems = new List<UpdateLineItem>()
                    {
                        new UpdateLineItem()
                        {
                            ServiceLineNumber = 2
                        }
                    }
                }
                );
            Assert.False(result.Failure != null);
        }

        [Fact(Skip = "This should not be automated since it will actually update a Repair Order.")]
        public void Test_Delete_RepairOrder_Line_PartLines_By_PartNumber()
        {
            var api = Credentials.GetAPI();
            var result = api.DeleteRepairOrderLines(
                new DeleteRepairOrderLinesRequest(Credentials.EnterpriseCode, Credentials.DealerNumber)
                {
                    RepairOrderNumber = "6041150",
                    LineItems = new List<UpdateLineItem>()
                    {
                        new UpdateLineItem()
                        {
                            ServiceLineNumber = 2,
                            Part = new PartBase()
                            {
                                PartNumber = "ABCDPART2"
                            }
                        },
                        new UpdateLineItem()
                        {
                            ServiceLineNumber = 2,
                            Part = new PartBase()
                            {
                                PartNumber = "60100S30A90ZZ"
                            }
                        },
                    }
                }
                );
            Assert.False(result.Failure != null);
        }
    }
}
