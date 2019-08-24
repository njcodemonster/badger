using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
        event_type_po_id = 2,
        event_type_po_note_create_id=6,
        event_type_po_document_create_id = 7,
        event_type_po_update_id = 8,
        event_type_po_specific_update_id = 9,
        event_type_po_delete_id = 24,
        event_type_po_delete_document_id = 31
    }

    public static class HtmlEnumExtensions
    {
        public static HtmlString EnumToString<T>(this IHtmlHelper helper)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            var values = Enum.GetValues(typeof(T)).Cast<int>();
            Type type = typeof(T);
            foreach (var val in values)
            {
                var memInfo = type.GetMember(type.GetEnumName(val));
                var descriptionAttribute = memInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault() as DescriptionAttribute;
                if (descriptionAttribute != null)
                {
                    keyValuePairs.Add(Enum.GetName(type, val), descriptionAttribute.Description);
                }
            }

            var enumDictionary = values.ToDictionary(value => Enum.GetName(type, value));

            if (keyValuePairs.Count > 0)
                return new HtmlString(JsonConvert.SerializeObject(keyValuePairs));
            else
                return new HtmlString(JsonConvert.SerializeObject(enumDictionary));
        }
    }
}
