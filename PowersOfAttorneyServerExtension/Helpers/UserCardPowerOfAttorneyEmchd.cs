using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;

using System;

using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyEMHCDData;

namespace PowersOfAttorneyServerExtension.Helpers
{
    internal partial class UserCardPowerOfAttorney
    {
        /// <summary>
        /// Код формы по КНД
        /// </summary>
        public string GenKnd => "0711001"; // Взят пример КНД

        /// <summary>
        /// Тип доверителя
        /// </summary>
        public GenPrincipalTypes? GenPrincipalType => genMchdSection.GetEnumValue<GenPrincipalTypes>("principalType");

        /// <summary>
        /// Единоличным исполнительным органом выступает индивидуальный предприниматель
        /// </summary>
        public bool? GenSolePropSEB => genMchdSection.GetBoolValue("solePropSEB");

        /// <summary>
        /// Вид полномочий единоличного исполнительного органа
        /// </summary>
        public GenPowersTypeOfSEBTypes? GenpPowersTypeOfSEB => genMchdSection.GetEnumValue<GenPowersTypeOfSEBTypes>("powersTypeOfSEB");

        /// <summary>
        /// Единоличным исполнительным органом выступает управляющая компания
        /// </summary>
        public bool? GenMngtCompanySEB => genMchdSection.GetBoolValue("mngtCompanySEB");

        /// <summary>
        /// Единоличным исполнительным органом выступает физическое лицо
        /// </summary>
        public bool? GenIndSEB => genMchdSection.GetBoolValue("indSEB");

        /// <summary>
        /// ИНН руководителя организации
        /// </summary>
        public string GenCeoIIN => genMchdSection.GetStringValue("ceoIIN");

        /// <summary>
        /// СНИЛС руководителя организации
        /// </summary>
        public string GenCeoSNILS => genMchdSection.GetStringValue("ceoSNILS");

        /// <summary>
        /// Должность руководителя организации
        /// </summary>
        public string GenCeoPosition => genMchdSection.GetStringValue("ceoPosition");

        /// <summary>
        /// Регистрационный номер доверенности в реестре нотариальных действий
        /// </summary>
        public string GenPoaRegNumNotarRegistry => genMchdSection.GetStringValue("poaRegNumNotarRegistry");

        /// <summary>
        /// Дополнительный идентификатор доверенности
        /// </summary>
        public string GenAddPOAID => genMchdSection.GetStringValue("addPOAID");

        /// <summary>
        /// Дата внутренней регистрации доверенности
        /// </summary>
        public DateTime? GenPoaInternRegDate => genMchdSection.GetDateValue("poaInternRegDate");

        /// <summary>
        /// Дата совершения (выдачи) доверенности
        /// </summary>
        public DateTime? GenPoaDateOfIssue => genMchdSection.GetDateValue("poaDateOfIssue");

        /// <summary>
        /// Срок действия доверенности
        /// </summary>
        public DateTime? GenPoaExpirationDate => genMchdSection.GetDateValue("poaExpirationDate");

        /// <summary>
        /// Код налогового органа, в который представляется доверенность
        /// </summary>
        public string GenTaxAuthPOASubmit => genMchdSection.GetStringValue("taxAuthPOASubmit");

        /// <summary>
        /// Сведения об информационной системе, в которой формируется доверенность
        /// </summary>
        public string GenInfsysGenPOA => genMchdSection.GetStringValue("infsysGenPOA");

        /// <summary>
        /// Тип представителя
        /// </summary>
        public GenRepresentativeTypes? GenRepresentativeType => genMchdSection.GetEnumValue<GenRepresentativeTypes>("representativeType");

        /// <summary>
        /// Место совершения доверенности
        /// </summary>
        public string GenPoaIssuePlace => genMchdSection.GetStringValue("poaIssuePlace");

        /// <summary>
        /// Направление сформированного электронного документа в ЛК доверителя на сайте ЕПГУ
        /// </summary>
        public bool? GenSendDocPrinEPGU => genMchdSection.GetBoolValue("sendDocPrinEPGU");

        /// <summary>
        /// Направление сформированного электронного документа в ЛК поверенного на сайте ЕПГУ
        /// </summary>
        public bool? GenSendDocReprEPGU => genMchdSection.GetBoolValue("sendDocReprEPGU");

        /// <summary>
        /// Направление сформированного электронного документа в ЛК заявителя на сайте ЕПГУ
        /// </summary>
        public bool? GenSendDocApplFNP => genMchdSection.GetBoolValue("sendDocApplFNP");

        /// <summary>
        /// Направление сформированного электронного документа в ЛК поверенного на сайте ЕПГУ
        /// </summary>
        public bool? GenSendDocReprFNP => genMchdSection.GetBoolValue("sendDocReprFNP");

        /// <summary>
        /// Сведения об уплате за совершение нотариального действия
        /// </summary>
        public decimal? GenPayNotarAction => genMchdSection.GetDecimalValue("payNotarAction");

        /// <summary>
        /// Предоставленная льгота на сумму
        /// </summary>
        public decimal? GenAmountOfBenefGranted => genMchdSection.GetDecimalValue("amountOfBenefGranted");

        /// <summary>
        /// Способ выдачи электронного нотариального документа
        /// </summary>
        public string GenMethOfIssElNotarDoc => genMchdSection.GetStringValue("methOfIssElNotarDoc");

        /// <summary>
        /// Иной способ выдачи
        /// </summary>
        public string GenAnotherMethOfIss => genMchdSection.GetStringValue("anotherMethOfIss");

        /// <summary>
        /// Дополнительные сведения
        /// </summary>
        public string GenAddInfo => genMchdSection.GetStringValue("addInfo");

        /// <summary>
        /// Иные сведения удостоверительной надписи
        /// </summary>
        public string GenOtherInfoOfCertInscr => genMchdSection.GetStringValue("otherInfoOfCertInscr");

        /// <summary>
        /// Должность нотариуса
        /// </summary>
        public string GenNotaryPosition => genMchdSection.GetStringValue("notaryPosition");

        /// <summary>
        /// Регистрационный номер нотариуса в Минюсте
        /// </summary>
        public string GenRegNumOfNotaryInMoJ => genMchdSection.GetStringValue("regNumOfNotaryInMoJ");

        /// <summary>
        /// Должность ВРИО нотариуса
        /// </summary>
        public string GenPositionOfInterimNotary => genMchdSection.GetStringValue("positionOfInterimNotary");

        /// <summary>
        /// Регистрационный номер лица, сдавшего квалификационный экзамен в Минюсте
        /// </summary>
        public string GenRegNumOfIntNotarInMoJ => genMchdSection.GetStringValue("regNumOfIntNotarInMoJ");

        /// <summary>
        /// Хеш PDF документа
        /// </summary>
        public string GenPdfDocHash => genMchdSection.GetStringValue("pdfDocHash");

        /// <summary>
        /// Изображение подписи
        /// </summary>
        public string GenSignatureImage => genMchdSection.GetStringValue("signatureImage");

        /// <summary>
        /// Хеш подписи
        /// </summary>
        public string GenSignatureHash => genMchdSection.GetStringValue("signatureHash");

        /// <summary>
        /// Дата и время подписи
        /// </summary>
        public DateTime? GenDateTimeOfSignature => genMchdSection.GetDateValue("dateTimeOfSignature");

        /// <summary>
        /// Дата и время подписи
        /// </summary>
        public RevocationPossibleType? GenSignTransferIrrevPOA => genMchdSection.GetEnumValue<RevocationPossibleType>("signTransferIrrevPOA");

        /// <summary>
        /// Описание условия отзыва безотзывной доверенности
        /// </summary>
        public string GenConditRevocIrrevPOA => genMchdSection.GetStringValue("conditRevocIrrevPOA");

        /// <summary>
        /// Наименование юридического лица
        /// </summary>
        public string GenEntityPrinName => genMchdSection.GetStringValue("entityPrinName");

        /// <summary>
        /// ИНН юридического лица-доверителя
        /// </summary>
        public string GenEntityPrinINN => genMchdSection.GetStringValue("entityPrinINN");

        /// <summary>
        /// КПП юридического лица-доверителя
        /// </summary>
        public string GenEntityPrinKPP => genMchdSection.GetStringValue("entityPrinKPP");

        /// <summary>
        /// ОГРН юрлица-доверителя
        /// </summary>
        public string GenEntPrinOGRN => genMchdSection.GetStringValue("entPrinOGRN");

        /// <summary>
        /// Наименование учредительного документа юрлица-доверителя
        /// </summary>
        public string GenConstDocumentEntPrin => genMchdSection.GetStringValue("constDocumentEntPrin");

        /// <summary>
        /// Контактный телефон юрлица-доверителя
        /// </summary>
        public string GenTelNumEntPrin => genMchdSection.GetStringValue("telNumEntPrin");

        /// <summary>
        /// Адрес электронной почты юрлица-доверителя
        /// </summary>
        public string GenEmailEntPrin => genMchdSection.GetStringValue("emailEntPrin");

        /// <summary>
        /// Номер записи руководителя организации в едином регистре населения
        /// </summary>
        public string GenCeoRecordNumUnifPopReg => genMchdSection.GetStringValue("ceoRecordNumUnifPopReg");

        /// <summary>
        /// Номер записи представителя в едином регистре населения
        /// </summary>
        public string GenReprRecordNumUnifPopReg => genMchdSection.GetStringValue("reprRecordNumUnifPopReg");

        /// <summary>
        /// Дата рождения руководителя организации
        /// </summary>
        public DateTime? GenCeoDateOfBirth => genMchdSection.GetDateValue("ceoDateOfBirth");

        /// <summary>
        /// Дата рождения представителя
        /// </summary>
        public DateTime? GenReprDateOfBirth => genMchdSection.GetDateValue("reprDateOfBirth");

        /// <summary>
        /// Место рождения руководителя организации
        /// </summary>
        public string GenCeoPlaceOfBirth => genMchdSection.GetStringValue("ceoPlaceOfBirth");

        /// <summary>
        /// Место рождения представителя
        /// </summary>
        public string GenReprPlaceOfBirth => genMchdSection.GetStringValue("reprPlaceOfBirth");

        /// <summary>
        /// Статус руководителя организации, как участника нотариального действия
        /// </summary>
        public NotarialActionParticipantStatus? GenNotarStatOfSoleExBody => genMchdSection.GetEnumValue<NotarialActionParticipantStatus>("notarStatOfSoleExBody");

        /// <summary>
        /// Признак наличия гражданства руководителя организации
        /// </summary>
        public CitizenshipType? GenCeoCitizenshipSign => genMchdSection.GetEnumValue<CitizenshipType>("ceoCitizenshipSign");

        /// <summary>
        /// Признак наличия гражданства представителя
        /// </summary>
        public CitizenshipType? GenReprCitizenshipSign => genMchdSection.GetEnumValue<CitizenshipType>("reprCitizenshipSign");

        /// <summary>
        /// Гражданство руководителя
        /// </summary>
        public string GenCeoCitizenship => genMchdSection.GetStringValue("ceoCitizenship");

        /// <summary>
        /// Гражданство представителя
        /// </summary>
        public string GenReprCitizenship => genMchdSection.GetStringValue("reprCitizenship");

        /// <summary>
        /// Контактный телефон руководителя организации
        /// </summary>
        public string GenCeoPhoneNum => genMchdSection.GetStringValue("ceoPhoneNum");

        /// <summary>
        /// Контактный телефон представителя
        /// </summary>
        public string GenReprPhoneNum => genMchdSection.GetStringValue("reprPhoneNum");

        /// <summary>
        /// Адрес электронной почты руководителя организации
        /// </summary>
        public string GenCeoEmail => genMchdSection.GetStringValue("ceoEmail");

        /// <summary>
        /// Адрес электронной почты представителя
        /// </summary>
        public string GenReprEmail => genMchdSection.GetStringValue("reprEmail");

        /// <summary>
        /// Наименование документа, подтверждающий полномочия руководителя организации
        /// </summary>
        public string GenDocConfAuthCEO => genMchdSection.GetStringValue("docConfAuthCEO");

        /// <summary>
        /// Дата выдачи документа, подтверждающего полномочия руководителя организации
        /// </summary>
        public DateTime? GenDateIssDocConfAuthCEO => genMchdSection.GetDateValue("dateIssDocConfAuthCEO");

        /// <summary>
        /// Наименование органа, выдавшего документ, подтверждающий полномочия руководителя организации
        /// </summary>
        public string GenAuthIssDocConfCEOAuth => genMchdSection.GetStringValue("authIssDocConfCEOAuth");

        /// <summary>
        /// Сведения об удостоверении документа, подтверждающего полномочия руководителя организации
        /// </summary>
        public string GenInfoAttestDocConfCEOAuth => genMchdSection.GetStringValue("infoAttestDocConfCEOAuth");

        /// <summary>
        /// Адрес руководителя организации_Субъект Российской Федерации
        /// </summary>
        public string GenCeoAddrSubRussia => genMchdSection.GetStringValue("ceoAddrSubRussia");

        /// <summary>
        /// Идентификатор адреса руководителя организации по ФИАС
        /// </summary>
        public string GenCeoFIASAddrID => genMchdSection.GetStringValue("ceoFIASAddrID");

        /// <summary>
        /// Адрес руководителя организации в РФ
        /// </summary>
        public string GenCeoAddrRussia => genMchdSection.GetStringValue("ceoAddrRussia");

        /// <summary>
        /// ФИАС адрес руководителя организации в Российской Федерации
        /// </summary>
        public string GenFiasCEOAddrRussia => genMchdSection.GetStringValue("fiasCEOAddrRussia");

        /// <summary>
        /// Адрес представителя_Субъект Российской Федерации
        /// </summary>
        public string GenReprAddrSubRussia => genMchdSection.GetStringValue("reprAddrSubRussia");

        /// <summary>
        /// Идентификатор адреса представителя по ФИАС
        /// </summary>
        public string GenReprFIASAddrID => genMchdSection.GetStringValue("reprFIASAddrID");

        /// <summary>
        /// Адрес представителя в Российской Федерации
        /// </summary>
        public string GenReprAddrRussia => genMchdSection.GetStringValue("reprAddrRussia");

        /// <summary>
        /// ФИАС адрес представителя в Российской Федерации
        /// </summary>
        public string GenFiasReprAddrRussia => genMchdSection.GetStringValue("fiasReprAddrRussia");

        /// <summary>
        /// Адрес юридического лица_Субъект Российской Федерации
        /// </summary>
        public string GenEntAddrSubRussia => genMchdSection.GetStringValue("entAddrSubRussia");

        /// <summary>
        /// Идентификатор адреса юрлица по ФИАС
        /// </summary>
        public string GenEntFIASAddrID => genMchdSection.GetStringValue("entFIASAddrID");

        /// <summary>
        /// Адрес юрлица в РФ
        /// </summary>
        public string GenEntAddrRussia => genMchdSection.GetStringValue("entAddrRussia");

        /// <summary>
        /// ФИАС адрес юрлица в Российской Федерации
        /// </summary>
        public string GenFiasEntAddrRussia => genMchdSection.GetStringValue("fiasEntAddrRussia");

        /// <summary>
        /// Серия и номер документа, удостоверяющего личность руководителя организации
        /// </summary>
        public string GenSerNumCEOIDDoc => genMchdSection.GetStringValue("serNumCEOIDDoc");

        /// <summary>
        /// Дата выдачи документ, удостоверяющего личность руководителя организации
        /// </summary>
        public DateTime? GenDateIssCEOIDDoc => genMchdSection.GetDateValue("dateIssCEOIDDoc");

        /// <summary>
        /// Наименование органа, выдавшего документ, удостоверяющего личность руководителя организации
        /// </summary>
        public string GenAuthIssCEOIDDoc => genMchdSection.GetStringValue("authIssCEOIDDoc");

        /// <summary>
        /// Код подразделения органа, выдавшего документ, удостоверяющий личность руководителя организации
        /// </summary>
        public string GenCodeAuthDivIssCEOIDDoc => genMchdSection.GetStringValue("codeAuthDivIssCEOIDDoc");

        /// <summary>
        /// Фамилия руководителя организации
        /// </summary>
        public string GenCeoLastName => genMchdSection.GetStringValue("ceoLastName");

        /// <summary>
        /// Имя руководителя организации
        /// </summary>
        public string GenCeoName => genMchdSection.GetStringValue("ceoName");

        /// <summary>
        /// Отчество руководителя организации
        /// </summary>
        public string GenCeoMiddleName => genMchdSection.GetStringValue("ceoMiddleName");

        /// <summary>
        /// Код вида документа, удостоверяющего личность представителя
        /// </summary>
        public int? GenTypeCodeReprIDDoc => genMchdSection.GetIntValue("typeCodeReprIDDoc");

        /// <summary>
        /// Серия и номер документа, удостоверяющего личность представителя
        /// </summary>
        public string GenSerNumReprIDDoc => genMchdSection.GetStringValue("serNumReprIDDoc");

        /// <summary>
        /// Дата выдачи документ, удостоверяющего личность представителя
        /// </summary>
        public DateTime? GenDateIssReprIDDoc => genMchdSection.GetDateValue("dateIssReprIDDoc");

        /// <summary>
        /// Наименование органа, выдавшего документ, удостоверяющего личность представителя
        /// </summary>
        public string GenAuthIssReprIDDoc => genMchdSection.GetStringValue("authIssReprIDDoc");

        /// <summary>
        /// Код подразделения органа, выдавшего документ, удостоверяющий личность представителя
        /// </summary>
        public string GenCodeAuthDivIssReprIDDoc => genMchdSection.GetStringValue("codeAuthDivIssReprIDDoc");

        /// <summary>
        /// Дата истечения срока действия документа, удостоверяющего личность руководителя организации
        /// </summary>
        public DateTime? GenDateExpCEOIDDoc => genMchdSection.GetDateValue("dateExpCEOIDDoc");

        /// <summary>
        /// Код вида документа, удостоверяющего личность руководителя организации
        /// </summary>
        public int? GenTypeCodeCEOIDDoc => genMchdSection.GetIntValue("typeCodeCEOIDDoc");

        /// <summary>
        /// Дата истечения срока действия документа, удостоверяющего личность представителя
        /// </summary>
        public DateTime? GenDateExpReprIDDoc => genMchdSection.GetDateValue("dateExpReprIDDoc");

        /// <summary>
        /// Фамилия представителя
        /// </summary>
        public string GenReprLastName => genMchdSection.GetStringValue("reprLastName");

        /// <summary>
        /// Имя представителя
        /// </summary>
        public string GenReprName => genMchdSection.GetStringValue("reprName");

        /// <summary>
        /// Отчество представителя
        /// </summary>
        public string GenReprMiddleName => genMchdSection.GetStringValue("reprMiddleName");

        /// <summary>
        /// Внутренний номер доверенности
        /// </summary>
        public string GenInternalPOANumber => genMchdSection.GetStringValue("internalPOANumber");

        /// <summary>
        /// Единый регистрационный номер доверенности
        /// </summary>
        public Guid? GenSinglePOAregnumber => genMchdSection.GetGuidValue("singlePOAregnumber");

        /// <summary>
        /// Юрлицо-доверитель
        /// </summary>
        public StaffUnit GenEntityPrincipal => genMchdSection.GetReferenceFieldValue<StaffUnit>(context, "entityPrincipal");

        /// <summary>
        /// Руководитель организации
        /// </summary>
        public StaffEmployee GenCeo => genMchdSection.GetReferenceFieldValue<StaffEmployee>(context, "ceo");

        /// <summary>
        /// Представитель
        /// </summary>
        public StaffEmployee GenRepresentative => genMchdSection.GetReferenceFieldValue<StaffEmployee>(context, "representative");

        /// <summary>
        /// ИНН представителя
        /// </summary>
        public string GenRepresentativeINN => genMchdSection.GetStringValue("representativeINN");

        /// <summary>
        /// СНИЛС представителя
        /// </summary>
        public string GenRepresentativeSNILS => genMchdSection.GetStringValue("representativeSNILS");

        /// <summary>
        /// Должность представителя
        /// </summary>
        public string GenRepresentativePosition => genMchdSection.GetStringValue("representativePosition");

        /// <summary>
        /// Фамилия нотариуса
        /// </summary>
        public string GenNotayLastName => genMchdSection.GetStringValue("notayLastName");

        /// <summary>
        /// Имя нотариуса
        /// </summary>
        public string GenNotaryName => genMchdSection.GetStringValue("notaryName");

        /// <summary>
        /// Отчество нотариуса
        /// </summary>
        public string GenNotaryMiddleName => genMchdSection.GetStringValue("notaryMiddleName");

        /// <summary>
        /// Фамилия ВРИО нотариуса
        /// </summary>
        public string GenInterimNotaryLastName => genMchdSection.GetStringValue("interimNotaryLastName");

        /// <summary>
        /// Имя ВРИО нотариуса
        /// </summary>
        public string GenInterimNotarytName => genMchdSection.GetStringValue("interimNotarytName");

        /// <summary>
        /// Отчество ВРИО нотариуса
        /// </summary>
        public string GenInterimNotaryMiddleName => genMchdSection.GetStringValue("interimNotaryMiddleName");

        /// <summary>
        /// Фамилия лица, подписавшего доверенность
        /// </summary>
        public string GenPoaSignerLastName => genMchdSection.GetStringValue("poaSignerLastName");

        /// <summary>
        /// Имя лица, подписавшего доверенность
        /// </summary>
        public string GenPoaSignerName => genMchdSection.GetStringValue("poaSignerName");

        /// <summary>
        /// Отчество лица, подписавшего доверенность
        /// </summary>
        public string GenPoaSignerMiddleName => genMchdSection.GetStringValue("poaSignerMiddleName");

        /// <summary>
        /// Условие отзыва безотзывной доверенности
        /// </summary>
        public RevocationCondition? GenRevocIrrevPOA => genMchdSection.GetEnumValue<RevocationCondition>("revocIrrevPOA");

        /// <summary>
        /// Признак возможности оформления передоверия
        /// </summary>
        public GenPossibilityOfSubstitutionTypes? GenPossibilityOfSubstitution => genMchdSection.GetEnumValue<GenPossibilityOfSubstitutionTypes>("possibilityOfSubstitution");

        /// <summary>
        /// Пол руководителя организации
        /// </summary>
        public PowerOfAttorneyEMHCDData.Gender? GenCeoGender => genMchdSection.GetEnumValue<PowerOfAttorneyEMHCDData.Gender>("ceoGender");

        /// <summary>
        /// Пол представителя
        /// </summary>
        public PowerOfAttorneyEMHCDData.Gender? GenGenderOfRepresentative => genMchdSection.GetEnumValue<PowerOfAttorneyEMHCDData.Gender>("genderOfRepresentative");

        /// <summary>
        /// Вид доверенности
        /// </summary>
        public GenPoaKindTypes? GenPoaKind => genMchdSection.GetEnumValue<GenPoaKindTypes>("poaKind");

        /// <summary>
        /// Код налогового органа, в отношении которого действует доверенность
        /// </summary>
        public string GenTaxAuthPOAValid => genMchdSection.GetStringValue("taxAuthPOAValid");

        public NotarialActionParticipantStatus? GenNotarStatusOfEntPrin => genMchdSection.GetEnumValue<NotarialActionParticipantStatus>("notarStatusOfEntPrin");


        /// <summary>
        /// Вид доверенности
        /// </summary>
        internal enum GenPoaKindTypes
        {
            /// <summary>
            /// С возможностью отзыва
            /// </summary>
            ordinaryPOA = 1,
            /// <summary>
            /// Без возможности отзыва
            /// </summary>
            irrecovablePOA = 2
        }

  
        /// <summary>
        /// Признак возможности оформления передоверия
        /// </summary>
        internal enum GenPossibilityOfSubstitutionTypes
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


        /// <summary>
        /// 
        /// </summary>
        internal enum GenPrincipalTypes
        {
            /// <summary>
            /// Юридическое лицо
            /// </summary>
            entity = 1,
            /// <summary>
            /// Иностранное юридическое лицо
            /// </summary>
            foreignEntity = 2,
            /// <summary>
            /// Индивидуальный предприниматель
            /// </summary>
            soleProprietor = 3,
            /// <summary>
            /// Физическое лицо
            /// </summary>
            individual = 4
        }

        /// <summary>
        /// Вид полномочий единоличного исполнительного органа
        /// </summary>
        internal enum GenPowersTypeOfSEBTypes
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

        /// <summary>
        /// Тип представителя
        /// </summary>
        internal enum GenRepresentativeTypes
        {
            /// <summary>
            /// Юридическое лицо
            /// </summary>
            entity = 1,
            /// <summary>
            /// Индивидуальный предприниматель
            /// </summary>
            soleProprietor = 2,
            /// <summary>
            /// Физическое лицо
            /// </summary>
            individual = 3,
            /// <summary>
            /// Филиал (обособленное подразделение) юридического лица
            /// </summary>
            rusEntityBranch = 4,
            /// <summary>
            /// Филиал (аккредитованное представительство) иностранного юридического лица
            /// </summary>
            foreignEntBranch = 5
        }
    }
}
