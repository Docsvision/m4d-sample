using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DocsVision.BackOffice.ObjectModel;
using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSDOVEL502Data;

namespace PowersOfAttorney.UserCard.Common.Helpers
{
    public partial class UserCardPowerOfAttorney 
    {
        // <summary>
        /// Код формы по КНД
        /// </summary>
        public string GenKnd502 => "1110310"; // Взят пример КНД

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
        public enum GenPossibilityOfSubstitution502Type
        {
            /// <summary>
            /// Передоверие возможно
            /// </summary>
            substitutionIsPossible = 1,

            /// <summary>
            /// Без права передоверия 
            /// </summary>
            withoutRightOfSubstitution = 2
        }

        /// <summary>
        /// Не понятно лучше засунуть в <see cref="Fields"> или оставить так
        /// или отсавить так, но засунуть в файл UserCardPowerOfAttorney.cs
        /// </summary>
        private static class AdditionalFields 
        {
            // [refId] Юридическое лицо, действующее от имени доверителя без доверенности
            public const string EntityWithoutPOA = "EntityWithoutPOA";
            // [string] КПП юридического лица, действующего от имени доверителя без доверенности
            public const string KPPEntityWithoutPOA = "KPPEntityWithoutPOA";
            // [string] ИНН юридического лица, действующего от имени доверителя без доверенности
            public const string INNEntityWithoutPOA = "INNEntityWithoutPOA";           
            // [enum] Тип лица действующего от имени доверителя
            public const string ExecutiveBodyType = "ExecutiveBodyType";
            // [refId] Организация-представитель 
            public const string EntityRepresentative = "EntityRepresentative";
            // [enum] Тип представителя
            public const string RepresentativeType = "RepresentativeType";
            // [string] ИНН организации представителя
            public const string INNEntityRepresentative = "INNEntityRepresentative";
            // [string] КПП оргранизации представителя
            public const string KPPEntityRepresentative = "KPPEntityRepresentative";
            // [enum] Признак возможности оформления передоверия
            public const string PossibilityOfSubstitution502 = "PossibilityOfSubstitution502";
            // [bool] Доверенность формируется на основании доверенности, ранее выданной в порядке передоверия
            public const string SubstitutionPOAInBasis = "substitutionPOAInBasis";
        }
                
        public Guid InstanceId => document.GetObjectId();
        public string KPPEntityWithoutPOA => poaAdditionalSection.GetStringValue(AdditionalFields.KPPEntityWithoutPOA);
        public string INNEntityWithoutPOA => poaAdditionalSection.GetStringValue(AdditionalFields.INNEntityWithoutPOA);
        // ИНН организации представителя
        public string INNEntityRepresentative => poaAdditionalSection.GetStringValue(AdditionalFields.INNEntityRepresentative);
        // КПП оргранизации представителя
        public string KPPEntityRepresentative => poaAdditionalSection.GetStringValue(AdditionalFields.KPPEntityRepresentative);
        // Организация-представитель 
        public NullableReference<StaffUnit> EntityRepresentative => poaAdditionalSection.GetReferenceFieldValue<StaffUnit>(context, AdditionalFields.EntityRepresentative);
        // Юридическое лицо, действующее от имени доверителя без доверенности
        public NullableReference<StaffUnit> EntityWithoutPOA => poaAdditionalSection.GetReferenceFieldValue<StaffUnit>(context, AdditionalFields.EntityWithoutPOA);
        // Тип лица действующего от имени доверителя
        public ExecutiveBodyType? GenExecutiveBodyType => poaAdditionalSection.GetEnumValue<ExecutiveBodyType>(AdditionalFields.ExecutiveBodyType);
        // Тип представителя
        public RepresentativeType? GenRepresentativeType502 => poaAdditionalSection.GetEnumValue<RepresentativeType>(AdditionalFields.RepresentativeType);
        // Признак возможности оформления передоверия
        public GenPossibilityOfSubstitution502Type? GenPossibilityOfSubstitution502 => poaAdditionalSection.GetEnumValue<GenPossibilityOfSubstitution502Type>(AdditionalFields.PossibilityOfSubstitution502);
        // Доверенность формируется на основании доверенности, ранее выданной в порядке передоверия
        public bool? SubstitutionPOAInBasis => genMchdSection.GetBoolValue(AdditionalFields.SubstitutionPOAInBasis);
    }
}
