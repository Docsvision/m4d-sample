using DocsVision.BackOffice.CardLib.CardDefs;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.ObjectModel;

using System;
using System.Collections.Generic;
using System.Linq;


namespace PowersOfAttorneyServerExtension.Helpers
{
    internal class UserCardPowerOfAttorney
    {
        private static Guid machineReadablePowerOfAttorneySectionId = new Guid("3B9A9466-7569-4142-AF68-DE60263A9640");
        private static Guid powersSectionId = new Guid("0838CD2A-27C4-4EE1-B599-2DF6586AD2A7");

        private readonly Document document;
        private readonly ObjectContext context;
        private readonly BaseCardSectionRow mrpSection;
        private readonly IList<BaseCardSectionRow> powersSection;
        private readonly IList<BaseCardSectionRow> signerSection;

        public UserCardPowerOfAttorney(Document document, ObjectContext context)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.context = context ?? throw new ArgumentNullException(nameof(context));

            mrpSection = (BaseCardSectionRow)document.GetSection(machineReadablePowerOfAttorneySectionId)[0];
            powersSection = (IList<BaseCardSectionRow>)document.GetSection(powersSectionId);
            signerSection = (IList<BaseCardSectionRow>)document.GetSection(CardDocument.Signers.ID);
        }

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
        public DateTime IssueDateOfDocumentProvingIdentityOfIAWPOA => mrpSection.GetDateValue(Fields.IssueDateOfDocumentProvingIdentityOfIAWPOA) ?? throw new ArgumentNullException("Дата выдачи документа, удостоверяющего физлицо, действующее без доверенности");

        /// <summary>
        /// Наименование учредительного документа организации-доверителя
        /// </summary>
        public string PrincipalOrganizationConstituentDocument => mrpSection.GetStringValue(Fields.NameOfConstituentDocumentofPrincipal);

        /// <summary>
        /// Тип уполномоченного представителя
        /// </summary>
        public int RepresentativeType => mrpSection.GetIntValue(Fields.TypeOfAuthorizedRepresentative) ?? throw new ArgumentNullException("Тип уполномоченного представителя");

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
        public DateTime RepresentativeIndividualDocumentIssueDate => mrpSection.GetDateValue(Fields.IssueDateOfDocumentProvingIdentityOfRepresentative) ?? throw new ArgumentNullException("Дата выдачи документа, удостоверяющего физлицо-представителя");

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
        public int CodeOfCitizenshipForForeignRepresentative => mrpSection.GetIntValue(Fields.CodeOfCitizenshipForForeignRepresentative) ?? throw new ArgumentNullException("Код страны гражданства иностранного представителя");

        /// <summary>
        /// Признак гражданства представителя
        /// </summary>
        public int RepresentativeCitizenshipType => mrpSection.GetIntValue(Fields.SignOfCitizenshipOfRepresentative) ?? throw new ArgumentNullException(" Признак гражданства представителя");

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
        public int PrincipalWithoutPowerOfAttorneyIndividualCountryCode => mrpSection.GetIntValue(Fields.CodeOfCitizenshipForForeignIAWPOA) ?? throw new ArgumentNullException("Код страны гражданства для иностранного физлица, действующего без доверенности");

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
        public int PrincipalType => mrpSection.GetIntValue(Fields.PrincipalType) ?? throw new ArgumentNullException("Тип доверителя");

        /// <summary>
        /// Доверенность, на основании которой осуществляется передоверие
        /// </summary>
        public Document ParentalPowerOfAttorneyUserCard => GetReferenceFromMrpSectionField<Document>(Fields.ParentalPowerOfAttorney);
        
        public PowerOfAttorney ParentalPowerOfAttorney
        {
            get
            {
                var machineReadablePowerOfAttorneySectionRow = ParentalPowerOfAttorneyUserCard.GetSection(machineReadablePowerOfAttorneySectionId)[0] as BaseCardSectionRow;
                var powerOfAttorney = GetReferenceFromSectionField<PowerOfAttorney>(machineReadablePowerOfAttorneySectionRow, Fields.PowerOfAttorneyCardId);
                return powerOfAttorney ?? throw new ArgumentNullException("Доверенность, на основании которой осуществляется передоверие, не найдена");
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
        public DateTime PowerOfAttorneyStartDate => mrpSection.GetDateValue(Fields.PowerOfAttorneyStartDate) ?? throw new ArgumentNullException("Дата совершения доверенности");

        /// <summary>
        /// Дата окончания действия доверенности
        /// </summary>
        public DateTime PowerOfAttorneyEndDate => mrpSection.GetDateValue(Fields.PowerOfAttorneyEndDate) ?? throw new ArgumentNullException("Дата окончания действия доверенности");

        /// <summary>
        /// Основная (первоначальная) доверенность
        /// </summary>
        public PowerOfAttorney BasePowerOfAttorney => GetReferenceFromMrpSectionField<PowerOfAttorney>(Fields.BasicPOA);

        /// <summary>
        /// Признак наличия гражданства лица, действующего без доверенности
        /// </summary>
        public int PrincipalWithoutPowerOfAttorneyIndividualCitizenship => mrpSection.GetIntValue(Fields.SignOfCitizenshipfIAWPOA) ?? throw new ArgumentNullException("Признак наличия гражданства лица, действующего без доверенности");

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
        public int RetrustType => mrpSection.GetIntValue(Fields.PossibilityOfSubstitution) ?? throw new ArgumentNullException("Возможность оформления передоверия");

        /// <summary>
        /// Совместные полномочия
        /// </summary>
        public int JointRepresentation => mrpSection.GetIntValue(Fields.JointPowers) ?? throw new ArgumentNullException("Совместные полномочия");

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
        public int RevocationPossibleType => mrpSection.GetIntValue(Fields.IrrevocablePowerOfAttorney) ?? throw new ArgumentNullException("Признак безотзывной доверенности");

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

        public IEnumerable<string> RepresentativePowers => powersSection.Select(t => t.GetStringValue(Fields.PowersTextDescription));

        public StaffEmployee Signer => GetReferenceFromSectionField<StaffEmployee>(signerSection.First(), CardDocument.Signers.Signer);

        public Guid? PowerOfAttorneyCardId
        {
            get { return mrpSection.GetGuidValue(Fields.PowerOfAttorneyCardId); }
            set { mrpSection[Fields.PowerOfAttorneyCardId] = value; }
        }

        public PowerOfAttorneyFNSDOVBBData ConvertToPowerOfAttorneyFNSDOVBBData()
        {
            try
            {
                return UserCardToPowerOfAttorneyDataConverter.Convert(this);
            }
            catch (Exception ex)
            {
                throw new Exception($"Заполнены не все обязательные данные или данные некорректны. Измените данные и повторите сохранение. Дополнительная информация: {ex}");
            }
        }


        private T GetReferenceFromMrpSectionField<T>(string fieldAlias) where T : ObjectBase
        {
            return GetReferenceFromSectionField<T>(mrpSection, fieldAlias);
        }

        private T GetReferenceFromSectionField<T>(BaseCardSectionRow row, string fieldAlias) where T : ObjectBase
        {
            var id = row.GetGuidValue(fieldAlias);

            return id != null ? context.GetObject<T>(id) : null;
        }

        private static class Fields
        {
            // Идентификатор системной карточки доверенности
            public const string PowerOfAttorneyCardId = "PowerOfAttorneyCardId";
            // Описание условия отзыва
            public const string DescriptionOfTheRevocationCondition = "DescriptionOfTheRevocationCondition";
            // Номер доверенности
            public const string PowerOfAttorneyID = "PowerOfAttorneyID";
            // Юридическое лицо, действующее без доверенности
            public const string EntityActingWithoutPowerOfAttorney = "EntityActingWithoutPowerOfAttorney";
            // СНИЛС физического лица, действующего без доверенности
            public const string SNILSOfIndividualActingWithoutPowerOfAttorney = "SNILSOfIndividualActingWithoutPowerOfAttorney";
            // Наименование учредительного документа организации, действующей без доверенности
            public const string NameOfConstituentDocumentofEntityActingWithoutPowerOfAttorney = "NameOfConstituentDocumentofEntityActingWithoutPowerOfAttorney";
            // Сведения об удостоверении документа, подтверждающего полномочия, если он удостоверен
            public const string InformationAboutCertificationOfDocumentConfirmingPowers = "InformationAboutCertificationOfDocumentConfirmingPowers";
            //Серия и номер документа, удостоверяющего физлицо, действующее без доверенности
            public const string SeriesAndNumberOfDocumentProvingIdentityOfIAWPOA = "SeriesAndNumberOfDocumentProvingIdentityOfIAWPOA";
            //Дата выдачи документа, удостоверяющего физлицо, действующее без доверенности
            public const string IssueDateOfDocumentProvingIdentityOfIAWPOA = "IssueDateOfDocumentProvingIdentityOfIAWPOA";
            // Наименование учредительного документа организации-доверителя
            public const string NameOfConstituentDocumentofPrincipal = "NameOfConstituentDocumentofPrincipal";
            // Тип уполномоченного представителя
            public const string TypeOfAuthorizedRepresentative = "TypeOfAuthorizedRepresentative";
            // ИНН физического лица-представителя
            public const string INNOfIndividualRepresentative = "INNOfIndividualRepresentative";
            // Физическое лицо-представитель
            public const string IndividualRepresentative = "IndividualRepresentative";
            // Адрес места жительства в РФ физлица-представителя
            public const string ResidentialAddressInRussiaForIndividualRepresentative = "ResidentialAddressInRussiaForIndividualRepresentative";
            // НН физического лица, действующего без доверенности
            public const string IINOfIndividualActingWithoutPowerOfAttorney = "IINOfIndividualActingWithoutPowerOfAttorney";
            // Код вида документа, удостоверяющего личность физлица-представителя
            public const string KindCodeOfDocumentProvingIdentityRepresentative = "KindCodeOfDocumentProvingIdentityRepresentative";
            // Дата выдачи документа, удостоверяющего физлицо-представителя
            public const string IssueDateOfDocumentProvingIdentityOfRepresentative = "IssueDateOfDocumentProvingIdentityOfRepresentative";
            // Индивидуальный предприниматель-представитель
            public const string SoleProprietorRepresentative = "SoleProprietorRepresentative";
            // СНИЛС представителя
            public const string SNILSOfRepresentative = "SNILSOfRepresentative";
            // Текстовое описание полномочий
            public const string TextPowersDescription = "TextPowersDescription";
            // Дата выдачи документа, подтверждающего полномочия физлица, действующего без доверенности
            public const string IssueDateOfDocumentConfirmingPowersOfIAWPOA = "IssueDateOfDocumentConfirmingPowersOfIAWPOA";
            // Организация-представитель
            public const string EntityRepresentative = "EntityRepresentative";
            // Наименование учредительного документа организации-представителя
            public const string NameOfConstituentDocumentofRepresentative = "NameOfConstituentDocumentofRepresentative";
            // Код страны гражданства иностранного представителя
            public const string CodeOfCitizenshipForForeignRepresentative = "CodeOfCitizenshipForForeignRepresentative";
            // Признак гражданства представителя
            public const string SignOfCitizenshipOfRepresentative = "SignOfCitizenshipOfRepresentative";
            // Место рождения представителя
            public const string PlaceOfBirthOfRepresentative = "PlaceOfBirthOfRepresentative";
            // Наименование органа, выдавшего документ, удостоверяющий личность представителя
            public const string NameOfAuthorityIssuedDocumentConfirmingidentityOfRepresentative = "NameOfAuthorityIssuedDocumentConfirmingidentityOfRepresentative";
            // Наименование документа, подтверждающего полномочия физлица, действующего без доверенности
            public const string NameOfDocumentConfirmingPowersOfIAWPOA = "NameOfDocumentConfirmingPowersOfIAWPOA";
            // Наименование органа, выдавшего документ, удостоверяющий физлицо, действующее без доверенности
            public const string NameOfAuthorityIssuedDocumentProvingIdentityOfIAWPOA = "NameOfAuthorityIssuedDocumentProvingIdentityOfIAWPOA";
            // Код страны гражданства для иностранного физлица, действующего без доверенности
            public const string CodeOfCitizenshipForForeignIAWPOA = "CodeOfCitizenshipForForeignIAWPOA";
            // Серия и номер документа, удостоверяющего личность физлица-представителя
            public const string SeriesAndNumberOfDocumentProvingIdentityOfRepresentative = "SeriesAndNumberOfDocumentProvingIdentityOfRepresentative";
            // Место рождения физлица, действующего без доверенности
            public const string PlaceOfBirthOfIndividualActingWithoutPowerOfAttorney = "PlaceOfBirthOfIndividualActingWithoutPowerOfAttorney";
            // Номер записи об аккредитации
            public const string AccreditationRecordNumber = "AccreditationRecordNumber";
            // Тип доверителя
            public const string PrincipalType = "PrincipalType";
            //Указать юридическое лицо
            public const string IndicateEntity = "IndicateEntity";
            //Доверенность, на основании которой осуществляется передоверие
            public const string ParentalPowerOfAttorney = "parentalPowerOfAttorney";
            // Сведения об информационной системе, предоставляющей информацию о доверенности
            public const string InfoSysProvPOAData = "infoSysProvPOAData";
            // Доверитель (сотрудник в справочнике)
            public const string EmplPrincipal = "emplPrincipal";
            // Доверитель (организация в справочнике)
            public const string OrgPrincipal = "orgPrincipal";
            // Кем выдан документ, подтвержающий полномочия физлица, действующего без доверенности
            public const string WhoIssuedDoctConfPowersIAWPOA = "whoIssuedDoctConfPowersIAWPOA";
            // Дата совершения доверенности
            public const string PowerOfAttorneyStartDate = "PowerOfAttorneyStartDate";
            // Дата окончания действия доверенности
            public const string PowerOfAttorneyEndDate = "PowerOfAttorneyEndDate";
            //Основная (первоначальная) доверенность
            public const string BasicPOA = "BasicPOA";
            //Признак наличия гражданства лица, действующего без доверенности
            public const string SignOfCitizenshipfIAWPOA = "SignOfCitizenshipfIAWPOA";
            // Код подразделения органа, выдавшего документ, удостоверяющий физлицо, действующее без доверенности
            public const string SubdivisionCodeOfAuthorityIssuedDocumentProvingIdentityOfIAWPOA = "SubdivisionCodeOfAuthorityIssuedDocumentProvingIdentityOfIAWPOA";
            // Код подразделения органа, выдавшего документ, удостоверяющий личность представителя
            public const string SubdivisionCodeOfAuthorityIssuedDocumentConfirmingRepresentativeID = "SubdivisionCodeOfAuthorityIssuedDocumentConfirmingRepresentativeID";
            // Адрес места жительства в РФ физлица, действующего без доверенности
            public const string ResidentialAddressInRussiaForIndividualActingWithoutPowerOfAttorney = "ResidentialAddressInRussiaForIndividualActingWithoutPowerOfAttorney";
            // Cубъект РФ для физлица, действующего без доверенности
            public const string CodeOfSubjectOfRussiaForIAWPOA = "CodeOfSubjectOfRussiaForIAWPOA";
            // Адрес регистрации в РФ юрлица, действующего без доверенности
            public const string RegistrationAddressInRussiaOfEAWPA = "RegistrationAddressInRussiaOfEAWPA";
            // Фактический адрес нахождения в РФ юрлица, действующего без доверенности
            public const string ActualAddressInRussiaOfEAWPA = "ActualAddressInRussiaOfEAWPA";
            // Адрес регистрации в РФ организации-представителя
            public const string RegistrationAddressInRussiaOfOrganizationRepresantative = "RegistrationAddressInRussiaOfOrganizationRepresantative";
            // Cубъект РФ места жительства физлица-представителя
            public const string CodeOfSubjectOfRussiaForIndividualRepresentative = "CodeOfSubjectOfRussiaForIndividualRepresentative";
            // Cубъект РФ для адреса регистрации юрлица, действующего без доверенности
            public const string CodeOfSubjectOfRussiaForRegistrationAddressOfEAWPA = "CodeOfSubjectOfRussiaForRegistrationAddressOfEAWPA";
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
            public const string PossibilityOfSubstitution = "PossibilityOfSubstitution";
            // Совместные полномочия
            public const string JointPowers = "JointPowers";
            // Передоверие безотзывной доверенности
            public const string TransferOfIrrevocablePowerOfAttorney = "TransferOfIrrevocablePowerOfAttorney";
            // Полномочия при передоверии
            public const string LossOfPowersUponSubstitution = "LossOfPowersUponSubstitution";
            // Код вида документа, удостоверяющий физлицо, действующее без доверенности
            public const string KindCodeOfDocumentProvingIdentityOfIAWPOA = "KindCodeOfDocumentProvingIdentityOfIAWPOA";
            // Признак безотзывной доверенности
            public const string IrrevocablePowerOfAttorney = "IrrevocablePowerOfAttorney";
            // Условие отзыва
            public const string RevocationCondition = "RevocationCondition";
            // Физическое лицо, действующее без доверенности
            public const string IndividualActingWithoutPowerOfAttorney = "IndividualActingWithoutPowerOfAttorney";

            // Текстовое описание полномочий
            public const string PowersTextDescription = "PowersTextDescription";
            // Код полномочий
            public const string PowersCode = "PowersCode";
        }
    }
}