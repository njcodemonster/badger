using System.ComponentModel;

namespace CommonHelper
{
    public enum ClaimerType
    {
        InspectClaimer = 1,
        PublishClaimer = 2
    }

    public enum EventType
    {
        note_type = 4,
        [Description("Purchase order created by user =%%userId%% with purchase order id= %%entityId%%")]
        po_created = 2,
        event_type_po_note_create_id=6,
        event_type_po_document_create_id = 7,
        event_type_po_update_id = 8,
        event_type_po_specific_update_id = 9,
        event_type_po_delete_id = 24,
        event_type_po_delete_document_id = 31
    }
}
