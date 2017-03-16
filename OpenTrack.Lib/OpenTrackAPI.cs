using OpenTrack.ManualSoap;
using OpenTrack.ManualSoap.Common;
using OpenTrack.ManualSoap.Requests;
using OpenTrack.ManualSoap.Responses;
using OpenTrack.Requests;
using OpenTrack.Responses;
using OpenTrack.ServiceAPI;
using OpenTrack.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;
using GetClosedRepairOrdersRequest = OpenTrack.Requests.GetClosedRepairOrdersRequest;
using UpdateRepairOrderLinesRequest = OpenTrack.Requests.UpdateRepairOrderLinesRequest;
using VehicleLookupResponse = OpenTrack.Responses.VehicleLookupResponse;
using VehicleLookupResponseVehicle = OpenTrack.Responses.VehicleLookupResponseVehicle;

namespace OpenTrack
{
    // Staging URLs
    // https://otstaging.arkona.com/OpenTrack/WebService.asmx
    // https://otstaging.arkona.com/opentrack/ServiceAPI.asmx
    // https://otstaging.arkona.com/OpenTrack/PartsAPI.asmx

    // Pre-Prod URLs
    // https://otcert.arkona.com/OpenTrack/WebService.asmx
    // https://otcert.arkona.com/OpenTrack/ServiceAPI.asmx
    // https://otcert.arkona.com/OpenTrack/PartsAPI.asmx

    // Production URLs
    // https://ot.dms.dealertrack.com/WebService.asmx
    // https://ot.dms.dealertrack.com/ServiceAPI.asmx
    // https://ot.dms.dealertrack.com/PartsAPI.asmx

    /// <summary>
    /// Basic implementation of the OpenTrack API interface that performs and processes the requests.
    /// </summary>
    public class OpenTrackAPI : IOpenTrackAPI
    {
        private const string STAR_STANDARD_PROCESS_MESSAGE_ACTION = "\"http://www.starstandards.org/webservices/2005/10/transport/operations/ProcessMessage\"";

        private const string SERVICE_PRICING_LOOKUP_ACTION = "opentrack.dealertrack.com/ServicePricingLookup";

        /// <summary>
        /// The Base Url of the web service end points, i.e. https://ot.dms.dealertrack.com
        /// </summary>
        public String BaseUrl { get; private set; }

        /// <summary>
        /// The username to authenticate with the web services
        /// </summary>
        public String Username { get; private set; }

        /// <summary>
        /// The password to authenticate with the web services
        /// </summary>
        public String Password { get; private set; }

        /// <summary>
        /// The amount of time to wait before timing out the request
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// A hook to get the raw WCF message for the request before sending
        /// </summary>
        public Action<Message> OnSend { get; set; }

        /// <summary>
        /// A hook to get the raw WCF message for the response before parsing
        /// </summary>
        public Action<Message> OnReceive { get; set; }

        /// <summary>
        /// A hook to get the raw SOAP message being sent using the new custom SOAP client.
        /// </summary>
        public Action<string> OnManualSoapSend { get; set; }

        /// <summary>
        /// A hook to get the raw SOAP message being received using the new custom SOAP client.
        /// </summary>
        public Action<string> OnManualSoapReceive { get; set; }

        /// <summary>
        /// Whether or not to buffer to stream responses from the web services. Defaults to buffered.
        /// </summary>
        public TransferMode TransferMode { get; set; }

        /// <summary>
        /// Create a new instance of the interface to the OpenTrack web services
        /// </summary>
        /// <param name="BaseUrl">The Base Url of the web service end points, i.e. https://ot.dms.dealertrack.com</param>
        /// <param name="Username">The username to authenticate with the web services</param>
        /// <param name="Password">The password to authenticate with the web services</param>
        public OpenTrackAPI(String BaseUrl, String Username, String Password)
        {
            if (String.IsNullOrWhiteSpace(BaseUrl)) throw new ArgumentNullException("Invalid Url provided.");
            if (String.IsNullOrWhiteSpace(Username)) throw new ArgumentNullException("Invalid Username provided.");
            if (String.IsNullOrWhiteSpace(Password)) throw new ArgumentNullException("Invalid Password provided.");

            this.BaseUrl = BaseUrl;
            this.Username = Username;
            this.Password = Password;

            this.Timeout = TimeSpan.FromMinutes(2);

            this.TransferMode = System.ServiceModel.TransferMode.Buffered;
        }

        public IEnumerable<OpenRepairOrderLookupResponseOpenRepairOrder> FindOpenRepairOrders(OpenRepairOrderLookup query)
        {
            return SubmitRequest<OpenRepairOrderLookupResponse>(query).Items;
        }

        public IEnumerable<ClosedRepairOrderLookupResponseClosedRepairOrder> FindClosedRepairOrders(GetClosedRepairOrderRequest query)
        {
            return SubmitRequest<ClosedRepairOrderLookupResponse>(query).Items;
        }

        public IEnumerable<ServiceWritersTableServiceWriterRecord> GetServiceAdvisors(ServiceWritersTableRequest query)
        {
            return SubmitRequest<ServiceWritersTable>(query).Items;
        }

        public IEnumerable<ServiceTechsTableServiceTechRecord> GetTechnicians(ServiceTechsTableRequest query)
        {
            return SubmitRequest<ServiceTechsTable>(query).Items;
        }

        public IEnumerable<ServiceLaborOpcodesTableServiceLaborOpcodeRecord> GetOpcodes(ServiceLaborOpcodesTableRequest query)
        {
            return SubmitRequest<ServiceLaborOpcodesTable>(query).Items;
        }

        public IEnumerable<PartsInventoryResponsePart> GetPartsInventory(PartsInventoryRequest query)
        {
            return SubmitRequest<PartsInventoryResponse>(query).Items;
        }

        public IEnumerable<PartsTransactionsResponseTransactions> GetPartsTransactions(PartsTransactionsRequest query)
        {
            return SubmitRequest<PartsTransactionsResponse>(query).Items;
        }

        public AddRepairOrderResponse AddRepairOrder(AddRepairOrderRequest query)
        {
            return SubmitRequest<AddRepairOrderResponse>(query);
        }

        public AddRepairOrderLinesResponse AddRepairOrderLines(AddRepairOrderLinesRequest query)
        {
            return SubmitRequest<AddRepairOrderLinesResponse>(query);
        }

        public DeleteRepairOrderLinesResponse DeleteRepairOrderLines(Requests.DeleteRepairOrderLinesRequest query)
        {
            return GetROService().DeleteRepairOrderLines(query.Dealer, query.Request);
        }

        public CustomerSearchResponse FindCustomers(CustomerSearchRequest query)
        {
            return SubmitRequest<CustomerSearchResponse>(query);
        }

        public CustomerLookupResponseCustomer GetCustomer(CustomerLookupRequest query)
        {
            return SubmitRequest<CustomerLookupResponse>(query).Items.Single();
        }

        public CustomerAddResponse AddCustomer(CustomerAddRequest query)
        {
            return SubmitRequest<CustomerAddResponse>(query);
        }

        public CustomerUpdateResponse UpdateCustomer(CustomerUpdateRequest query)
        {
            return SubmitRequest<CustomerUpdateResponse>(query);
        }

        public IEnumerable<VehicleInventoryResponseVehicle> GetVehicleInventory(VehicleInventoryRequest query)
        {
            return SubmitRequest<VehicleInventoryResponse>(query).Items;
        }

        public VehicleLookupResponseVehicle GetVehicle(VehicleLookupRequest query)
        {
            return SubmitRequest<VehicleLookupResponse>(query).Items.SingleOrDefault();
        }

        public IEnumerable<VehicleSearchResponseVehicleSearchResult> FindVehicles(VehicleSearchRequest query)
        {
            return SubmitRequest<VehicleSearchResponse>(query).Items;
        }

        public VehicleAddResponse AddVehicle(VehicleAddRequest query)
        {
            return this.SubmitRequest<VehicleAddResponse>(query);
        }

        public VehicleUpdateResponse UpdateVehicle(VehicleUpdateRequest query)
        {
            return this.SubmitRequest<VehicleUpdateResponse>(query);
        }

        public IEnumerable<AppointmentLookupResponseAppointment> FindAppointments(AppointmentLookupRequest query)
        {
            return SubmitRequest<AppointmentLookupResponse>(query).Items;
        }

        public IEnumerable<PartsManufacturersTablePartsManufacturer> GetPartManufacturers(PartsManufacturersTableRequest query)
        {
            return SubmitRequest<PartsManufacturersTable>(query).Items;
        }

        public IEnumerable<PartsStockingGroupsTablePartsStockingGroup> GetPartsStockingGroups(PartsStockingGroupsTableRequest query)
        {
            return SubmitRequest(query).Items;
        }

        public AppointmentAddResponse AddAppointment(AppointmentAddRequest query)
        {
            return SubmitRequest<AppointmentAddResponse>(query);
        }

        public AppointmentUpdateResponse UpdateAppointment(AppointmentUpdateRequest query)
        {
            return SubmitRequest<AppointmentUpdateResponse>(query);
        }

        public AppointmentDeleteResponse DeleteAppointment(AppointmentDeleteRequest query)
        {
            return SubmitRequest<AppointmentDeleteResponse>(query);
        }

        public IEnumerable<ServiceAPI.ClosedRepairOrder> GetClosedRepairOrders(GetClosedRepairOrdersRequest request)
        {
            return GetROService().GetClosedRepairOrders(request.Dealer, request.Request).ClosedRepairOrders;
        }

        public ServiceAPI.ClosedRepairOrder GetClosedRepairOrderDetails(GetClosedRepairOrderDetailsRequest request)
        {
            return GetROService().GetClosedRepairOrderDetails(request.Dealer, request.Request).ClosedRepairOrder;
        }

        public ServiceAPI.UpdateRepairOrderLinesResponse UpdateRepairOrderLines(UpdateRepairOrderLinesRequest request)
        {
            return GetROService().UpdateRepairOrderLines(request.Dealer, request.Request);
        }

        public PartAddResponse AddPart(PartAdd partAdd)
        {
            var contentId = Guid.NewGuid().ToString();

            var envelope = new Envelope<StarRequestBody<PartAddContent>>
            {
                Header = new Header(),
                Body = new StarRequestBody<PartAddContent>
                {
                    ProcessMessage = new ProcessMessage<PartAddContent>
                    {
                        Payload = new Payload<PartAddContent>
                        {
                            Content = new PartAddContent
                            {
                                Id = contentId,
                                PartAdd = partAdd
                            }
                        }
                    }
                }
            };
            AddSecurityHeaderToEnvelope(envelope);
            AddPayloadManifestToHeader(envelope, contentId, "PartAdd");

            var manualSoapClient = new ManualSoapClient(OnManualSoapSend, OnManualSoapReceive);
            var response = manualSoapClient
                .ExecuteRequest<StarResponseBody<PartAddResponseContent>, StarRequestBody<PartAddContent>>
                (string.Format("{0}/{1}", this.BaseUrl, "WebService.asmx"),
                    STAR_STANDARD_PROCESS_MESSAGE_ACTION, envelope);

            return response.Body.ProcessMessageResponse.Payload.Content.PartAddResponse;
        }

        public ServicePricingLookupResult ServicePricingLookup(ServicePricingLookupRequestBody request)
        {
            var url = string.Format("{0}/{1}", this.BaseUrl, "ServiceAPI.asmx");
            var envelope = new Envelope<ServicePricingLookupRequestBody>
            {
                Header = new Header(),
                Body = request
            };

            AddSecurityHeaderToEnvelope(envelope);

            var manualSoapClient = new ManualSoapClient(OnManualSoapSend, OnManualSoapReceive);
            var response =
                manualSoapClient.ExecuteRequest<ServicePricingLookupResponseBody, ServicePricingLookupRequestBody>(url,
                    SERVICE_PRICING_LOOKUP_ACTION, envelope);

            return response.Body.ServicePricingLookupResponse.ServicePricingLookupResult;
        }

        public PartsAPI.PartsPricingLookupResponse GetPartsPricing(PartsPricingLookupRequest request)
        {
            return GetPartsService().PartPricingLookup(new PartsAPI.DealerInfo
                    {
                        EnterpriseCode = request.EnterpriseCode,
                        CompanyNumber = request.CompanyNumber,
                        ServerName = request.ServerName
                    },
                    request.Request);
        }

        /// <summary>
        /// Submit the prepared request to the OpenTrack API and get the response back for processing.
        /// </summary>
        /// <param name="request"></param>
        /// <remarks>
        /// Some of this is cribbed from http://www.starstandard.org/guidelines/Architecture/QuickStart2011v1/ch05s04.html#NETClient.
        /// Go ahead and read some of the STAR spec. It'll make you weep for humanity.
        /// </remarks>
        internal virtual T SubmitRequest<T>(IRequest<T> request)
        {
            // <soap:Envelope>
            //            <soap:Header>
            //                <wsse:Security>
            //                    <wsse:UsernameToken>
            //                        <wsse:Username>${USERNAME}</wsse:Username>
            //                        <wsse:Password>${PASSWORD}</wsse:Password>
            //                    </wsse:UsernameToken>
            //                </wsse:Security>
            //                <tran:PayloadManifest>
            //                    <tran:manifest contentID="Content0" namespaceURI="CrownDMSInterop" element="1" relatedID="1" version="1.0"/>
            //                </tran:PayloadManifest>
            //            </soap:Header>
            //            <soap:Body>
            //                <tran:ProcessMessage>
            //                    <tran:payload>
            //                        <tran:content id="Content0">
            //                          ${REQUEST_CONTENT}
            //                        </tran:content>
            //               </tran:payload>
            //        </tran:ProcessMessage>
            //    </soap:Body>
            // </soap:Envelope>

            using (var svc = GetStarService())
            {
                var xml = new XmlDocument();

                // Load up the request XML into a document object.
                xml.LoadXml(request.XML);

                // Create a unique request identifier.
                var requestId = Guid.NewGuid().ToString();

                var element = xml.DocumentElement;

                // Create the message payload that will be processed.
                var payload = new OpenTrack.WebService.Payload()
                {
                    content = new OpenTrack.WebService.Content[]
                    {
                        new OpenTrack.WebService.Content()
                        {
                            id = requestId,
                            Any = element
                        }
                    }
                };

                // Tell the web service how to interpret the XML we're sending along.
                var manifest = new OpenTrack.WebService.PayloadManifest()
                {
                    manifest = new[]
                    {
                        new OpenTrack.WebService.Manifest()
                        {
                            element = element.LocalName,
                            namespaceURI = element.NamespaceURI,
                            contentID = requestId
                        }
                    }
                };

                // Send the message and it'll load the response into the same object.
                svc.ProcessMessage(ref manifest, ref payload);

                // Take the element from the first content item of the response payload.
                var response = payload.content[0].Any;

                // Check for errors
                ErrorCheck(response);

                // Process the request with the appropriate parser/handler.
                return request.ProcessResponse(response);
            }
        }

        internal virtual void ErrorCheck(XmlElement xml)
        {
            // now using linq to xml so we can find the Error element no matter how deep it is.
            var linqElement = XElement.Parse(xml.OuterXml);
            var errorElement = linqElement.Descendants().FirstOrDefault(x => x.Name == "Error");
            if (errorElement == null)
            {
                // no Error element present so we do not throw.
                return;
            }

            // there is an Error element so we should throw.
            // try and get the specific error code if possible, throw Unknown if none provided.
            var errorCodeElement = errorElement.Descendants().FirstOrDefault(x => x.Name == "Code");
            var messageElement = errorElement.Descendants().FirstOrDefault(x => x.Name == "Message");
            var errorCode = errorCodeElement == null ? "Unknown" : errorCodeElement.Value;
            var message = messageElement == null ? errorElement.Value : messageElement.Value;

            var openTrackException = new OpenTrackException(errorCode, message, xml);

            // In addition to the single ErrorCode and ErrorMessage properties above, we also populate the ErrorItems
            // collection with all the errors from the response, for clients that want to handle multiple errors at once.
            try
            {
                var errors = linqElement.Descendants().Where(x => x.Name == "Error");
                if (errors.Any())
                {
                    openTrackException.ErrorItems = new List<OpenTrackErrorItem>();
                    foreach (var error in errors)
                    {
                        var errorItem = new OpenTrackErrorItem(
                            error.Descendants().First(x => x.Name == "Code").Value,
                            error.Descendants().First(x => x.Name == "Message").Value);
                        openTrackException.ErrorItems.Add(errorItem);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error details to the console, and proceed to the basic throw below.
                Console.WriteLine("Error parsing multiple errors: " + ex);
            }

            throw openTrackException;
        }

        internal virtual ServiceAPI.ServiceAPISoapClient GetROService()
        {
            String Url = String.Format("{0}/{1}", this.BaseUrl, "ServiceAPI.asmx");

            var client = new ServiceAPI.ServiceAPISoapClient(GetBinding(), new EndpointAddress(Url));

            client.ClientCredentials.UserName.UserName = this.Username;
            client.ClientCredentials.UserName.Password = this.Password;

            client.Endpoint.EndpointBehaviors.Add(new MessageInspectorBehavior(this.OnSend, this.OnReceive));

            return client;
        }

        internal virtual PartsAPI.PartsAPISoapClient GetPartsService()
        {
            String Url = String.Format("{0}/{1}", this.BaseUrl, "PartsAPI.asmx");

            var client = new PartsAPI.PartsAPISoapClient(GetBinding(), new EndpointAddress(Url));

            client.ClientCredentials.UserName.UserName = this.Username;
            client.ClientCredentials.UserName.Password = this.Password;

            client.Endpoint.EndpointBehaviors.Add(new MessageInspectorBehavior(this.OnSend, this.OnReceive));

            return client;
        }

        /// <summary>
        /// Return a configured proxy reference to the web service.
        /// </summary>
        internal virtual WebService.starTransportClient GetStarService()
        {
            String Url = String.Format("{0}/{1}", this.BaseUrl, "WebService.asmx");

            // Create a client with the given endpoint.
            var client = new WebService.starTransportClient(GetBinding(), new EndpointAddress(Url));

            client.ClientCredentials.UserName.UserName = this.Username;
            client.ClientCredentials.UserName.Password = this.Password;

            client.Endpoint.EndpointBehaviors.Add(new MessageInspectorBehavior(this.OnSend, this.OnReceive));

            return client;
        }

        internal virtual Binding GetBinding()
        {
            // We need to send the credential along with the message.
            return new BasicHttpsBinding(BasicHttpsSecurityMode.TransportWithMessageCredential)
            {
                // We could be getting back a lot of data. Let's just try and get it all!
                MaxReceivedMessageSize = int.MaxValue,
                SendTimeout = this.Timeout,
                TransferMode = this.TransferMode
            };
        }

        private void AddSecurityHeaderToEnvelope<TBody>(Envelope<TBody> envelope)
        {
            var createTimeUtc = DateTime.UtcNow;
            envelope.Header.Security = new SecurityHeader
            {
                Timestamp = new Timestamp
                {
                    Created = createTimeUtc.ToString("o"),
                    Expires = createTimeUtc.AddMinutes(5).ToString("o"),
                    Id = Guid.NewGuid().ToString()
                },
                UserNameToken = new UserNameToken
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = Username,
                    Password = new Password
                    {
                        Value = Password,
                        Type =
                            "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"
                    }
                }
            };
        }

        private void AddPayloadManifestToHeader<TBody>(Envelope<TBody> envelope, string contentId, string element)
        {
            envelope.Header.PayloadManifest = new PayloadManifest
            {
                Manifest = new Manifest
                {
                    ContentId = contentId,
                    Element = element,
                    NamespaceUri = ""
                }
            };
        }
    }
}