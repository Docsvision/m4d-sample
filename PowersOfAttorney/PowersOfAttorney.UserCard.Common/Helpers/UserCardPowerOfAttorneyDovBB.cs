﻿using DocsVision.BackOffice.ObjectModel;

using System;
using System.Collections.Generic;
using System.Linq;

using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSData;

namespace PowersOfAttorney.UserCard.Common.Helpers
{
    public partial class UserCardPowerOfAttorney
    {
        /// <summary>
        /// Описание условия отзыва
        /// </summary>
        public string RevocationConditionDescription => genMchdSection.GetStringValue(Fields.DescriptionOfTheRevocationCondition);

        /// <summary>
        /// Номер доверенности
        /// </summary>
        /// <remarks>
        /// В качестве альтернативного идентификатора доверенности берём идентификатор пользовательской карточки доверенности
        /// </remarks>
        public Guid PowerOfAttorneyId => genMchdSection.GetGuidValue(Fields.PowerOfAttorneyNumber) ?? document.GetObjectId();

        /// <summary>
        /// Юридическое лицо, действующее без доверенности
        /// </summary>
        public NullableReference<StaffUnit> PrincipalWithoutPowerOfAttorneyOrganization => genMchdSection.GetReferenceFieldValue<StaffUnit>(context, Fields.EntityActingWithoutPowerOfAttorney);

        /// <summary>
        /// СНИЛС физического лица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualSnils => genMchdSection.GetStringValue(Fields.SNILSOfIndividualActingWithoutPowerOfAttorney);

        /// <summary>
        /// Наименование учредительного документа организации, действующей без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyOrganizationConstituentDocument => genMchdSection.GetStringValue(Fields.NameOfConstituentDocumentofEntityActingWithoutPowerOfAttorney);

        /// <summary>
        /// Сведения об удостоверении документа, подтверждающего полномочия, если он удостоверен
        /// </summary>
        public string InformationAboutCertificationOfDocumentConfirmingPowers => genMchdSection.GetStringValue(Fields.InformationAboutCertificationOfDocumentConfirmingPowers);

        /// <summary>
        /// Серия и номер документа, удостоверяющего физлицо, действующее без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualDocumentSeries => genMchdSection.GetStringValue(Fields.SeriesAndNumberOfDocumentProvingIdentityOfIAWPOA);

        /// <summary>
        /// Дата выдачи документа, удостоверяющего физлицо, действующее без доверенности
        /// </summary>
        public DateTime IssueDateOfDocumentProvingIdentityOfIAWPOA => genMchdSection.GetDateValue(Fields.IssueDateOfDocumentProvingIdentityOfIAWPOA) ?? throw new ArgumentNullException(Resources.Error_EmptyIssueDateOfDocumentProvingIdentity);

        /// <summary>
        /// Наименование учредительного документа организации-доверителя
        /// </summary>
        public string PrincipalOrganizationConstituentDocument => genMchdSection.GetStringValue(Fields.NameOfConstituentDocumentofPrincipal);

        /// <summary>
        /// ИНН физического лица-представителя
        /// </summary>
        public string RepresentativeIndividualInn => genMchdSection.GetStringValue(Fields.INNOfIndividualRepresentative);

        /// <summary>
        /// Физическое лицо-представитель
        /// </summary>
        public NullableReference<StaffEmployee> RepresentativeIndividual => genMchdSection.GetReferenceFieldValue<StaffEmployee>(context, Fields.IndividualRepresentative);

        /// <summary>
        /// Адрес места жительства в РФ физлица-представителя
        /// </summary>
        public string RepresentativeIndividualAddress => genMchdSection.GetStringValue(Fields.ResidentialAddressInRussiaForIndividualRepresentative);

        /// <summary>
        /// ИНН физического лица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualInn => genMchdSection.GetStringValue(Fields.IINOfIndividualActingWithoutPowerOfAttorney);

        /// <summary>
        /// Код вида документа, удостоверяющего личность физлица-представителя
        /// </summary>
        public DocumentKindTypes? RepresentativeIndividualDocumentKind => genMchdSection.GetEnumValue<DocumentKindTypes>(Fields.KindCodeOfDocumentProvingIdentityRepresentative);

        /// <summary>
        /// Дата выдачи документа, удостоверяющего физлицо-представителя
        /// </summary>
        public DateTime RepresentativeIndividualDocumentIssueDate => genMchdSection.GetDateValue(Fields.IssueDateOfDocumentProvingIdentityOfRepresentative) ?? throw new ArgumentNullException(Resources.Error_EmptyRepresentativeIndividualDocumentIssueDate);

        /// <summary>
        /// СНИЛС представителя
        /// </summary>
        public string RepresentativeIndividualSnils => genMchdSection.GetStringValue(Fields.SNILSOfRepresentative);

        /// <summary>
        /// Дата выдачи документа, подтверждающего полномочия физлица, действующего без доверенности
        /// </summary>
        public DateTime? PrincipalWithoutPowerOfAttorneyIndividualDocumentIssueDate => genMchdSection.GetDateValue(Fields.IssueDateOfDocumentConfirmingPowersOfIAWPOA);

        /// <summary>
        /// Код страны гражданства иностранного представителя
        /// </summary>
        public int CodeOfCitizenshipForForeignRepresentative => genMchdSection.GetIntValue(Fields.CodeOfCitizenshipForForeignRepresentative) ?? throw new ArgumentNullException(Resources.Error_EmptyCodeOfCitizenshipForForeignRepresentative);

        /// <summary>
        /// Признак гражданства представителя
        /// </summary>
        public CitizenshipType RepresentativeCitizenshipType => genMchdSection.GetEnumValue<CitizenshipType>(Fields.SignOfCitizenshipOfRepresentative) ?? throw new ArgumentNullException(Resources.Error_EmptyRepresentativeCitizenshipType);

        /// <summary>
        /// Место рождения представителя
        /// </summary>
        public string RepresentativeIndividualBirthPlace => genMchdSection.GetStringValue(Fields.PlaceOfBirthOfRepresentative);

        /// <summary>
        /// Наименование органа, выдавшего документ, удостоверяющий личность представителя
        /// </summary>
        public string RepresentativeIndividualDocumentIssuer => genMchdSection.GetStringValue(Fields.NameOfAuthorityIssuedDocumentConfirmingidentityOfRepresentative);

        /// <summary>
        /// Наименование документа, подтверждающего полномочия физлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualDocument => genMchdSection.GetStringValue(Fields.NameOfDocumentConfirmingPowersOfIAWPOA);

        /// <summary>
        /// Наименование органа, выдавшего документ, удостоверяющий физлицо, действующее без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualDocumentIssuer => genMchdSection.GetStringValue(Fields.NameOfAuthorityIssuedDocumentProvingIdentityOfIAWPOA);

        /// <summary>
        /// Код страны гражданства для иностранного физлица, действующего без доверенности
        /// </summary>
        public int PrincipalWithoutPowerOfAttorneyIndividualCountryCode => genMchdSection.GetIntValue(Fields.CodeOfCitizenshipForForeignIAWPOA) ?? throw new ArgumentNullException(Resources.Error_EmptyPrincipalWithoutPowerOfAttorneyIndividualCountryCode);

        /// <summary>
        /// Серия и номер документа, удостоверяющего личность физлица-представителя
        /// </summary>
        public string RepresentativeIndividualDocumentSeries => genMchdSection.GetStringValue(Fields.SeriesAndNumberOfDocumentProvingIdentityOfRepresentative);

        /// <summary>
        /// Место рождения физлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualBirthPlace => genMchdSection.GetStringValue(Fields.PlaceOfBirthOfIndividualActingWithoutPowerOfAttorney);

        /// <summary>
        /// Тип доверителя
        /// </summary>
        public GenPrincipalTypes PrincipalType => genMchdSection.GetEnumValue<GenPrincipalTypes>(Fields.PrincipalType) ?? throw new ArgumentNullException(Resources.Error_EmptyPrincipalType);

        /// <summary>
        /// Доверенность, на основании которой осуществляется передоверие
        /// </summary>
        public NullableReference<Document> ParentalPowerOfAttorneyUserCard => genMchdSection.GetReferenceFieldValue<Document>(context, Fields.ParentalPowerOfAttorney);

        public PowerOfAttorney ParentalPowerOfAttorney
        {
            get
            {
                if (!ParentalPowerOfAttorneyUserCard.HasValue)
                    throw new ArgumentNullException(Resources.Error_ParentalPowerOfAttorneyNotFound);

                var machineReadablePowerOfAttorneySectionRow = ParentalPowerOfAttorneyUserCard.Value.GetSection(generalMachineReadablePowerOfAttorneySectionId)[0] as BaseCardSectionRow;
                var powerOfAttorney = machineReadablePowerOfAttorneySectionRow?.GetReferenceFieldValue<PowerOfAttorney>(context, Fields.PowerOfAttorneyCardId);
                return powerOfAttorney.Value ?? throw new ArgumentNullException(Resources.Error_ParentalPowerOfAttorneyNotFound);
            }
        }

        /// <summary>
        /// Доверитель организация
        /// </summary>
        public NullableReference<StaffUnit> PrincipalOrganization => genMchdSection.GetReferenceFieldValue<StaffUnit>(context, Fields.OrgPrincipal);

        /// <summary>
        /// Кем выдан документ, подтвержающий полномочия физлица, действующего без доверенности
        /// </summary>
        public string WhoIssuedDoctConfPowersIAWPOA => genMchdSection.GetStringValue(Fields.WhoIssuedDoctConfPowersIAWPOA);

        /// <summary>
        /// Дата совершения доверенности
        /// </summary>
        public DateTime PowerOfAttorneyStartDate => genMchdSection.GetDateValue(Fields.PowerOfAttorneyStartDate) ?? throw new ArgumentNullException(Resources.Error_EmptyPowerOfAttorneyStartDate);

        /// <summary>
        /// Дата окончания действия доверенности
        /// </summary>
        public DateTime PowerOfAttorneyEndDate => genMchdSection.GetDateValue(Fields.PowerOfAttorneyEndDate) ?? throw new ArgumentNullException(Resources.Error_EmptyPowerOfAttorneyEndDate);

        /// <summary>
        /// Признак наличия гражданства лица, действующего без доверенности
        /// </summary>
        public CitizenshipType PrincipalWithoutPowerOfAttorneyIndividualCitizenship => genMchdSection.GetEnumValue<CitizenshipType>(Fields.SignOfCitizenshipfIAWPOA) ?? throw new ArgumentNullException(Resources.Error_EmptyPrincipalWithoutPowerOfAttorneyIndividualCitizenship);

        /// <summary>
        /// Код подразделения органа, выдавшего документ, удостоверяющий физлицо, действующее без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualDocumentIssuerCode => genMchdSection.GetStringValue(Fields.SubdivisionCodeOfAuthorityIssuedDocumentProvingIdentityOfIAWPOA);

        /// <summary>
        /// Код подразделения органа, выдавшего документ, удостоверяющий личность представителя
        /// </summary>
        public string RepresentativeIndividualDocumentIssuerCode => genMchdSection.GetStringValue(Fields.SubdivisionCodeOfAuthorityIssuedDocumentConfirmingRepresentativeID);

        /// <summary>
        /// Адрес места жительства в РФ физлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualAddress => genMchdSection.GetStringValue(Fields.ResidentialAddressInRussiaForIndividualActingWithoutPowerOfAttorney);

        /// <summary>
        /// Cубъект РФ для физлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualAddressSubjectRfCode => genMchdSection.GetStringValue(Fields.CodeOfSubjectOfRussiaForIAWPOA);

        /// <summary>
        /// Адрес регистрации в РФ юрлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyOrganizationLegalAddress => genMchdSection.GetStringValue(Fields.RegistrationAddressInRussiaOfEAWPA);

        /// <summary>
        /// Фактический адрес нахождения в РФ юрлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyOrganizationAddress => genMchdSection.GetStringValue(Fields.ActualAddressInRussiaOfEAWPA);
        
        /// <summary>
        /// Cубъект РФ места жительства физлица-представителя
        /// </summary>
        public string RepresentativeIndividualAddressSubjectRfCode => genMchdSection.GetStringValue(Fields.CodeOfSubjectOfRussiaForIndividualRepresentative);

        /// <summary>
        /// Cубъект РФ для адреса регистрации юрлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyOrganizationLegalAddressSubjectRfCode => genMchdSection.GetStringValue(Fields.CodeOfSubjectOfRussiaForRegistrationAddressOfEAWPA);

        /// <summary>
        /// Cубъект РФ для адреса регистрации организации-представителя
        /// </summary>
        public string RepresentativeOrganizationLegalAddressSubjectRfCode => genMchdSection.GetStringValue(Fields.CodeOfSubjectOfRussiaOfRegistrationAddressOfEntityRepresentative);

        /// <summary>
        /// Cубъект РФ для адреса фактического места нахождения организации-представителя
        /// </summary>
        public string RepresentativeOrganizationAddressSubjectRfCode => genMchdSection.GetStringValue(Fields.CodeOfSubjectOfRussiaOfActualAddressOfEntityRepresentative);


        /// <summary>
        /// Cубъект РФ для фактического адреса нахождения юрлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyOrganizationAddressSubjectRfCode => genMchdSection.GetStringValue(Fields.SubjectOfRussiaForActualAddressEAWPA);

        /// <summary>
        /// Возможность оформления передоверия
        /// </summary>        
        public GenPossibilityOfSubstitutionTypes002 RetrustType => genMchdSection.GetEnumValue<GenPossibilityOfSubstitutionTypes002>(Fields.PossibilityOfSubstitution) ?? throw new ArgumentNullException(Resources.Error_EmptyRetrustType);

        /// <summary>
        /// Совместные полномочия
        /// </summary>
        public JointRepresentationType JointRepresentation => genMchdSection.GetEnumValue<JointRepresentationType>("jointExerPowers") ?? throw new ArgumentNullException(Resources.Error_EmptyJointRepresentation);

        /// <summary>
        /// Признак передоверия безотзывной доверенности
        /// </summary>
        public int? RetrustRevocationPossibleType => genMchdSection.GetIntValue(Fields.TransferOfIrrevocablePowerOfAttorney);

        /// <summary>
        /// Признак утраты полномочий при передоверии
        /// </summary>
        public int? LossOfAuthorityType => genMchdSection.GetIntValue(Fields.LossOfPowersUponSubstitution);

        /// <summary>
        /// Код вида документа, удостоверяющий физлицо, действующее без доверенности
        /// </summary>
        public DocumentKindTypes? PrincipalWithoutPowerOfAttorneyIndividualDocumentKindCode => genMchdSection.GetEnumValue<DocumentKindTypes>(Fields.KindCodeOfDocumentProvingIdentityOfIAWPOA);

        /// <summary>
        /// Признак безотзывной доверенности
        /// </summary>
        public int RevocationPossibleType => genMchdSection.GetIntValue(Fields.IrrevocablePowerOfAttorney) ?? throw new ArgumentNullException(Resources.Error_EmptyRevocationPossibleType);

        /// <summary>
        /// Признак безотзывной доверенности
        /// </summary>
        public int? RevocationCondition => genMchdSection.GetIntValue(Fields.RevocationCondition);

        /// <summary>
        /// Физическое лицо, действующее без доверенности
        /// </summary>
        public NullableReference<StaffEmployee> PrincipalWithoutPowerOfAttorneyIndividual => genMchdSection.GetReferenceFieldValue<StaffEmployee>(context, Fields.IndividualActingWithoutPowerOfAttorney);


        /// <summary>
        /// Признак доверенности в рамках передоверия
        /// </summary>
        public bool IsRetrusted() => genMchdSection[Fields.OriginalPowerOfAttorney] != null || genMchdSection[Fields.ParentalPowerOfAttorney] != null;


        public IEnumerable<PowersCode> RepresentativePowersCodes =>  powersWithCodesSection?.Select(t => GetPowersCode(t)) ?? Enumerable.Empty<PowersCode>();

        public IEnumerable<string> RepresentativePowersText => powersWithTextSection?.Select(t => t.GetStringValue(Fields.PowersTextDescription)) ?? Enumerable.Empty<string>();

        public NullableReference<StaffEmployee> Signer => genMchdSection.GetReferenceFieldValue<StaffEmployee>(context, "ceo");

        public Guid? PowerOfAttorneyCardId
        {
            get { return genMchdSection.GetGuidValue(Fields.PowerOfAttorneyCardId); }
            set { genMchdSection[Fields.PowerOfAttorneyCardId] = value; }
        }


        // Идентификатор конечного получателя файла доверенности
        public string GenFinalRecipientTaxID => genMchdSection.GetStringValue("taxAuthPOAValid");

        public enum DocumentKindTypes
        {
            //
            // Summary:
            //     Паспорт иностранного гражданина
            //
            ForeignPassport = 10,

            //
            // Summary:
            //     Паспорт гражданина Российской Федерации
            //
            // Remarks:
            //     Код 21
            Passport = 21
        }

        /// <summary>
        /// Признак возможности оформления передоверия
        /// </summary>
        public enum GenPossibilityOfSubstitutionTypes002
        {
            /// <summary>
            /// Без права передоверия
            /// </summary>
            withoutSubstitution = 1,
            /// <summary>
            /// Однократное передоверие
            /// </summary>
            onetimeSubstitution = 2,
            /// <summary>
            /// С правом последующего передоверия
            /// </summary>
            subsequentSubstitution = 3
        }

        private PowersCode GetPowersCode(BaseCardSectionRow powerRow)
        {
            if (powerRow is null)
                throw new ArgumentNullException(nameof(powerRow));

            return powerRow.GetReferenceFieldValue<PowersCode>(context, Fields.PowersCode).GetValueOrThrow(Resources.Error_UnableToGetPowersCodeFromRow, powerRow.GetObjectId());
        }
    }

}
