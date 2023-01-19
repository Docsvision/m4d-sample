using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;

using System;
using System.Collections.Generic;
using System.Linq;

using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSData;
using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSDOVBBData;



namespace PowersOfAttorneyServerExtension.Helpers
{
    internal class PowerOfAttorneyFNSDOVBBDataProxy
    {
        public static readonly Guid PowerOfAttorneyTypeId = new Guid("1395D276-DE1F-46D3-B724-242D6889ECEB");
        
        private readonly UserCardPowerOfAttorney userCard;

        private const int PrincipalTypeRussianEntity = 2;
        private const int LossOfPowersUponSubstitutionLost = 1;


        private PowerOfAttorneyFNSDOVBBDataProxy(UserCardPowerOfAttorney userCard)
        {
            this.userCard = userCard;
        }

        public static PowerOfAttorneyFNSDOVBBData Create(UserCardPowerOfAttorney userCard)
        {
            return new PowerOfAttorneyFNSDOVBBDataProxy(userCard)
                .GetPowerOfAttorneyFNSDOVBBData();
        }

        private PowerOfAttorneyFNSDOVBBData GetPowerOfAttorneyFNSDOVBBData()
        {
            return new PowerOfAttorneyFNSDOVBBData(userCard.PowerOfAttorneyId,
                                                   CreatePowerOfAttorneyDocument());
        }

        private PowerOfAttorneyDocument CreatePowerOfAttorneyDocument()
        {
            return userCard.IsRetrusted()
                ? new PowerOfAttorneyDocument(CreateRetrustPowerOfAttorneyInfoData())
                : new PowerOfAttorneyDocument(GetPowerOfAttorneyDocumentData());
        }

        private PowerOfAttorneyDocumentData GetPowerOfAttorneyDocumentData()
        {
            return new PowerOfAttorneyDocumentData(GetPowerOfAttorneyInfo(),
                                                   GetPrincipalInfo(),
                                                   GetRepresentativeInfo(),
                                                   GetRepresentativePowersInfo());
        }

        private RetrustPowerOfAttorneyInfoData CreateRetrustPowerOfAttorneyInfoData()
        {
            return new RetrustPowerOfAttorneyInfoData(GetRetrustPowerOfAttorneyInfo(),
                                                      GetDelegatedAuthorityPrincipalInfo(),
                                                      GetRepresentativeInfo(),
                                                      GetRepresentativePowersInfo());
        }

        private PowerOfAttorneyInfo GetPowerOfAttorneyInfo()
        {
            var powerOfAttorneyId = userCard.PowerOfAttorneyId;
            var startDate = userCard.PowerOfAttorneyStartDate;
            var endDate = userCard.PowerOfAttorneyEndDate;
            var retrustType = (PowerOfAttorneyRetrustType)userCard.RetrustType;
            var jointRepresentationType = ConverToEnumWithFixIndex<JointRepresentationType>(userCard.JointRepresentation);
            var irrevocablePowerOfAttorneyInfo = GetIrrevocablePowerOfAttorneyInfo();
            // Для заполнения МЧД требуется название организации-владельца информационной системы - возьмём данные подписанта
            var organizationName = userCard.Signer.Unit.FullName;
            var lossOfAuthorityType = userCard.LossOfAuthorityType == LossOfPowersUponSubstitutionLost ? PowerOfAttorneyLossOfAuthorityType.Lost : PowerOfAttorneyLossOfAuthorityType.NotLost;

            return new PowerOfAttorneyInfo(powerOfAttorneyId, startDate, endDate, retrustType, jointRepresentationType, irrevocablePowerOfAttorneyInfo, organizationName, lossOfAuthorityType: lossOfAuthorityType);
        }


        private List<RepresentativePowerInfo> GetRepresentativePowersInfo()
        {
            return userCard.RepresentativePowers.Select(pow => new RepresentativePowerInfo(pow)).ToList();
        }

        private RepresentativeInfo GetRepresentativeInfo()
        {
            var individualInfo = GetIndividualPrincipalInfo();

            return new RepresentativeInfo(individualInfo);
        }

        private IndividualInfo2 GetIndividualPrincipalInfo()
        {
            var employee = userCard.RepresentativeIndividual;
            var inn = userCard.RepresentativeIndividualInn;
            var snils = userCard.RepresentativeIndividualSnils;
            var individualInfo = GetRepresentativeIndividualInfo(employee);
            var fio = GetFio(employee);

            return new IndividualInfo2(inn, snils, individualInfo, fio);
        }

        private FIO GetFio(StaffEmployee employee)
        {
            return new FIO(employee.LastName, employee.FirstName, employee.MiddleName);
        }

        private IndividualInfo GetRepresentativeIndividualInfo(StaffEmployee employee)
        {
            var birthDate = employee.BirthDate;
            var citizenshipType = ConverToEnumWithFixIndex<CitizenshipType>(userCard.RepresentativeCitizenshipType);
            var birthPlace = userCard.RepresentativeIndividualBirthPlace;
            var phone = employee.Phone;
            var gender = GetGender(employee);
            var residenceAddress = GetRepresentativeIndividualResidenceAddress();
            var identityCard = GetRepresentativeIdentityCard();

            return new IndividualInfo(birthDate, citizenshipType, birthPlace, phone, gender, residenceAddress: residenceAddress, identityCard: identityCard);
        }

        private IdentityCardOfIndividual GetRepresentativeIdentityCard()
        {
            var documentKind = (DocumentKindCode)userCard.RepresentativeIndividualDocumentKind;
            var documentSeriesNumber = userCard.RepresentativeIndividualDocumentSeries;
            var issueDate = userCard.RepresentativeIndividualDocumentIssueDate;
            var issuer = userCard.RepresentativeIndividualDocumentIssuer;
            var issuerCode = userCard.RepresentativeIndividualDocumentIssuerCode;

            return new IdentityCardOfIndividual(documentKind, documentSeriesNumber, issueDate, issuer, issuerCode);
        }

        private AddressInfo GetRepresentativeIndividualResidenceAddress()
        {
            return new AddressInfo(userCard.RepresentativeIndividualAddressSubjectRfCode, userCard.RepresentativeIndividualAddress);
        }

        private Gender? GetGender(StaffEmployee employee)
        {
            if (employee.Gender != StaffEmployeeGender.None)
                return null;

            return (Gender)employee.Gender;
        }

        private DelegatedAuthorityPrincipalInfo GetDelegatedAuthorityPrincipalInfo()
        {
            // Пример рассчитан только на доверителя-организацию 
            if (userCard.PrincipalType != PrincipalTypeRussianEntity)
            {
                throw new NotSupportedException();
            }

            return new DelegatedAuthorityPrincipalInfo(GetLegalEntityInfo(), GetFio(userCard.Signer));
        }

        private LegalEntityInfo GetLegalEntityInfo()
        {
            var princOrg = userCard.PrincipalOrganization;
            var name = princOrg.FullName;
            var inn = princOrg.INN;
            var kpp = princOrg.KPP;
            var ogrn = princOrg.OGRN;
            var constituentDocument = userCard.PrincipalOrganizationConstituentDocument;
            var phone = princOrg.Phone;
            var registrationAddress = GetLegalAddress(princOrg);
            var actualAddress = GetAddress(princOrg);

            return new LegalEntityInfo(name, inn, kpp, ogrn, constituentDocument, phone, registrationAddress: registrationAddress, actualAddress: actualAddress);
        }

        private AddressInfo GetLegalAddress(StaffUnit staffUnit)
        {
            var address = staffUnit.Addresses.FirstOrDefault(t => t.AddressType == StaffAddresseAddressType.LegalAddress);
            if (address == null)
                return null;

            return new AddressInfo(userCard.RepresentativeOrganizationLegalAddressSubjectRfCode, address.Address);
        }

        private AddressInfo GetAddress(StaffUnit staffUnit)
        {
            var address = staffUnit.Addresses.FirstOrDefault(t => t.AddressType == StaffAddresseAddressType.ContactAddress);
            if (address == null)
                return null;

            return new AddressInfo(userCard.RepresentativeOrganizationAddressSubjectRfCode, address.Address);
        }

        private RetrustPowerOfAttorneyInfo GetRetrustPowerOfAttorneyInfo()
        {
            var parentPowerOfAttorneySerializedData = userCard.ParentalPowerOfAttorney.MainInfo.MachineReadablePowerOfAttorney;
            var powerOfAttorneyInfo = GetPowerOfAttorneyInfo();
            
            return new RetrustPowerOfAttorneyInfo(parentPowerOfAttorneySerializedData, powerOfAttorneyInfo);
        }

        private IrrevocablePowerOfAttorneyInfo GetIrrevocablePowerOfAttorneyInfo()
        {
            RevocationPossibleType revocationPossibleType = ConverToEnumWithFixIndex<RevocationPossibleType>(userCard.RevocationPossibleType);
            RetrustRevocationPossibleType? retrustRevocationPossibleType = ConverToEnumWithFixIndex<RetrustRevocationPossibleType>(userCard.RetrustRevocationPossibleType);
            RevocationCondition? revocationCondition = ConverToEnumWithFixIndex<RevocationCondition>(userCard.RevocationCondition);
            string revocationConditionDescription = userCard.RevocationConditionDescription;

            return new IrrevocablePowerOfAttorneyInfo(revocationPossibleType, retrustRevocationPossibleType, revocationCondition, revocationConditionDescription);
        }

        private PrincipalInfo GetPrincipalInfo()
        {
            // Пример рассчитан только на доверителя-организацию 
            if (userCard.PrincipalType != PrincipalTypeRussianEntity)
            {
                throw new NotSupportedException();
            }

            return new PrincipalInfo(GetRussianLegalEntityPrincipalInfo(), GetFio(userCard.Signer));
        }

        private RussianLegalEntityPrincipalInfo GetRussianLegalEntityPrincipalInfo()
        {
            return new RussianLegalEntityPrincipalInfo(GetRussianEntityInfo(), GetPrincipalWithoutPowerOfAttorneyInfo());
        }

        private PrincipalWithoutPowerOfAttorneyInfo GetPrincipalWithoutPowerOfAttorneyInfo()
        {
            return new PrincipalWithoutPowerOfAttorneyInfo(GetPrincipalWithoutPowerOfAttorneyIndividualInfo(), GetPrincipalWithoutPowerOfAttorneyLegalEntityInfo());
        }

        private IndividualInfo0 GetPrincipalWithoutPowerOfAttorneyIndividualInfo()
        {
            var emlpoyee = userCard.PrincipalWithoutPowerOfAttorneyIndividual;
            var snils = userCard.PrincipalWithoutPowerOfAttorneyIndividualSnils;
            var individualInfo = GetPrincipalWithoutPowerOfAttorneyIndividual1Info(emlpoyee);
            var position = emlpoyee.PositionName;
            var authorityConfirmingDocumentName = userCard.PrincipalWithoutPowerOfAttorneyIndividualDocument;
            var inn = userCard.PrincipalWithoutPowerOfAttorneyIndividualInn;
            var authorityConfirmingDocument = GetConfirmationOfAuthorityDocument();


            return new IndividualInfo0(snils, individualInfo, position, authorityConfirmingDocumentName, inn, authorityConfirmingDocument);
        }

        private ConfirmationOfAuthorityDocument GetConfirmationOfAuthorityDocument()
        {
            return new ConfirmationOfAuthorityDocument(userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentIssueDate, userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentIssuer, userCard.PrincipalWithoutPowerOfAttorneyIndividualDocument);
        }

        private LegalEntityInfo GetPrincipalWithoutPowerOfAttorneyLegalEntityInfo()
        {
            var princOrg = userCard.PrincipalWithoutPowerOfAttorneyOrganization;
           
            var name = princOrg.FullName;
            var inn = princOrg.INN;
            var kpp = princOrg.KPP;
            var ogrn = princOrg.OGRN;
            var constituentDocument = userCard.PrincipalWithoutPowerOfAttorneyOrganizationConstituentDocument;
            var phone = princOrg.Phone;

            var legalAddress = new AddressInfo(userCard.PrincipalWithoutPowerOfAttorneyOrganizationLegalAddressSubjectRfCode, userCard.PrincipalWithoutPowerOfAttorneyOrganizationLegalAddress);

            var actualAddress = new AddressInfo(userCard.PrincipalWithoutPowerOfAttorneyOrganizationAddressSubjectRfCode, userCard.PrincipalWithoutPowerOfAttorneyOrganizationAddress);

            return new LegalEntityInfo(name, inn, kpp, ogrn, constituentDocument, phone, registrationAddress: legalAddress, actualAddress: actualAddress);
        }

        private IndividualInfo GetPrincipalWithoutPowerOfAttorneyIndividual1Info(StaffEmployee emlpoyee)
        {
            var birthDate = emlpoyee.BirthDate;
            var citizenshipType = ConverToEnumWithFixIndex<CitizenshipType>(userCard.PrincipalWithoutPowerOfAttorneyIndividualCitizenship);
            var birthPlace = userCard.PrincipalWithoutPowerOfAttorneyIndividualBirthPlace;
            var phone = emlpoyee.Phone;
            var gender = GetGender(emlpoyee);
            var residenceAddress = new AddressInfo(userCard.PrincipalWithoutPowerOfAttorneyIndividualAddressSubjectRfCode, userCard.PrincipalWithoutPowerOfAttorneyIndividualAddress);
            var identityCard = GetIdentityCard();

            return new IndividualInfo(birthDate, citizenshipType, birthPlace, phone, gender, residenceAddress: residenceAddress, identityCard: identityCard);
        }

        private IdentityCardOfIndividual GetIdentityCard()
        {
            var kind = (DocumentKindCode)userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentKindCode;
            var seriesNumber = userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentSeries;
            var issueDate = userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentIssueDate;
            var issuer = userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentIssuer;
            var issuerCode = userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentIssuerCode;
            
            return new IdentityCardOfIndividual(kind, seriesNumber, issueDate, issuer, issuerCode);
        }

        private RussianEntityInfo GetRussianEntityInfo()
        {
            var princOrg = userCard.PrincipalOrganization;
            var name = princOrg.FullName;
            var inn = princOrg.INN;
            var kpp = princOrg.KPP;
            var ogrn = princOrg.OGRN;
            var legalAddress = princOrg.Addresses.FirstOrDefault(t => t.AddressType == StaffAddresseAddressType.LegalAddress)?.Address;
            var actualAddress = princOrg.Addresses.FirstOrDefault(t => t.AddressType == StaffAddresseAddressType.ContactAddress)?.Address;

            return new RussianEntityInfo(name, ogrn, inn, kpp, legalAddress, actualAddress);
        }

        private static TEnum ConverToEnumWithFixIndex<TEnum>(int enumIndex) where TEnum : struct
        {
            // В ПКД нумерация с 0 - в СКД с 1
            return (TEnum)(object)(enumIndex + 1);
        }

        private static TEnum? ConverToEnumWithFixIndex<TEnum>(int? enumIndex) where TEnum : struct
        {
            if (enumIndex == null)
                return null;

            return ConverToEnumWithFixIndex<TEnum>(enumIndex.Value);
        }
    }
}