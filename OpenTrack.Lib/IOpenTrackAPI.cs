using OpenTrack.ManualSoap.Requests;
using OpenTrack.ManualSoap.Responses;
using OpenTrack.Requests;
using OpenTrack.Responses;
using System;
using System.Collections.Generic;
using OpenTrack.ServiceAPI;
using GetClosedRepairOrdersRequest = OpenTrack.Requests.GetClosedRepairOrdersRequest;
using UpdateRepairOrderLinesRequest = OpenTrack.Requests.UpdateRepairOrderLinesRequest;
using VehicleLookupResponseVehicle = OpenTrack.Responses.VehicleLookupResponseVehicle;

namespace OpenTrack
{
    /// <summary>
    /// Provides an interface to interacting with the OpenTrack API
    /// </summary>
    public interface IOpenTrackAPI
    {
        IEnumerable<ServiceLaborOpcodesTableServiceLaborOpcodeRecord> GetOpcodes(ServiceLaborOpcodesTableRequest query);

        /// <summary>
        /// Returns a list of open repair orders matching the given query criteria.
        /// </summary>
        IEnumerable<OpenRepairOrderLookupResponseOpenRepairOrder> FindOpenRepairOrders(OpenRepairOrderLookup query);

        /// <summary>
        /// Returns a list of closed repair orders matching the given query criteria.
        /// </summary>
        [Obsolete("This method is being phased out by DealerTrack")]
        IEnumerable<ClosedRepairOrderLookupResponseClosedRepairOrder> FindClosedRepairOrders(GetClosedRepairOrderRequest query);

        /// <summary>
        /// Returns a list of service advisors for the dealership.
        /// </summary>
        IEnumerable<ServiceWritersTableServiceWriterRecord> GetServiceAdvisors(ServiceWritersTableRequest query);

        /// <summary>
        /// Returns a list of technicians for the dealership.
        /// </summary>
        IEnumerable<ServiceTechsTableServiceTechRecord> GetTechnicians(ServiceTechsTableRequest query);

        /// <summary>
        ///  Get the parts inventory or a specific part in inventory from the DMS Parts Inventory database.
        /// </summary>
        IEnumerable<PartsInventoryResponsePart> GetPartsInventory(PartsInventoryRequest query);

        /// <summary>
        ///  Get the parts inventory or a specific part in inventory from the DMS Parts Inventory database.
        /// </summary>
        IEnumerable<PartsTransactionsResponseTransactions> GetPartsTransactions(PartsTransactionsRequest query);

        /// <summary>
        /// Retrieve parts pricing for a specific customer, pricing level, or VIN
        /// </summary>
        PartsAPI.PartsPricingLookupResponse GetPartsPricing(PartsPricingLookupRequest request);

        /// <summary>
        /// Add a new repair order to the DMS.
        /// </summary>
        AddRepairOrderResponse AddRepairOrder(AddRepairOrderRequest query);

        /// <summary>
        /// Add repair order lines to an existing repair order.
        /// </summary>
        AddRepairOrderLinesResponse AddRepairOrderLines(AddRepairOrderLinesRequest query);
        /// <summary>
        /// This method will delete repair order lines on an existing repair order that do not have part lines. 
        /// You can also delete part lines via their part number but this must be done in a separate call from deleting the line itself.
        /// </summary>
        DeleteRepairOrderLinesResponse DeleteRepairOrderLines(Requests.DeleteRepairOrderLinesRequest query);
        /// <summary>
        /// Find a list of customers matching the given criteria.
        /// </summary>
        CustomerSearchResponse FindCustomers(CustomerSearchRequest query);

        /// <summary>
        /// Find a single customer by number.
        /// </summary>
        CustomerLookupResponseCustomer GetCustomer(CustomerLookupRequest query);

        /// <summary>
        /// Add a new customer to the DMS. Must have DataToken property filled in from the customer search or lookup methods.
        /// </summary>
        CustomerAddResponse AddCustomer(CustomerAddRequest query);

        /// <summary>
        /// Update an existing customer in the DMS.
        /// </summary>
        CustomerUpdateResponse UpdateCustomer(CustomerUpdateRequest query);

        /// <summary>
        /// Return a list of vehicles in the DMS inventory.
        /// </summary>
        IEnumerable<VehicleInventoryResponseVehicle> GetVehicleInventory(VehicleInventoryRequest query);

        /// <summary>
        /// Return a single vehicle's information by VIN or Stock Number. Will return null if none were found.
        /// </summary>
        VehicleLookupResponseVehicle GetVehicle(VehicleLookupRequest query);

        /// <summary>
        /// Find vehicles matching given criteria in the DMS.
        /// </summary>
        IEnumerable<VehicleSearchResponseVehicleSearchResult> FindVehicles(VehicleSearchRequest query);

        /// <summary>
        /// Add a new vehicle to the DMS.
        /// </summary>
        VehicleAddResponse AddVehicle(VehicleAddRequest query);

        /// <summary>
        /// Update a vehicle in the DMS.
        /// </summary>
        VehicleUpdateResponse UpdateVehicle(VehicleUpdateRequest query);

        /// <summary>
        /// Returns a list of appointments which match the given criteria in the DMS.
        /// </summary>
        IEnumerable<AppointmentLookupResponseAppointment> FindAppointments(AppointmentLookupRequest query);

        /// <summary>
        /// Add a new appointment to the DMS. Returns the created appointment number.
        /// </summary>
        AppointmentAddResponse AddAppointment(AppointmentAddRequest query);

        /// <summary>
        /// Update an existing appointment in the DMS.
        /// </summary>
        AppointmentUpdateResponse UpdateAppointment(AppointmentUpdateRequest query);

        /// <summary>
        /// Delete an existing appointment in the DMS.
        /// </summary>
        AppointmentDeleteResponse DeleteAppointment(AppointmentDeleteRequest query);

        /// <summary>
        /// Retrieves the list of parts manufacturers configured for a dealership
        /// </summary>
        IEnumerable<PartsManufacturersTablePartsManufacturer> GetPartManufacturers(PartsManufacturersTableRequest query);

        /// <summary>
        /// Retreives the list of parts stocking groups configured for a dealership.
        /// </summary>
        IEnumerable<PartsStockingGroupsTablePartsStockingGroup> GetPartsStockingGroups(PartsStockingGroupsTableRequest query);
        
        /// <summary>
        /// Retrieves a list of closed repair order headers for a given dealership and timeframe
        /// </summary>
        IEnumerable<ServiceAPI.ClosedRepairOrder> GetClosedRepairOrders(GetClosedRepairOrdersRequest request);

        /// <summary>
        /// Retrieve the details of a single clsoed repair order by number
        /// </summary>
        ServiceAPI.ClosedRepairOrder GetClosedRepairOrderDetails(GetClosedRepairOrderDetailsRequest request);

        /// <summary>
        /// Updates a list of lines on a repair order.
        /// </summary>
        ServiceAPI.UpdateRepairOrderLinesResponse UpdateRepairOrderLines(UpdateRepairOrderLinesRequest request);

        /// <summary>
        /// Adds a part to parts inventory
        /// </summary>
        PartAddResponse AddPart(PartAdd partAdd);

        /// <summary>
        /// Looks up service pricing for an op code.
        /// </summary>
        ServicePricingLookupResult ServicePricingLookup(ServicePricingLookupRequestBody request);
    }
}