using DocsVision.BackOffice.CardLib.CardDefs;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectModel;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PowersOfAttorneyServerExtension.Helpers
{
    internal partial class UserCardPowerOfAttorney
    {
        /// <summary>
        /// Описание условия отзыва
        /// </summary>
        public string RevocationConditionDescription => mrpSection.GetStringValue(Fields.DescriptionOfTheRevocationCondition);

        /// <summary>
        /// Номер доверенности
        /// </summary>
        /// <remarks>
        /// В качестве альтернативного идентификатора доверенности берём идентификатор пользовательской карточки доверенности
        /// </remarks>
        public Guid PowerOfAttorneyId => mrpSection.GetGuidValue(Fields.PowerOfAttorneyID) ?? document.GetObjectId();

        /// <summary>
        /// Юридическое лицо, действующее без доверенности
        /// </summary>
        public StaffUnit PrincipalWithoutPowerOfAttorneyOrganization => GetReferenceFromMrpSectionField<StaffUnit>(Fields.EntityActingWithoutPowerOfAttorney);

        /// <summary>
        /// СНИЛС физического лица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualSnils => mrpSection.GetStringValue(Fields.SNILSOfIndividualActingWithoutPowerOfAttorney);

        /// <summary>
        /// Наименование учредительного документа организации, действующей без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyOrganizationConstituentDocument => mrpSection.GetStringValue(Fields.NameOfConstituentDocumentofEntityActingWithoutPowerOfAttorney);

        /// <summary>
        /// Сведения об удостоверении документа, подтверждающего полномочия, если он удостоверен
        /// </summary>
        public string InformationAboutCertificationOfDocumentConfirmingPowers => mrpSection.GetStringValue(Fields.InformationAboutCertificationOfDocumentConfirmingPowers);

        /// <summary>
        /// Серия и номер документа, удостоверяющего физлицо, действующее без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualDocumentSeries => mrpSection.GetStringValue(Fields.SeriesAndNumberOfDocumentProvingIdentityOfIAWPOA);

        /// <summary>
        /// Дата выдачи документа, удостоверяющего физлицо, действующее без доверенности
        /// </summary>
        public DateTime IssueDateOfDocumentProvingIdentityOfIAWPOA => mrpSection.GetDateValue(Fields.IssueDateOfDocumentProvingIdentityOfIAWPOA) ?? throw new ArgumentNullException(Resources.Error_EmptyIssueDateOfDocumentProvingIdentity);

        /// <summary>
        /// Наименование учредительного документа организации-доверителя
        /// </summary>
        public string PrincipalOrganizationConstituentDocument => mrpSection.GetStringValue(Fields.NameOfConstituentDocumentofPrincipal);

        /// <summary>
        /// Тип уполномоченного представителя
        /// </summary>
        public int RepresentativeType => mrpSection.GetIntValue(Fields.TypeOfAuthorizedRepresentative) ?? throw new ArgumentNullException(Resources.Error_EmptyRepresentativeType);

        /// <summary>
        /// ИНН физического лица-представителя
        /// </summary>
        public string RepresentativeIndividualInn => mrpSection.GetStringValue(Fields.INNOfIndividualRepresentative);

        /// <summary>
        /// Физическое лицо-представитель
        /// </summary>
        public StaffEmployee RepresentativeIndividual => GetReferenceFromMrpSectionField<StaffEmployee>(Fields.IndividualRepresentative);

        /// <summary>
        /// Адрес места жительства в РФ физлица-представителя
        /// </summary>
        public string RepresentativeIndividualAddress => mrpSection.GetStringValue(Fields.ResidentialAddressInRussiaForIndividualRepresentative);

        /// <summary>
        /// ИНН физического лица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualInn => mrpSection.GetStringValue(Fields.IINOfIndividualActingWithoutPowerOfAttorney);

        /// <summary>
        /// Код вида документа, удостоверяющего личность физлица-представителя
        /// </summary>
        public int? RepresentativeIndividualDocumentKind => mrpSection.GetIntValue(Fields.KindCodeOfDocumentProvingIdentityRepresentative);

        /// <summary>
        /// Дата выдачи документа, удостоверяющего физлицо-представителя
        /// </summary>
        public DateTime RepresentativeIndividualDocumentIssueDate => mrpSection.GetDateValue(Fields.IssueDateOfDocumentProvingIdentityOfRepresentative) ?? throw new ArgumentNullException(Resources.Error_EmptyRepresentativeIndividualDocumentIssueDate);

        /// <summary>
        /// Индивидуальный предприниматель-представитель
        /// </summary>
        public StaffUnit RepresentativeSoleProprietor => GetReferenceFromMrpSectionField<StaffUnit>(Fields.SoleProprietorRepresentative);

        /// <summary>
        /// СНИЛС представителя
        /// </summary>
        public string RepresentativeIndividualSnils => mrpSection.GetStringValue(Fields.SNILSOfRepresentative);

        /// <summary>
        /// Дата выдачи документа, подтверждающего полномочия физлица, действующего без доверенности
        /// </summary>
        public DateTime? PrincipalWithoutPowerOfAttorneyIndividualDocumentIssueDate => mrpSection.GetDateValue(Fields.IssueDateOfDocumentConfirmingPowersOfIAWPOA);

        /// <summary>
        /// Организация-представитель
        /// </summary>
        public StaffUnit RepresentativeEntity => GetReferenceFromMrpSectionField<StaffUnit>(Fields.EntityRepresentative);

        /// <summary>
        /// Наименование учредительного документа организации-представителя
        /// </summary>
        public string RepresentativeEntityDocument => mrpSection.GetStringValue(Fields.NameOfConstituentDocumentofRepresentative);

        /// <summary>
        /// Код страны гражданства иностранного представителя
        /// </summary>
        public int CodeOfCitizenshipForForeignRepresentative => mrpSection.GetIntValue(Fields.CodeOfCitizenshipForForeignRepresentative) ?? throw new ArgumentNullException(Resources.Error_EmptyCodeOfCitizenshipForForeignRepresentative);

        /// <summary>
        /// Признак гражданства представителя
        /// </summary>
        public int RepresentativeCitizenshipType => mrpSection.GetIntValue(Fields.SignOfCitizenshipOfRepresentative) ?? throw new ArgumentNullException(Resources.Error_EmptyRepresentativeCitizenshipType);

        /// <summary>
        /// Место рождения представителя
        /// </summary>
        public string RepresentativeIndividualBirthPlace => mrpSection.GetStringValue(Fields.PlaceOfBirthOfRepresentative);

        /// <summary>
        /// Наименование органа, выдавшего документ, удостоверяющий личность представителя
        /// </summary>
        public string RepresentativeIndividualDocumentIssuer => mrpSection.GetStringValue(Fields.NameOfAuthorityIssuedDocumentConfirmingidentityOfRepresentative);

        /// <summary>
        /// Наименование документа, подтверждающего полномочия физлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualDocument => mrpSection.GetStringValue(Fields.NameOfDocumentConfirmingPowersOfIAWPOA);

        /// <summary>
        /// Наименование органа, выдавшего документ, удостоверяющий физлицо, действующее без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualDocumentIssuer => mrpSection.GetStringValue(Fields.NameOfAuthorityIssuedDocumentProvingIdentityOfIAWPOA);

        /// <summary>
        /// Код страны гражданства для иностранного физлица, действующего без доверенности
        /// </summary>
        public int PrincipalWithoutPowerOfAttorneyIndividualCountryCode => mrpSection.GetIntValue(Fields.CodeOfCitizenshipForForeignIAWPOA) ?? throw new ArgumentNullException(Resources.Error_EmptyPrincipalWithoutPowerOfAttorneyIndividualCountryCode);

        /// <summary>
        /// Серия и номер документа, удостоверяющего личность физлица-представителя
        /// </summary>
        public string RepresentativeIndividualDocumentSeries => mrpSection.GetStringValue(Fields.SeriesAndNumberOfDocumentProvingIdentityOfRepresentative);

        /// <summary>
        /// Место рождения физлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualBirthPlace => mrpSection.GetStringValue(Fields.PlaceOfBirthOfIndividualActingWithoutPowerOfAttorney);

        /// <summary>
        /// Номер записи об аккредитации
        /// </summary>
        public string RepresentativeForeignAccreditationRecordNumber => mrpSection.GetStringValue(Fields.AccreditationRecordNumber);

        /// <summary>
        /// Тип доверителя
        /// </summary>
        public int PrincipalType => mrpSection.GetIntValue(Fields.PrincipalType) ?? throw new ArgumentNullException(Resources.Error_EmptyPrincipalType);

        /// <summary>
        /// Доверенность, на основании которой осуществляется передоверие
        /// </summary>
        public Document ParentalPowerOfAttorneyUserCard => GetReferenceFromMrpSectionField<Document>(Fields.ParentalPowerOfAttorney);

        public PowerOfAttorney ParentalPowerOfAttorney
        {
            get
            {
                var machineReadablePowerOfAttorneySectionRow = ParentalPowerOfAttorneyUserCard.GetSection(machineReadablePowerOfAttorneySectionId)[0] as BaseCardSectionRow;
                var powerOfAttorney = machineReadablePowerOfAttorneySectionRow.GetReferenceFieldValue<PowerOfAttorney>(context, Fields.PowerOfAttorneyCardId);
                return powerOfAttorney ?? throw new ArgumentNullException(Resources.Error_ParentalPowerOfAttorneyNotFound);
            }
        }


        /// <summary>
        /// Доверитель физлицо
        /// </summary>
        public StaffEmployee PrinciplalIndividual => GetReferenceFromMrpSectionField<StaffEmployee>(Fields.EmplPrincipal);

        /// <summary>
        /// Доверитель организация
        /// </summary>
        public StaffUnit PrincipalOrganization => GetReferenceFromMrpSectionField<StaffUnit>(Fields.OrgPrincipal);

        /// <summary>
        /// Кем выдан документ, подтвержающий полномочия физлица, действующего без доверенности
        /// </summary>
        public string WhoIssuedDoctConfPowersIAWPOA => mrpSection.GetStringValue(Fields.WhoIssuedDoctConfPowersIAWPOA);

        /// <summary>
        /// Дата совершения доверенности
        /// </summary>
        public DateTime PowerOfAttorneyStartDate => mrpSection.GetDateValue(Fields.PowerOfAttorneyStartDate) ?? throw new ArgumentNullException(Resources.Error_EmptyPowerOfAttorneyStartDate);

        /// <summary>
        /// Дата окончания действия доверенности
        /// </summary>
        public DateTime PowerOfAttorneyEndDate => mrpSection.GetDateValue(Fields.PowerOfAttorneyEndDate) ?? throw new ArgumentNullException(Resources.Error_EmptyPowerOfAttorneyEndDate);

        /// <summary>
        /// Основная (первоначальная) доверенность
        /// </summary>
        public PowerOfAttorney BasePowerOfAttorney => GetReferenceFromMrpSectionField<PowerOfAttorney>(Fields.BasicPOA);

        /// <summary>
        /// Признак наличия гражданства лица, действующего без доверенности
        /// </summary>
        public int PrincipalWithoutPowerOfAttorneyIndividualCitizenship => mrpSection.GetIntValue(Fields.SignOfCitizenshipfIAWPOA) ?? throw new ArgumentNullException(Resources.Error_EmptyPrincipalWithoutPowerOfAttorneyIndividualCitizenship);

        /// <summary>
        /// Код подразделения органа, выдавшего документ, удостоверяющий физлицо, действующее без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualDocumentIssuerCode => mrpSection.GetStringValue(Fields.SubdivisionCodeOfAuthorityIssuedDocumentProvingIdentityOfIAWPOA);

        /// <summary>
        /// Код подразделения органа, выдавшего документ, удостоверяющий личность представителя
        /// </summary>
        public string RepresentativeIndividualDocumentIssuerCode => mrpSection.GetStringValue(Fields.SubdivisionCodeOfAuthorityIssuedDocumentConfirmingRepresentativeID);

        /// <summary>
        /// Адрес места жительства в РФ физлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualAddress => mrpSection.GetStringValue(Fields.ResidentialAddressInRussiaForIndividualActingWithoutPowerOfAttorney);

        /// <summary>
        /// Cубъект РФ для физлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyIndividualAddressSubjectRfCode => mrpSection.GetStringValue(Fields.CodeOfSubjectOfRussiaForIAWPOA);

        /// <summary>
        /// Адрес регистрации в РФ юрлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyOrganizationLegalAddress => mrpSection.GetStringValue(Fields.RegistrationAddressInRussiaOfEAWPA);

        /// <summary>
        /// Фактический адрес нахождения в РФ юрлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyOrganizationAddress => mrpSection.GetStringValue(Fields.ActualAddressInRussiaOfEAWPA);

        /// <summary>
        /// Адрес регистрации в РФ организации-представителя
        /// </summary>
        public string RepresentativeEntityLegalAddress => mrpSection.GetStringValue(Fields.RegistrationAddressInRussiaOfOrganizationRepresantative);

        /// <summary>
        /// Cубъект РФ места жительства физлица-представителя
        /// </summary>
        public string RepresentativeIndividualAddressSubjectRfCode => mrpSection.GetStringValue(Fields.CodeOfSubjectOfRussiaForIndividualRepresentative);

        /// <summary>
        /// Cубъект РФ для адреса регистрации юрлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyOrganizationLegalAddressSubjectRfCode => mrpSection.GetStringValue(Fields.CodeOfSubjectOfRussiaForRegistrationAddressOfEAWPA);

        /// <summary>
        /// Cубъект РФ для адреса регистрации организации-представителя
        /// </summary>
        public string RepresentativeOrganizationLegalAddressSubjectRfCode => mrpSection.GetStringValue(Fields.CodeOfSubjectOfRussiaOfRegistrationAddressOfEntityRepresentative);

        /// <summary>
        /// Cубъект РФ для адреса фактического места нахождения организации-представителя
        /// </summary>
        public string RepresentativeOrganizationAddressSubjectRfCode => mrpSection.GetStringValue(Fields.CodeOfSubjectOfRussiaOfActualAddressOfEntityRepresentative);

        /// <summary>
        /// Адрес фактического места нахождения в РФ организации-представителя
        /// </summary>
        public string RepresentativeOrganizationAddress => mrpSection.GetStringValue(Fields.ActualAddressInRussiaOfEntityRepresentative);


        /// <summary>
        /// Cубъект РФ для фактического адреса нахождения юрлица, действующего без доверенности
        /// </summary>
        public string PrincipalWithoutPowerOfAttorneyOrganizationAddressSubjectRfCode => mrpSection.GetStringValue(Fields.SubjectOfRussiaForActualAddressEAWPA);

        /// <summary>
        /// Возможность оформления передоверия
        /// </summary>
        public int RetrustType => mrpSection.GetIntValue(Fields.PossibilityOfSubstitution) ?? throw new ArgumentNullException(Resources.Error_EmptyRetrustType);

        /// <summary>
        /// Совместные полномочия
        /// </summary>
        public int JointRepresentation => mrpSection.GetIntValue(Fields.JointPowers) ?? throw new ArgumentNullException(Resources.Error_EmptyJointRepresentation);

        /// <summary>
        /// Признак передоверия безотзывной доверенности
        /// </summary>
        public int? RetrustRevocationPossibleType => mrpSection.GetIntValue(Fields.TransferOfIrrevocablePowerOfAttorney);

        /// <summary>
        /// Признак утраты полномочий при передоверии
        /// </summary>
        public int? LossOfAuthorityType => mrpSection.GetIntValue(Fields.LossOfPowersUponSubstitution);

        /// <summary>
        /// Код вида документа, удостоверяющий физлицо, действующее без доверенности
        /// </summary>
        public int? PrincipalWithoutPowerOfAttorneyIndividualDocumentKindCode => mrpSection.GetIntValue(Fields.KindCodeOfDocumentProvingIdentityOfIAWPOA);

        /// <summary>
        /// Признак безотзывной доверенности
        /// </summary>
        public int RevocationPossibleType => mrpSection.GetIntValue(Fields.IrrevocablePowerOfAttorney) ?? throw new ArgumentNullException(Resources.Error_EmptyRevocationPossibleType);

        /// <summary>
        /// Признак безотзывной доверенности
        /// </summary>
        public int? RevocationCondition => mrpSection.GetIntValue(Fields.RevocationCondition);

        /// <summary>
        /// Физическое лицо, действующее без доверенности
        /// </summary>
        public StaffEmployee PrincipalWithoutPowerOfAttorneyIndividual => GetReferenceFromMrpSectionField<StaffEmployee>(Fields.IndividualActingWithoutPowerOfAttorney);


        /// <summary>
        /// Признак доверенности в рамках передоверия
        /// </summary>
        public bool IsRetrusted() => mrpSection[Fields.ParentalPowerOfAttorney] != null;

        public IEnumerable<string> RepresentativePowers => powersWithCodesSection?.Select(t => t.GetStringValue(Fields.PowersTextDescription)) ?? Enumerable.Empty<string>();

        public StaffEmployee Signer => signerSection.First().GetReferenceFieldValue<StaffEmployee>(context, CardDocument.Signers.Signer);

        public Guid? PowerOfAttorneyCardId
        {
            get { return mrpSection.GetGuidValue(Fields.PowerOfAttorneyCardId); }
            set { mrpSection[Fields.PowerOfAttorneyCardId] = value; }
        }


        // Идентификатор конечного получателя файла доверенности
        public string GenFinalRecipientTaxID => genMchdSection.GetStringValue("taxAuthPOAValid");

        private T GetReferenceFromMrpSectionField<T>(string fieldAlias) where T : ObjectBase
        {
            return mrpSection.GetReferenceFieldValue<T>(context, fieldAlias);
        }
    }
}
