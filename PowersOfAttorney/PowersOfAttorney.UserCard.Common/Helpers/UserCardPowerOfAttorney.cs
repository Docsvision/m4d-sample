using DocsVision.BackOffice.CardLib.CardDefs;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectModel;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PowersOfAttorney.UserCard.Common.Helpers
{
    public partial class UserCardPowerOfAttorney
    {
        private static Guid generalMachineReadablePowerOfAttorneySectionId = new Guid("29c1b4ef-48e4-47f0-ac67-c42cf68de986");
        private static Guid powersWithCodesSectionId = new Guid("0838CD2A-27C4-4EE1-B599-2DF6586AD2A7");
        private static Guid powersWithTextSectionId = new Guid("D78E3824-618D-44E4-98FF-045502363C3B");

        private readonly Document document;
        private readonly ObjectContext context;

        private readonly BaseCardSectionRow mainInfoSection;
        private readonly BaseCardSectionRow genMchdSection;

        private readonly IList<BaseCardSectionRow> powersWithCodesSection;
        private readonly IList<BaseCardSectionRow> powersWithTextSection;

        public UserCardPowerOfAttorney(Document document, ObjectContext context)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            var genMchdSectionRows = (IList<BaseCardSectionRow>)document.GetSection(generalMachineReadablePowerOfAttorneySectionId);
            if (!genMchdSectionRows.Any())
                throw new ApplicationException(Resources.Error_EmptyMachineReadablePowerOfAttorney);
            
            genMchdSection = genMchdSectionRows[0];

            mainInfoSection = ((IList<BaseCardSectionRow>)document.GetSection(CardDocument.MainInfo.ID))[0];

            powersWithCodesSection = (IList<BaseCardSectionRow>)document.GetSection(powersWithCodesSectionId);
            powersWithTextSection = (IList<BaseCardSectionRow>)document.GetSection(powersWithTextSectionId);
        }


        /// <summary>
        /// Признак утраты полномочий при передоверии
        /// </summary>
        internal enum LossPowersSubstTypes
        {
            /// <summary>
            /// Не утрачиваются
            /// </summary>
            notLost = 1,
            /// <summary>
            /// Утрачиваются
            /// </summary>
            lost = 2
        }

        /// <summary>
        /// Признак совместного осуществления полномочий
        /// </summary>
        internal enum JointExerPowersTypes
        {
            /// <summary>
            /// Индивидуальные
            /// </summary>
            individual = 1,
            /// <summary>
            /// Совместные
            /// </summary>
            joint = 2
        }

        private static class Fields
        {
            // Идентификатор системной карточки доверенности
            public const string PowerOfAttorneyCardId = "POASysCardId";
            // Описание условия отзыва
            public const string DescriptionOfTheRevocationCondition = "conditRevocIrrevPOA";
            // Номер доверенности
            public const string PowerOfAttorneyNumber = "singlePOAregnumber";
            // Юридическое лицо, действующее без доверенности
            public const string EntityActingWithoutPowerOfAttorney = "EntityActingWithoutPowerOfAttorney";
            // СНИЛС физического лица, действующего без доверенности
            public const string SNILSOfIndividualActingWithoutPowerOfAttorney = "ceoSNILS";
            // Наименование учредительного документа организации, действующей без доверенности
            public const string NameOfConstituentDocumentofEntityActingWithoutPowerOfAttorney = "NameOfConstituentDocumentofEntityActingWithoutPowerOfAttorney";
            // Сведения об удостоверении документа, подтверждающего полномочия, если он удостоверен
            public const string InformationAboutCertificationOfDocumentConfirmingPowers = "infoAttestDocConfCEOAuth";
            //Серия и номер документа, удостоверяющего физлицо, действующее без доверенности
            public const string SeriesAndNumberOfDocumentProvingIdentityOfIAWPOA = "serNumCEOIDDoc";
            //Дата выдачи документа, удостоверяющего физлицо, действующее без доверенности
            public const string IssueDateOfDocumentProvingIdentityOfIAWPOA = "dateIssCEOIDDoc";
            // Наименование учредительного документа организации-доверителя
            public const string NameOfConstituentDocumentofPrincipal = "constDocumentEntPrin";
            // Тип уполномоченного представителя
            public const string TypeOfAuthorizedRepresentative = "TypeOfAuthorizedRepresentative";
            // ИНН физического лица-представителя
            public const string INNOfIndividualRepresentative = "representativeINN";
            // Физическое лицо-представитель
            public const string IndividualRepresentative = "representative";
            // Адрес места жительства в РФ физлица-представителя
            public const string ResidentialAddressInRussiaForIndividualRepresentative = "reprAddrRussia";
            // НН физического лица, действующего без доверенности
            public const string IINOfIndividualActingWithoutPowerOfAttorney = "ceoIIN";
            // Код вида документа, удостоверяющего личность физлица-представителя
            public const string KindCodeOfDocumentProvingIdentityRepresentative = "typeCodeReprIDDoc";
            // Дата выдачи документа, удостоверяющего физлицо-представителя
            public const string IssueDateOfDocumentProvingIdentityOfRepresentative = "dateIssReprIDDoc";
            // Индивидуальный предприниматель-представитель
            public const string SoleProprietorRepresentative = "SoleProprietorRepresentative";
            // СНИЛС представителя
            public const string SNILSOfRepresentative = "representativeSNILS";
            // Текстовое описание полномочий
            public const string TextPowersDescription = "TextPowersDescription";
            // Дата выдачи документа, подтверждающего полномочия физлица, действующего без доверенности
            public const string IssueDateOfDocumentConfirmingPowersOfIAWPOA = "dateIssDocConfAuthCEO";
            // Организация-представитель
            public const string EntityRepresentative = "EntityRepresentative";
            // Наименование учредительного документа организации-представителя
            public const string NameOfConstituentDocumentofRepresentative = "NameOfConstituentDocumentofRepresentative";
            // Код страны гражданства иностранного представителя
            public const string CodeOfCitizenshipForForeignRepresentative = "reprCitizenship";
            // Признак гражданства представителя
            public const string SignOfCitizenshipOfRepresentative = "reprCitizenshipSign";
            // Место рождения представителя
            public const string PlaceOfBirthOfRepresentative = "reprDateOfBirth";
            // Наименование органа, выдавшего документ, удостоверяющий личность представителя
            public const string NameOfAuthorityIssuedDocumentConfirmingidentityOfRepresentative = "authIssReprIDDoc";
            // Наименование документа, подтверждающего полномочия физлица, действующего без доверенности
            public const string NameOfDocumentConfirmingPowersOfIAWPOA = "docConfAuthCEO";
            // Наименование органа, выдавшего документ, удостоверяющий физлицо, действующее без доверенности
            public const string NameOfAuthorityIssuedDocumentProvingIdentityOfIAWPOA = "authIssCEOIDDoc";
            // Код страны гражданства для иностранного физлица, действующего без доверенности
            public const string CodeOfCitizenshipForForeignIAWPOA = "ceoCitizenship";
            // Серия и номер документа, удостоверяющего личность физлица-представителя
            public const string SeriesAndNumberOfDocumentProvingIdentityOfRepresentative = "serNumReprIDDoc";
            // Место рождения физлица, действующего без доверенности
            public const string PlaceOfBirthOfIndividualActingWithoutPowerOfAttorney = "ceoPlaceOfBirth";
            // Номер записи об аккредитации
            public const string AccreditationRecordNumber = "AccreditationRecordNumber";
            // Тип доверителя
            public const string PrincipalType = "principalType";
            //Указать юридическое лицо
            public const string IndicateEntity = "mngtCompanySEB";
            //Первоначальная доверенность
            public const string OriginalPowerOfAttorney = "OriginalPOACardLink";
            //Доверенность, на основании которой осуществляется передоверие
            public const string ParentalPowerOfAttorney = "ParentalPOACardLink";
            // Сведения об информационной системе, предоставляющей информацию о доверенности
            public const string InfoSysProvPOAData = "infoSysProvPOAData";
            // Доверитель (сотрудник в справочнике)
            public const string EmplPrincipal = "emplPrincipal";
            // Доверитель (организация в справочнике)
            public const string OrgPrincipal = "entityPrincipal";
            // Кем выдан документ, подтвержающий полномочия физлица, действующего без доверенности
            public const string WhoIssuedDoctConfPowersIAWPOA = "authIssDocConfCEOAuth";
            // Дата совершения доверенности
            public const string PowerOfAttorneyStartDate = "poaDateOfIssue";
            // Дата окончания действия доверенности
            public const string PowerOfAttorneyEndDate = "poaExpirationDate";
            //Основная (первоначальная) доверенность
            public const string BasicPOA = "BasicPOA";
            //Признак наличия гражданства лица, действующего без доверенности
            public const string SignOfCitizenshipfIAWPOA = "ceoCitizenshipSign";
            // Код подразделения органа, выдавшего документ, удостоверяющий физлицо, действующее без доверенности
            public const string SubdivisionCodeOfAuthorityIssuedDocumentProvingIdentityOfIAWPOA = "codeAuthDivIssCEOIDDoc";
            // Код подразделения органа, выдавшего документ, удостоверяющий личность представителя
            public const string SubdivisionCodeOfAuthorityIssuedDocumentConfirmingRepresentativeID = "codeAuthDivIssReprIDDoc";
            // Адрес места жительства в РФ физлица, действующего без доверенности
            public const string ResidentialAddressInRussiaForIndividualActingWithoutPowerOfAttorney = "ceoAddrRussia";
            // Cубъект РФ для физлица, действующего без доверенности
            public const string CodeOfSubjectOfRussiaForIAWPOA = "ceoAddrSubRussia";
            // Адрес регистрации в РФ юрлица, действующего без доверенности
            public const string RegistrationAddressInRussiaOfEAWPA = "RegistrationAddressInRussiaOfEAWPA";
            // Фактический адрес нахождения в РФ юрлица, действующего без доверенности
            public const string ActualAddressInRussiaOfEAWPA = "ActualAddressInRussiaOfEAWPA";
            // Адрес регистрации в РФ организации-представителя
            public const string RegistrationAddressInRussiaOfOrganizationRepresantative = "RegistrationAddressInRussiaOfOrganizationRepresantative";
            // Cубъект РФ места жительства физлица-представителя
            public const string CodeOfSubjectOfRussiaForIndividualRepresentative = "reprAddrSubRussia";
            // Cубъект РФ для адреса регистрации юрлица, действующего без доверенности
            public const string CodeOfSubjectOfRussiaForRegistrationAddressOfEAWPA = "SubjectOfRussiaForRegistrationAddressOfEAWPA";
            // Cубъект РФ для адреса регистрации организации-представителя
            public const string CodeOfSubjectOfRussiaOfRegistrationAddressOfEntityRepresentative = "CodeOfSubjectOfRussiaOfRegistrationAddressOfEntityRepresentative";
            // Cубъект РФ для адреса фактического места нахождения организации-представителя
            public const string CodeOfSubjectOfRussiaOfActualAddressOfEntityRepresentative = "CodeOfSubjectOfRussiaOfActualAddressOfEntityRepresentative";
            // Адрес фактического места нахождения в РФ организации-представителя
            public const string ActualAddressInRussiaOfEntityRepresentative = "ActualAddressInRussiaOfEntityRepresentative";
            // Cубъект РФ для фактического адреса нахождения юрлица, действующего без доверенности
            public const string SubjectOfRussiaForActualAddressEAWPA = "SubjectOfRussiaForActualAddressEAWPA";
            // Тип доверенности
            public const string PowerOfAttorneyType = "PowerOfAttorneyType";
            // Возможность оформления передоверия
            public const string PossibilityOfSubstitution = "possibilityOfSubstitution";
            // Совместные полномочия
            public const string JointPowers = "jointExerPowers";
            // Передоверие безотзывной доверенности
            public const string TransferOfIrrevocablePowerOfAttorney = "TransferOfIrrevocablePowerOfAttorney";
            // Полномочия при передоверии
            public const string LossOfPowersUponSubstitution = "lossPowersTransfer";
            // Код вида документа, удостоверяющий физлицо, действующее без доверенности
            public const string KindCodeOfDocumentProvingIdentityOfIAWPOA = "typeCodeCEOIDDoc";
            // Признак безотзывной доверенности
            public const string IrrevocablePowerOfAttorney = "IrrevocablePowerOfAttorney";
            // Условие отзыва
            public const string RevocationCondition = "RevocationCondition";
            // Физическое лицо, действующее без доверенности
            public const string IndividualActingWithoutPowerOfAttorney = "ceo";
            // Текстовое описание полномочий
            public const string PowersTextDescription = "PowersTextDescription";
            // Код полномочий
            public const string PowersCode = "PowersCode";
            // Признак совместного осуществления полномочий
            public const string JointExerPowers = "jointExerPowers";
            // Признак совместного осуществления полномочий в секции кодов доверенностей
            public const string JointExerPowers1 = "jointExerPowers1";
            

            // Признак утраты полномочий при передоверии
            public const string LossPowersSubst = "lossPowersSubst";
        }
    }
}