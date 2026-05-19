public class VendorApprovedEvent : BaseEvent
{
    public Guid VendorId { get; }
    public string VendorName { get; }

    public VendorApprovedEvent(Guid vendorId, string vendorName)
    {
        VendorId = vendorId;
        VendorName = vendorName;
    }
}