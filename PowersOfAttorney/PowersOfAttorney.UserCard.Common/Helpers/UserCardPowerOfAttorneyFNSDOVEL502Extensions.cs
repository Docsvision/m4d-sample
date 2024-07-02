using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DocsVision.BackOffice.ObjectModel;
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

        public static PowerOfAttorneyRevocationData ConvertToPowerOfAttorneyFNSDOVEL502RevocationData(this UserCardPowerOfAttorney userCard, ObjectContext context, string revocationReason)
        {
            return RevocationFNSDOVEL502Converter.Convert(userCard, context, revocationReason);
        }
        
        private class Converter
        {
            private readonly UserCardPowerOfAttorney userCard;
            private readonly ObjectContext objectContext;
            private readonly bool isRetrusted;
            private readonly GenPrincipalTypes? principalType;
            private readonly RepresentativeType representativeType;

            private PowerOfAttorneyFNSDOVEL502Data poaData;

            public static PowerOfAttorneyFNSDOVEL502Data Convert(UserCardPowerOfAttorney userCard, ObjectContext objectContext)
            {
                return new Converter(userCard, objectContext).Convert();
            }

            private Converter(UserCardPowerOfAttorney userCard, ObjectContext objectContext)
            {
                this.userCard = userCard ?? throw new ArgumentNullException(nameof(Converter.userCard));
                this.objectContext = objectContext ?? throw new ArgumentNullException(nameof(objectContext));
                isRetrusted = this.userCard.IsRetrusted();
                principalType = this.userCard.GenPrincipalType;
                representativeType = this.userCard.GenRepresentativeType502.Value;
            }

            private PowerOfAttorneyFNSDOVEL502Data Convert()
            {
                poaData = new PowerOfAttorneyFNSDOVEL502Data();

                poaData.RecipientID = userCard.GenTaxAuthPOASubmit;
                poaData.FinalRecipientID = userCard.GenFinalRecipientTaxID;
                poaData.SenderID = GetSenderID();
                poaData.Document = CreatePowerOfAttorteyDocument();

                return poaData;
            }

            #region Первоначальная доверенность

            /* Состав и структура документа (Документ) */
            private PowerOfAttorneyDocument CreatePowerOfAttorteyDocument()
            {
                var document = new PowerOfAttorneyDocument();
                /* Единый регистрационный номер доверенности (Обязательный) */
                document.PowerOfAttorneyNumber = userCard.InstanceId;
                /* Код налогового органа, в который представляется доверенность, созданная в электронной
                 * форме (Обязательный) 
                 */
                document.TaxAuthorityCode = userCard.GenTaxAuthPOASubmit;
                /* Сведения о физическом лице, подписывающем доверенность от имени доверителя (российской
                 * организации / иностранной организации) без доверенности или подписывающем доверенность
                 * от своего имени, в том числе при передаче полномочий другому лицу в порядке передоверия
                 * (Обязательный)
                 */
                document.Signer = GetFIO(userCard.Signer.GetValueOrThrow(Resources.Error_EmptySigner));
                /* Доверенность (Обязательный с условием) 
                 * Обязательный при создании первоначальной доверенности
                 */
                if (!isRetrusted)
                    document.PowerOfAttorneyData = CreatePowerOfAttorneyDocumentData();
                else
                    document.RetrustPowerOfAttorneyData = CreateRetrustPowerOfAttorneyDocumentData();

                return document;
            }

            /* Доверенность (Довер) - 4.3 */
            private PowerOfAttorneyDocumentData CreatePowerOfAttorneyDocumentData()
            {
                var document = new PowerOfAttorneyDocumentData();
                /* Сведения доверенности (Обязательный) */
                document.PowerOfAttorney = GetPowerOfAttorneyInfo();
                /* Сведения о доверителе (Обязательный) */
                document.Principal = CreatePrincipalInfo();
                /* Сведения об уполномоченном представителе (Обязательный) */
                document.Representative = GetRepresentativeInfo();
                /* Признак (код) области полномочий уполномоченного представителя (Обязательный) */
                document.RepresentativePowers = GetRepresentativePowers();
                return document;
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
                var paoInfo = new PowerOfAttorneyInfo
                {
                    /* Номер доверенности */
                    InternalPowerOfAttorneyNumber = userCard?.GenInternalPOANumber,
                    /* Дата совершения (выдачи) доверенности (Обязательный) */
                    PowerOfAttorneyStartDate = userCard.GenPoaDateOfIssue ?? throw new ApplicationException(Resources.Error_PoaDateOfIssueIsEmpty),
                    /* Дата окончания срока действия доверенности (Обязательный) */
                    PowerOfAttorneyEndDate = userCard.GenPoaExpirationDate ?? throw new ApplicationException(Resources.Error_EmptyPowerOfAttorneyEndDate),
                    /* Признак возможности оформления передоверия (Обязательный) */
                    RetrustType = Convert(userCard.GenPossibilityOfSubstitution502 ?? throw new ApplicationException(Resources.Error_PossibilityOfSubstitutionIsEmpty)),
                };
                if (!string.IsNullOrEmpty(userCard.GenTaxAuthPOAValid))
                {
                    /* Код налогового органа, в отношении которого действует доверенность */
                    paoInfo.TaxCodes = new List<string> { userCard.GenTaxAuthPOAValid };
                }
                return paoInfo;
            }


            /* Сведения о доверителе (СвДоверит) - 4.5 */
            private PrincipalInfo CreatePrincipalInfo()
            {
                // Поддерживаем только юр. лица
                if (principalType == GenPrincipalTypes.entity)
                {
                    return new PrincipalInfo
                    {
                        RussianCompany = GetRussianCompanyInfo()
                    };
                }

                throw new ArgumentOutOfRangeException(nameof(userCard.GenPrincipalType));
            }

            /* Сведения о российской организации (НПЮЛ) - 4.6 */
            private RussianCompanyInfo GetRussianCompanyInfo()
            {
                var russianCompany = new RussianCompanyInfo
                {
                    /* Наименование организации (Обязательный) */
                    Name = userCard.PrincipalOrganization.GetValueOrThrow(Resources.Error_EmptyPrincipalOrganization).Name,
                    /* ИНН организации (Обязательный) */
                    Inn = userCard?.GenEntityPrinINN,
                    /* КПП организации (Обязательный) */
                    Kpp = userCard?.GenEntityPrinKPP,
                    /* ОГРН (Обязательный) */
                    Ogrn = userCard?.GenEntPrinOGRN,
                    /* Адрес юридического лица в Российской Федерации (Обязательный) */
                    LegalAddress = GetAddressInfo(),
                    /* Сведения о лице, действующем от имени юридического лица без доверенности (Обязательный) */
                    PrincipalWithoutPowerOfAttorney = CreatePrincipalWithoutPowerOfAttorney()
                };
                return russianCompany;
            }

            /* Сведения о лице, действующем от имени юридического лица без доверенности (ЛицоБезДов) */
            private PrincipalWithoutPowerOfAttorneyInfo CreatePrincipalWithoutPowerOfAttorney()
            {
                var executiveType = userCard.GenExecutiveBodyType.Value;
                var result = new PrincipalWithoutPowerOfAttorneyInfo();
                switch (executiveType)
                {
                    case ExecutiveBodyType.entity:
                        /* Сведения об организации (Обязательный с условием) */
                        result.Organization = CreatePrincipalOrganizationInfo();
                        result.Individual = GetIndividualInfo();
                        break;
                    case ExecutiveBodyType.individual:
                        /* Сведения о физическом лице, в том числе индивидуальном предпринимателе (Обязательный) */
                        result.Individual = GetIndividualInfo();
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(userCard.GenExecutiveBodyType));
                }

                return result;                            
            }

            /* Сведения по физическому лицу (СвФЛ) - 4.8 */
            private IndividualInfo GetIndividualInfo()
            {
                return new IndividualInfo
                {
                    /* ИНН физического лица */
                    Inn = userCard?.GenCeoIIN,
                    /* СНИЛС (Обязательный) */
                    Snils = userCard?.GenCeoSNILS,
                    /* Гражданство */
                    Citizenship = userCard?.GenCeoCitizenship,
                    /* Дата рождения */
                    BirthDate = userCard?.GenCeoDateOfBirth,
                    /* Должность */
                    Position = userCard?.GenCeoPosition
                };
            }

            /* Сведения об организации (СвОргТип) - 4.18 */
            private OrganizationInfo CreatePrincipalOrganizationInfo()
            {
                return new OrganizationInfo
                {
                    /* Наименование организации / Наименование обособленного подразделения организации (Обязательный) */
                    Name = userCard?.EntityWithoutPOA.GetValueOrThrow(Resources.Error_EmptyEnitityWithoutPOAOrganization).Name,
                    /* ИНН организации (Обязательный) */
                    Inn = userCard?.INNEntityWithoutPOA,
                    /* КПП организации (Обязательный) */
                    Kpp = userCard?.KPPEntityWithoutPOA
                };
            }

            /* Сведения об организации (СвОргТип) - 4.18 */
            private OrganizationInfo CreateRepresentativeOrganizationInfo()
            {
                return new OrganizationInfo
                {
                    /* Наименование организации / Наименование обособленного подразделения организации (Обязательный) */
                    Name = userCard?.EntityRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeOrganization).Name,
                    /* ИНН организации (Обязательный) */
                    Inn = userCard?.INNEntityRepresentative,
                    /* КПП организации (Обязательный) */
                    Kpp = userCard?.KPPEntityRepresentative
                };
            }

            /* Сведения о российском юридическом лице (СведЮЛТип) - 4.19 */
            private RussianCompanyInfo1 GetRussianCompanyInfo1(UserCardPowerOfAttorney originaUserCardPowerOfAttorney)
            {
                var entityPrincipal = originaUserCardPowerOfAttorney.GenEntityPrincipal.GetValueOrThrow(Resources.Error_EmptyPrincipalOrganization);

                return new RussianCompanyInfo1
                {
                    Name = entityPrincipal.Name,
                    Inn = originaUserCardPowerOfAttorney.GenEntityPrinINN,
                    Kpp = originaUserCardPowerOfAttorney.GenEntityPrinKPP,
                    Ogrn = originaUserCardPowerOfAttorney.GenEntPrinOGRN
                };
            }

            /* Сведения об уполномоченном представителе (уполномоченных представителях) (СвПредТип) - 4.20 */
            private RepresentativeInfo GetRepresentativeInfo(bool isRetrust = false)
            {
                var result = new RepresentativeInfo();
                switch (representativeType)
                {
                    case RepresentativeType.entity:
                        /* Сведения об организации (Обязательный с условием) */
                        result.Organization = CreateRepresentativeOrganizationInfo();
                        result.Individual = GetIndividualInfo2(!isRetrust || userCard.GenExecutiveBodyType.HasValue && userCard.GenExecutiveBodyType.Value == ExecutiveBodyType.entity);
                        break;
                    case RepresentativeType.individual:
                        /* Сведения о физическом лице, в том числе индивидуальном предпринимателе (Обязательный) */
                        result.Individual = GetIndividualInfo2(false);
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(userCard.GenRepresentativeType));
                }

                return result;
            }


            private string GetSenderID()
            {
                //     Значение: для организаций – девятнадцатиразрядный код (ИНН и КПП организации)
                //     для физических лиц – двенадцатиразрядный код (ИНН физического лица, при наличии.
                //     При отсутствии ИНН – последовательность из двенадцати нулей)    
                //     НО!!! В примере разрешены только юр. лица
                if (principalType.Value == GenPrincipalTypes.entity)
                {
                    if (isRetrusted)
                    {
                        var originalPOAUserCard = GetOriginalPowerOfAttorneyUserCard();
                        return $"{originalPOAUserCard.GenEntityPrinINN}{originalPOAUserCard.GenEntityPrinKPP}";
                    }
                    else
                    {
                        return $"{userCard.GenEntityPrinINN}{userCard.GenEntityPrinKPP}";
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(principalType));
            }

            private UserCardPowerOfAttorney GetOriginalPowerOfAttorneyUserCard()
            {
                return GetUserCardPowerOfAttorney(userCard.GenOriginalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_UnableToGetOriginalPowerOfAttorneyUserCard));
            }

            private UserCardPowerOfAttorney GetUserCardPowerOfAttorney(Document document)
            {
                if (document is null)
                    throw new ArgumentNullException(nameof(document));
                return new UserCardPowerOfAttorney(document, objectContext);
            }


            /* Сведения о физическом лице, в том числе индивидуальном предпринимателе (СвФизЛицТип) - 4.21 */
            private IndividualInfo2 GetIndividualInfo2(bool isEnity)
            {
                var info = new IndividualInfo2();
                /* ИНН физического лица */
                info.Inn = userCard?.GenRepresentativeINN;
                /* СНИЛС (Обязательный) */
                info.Snils = userCard?.GenRepresentativeSNILS;
                /* Фамилия, имя, отчество физического лица (Обязательный) */
                if (isEnity)
                {
                    var representative = userCard.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual);
                    info.Fio = new FIO
                    {
                        LastName = representative.LastName,
                        MiddleName = representative?.MiddleName,
                        FirstName = representative.FirstName
                    };
                }
                else
                {
                    info.Fio = new FIO
                    {
                        LastName = userCard.GenReprLastName,
                        FirstName = userCard.GenReprName,
                        MiddleName = userCard?.GenReprMiddleName
                    };
                }

                /* Сведения о документе, удостоверяющем личность физического лица (Обязательный) */
                info.IdentityCard = CreateRepresentativeIdentityCardInfo();
                return info;
            }

            /* Сведения по физическому лицу (СведФЛТип) - 4.22 */
            private IndividualInfo3 CreateIndividualInfo3()
            {
                return new IndividualInfo3
                {
                    /* ИНН физического лица */
                    Inn = userCard?.GenCeoIIN,
                    /* СНИЛС (Обязательный) */
                    Snils = userCard?.GenCeoSNILS,                    
                    /* Дата рождения */
                    BirthDate = userCard?.GenCeoDateOfBirth,
                    /* Сведения о документе, удостоверяющем личность физического лица (Обязательный) */
                    IdentityCard = CreatePrincipalIdentityCardInfo()
                };
            }

            /* Адрес доверителя в доверенности (АдрДовТип) - 4.23 */
            private AddressInfo GetAddressInfo()
            {
                return new AddressInfo
                {
                    /* Адрес в русской транскрипции (Обязательный с условием) */
                    AddressKyr = userCard.GenEntAddrRussia
                };
            }

            /* Сведения о документе, удостоверяющем личность физического лица (УдЛичнФЛТип) - 4.24 */
            private IdentityCardInfo CreateRepresentativeIdentityCardInfo()
            {
                var documentKindCode = userCard.RepresentativeIndividualDocumentKind;
                string authIssuer;
                if (documentKindCode == null)
                    return null;
                if (documentKindCode == DocumentKindTypes.Passport)
                {
                    authIssuer = userCard?.GenAuthIssCEOIDDoc ?? throw new ArgumentException(Resources.Error_AuthIssIDDoc);
                }
                else
                {
                    authIssuer = userCard?.GenAuthIssCEOIDDoc;
                }                
                return new IdentityCardInfo
                {
                    /* Код вида документа (Обязательный) */
                    DocumentKindCode = ConvertDocumentKind(userCard.RepresentativeIndividualDocumentKind.Value),
                    /* Серия и номер документа (Обязательный) */
                    DocumentSerialNumber = userCard?.RepresentativeIndividualDocumentSeries,
                    /* Дата выдачи документа (Обязательный) */
                    IssueDate = userCard?.RepresentativeIndividualDocumentIssueDate,
                    /* Наименование органа, выдавшего документ (Обязательный с условием) */
                    Issuer = authIssuer
                };
            }

            /* Сведения о документе, удостоверяющем личность физического лица (УдЛичнФЛТип) - 4.24 */
            private IdentityCardInfo CreatePrincipalIdentityCardInfo()
            {
                var documentKindCode = userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentKindCode;
                string authIssuer;
                if (documentKindCode == null)
                    return null;
                if (documentKindCode == DocumentKindTypes.Passport)
                {
                    authIssuer = userCard?.GenAuthIssCEOIDDoc ?? throw new ArgumentException(Resources.Error_AuthIssIDDoc);
                } else
                {
                    authIssuer = userCard?.GenAuthIssCEOIDDoc;
                }
                return new IdentityCardInfo
                {
                    /* Код вида документа (Обязательный) */
                    DocumentKindCode = ConvertDocumentKind(documentKindCode.Value),
                    /* Серия и номер документа (Обязательный) */
                    DocumentSerialNumber = userCard?.GenSerNumCEOIDDoc,
                    /* Дата выдачи документа (Обязательный) */
                    IssueDate = userCard?.GenDateIssCEOIDDoc,
                    /* Наименование органа, выдавшего документ (Обязательный с условием) */
                    Issuer = authIssuer
                };
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
                return new FIO
                {
                    /* Фамилия (Обязательный) */
                    LastName = employee.LastName,
                    /* Имя (Обязательный) */
                    FirstName = employee.FirstName,
                    /* Отчество */
                    MiddleName = employee?.MiddleName
                };
            }

            #endregion


            #region Передоверие
            /* Передоверие (Передов) - 4.11 */
            private RetrustPowerOfAttorneyDocumentData CreateRetrustPowerOfAttorneyDocumentData()
            {
                var result = new RetrustPowerOfAttorneyDocumentData();
                /* Сведения доверенности, совершённой (выданной) в рамках передоверия(Обязательный) */
                result.PowerOfAttorney = CreateRetrustPowerOfAttorneyInfo();
                /* Сведения о доверителе в порядке передоверия(Обязательный) */
                result.Principal = CreateRetrustPrincipalInfo();
                /* Сведения об уполномоченном представителе(Обязательный) */
                result.Representative = GetRepresentativeInfo(true);
                /* Признак(код) области полномочий уполномоченного представителя(Обязательный) */
                result.RepresentativePowers = GetRepresentativePowers();
                return result;
            }



            /* Сведения о доверителе в порядке передоверия (СвДоверщ) - 4.17 */
            private RetrustPrincipalInfo CreateRetrustPrincipalInfo()
            {
                return new RetrustPrincipalInfo
                {
                    Organization = CreatePrincipalOrganizationInfo(),
                    Individual = CreateIndividualInfo3()
                };
            }

            /* Сведения о доверителе из доверенности, на основании которой осуществляется передоверие */
            private ParentPrincipalInfo CreateParentPrincipalInfo(UserCardPowerOfAttorney parentUserCardPowerOfAttorney)
            {
                var russianCompany = new OrganizationInfo
                {
                    Name = parentUserCardPowerOfAttorney.EntityWithoutPOA.GetValueOrThrow(Resources.Error_EmptyEnitityWithoutPOAOrganization).Name,
                    Inn = parentUserCardPowerOfAttorney.INNEntityWithoutPOA,
                    Kpp = parentUserCardPowerOfAttorney.KPPEntityWithoutPOA
                };
                //Заполняем только организацию, физ.лица в примере не рассматриваем
                return new ParentPrincipalInfo
                {
                    RussianCompany = russianCompany
                };
            }

            /* Сведения доверенности, совершённой (выданной) в рамках передоверия (СвДовП) - 4.12 */
            private RetrustPowerOfAttorneyInfo CreateRetrustPowerOfAttorneyInfo()
            {
                var originalPOA = userCard.GenOriginalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_UnableToGetOriginalPowerOfAttorneyUserCard);
                var parentPOA = userCard.GenParentalPowerOfAttorneyUserCard;

                var retrustPOAInfo = new RetrustPowerOfAttorneyInfo();
                // Если доверенность в рамках передоверия второго уровня
                if (parentPOA.HasValue)
                    /* Регистрационный номер доверенности, в отношении которой производится передоверие (Обязательный) */
                    retrustPOAInfo.ParentPowerOfAttorneyNumber = parentPOA.HasValue ? parentPOA.Value.GetObjectId() : originalPOA.GetObjectId();
                /* Номер доверенности */
                retrustPOAInfo.InternalPowerOfAttorneyNumber = userCard.GenInternalPOANumber;
                /* Дата совершения (выдачи) доверенности (Обязательный) */
                retrustPOAInfo.PowerOfAttorneyStartDate = userCard.GenPoaDateOfIssue;
                /* Дата окончания срока действия доверенности (Обязательный) */
                retrustPOAInfo.PowerOfAttorneyEndDate = userCard.GenPoaExpirationDate;
                /* Признак возможности оформления передоверия (Обязательный) */
                retrustPOAInfo.RetrustType = Convert(userCard.GenPossibilityOfSubstitution502 ?? throw new ApplicationException(Resources.Error_PossibilityOfSubstitutionIsEmpty));
                /* Сведения об основной доверенности (Обязательный) */
                retrustPOAInfo.PrimaryPowerOfAttorney = CreatePrimaryPowerOfAttorneyInfo(originalPOA);
                if (parentPOA.HasValue)
                {
                    /* Сведения о доверителе из доверенности, на основании которой осуществляется передоверие */
                    retrustPOAInfo.ParentPrincipal = CreateParentPrincipalInfo(GetParentalPowerOfAttorneyUserCard());
                }

                if (!string.IsNullOrEmpty(userCard.GenTaxAuthPOAValid))
                {
                    /* Код налогового органа, в отношении которого действует доверенность */
                    retrustPOAInfo.TaxCodes = new List<string> { userCard.GenTaxAuthPOAValid };
                }
                return retrustPOAInfo;
            }
           

            /* Сведения об основной доверенности (СвОснДов) - 4.13 */
            private PrimaryPowerOfAttorneyInfo CreatePrimaryPowerOfAttorneyInfo(Document document)
            {
                var primaryPOAInfo = new PrimaryPowerOfAttorneyInfo();
                /* Единый регистрационный номер основной доверенности (Обязательный) */
                primaryPOAInfo.PowerOfAttorneyNumber = document.GetObjectId();
                /* Сведения о доверителе основной доверенности (Обязательный) */
                primaryPOAInfo.Principal = CreatePrimaryPrincipalInfo(GetOriginalPowerOfAttorneyUserCard());
                return primaryPOAInfo;
            }

            /* Сведения об основной доверенности (СвОснДов) - 4.13 */
            private PrimaryPrincipalInfo CreatePrimaryPrincipalInfo(UserCardPowerOfAttorney originaUserCardPowerOfAttorney)
            {
                // в данном примере заполняем только юр. лицо (RussianCompany) - ? надо ли проверять originaUserCardPowerOfAttorney.GenPrincipalType == entity
                return new PrimaryPrincipalInfo
                {
                    /* Сведения о доверителе – российском юридическом лице (Обязательный с условием) */
                    RussianCompany = GetRussianCompanyInfo1(originaUserCardPowerOfAttorney)
                };
            }

            private RetrustType Convert(GenPossibilityOfSubstitution502Type substitution)
            {
                switch (substitution)
                {
                    case GenPossibilityOfSubstitution502Type.withoutRightOfSubstitution:
                        return RetrustType.None;
                    case GenPossibilityOfSubstitution502Type.substitutionIsPossible:
                        return RetrustType.Followed;
                }
                throw new ArgumentOutOfRangeException(nameof(substitution));
            }

            private UserCardPowerOfAttorney GetParentalPowerOfAttorneyUserCard()
            {
                return GetUserCardPowerOfAttorney(userCard.GenParentalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_UnableToGetParentalPowerOfAttorneyUserCard));
            }

            #endregion
        }
    }
}
