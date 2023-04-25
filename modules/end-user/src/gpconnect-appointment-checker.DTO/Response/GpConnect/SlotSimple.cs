using gpconnect_appointment_checker.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class SlotSimple
    {
        public List<SlotEntrySimple> CurrentSlotEntrySimple { get; set; }
        public List<SlotEntrySimple> PastSlotEntrySimple { get; set; }
        public List<List<SlotEntrySimple>> CurrentSlotEntriesByLocationGrouping => CurrentSlotEntrySimple?.GroupBy(l => l.LocationName).Select(grp => grp.ToList()).ToList();
        public List<List<SlotEntrySimple>> PastSlotEntriesByLocationGrouping => PastSlotEntrySimple?.GroupBy(l => l.LocationName).Select(grp => grp.ToList()).ToList();
        public List<Issue> Issue { get; set; }
        public int SlotCount => CurrentSlotEntrySimple.Count + PastSlotEntrySimple.Count;
        public int CurrentSlotCount => CurrentSlotEntrySimple.Count;
        public int PastSlotCount => PastSlotEntrySimple.Count;
        public bool SlotSearchNoIssues => Issue?.Count == 0;

        public string ExportStreamData => PastSlotEntrySimple.Concat(CurrentSlotEntrySimple).ConvertObjectToJsonData();

        public int SearchExportId { get; set; }

        public string ProviderError => Issue?.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Display;
        public string ProviderErrorCode => Issue?.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Code;
        public string ProviderErrorDiagnostics => StringExtensions.Coalesce(Issue?.FirstOrDefault()?.Diagnostics, Issue?.FirstOrDefault()?.Details.Text);                        
    }
}
