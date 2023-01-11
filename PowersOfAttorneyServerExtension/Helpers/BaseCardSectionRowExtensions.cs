using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectModel;

using System;
using System.Runtime.Remoting.Contexts;


namespace PowersOfAttorneyServerExtension.Helpers
{
    internal static class BaseCardSectionRowExtensions
    {
        public static string GetStringValue(this BaseCardSectionRow row, string field)
        {
            return row[field]?.ToString();
        }

        public static int? GetIntValue(this BaseCardSectionRow row, string field)
        {
            if (int.TryParse(row.GetStringValue(field), out int result))
            {
                return result;
            }

            return null;
        }

        public static Guid? GetGuidValue(this BaseCardSectionRow row, string field)
        {
            if (Guid.TryParse(row.GetStringValue(field), out Guid result))
            {
                return result;
            }

            return null;
        } 
        
        public static DateTime? GetDateValue(this BaseCardSectionRow row, string field)
        {
            if (DateTime.TryParse(row.GetStringValue(field), out DateTime result))
            {
                return result;
            }

            return null;
        }
    }
}