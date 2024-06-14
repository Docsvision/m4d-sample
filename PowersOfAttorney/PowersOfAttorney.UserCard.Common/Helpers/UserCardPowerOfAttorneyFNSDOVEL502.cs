using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocsVision.BackOffice.ObjectModel;
using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSDOVEL502Data;

namespace PowersOfAttorney.UserCard.Common.Helpers
{
    public partial class UserCardPowerOfAttorney 
    {

        /// <summary>
        /// Тип лица, действующего от имени доверителя без доверенности
        /// </summary>
        public enum ExecutiveBodyType
        {
            /// <summary>
            /// Юридическое лицо
            /// </summary>
            entity = 0,

            /// <summary>
            /// Физическое лицо
            /// </summary>
            individual = 1
        }

        /// <summary>
        /// Тип представителя
        /// </summary>
        public enum RepresentativeType 
        {
            /// <summary>
            /// Юридическое лицо
            /// </summary>
            entity = 0,

            /// <summary>
            /// Физическое лицо
            /// </summary>
            individual = 1
        }

        /* [ISSUE] Там короче есть енам в конструкторе разметок в мета данных свои названия, а в 
         PowerOfAttorneyFNSDOVEL502Data совои и они РАЗНЫЕ */
        ///// <summary>
        ///// Признак возможности оформления передоверия (5.02)
        ///// </summary>
        //public enum PossibilityOfSubstitution502Type
        //{
        //    /// <summary>
        //    /// Передоверие возможно
        //    /// </summary>
        //    substitutionIsPossible = 1,

        //    /// <summary>
        //    /// Без права передоверия 
        //    /// </summary>
        //    withoutRightOfSubstitution = 2
        //}

        /// <summary>
        /// Не понятно лучше засунуть в <see cref="Fields"> или оставить так
        /// или отсавить так, но засунуть в файл UserCardPowerOfAttorney.cs
        /// </summary>
        private static class AdditionalFields 
        {
            // [enum] Тип лица действующего от имени доверителя
            public const string ExecutiveBodyType = "ExecutiveBodyType";
            // [refId] Организация-представитель 
            public const string EntityRepresentative = "EntityRepresentative";
            // [string] КПП юр лица, дей-ого от им д-я
            public const string KPPEntityWithoutPOA = "KPPEntityWithoutPOA";
            // [string] ИНН лица, дей-ого от им д-я
            public const string INNEntityWithoutPOA = "INNEntityWithoutPOA";
            // [refId] Юр лицо, дей-ого от им д-я
            public const string EntityWithoutPOA = "EntityRepresentative";
            // [enum] Тип представителя
            public const string RepresentativeType = "RepresentativeType";
            // [string] ИНН организации представителя
            public const string INNEntityRepresentative = "INNEntityRepresentative";
            // [string] КПП оргранизации представителя
            public const string KPPEntityRepresentative = "KPPEntityRepresentative";
            // [enum] Признак возможности оформления передоверия
            public const string PossibilityOfSubstitution502 = "PossibilityOfSubstitution502";
        }
                
        public Guid InstanceId => document.GetObjectId();
        public string KPPEntityWithoutPOA => poaAdditionalSection.GetStringValue(AdditionalFields.KPPEntityWithoutPOA);
        public string INNEntityWithoutPOA => poaAdditionalSection.GetStringValue(AdditionalFields.INNEntityWithoutPOA);
        public string INNEntityRepresentative => poaAdditionalSection.GetStringValue(AdditionalFields.INNEntityRepresentative);
        public string KPPEntityRepresentative => poaAdditionalSection.GetStringValue(AdditionalFields.KPPEntityRepresentative);
        public NullableReference<StaffUnit> EntityRepresentative => poaAdditionalSection.GetReferenceFieldValue<StaffUnit>(context, AdditionalFields.EntityRepresentative);
        public NullableReference<StaffUnit> EntityWithoutPOA => poaAdditionalSection.GetReferenceFieldValue<StaffUnit>(context, AdditionalFields.EntityWithoutPOA);
        public ExecutiveBodyType? ExecutiveBodyEnumValue => poaAdditionalSection.GetEnumValue<ExecutiveBodyType>(AdditionalFields.ExecutiveBodyType);
        public RepresentativeType? RepresentativeEnumValue => poaAdditionalSection.GetEnumValue<RepresentativeType>(AdditionalFields.RepresentativeType);
        public RetrustType? PossibilityOfSubstitution502EnumValue => poaAdditionalSection.GetEnumValue<RetrustType>(AdditionalFields.PossibilityOfSubstitution502);
    }
}
