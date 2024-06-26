﻿using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectModel;

using System;


namespace PowersOfAttorney.UserCard.Common.Helpers
{
    internal static class BaseCardSectionRowExtensions
    {
        public static string GetStringValue(this BaseCardSectionRow row, string field)
        {
            return row[field]?.ToString().AsNullable();
        }

        public static TEnum? GetEnumValue<TEnum>(this BaseCardSectionRow row, string field) where TEnum : struct
        {
            var fValue = row[field];

            if (fValue != null && Enum.TryParse(fValue.ToString(), out TEnum result))
            {
                return result;
            }

            return null;
        }

        public static int? GetIntValue(this BaseCardSectionRow row, string field)
        {
            if (int.TryParse(row.GetStringValue(field), out int result))
            {
                return result;
            }

            return null;
        }


        public static bool? GetBoolValue(this BaseCardSectionRow row, string field)
        {
            if (bool.TryParse(row.GetStringValue(field), out bool result))
            {
                return result;
            }

            return null;
        }

        public static decimal? GetDecimalValue(this BaseCardSectionRow row, string field)
        {
            if (decimal.TryParse(row.GetStringValue(field), out decimal result))
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

        public static NullableReference<TResult> GetReferenceFieldValue<TResult>(this BaseCardSectionRow row, ObjectContext context, string fieldAlias) where TResult : ObjectBase
        {
            var id = row.GetGuidValue(fieldAlias);



            var value = id != null ? context.GetObject<TResult>(id) : null;
            return new NullableReference<TResult>(value);
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

    public class NullableReference<TValue>
    {
        public TValue Value { get; }
        public bool HasValue => Value != null;

        public NullableReference(TValue value)
        {
            Value = value;
        }

        public TValue GetValueOrThrow(string message)
        {
            if (!HasValue)
                throw new ArgumentException(message);

            return Value;
        }

        public TValue GetValueOrThrow(string format, object arg)
        {
            if (!HasValue)
                throw new ArgumentException(string.Format(format, arg));
            
            return Value;
        }
    }
}