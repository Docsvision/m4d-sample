using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.ObjectModel;

using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSData;
using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSDOVEL502Data;
using static PowersOfAttorney.UserCard.Common.Helpers.UserCardPowerOfAttorney;

namespace PowersOfAttorney.UserCard.Common.Helpers
{
    public static class UserCardPowerOfAttorneyFNSDOVEL502Extensions
    {
        public static PowerOfAttorneyData ConvertToPowerOfAttorneyFNSDOVEL502Data(this UserCardPowerOfAttorney userCard, ObjectContext context)
        {

            return Converter.Convert(userCard, context);
        }

        private class Converter
        {
            private readonly UserCardPowerOfAttorney userCard;
            private readonly ObjectContext context;
            private readonly bool isRetrusted;
            private readonly RepresentativeType representativeType;

            private PowerOfAttorneyFNSDOVEL502Data poaFNSDOVEL502Data;

            public static PowerOfAttorneyFNSDOVEL502Data Convert(UserCardPowerOfAttorney card, ObjectContext context)
            {
                return new Converter(card, context).Convert();
            }

            private Converter(UserCardPowerOfAttorney card, ObjectContext ctx)
            {
                userCard = card ?? throw new ArgumentNullException(nameof(userCard));
                context = ctx ?? throw new ArgumentNullException(nameof(ctx));
                isRetrusted = userCard.IsRetrusted();
                representativeType = userCard.RepresentativeEnumValue.Value;
            }

            private PowerOfAttorneyFNSDOVEL502Data Convert()
            {
                poaFNSDOVEL502Data = new PowerOfAttorneyFNSDOVEL502Data();
                poaFNSDOVEL502Data.Document = CreatePowerOfAttorteyDocument();
                var temp = userCard.GenTaxAuthPOASubmit;
                poaFNSDOVEL502Data.RecipientID = temp;
                if (string.IsNullOrEmpty(poaFNSDOVEL502Data.FinalRecipientID))
                {
                    poaFNSDOVEL502Data.FinalRecipientID = temp;
                }
                poaFNSDOVEL502Data.SenderID = GetSenderID();
                return poaFNSDOVEL502Data;
            }

            /* Состав и структура документа (Документ) */
            private PowerOfAttorneyDocument CreatePowerOfAttorteyDocument()
            {
                var result = new PowerOfAttorneyDocument();
                /* Единый регистрационный номер доверенности (Обязательный) */
                result.PowerOfAttorneyNumber = userCard.InstanceId;
                /* Код налогового органа, в который представляется доверенность, созданная в электронной
                 * форме (Обязательный) 
                 */
                result.TaxAuthorityCode = userCard.GenTaxAuthPOASubmit;
                /* Доверенность (Обязательный с условием) 
                 * Обязательный при создании первоначальной доверенности
                 */
                if (!isRetrusted)
                    result.PowerOfAttorneyData = GetPowerOfAttorneyDocumentData();
                else
                    result.RetrustPowerOfAttorneyData = GetRetrustPowerOfAttorneyDocumentData();
                /* Сведения о физическом лице, подписывающем доверенность от имени доверителя (российской
                 * организации / иностранной организации) без доверенности или подписывающем доверенность
                 * от своего имени, в том числе при передаче полномочий другому лицу в порядке передоверия
                 * (Обязательный)
                 */
                result.Signer = GetFIO(userCard.Signer.Value);
                return result;
            }

            /* Доверенность (Довер) - 4.3 */
            private PowerOfAttorneyDocumentData GetPowerOfAttorneyDocumentData()
            {
                var result = new PowerOfAttorneyDocumentData();
                /* Сведения доверенности (Обязательный) */
                result.PowerOfAttorney = GetPowerOfAttorneyInfo();
                /* Сведения о доверителе (Обязательный) */
                result.Principal = GetPrincipalInfo();
                /* Сведения об уполномоченном представителе (Обязательный) */
                result.Representative = GetRepresentativeInfo();
                /* Признак (код) области полномочий уполномоченного представителя (Обязательный) */
                result.RepresentativePowers = GetRepresentativePowers();
                return result;
            }

            private List<PowersCode> GetRepresentativePowers()
            {
                var result = userCard.GetPowersCodes().ToList();
                const int maxCodeSize = 2;
                foreach (var item in result)
                {
                    if (item.Code.Length > maxCodeSize)
                    {
                        item.Code = item.Code.Substring(item.Code.Length - maxCodeSize);
                    }
                }
                return result;
            }

            /* Сведения доверенности (СвДов) - 4.4 */
            private PowerOfAttorneyInfo GetPowerOfAttorneyInfo()
            {
                var result = new PowerOfAttorneyInfo();
                /* Номер доверенности */
                result.InternalPowerOfAttorneyNumber = userCard?.GenInternalPOANumber;
                /* Дата совершения (выдачи) доверенности (Обязательный) */
                result.PowerOfAttorneyStartDate = userCard?.GenPoaDateOfIssue;
                /* Дата окончания срока действия доверенности (Обязательный) */
                result.PowerOfAttorneyEndDate = userCard?.GenPoaExpirationDate;
                /* Признак возможности оформления передоверия (Обязательный) */
                result.RetrustType = userCard?.PossibilityOfSubstitution502EnumValue;
                /* Код налогового органа, в отношении которого действует доверенность */
                result.TaxCodes = GetTaxCodes();
                return result;
            }

            private List<string> GetTaxCodes()
            {
                return new List<string>() {
                    userCard?.GenTaxAuthPOAValid
                };
            }

            /* Сведения о доверителе (СвДоверит) - 4.5 */
            private PrincipalInfo GetPrincipalInfo()
            {
                var result = new PrincipalInfo();
                result.RussianCompany = GetRussianCompanyInfo();
                /* Сведения о физическом лице (Обязательный с условием) */
                result.Individual = GetIndividualInfo3();
                /* Сведения об иностранной организации (Обязательный с условием) */
                //result.ForeignCompany = GetForeignCompanyInfo();                
                return result;

            }

            /* Сведения о российской организации (НПЮЛ) - 4.6 */
            private RussianCompanyInfo GetRussianCompanyInfo()
            {
                var org = userCard.PrincipalOrganization.Value;
                var result = new RussianCompanyInfo();
                /* Наименование организации (Обязательный) */
                result.Name = org?.Name;
                /* ИНН организации (Обязательный) */
                result.Inn = userCard?.GenEntityPrinINN;
                /* КПП организации (Обязательный) */
                result.Kpp = userCard?.GenEntityPrinKPP;
                /* ОГРН (Обязательный) */
                result.Ogrn = userCard?.GenEntPrinOGRN;
                /* Адрес юридического лица в Российской Федерации (Обязательный) */
                result.LegalAddress = GetAddressInfo();
                /* Сведения о лице, действующем от имени юридического лица без доверенности (Обязательный) */
                result.PrincipalWithoutPowerOfAttorney = GetPrincipalWithoutPowerOfAttorneyInfo();
                return result;
            }

            /* Сведения о лице, действующем от имени юридического лица без доверенности (ЛицоБезДов) */
            private PrincipalWithoutPowerOfAttorneyInfo GetPrincipalWithoutPowerOfAttorneyInfo()
            {
                var result = new PrincipalWithoutPowerOfAttorneyInfo();
                switch (userCard.ExecutiveBodyEnumValue.Value)
                {
                    case ExecutiveBodyType.individual:
                        /* Сведения по физическому лицу (Обязательный) */
                        result.Individual = GetIndividualInfo();
                        break;
                    case ExecutiveBodyType.entity:
                        /* Сведения об организации (Обязательный с условием) */
                        //result.Organization = GetOrganizationInfo();
                        result.Individual = GetIndividualInfo();
                        break;
                }
                return result;
            }

            /* Сведения по физическому лицу (СвФЛ) - 4.8 */
            private IndividualInfo GetIndividualInfo()
            {
                var result = new IndividualInfo();
                /* ИНН физического лица */
                result.Inn = userCard?.GenCeoIIN;
                /* СНИЛС (Обязательный) */
                result.Snils = userCard?.GenCeoSNILS;
                /* Гражданство */
                result.Citizenship = userCard?.GenCeoCitizenship;
                /* Дата рождения */
                result.BirthDate = userCard?.GenCeoDateOfBirth;
                /* Должность */
                result.Position = userCard?.GenCeoPosition;
                return result;
            }

            /* Передоверие (Передов) - 4.11 */
            private RetrustPowerOfAttorneyDocumentData GetRetrustPowerOfAttorneyDocumentData()
            {
                var result = new RetrustPowerOfAttorneyDocumentData();
                /* Сведения доверенности, совершённой (выданной) в рамках передоверия(Обязательный) */
                result.PowerOfAttorney = GetRetrustPowerOfAttorneyInfo();
                /* Сведения о доверителе в порядке передоверия(Обязательный) */
                result.Principal = GetRetrustPrincipalInfo();
                /* Сведения об уполномоченном представителе(Обязательный) */
                result.Representative = GetRepresentativeInfo();
                /* Признак(код) области полномочий уполномоченного представителя(Обязательный) */
                result.RepresentativePowers = GetRepresentativePowers();
                return result;
            }

            /* Сведения доверенности, совершённой (выданной) в рамках передоверия (СвДовП) - 4.12 */
            private RetrustPowerOfAttorneyInfo GetRetrustPowerOfAttorneyInfo()
            {
                var result = new RetrustPowerOfAttorneyInfo();
                /* Регистрационный номер доверенности, в отношении которой производится передоверие (Обязательный) */
                result.ParentPowerOfAttorneyNumber = userCard.GenParentalPowerOfAttorneyUserCard.Value.GetObjectId();
                /* Номер доверенности */
                result.InternalPowerOfAttorneyNumber = userCard.GenInternalPOANumber;
                /* Дата совершения (выдачи) доверенности (Обязательный) */
                result.PowerOfAttorneyStartDate = userCard.GenPoaDateOfIssue;
                /* Дата окончания срока действия доверенности (Обязательный) */
                result.PowerOfAttorneyEndDate = userCard.GenPoaExpirationDate;
                /* Признак возможности оформления передоверия (Обязательный) */
                result.RetrustType = userCard.PossibilityOfSubstitution502EnumValue;
                /* Сведения об основной доверенности (Обязательный) */
                result.ParentPrincipal = GetParentPrincipalInfo();
                if(result.ParentPrincipal == null)
                {
                    /* Сведения о доверителе из доверенности, на основании которой осуществляется передоверие (Обязательный с условием) */
                    result.PrimaryPowerOfAttorney = GetPrimaryPowerOfAttorneyInfo();
                }                
                /* Код налогового органа, в отношении которого действует доверенность */
                result.TaxCodes = GetTaxCodes();
                return result;
            }

            /* Сведения об основной доверенности (СвОснДов) - 4.13 */
            private PrimaryPowerOfAttorneyInfo GetPrimaryPowerOfAttorneyInfo()
            {
                var result = new PrimaryPowerOfAttorneyInfo();
                /* Единый регистрационный номер основной доверенности (Обязательный) */
                result.PowerOfAttorneyNumber = userCard.GenOriginalPowerOfAttorneyUserCard.Value.GetObjectId();
                /* Сведения о доверителе основной доверенности (Обязательный) */
                result.Principal = GetPrimaryPrincipalInfo();
                return result;
            }

            /* Сведения об основной доверенности (СвОснДов) - 4.13 */
            private PrimaryPrincipalInfo GetPrimaryPrincipalInfo()
            {
                var result = new PrimaryPrincipalInfo();

                /* Сведения о доверителе – российском юридическом лице (Обязательный с условием) */
                result.RussianCompany = GetRussianCompanyInfo1();
                /* Сведения о доверителе – физическом лице (Обязательный с условием) */
                result.Individual = GetIndividualInfo2();
                return result;
            }


            /* Сведения о доверителе из доверенности, на основании которой осуществляется передоверие */
            private ParentPrincipalInfo GetParentPrincipalInfo()
            {                
                var result = new ParentPrincipalInfo();                
                if(userCard.GenOriginalPowerOfAttorneyUserCard == null)
                {
                    return null;
                }
                var poaOrigCard = userCard.GenOriginalPowerOfAttorneyUserCard.Value;
                var poaAddSection = poaOrigCard.GetSection(new Guid("28D30FFE-5255-4614-AC84-A2B468BC04A8"))[0] as BaseCardSectionRow;
                var origExecBodyType = poaAddSection.GetEnumValue<ExecutiveBodyType>("ExecutiveBodyType");

                switch (origExecBodyType.Value)
                {
                    case ExecutiveBodyType.individual:
                        /* Сведения о доверителе – физическом лице (Обязательный с условием) */
                        result.Individual = GetIndividualInfo2();
                        break;
                    case ExecutiveBodyType.entity:
                        /* Сведения о доверителе – российском юридическом лице (Обязательный с условием) */
                        result.RussianCompany = GetOrganizationInfoForRetrust();
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(ExecutiveBodyType));
                }                                
                return result;
            }

            private OrganizationInfo GetOrganizationInfoForRetrust() 
            {                
                var originalPOA = userCard.GenOriginalPowerOfAttorneyUserCard.Value;
                var poaSection = originalPOA.GetSection(new Guid("29C1B4EF-48E4-47F0-AC67-C42CF68DE986"))[0] as BaseCardSectionRow;                
                var entityPrincipalCardId = poaSection.GetGuid("entityPrincipal");
                var entityPrincipalCard = context.GetObject<StaffUnit>(entityPrincipalCardId);

                var result = new OrganizationInfo();
                result.Name = entityPrincipalCard.Name;
                result.Inn = poaSection.GetStringValue("entPrinINN");
                result.Kpp = poaSection.GetStringValue("entityPrinKPP");
                result.Ogrn = poaSection.GetStringValue("entPrinOGRN");
                
                return result;
            }

            /* Сведения о доверителе в порядке передоверия (СвДоверщ) - 4.17 */
            private RetrustPrincipalInfo GetRetrustPrincipalInfo()
            {
                var result = new RetrustPrincipalInfo();
                var poaOrigCard = userCard.GenOriginalPowerOfAttorneyUserCard.Value;
                var poaAddSection = poaOrigCard.GetSection(new Guid("28D30FFE-5255-4614-AC84-A2B468BC04A8"))[0] as BaseCardSectionRow;
                var origExecBodyType = poaAddSection.GetEnumValue<ExecutiveBodyType>("ExecutiveBodyType");
                switch (origExecBodyType.Value) 
                {
                    case ExecutiveBodyType.individual:
                        /* Сведения по физическому лицу (Обязательный) */
                        result.Individual = GetIndividualInfo3();
                        break;
                    case ExecutiveBodyType.entity:
                        /* Сведения по физическому лицу (Обязательный) */
                        result.Individual = GetIndividualInfo3();
                        /* Сведения об организации (Обязательный с условием) */
                        result.Organization = GetOrganizationInfo();
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(ExecutiveBodyType));
                }
                return result;
            }

            /* Сведения об организации (СвОргТип) - 4.18 */
            private OrganizationInfo GetOrganizationInfo()
            {
                var org = userCard?.EntityWithoutPOA.Value;
                var result = new OrganizationInfo();
                /* Наименование организации / Наименование обособленного подразделения организации (Обязательный) */
                result.Name = org?.Name;
                /* ИНН организации (Обязательный) */
                result.Inn = userCard?.INNEntityRepresentative;
                /* КПП организации (Обязательный) */
                result.Kpp = userCard?.KPPEntityRepresentative;
                /* ОГРН */
                result.Ogrn = org?.OGRN;
                return result;
            }

            /* Сведения о российском юридическом лице (СведЮЛТип) - 4.19 */
            private RussianCompanyInfo1 GetRussianCompanyInfo1()
            {
                /* [CRINGE]
                 * Насколько это тупо что я получаю тут данные из объекной модели, кажется они тут уже должны быть...
                 */
                var originalPOA = userCard.GenOriginalPowerOfAttorneyUserCard.Value;
                var poaSection = originalPOA.GetSection(new Guid("29C1B4EF-48E4-47F0-AC67-C42CF68DE986"))[0] as BaseCardSectionRow;
                var entityPrincipalCardId = poaSection.GetGuid("singleFormOfPowerOfAttorney");
                var entityPrincipalCard = context.GetObject<StaffUnit>(entityPrincipalCardId);
                var result = new RussianCompanyInfo1();
                /* Наименование организации (Обязательный) */
                result.Name = entityPrincipalCard?.Name;
                /* ИНН организации (Обязательный) */
                result.Inn = entityPrincipalCard?.INN;
                /* КПП организации (Обязательный) */
                result.Kpp = entityPrincipalCard?.KPP;
                /* ОГРН (Обязательный) */
                result.Ogrn = entityPrincipalCard?.OGRN;
                return result;
            }

            /* Сведения об уполномоченном представителе (уполномоченных представителях) (СвПредТип) - 4.20 */
            private RepresentativeInfo GetRepresentativeInfo()
            {
                var result = new RepresentativeInfo();
                switch (representativeType)
                {
                    case RepresentativeType.entity:
                        /* Сведения об организации (Обязательный с условием) */
                        result.Organization = GetOrganizationInfo();
                        result.Individual = GetIndividualInfo2();
                        break;
                    case RepresentativeType.individual:
                        /* Сведения о физическом лице, в том числе индивидуальном предпринимателе (Обязательный) */
                        result.Individual = GetIndividualInfo2();
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(userCard.RepresentativeEnumValue));
                }

                return result;
            }

            private string GetSenderID()
            {
                // Remarks:
                //     Значение: для организаций – девятнадцатиразрядный код (ИНН и КПП организации)
                //     для физических лиц – двенадцатиразрядный код (ИНН физического лица, при наличии.
                //     При отсутствии ИНН – последовательность из двенадцати нулей)                
                switch (representativeType)
                {
                    case RepresentativeType.entity:
                        {
                            if (isRetrusted)
                            {
                                var origCard = GetOriginalPowerOfAttorney(userCard.GenOriginalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_UnableToGetOriginalPowerOfAttorneyUserCard));
                                return $"{origCard.INNEntityRepresentative}{origCard.KPPEntityRepresentative}";
                            }
                            else
                            {
                                return $"{userCard.INNEntityRepresentative}{userCard.KPPEntityRepresentative}";
                            }
                        }

                    case RepresentativeType.individual:
                        {
                            const string INNEmpty = "000000000000";
                            if (isRetrusted)
                            {
                                var origCard = GetOriginalPowerOfAttorney(userCard.GenOriginalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_UnableToGetOriginalPowerOfAttorneyUserCard));
                                var val = string.IsNullOrEmpty(origCard.GenCeoIIN)
                                    ? origCard.GenCeoIIN
                                    : INNEmpty;
                                return val;
                            }
                            else
                            {
                                var val = string.IsNullOrEmpty(userCard.GenCeoIIN)
                                    ? userCard.GenCeoIIN
                                    : INNEmpty;
                                return val;
                            }
                        }

                    default:
                        throw new InvalidEnumArgumentException(nameof(representativeType));
                }
            }

            private UserCardPowerOfAttorney GetOriginalPowerOfAttorney(Document origPoa)
            {
                if (origPoa == null)
                    throw new ArgumentNullException(nameof(origPoa));
                return new UserCardPowerOfAttorney(origPoa, context);
            }

            /* Сведения о физическом лице, в том числе индивидуальном предпринимателе (СвФизЛицТип) - 4.21 */
            private IndividualInfo2 GetIndividualInfo2()
            {
                var result = new IndividualInfo2();
                /* ИНН физического лица */
                result.Inn = userCard?.GenCeoIIN;
                /* СНИЛС (Обязательный) */
                result.Snils = userCard?.GenCeoSNILS;
                /* Гражданство */
                result.Citizenship = userCard?.GenCeoCitizenship;
                /* Дата рождения */
                result.BirthDate = userCard?.GenCeoDateOfBirth;
                /* Фамилия, имя, отчество физического лица (Обязательный) */
                result.Fio = GetFIO(userCard.RepresentativeIndividual.Value);
                /* Сведения о документе, удостоверяющем личность физического лица (Обязательный) */
                result.IdentityCard = GetIdentityCardInfo();
                return result;
            }

            /* Сведения по физическому лицу (СведФЛТип) - 4.22 */
            private IndividualInfo3 GetIndividualInfo3()
            {
                var result = new IndividualInfo3();
                /* ИНН физического лица */
                result.Inn = userCard?.GenCeoIIN;                
                /* СНИЛС (Обязательный) */
                result.Snils = userCard?.GenCeoSNILS;
                /* Гражданство */
                result.Citizenship = userCard?.GenCeoCitizenship;
                /* Дата рождения */
                result.BirthDate = userCard?.GenCeoDateOfBirth;
                /* Сведения о документе, удостоверяющем личность физического лица (Обязательный) */
                result.IdentityCard = GetIdentityCardInfo();
                return result;
            }

            /* Адрес доверителя в доверенности (АдрДовТип) - 4.23 */
            private AddressInfo GetAddressInfo()
            {
                var result = new AddressInfo();
                /* Адрес в русской транскрипции (Обязательный с условием) */
                result.AddressKyr = userCard?.GenEntAddrRussia;
                return result;
            }

            /* Сведения о документе, удостоверяющем личность физического лица (УдЛичнФЛТип) - 4.24 */
            private IdentityCardInfo GetIdentityCardInfo()
            {
                var result = new IdentityCardInfo();
                /* Код вида документа (Обязательный) */
                result.DocumentKindCode = ConvertDocumentKind(userCard.RepresentativeIndividualDocumentKind.Value);
                /* Серия и номер документа (Обязательный) */
                result.DocumentSerialNumber = userCard?.RepresentativeIndividualDocumentSeries;
                /* Дата выдачи документа (Обязательный) */
                result.IssueDate = userCard?.RepresentativeIndividualDocumentIssueDate;
                /* Наименование органа, выдавшего документ (Обязательный с условием) */
                result.Issuer = userCard?.GenAuthIssReprIDDoc;
                ///* Код подразделения органа, выдавшего документ */
                //result.IssuerCode = null;
                return result;
            }

            private DocumentKindCode ConvertDocumentKind(DocumentKindTypes representativeIndividualDocumentKind)
            {
                switch (representativeIndividualDocumentKind)
                {
                    case DocumentKindTypes.ForeignPassport:
                        return DocumentKindCode.ForeignPassport;
                    case DocumentKindTypes.Passport:
                        return DocumentKindCode.Passport;
                }

                throw new ArgumentOutOfRangeException(nameof(representativeIndividualDocumentKind));
            }

            /* Фамилия, имя, отчество (ФИОТип) - 4.25 */
            private FIO GetFIO(StaffEmployee employee)
            {
                if (employee == null)
                    throw new ArgumentNullException(nameof(employee));
                var result = new FIO();
                /* Фамилия (Обязательный) */
                result.LastName = employee.LastName;
                /* Имя (Обязательный) */
                result.FirstName = employee.FirstName;
                /* Отчество */
                result.MiddleName = employee?.MiddleName;
                return result;
            }
        }
    }
}
