using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.WebClient.Diagnostics;

using PowersOfAttorneyServerExtension;
using PowersOfAttorneyServerExtension.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;

using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSData;

using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSDOVBBData;
using static PowersOfAttorneyServerExtension.Helpers.UserCardPowerOfAttorney;

internal static class UserCardPowerOfAttorneyFNSDOVBBExtensions
{
    public static PowerOfAttorneyData ConvertToPowerOfAttorneyFNSDOVBBData(this UserCardPowerOfAttorney userCard, IPowerOfAttorneyService powerOfAttorneyService)
    {
        try
        {
            return Converter.Convert(userCard, powerOfAttorneyService);
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex);
            throw new Exception(Resources.Error_IncorrectUserCardData);
        }
    }

    class Converter
    {
        private readonly UserCardPowerOfAttorney userCard;
        private readonly IPowerOfAttorneyService powerOfAttorneyService;
        private const int LossOfPowersUponSubstitutionLost = 1;


        private Converter(UserCardPowerOfAttorney userCard, IPowerOfAttorneyService powerOfAttorneyService)
        {
            this.userCard = userCard ?? throw new ArgumentNullException(nameof(userCard));
            this.powerOfAttorneyService = powerOfAttorneyService;
        }

        public static PowerOfAttorneyFNSDOVBBData Convert(UserCardPowerOfAttorney userCard, IPowerOfAttorneyService powerOfAttorneyService)
        {
            return new Converter(userCard, powerOfAttorneyService).Convert();
        }

        private PowerOfAttorneyFNSDOVBBData Convert()
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
            var retrustType = Convert(userCard.RetrustType);
            var irrevocablePowerOfAttorneyInfo = GetIrrevocablePowerOfAttorneyInfo();
            // Для заполнения МЧД требуется название организации-владельца информационной системы - возьмём данные подписанта
            var organizationName = userCard.Signer.Unit.Name;
            PowerOfAttorneyLossOfAuthorityType? lossOfAuthorityType = GetPowerOfAttorneyLossOfAuthorityType(userCard.LossOfAuthorityType);

            return new PowerOfAttorneyInfo(powerOfAttorneyId, startDate, endDate, retrustType, userCard.JointRepresentation, irrevocablePowerOfAttorneyInfo, organizationName, lossOfAuthorityType: lossOfAuthorityType);
        }

        private PowerOfAttorneyRetrustType Convert(GenPossibilityOfSubstitutionTypes002 retrustType)
        {
            switch (retrustType)
            {
                case GenPossibilityOfSubstitutionTypes002.withoutSubstitution:
                    return PowerOfAttorneyRetrustType.None;
                case GenPossibilityOfSubstitutionTypes002.onetimeSubstitution:
                    return PowerOfAttorneyRetrustType.OneTime;
                case GenPossibilityOfSubstitutionTypes002.subsequentSubstitution:
                    return PowerOfAttorneyRetrustType.Followed;
            }

            throw new ArgumentOutOfRangeException(nameof(retrustType));
        }

        private PowerOfAttorneyLossOfAuthorityType? GetPowerOfAttorneyLossOfAuthorityType(int? lossOfAuthorityType)
        {
            if (lossOfAuthorityType == null)
                return null;

            return userCard.LossOfAuthorityType == LossOfPowersUponSubstitutionLost
                ? PowerOfAttorneyLossOfAuthorityType.Lost
                : PowerOfAttorneyLossOfAuthorityType.NotLost;
        }

        private List<RepresentativePowerInfo> GetRepresentativePowersInfo()
        {
            var codes = userCard.RepresentativePowersCodes.Select(pow => new RepresentativePowerInfo(pow));
            if (codes.Any())
                return codes.ToList();

            return userCard.RepresentativePowersText.Select(pow => new RepresentativePowerInfo(pow)).ToList();
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
            var birthPlace = userCard.RepresentativeIndividualBirthPlace;
            var phone = employee.Phone;
            var gender = GetGender(employee);
            var residenceAddress = GetAddressInfo(userCard.RepresentativeIndividualAddressSubjectRfCode, userCard.RepresentativeIndividualAddress);
            var identityCard = GetRepresentativeIdentityCard();

            return new IndividualInfo(birthDate, userCard.RepresentativeCitizenshipType, birthPlace, phone, gender, residenceAddress: residenceAddress, identityCard: identityCard);
        }

        private IdentityCardOfIndividual GetRepresentativeIdentityCard()
        {
            if (userCard.RepresentativeIndividualDocumentKind == null)
                return null;

            var documentKind = Convert(userCard.RepresentativeIndividualDocumentKind.Value);
            var documentSeriesNumber = userCard.RepresentativeIndividualDocumentSeries;
            var issueDate = userCard.RepresentativeIndividualDocumentIssueDate;
            var issuer = userCard.RepresentativeIndividualDocumentIssuer;
            var issuerCode = userCard.RepresentativeIndividualDocumentIssuerCode;

            return new IdentityCardOfIndividual(documentKind, documentSeriesNumber, issueDate, issuer, issuerCode);
        }

        private DocumentKindCode Convert(DocumentKindTypes representativeIndividualDocumentKind)
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

        private AddressInfo GetAddressInfo(string rfCode, string address)
        {
            if (string.IsNullOrEmpty(rfCode) && string.IsNullOrEmpty(address))
                return null;

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
            if (userCard.PrincipalType != UserCardPowerOfAttorney.GenPrincipalTypes.entity)
            {
                throw new NotSupportedException();
            }

            return new DelegatedAuthorityPrincipalInfo(GetLegalEntityInfo(), GetFio(userCard.Signer));
        }

        private LegalEntityInfo GetLegalEntityInfo()
        {
            var princOrg = userCard.PrincipalOrganization;
            var name = princOrg.Name;
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

            return GetAddressInfo(userCard.RepresentativeOrganizationLegalAddressSubjectRfCode, address.Address);
        }

        private AddressInfo GetAddress(StaffUnit staffUnit)
        {
            var address = staffUnit.Addresses.FirstOrDefault(t => t.AddressType == StaffAddresseAddressType.ContactAddress);
            if (address == null)
                return null;

            return GetAddressInfo(userCard.RepresentativeOrganizationAddressSubjectRfCode, address.Address);
        }

        private RetrustPowerOfAttorneyInfo GetRetrustPowerOfAttorneyInfo()
        {
            var powerOfAttorneyInfo = GetPowerOfAttorneyInfo();
            var powerOfAttorneyContent = powerOfAttorneyService.GetMachineReadablePowerOfAttorneyContent(userCard.ParentalPowerOfAttorney);

            return new RetrustPowerOfAttorneyInfo(powerOfAttorneyContent, powerOfAttorneyInfo);
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
            if (userCard.PrincipalType != UserCardPowerOfAttorney.GenPrincipalTypes.entity)
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
            if (userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentIssueDate == null)
                return null;

            return new ConfirmationOfAuthorityDocument(userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentIssueDate.Value, userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentIssuer, userCard.PrincipalWithoutPowerOfAttorneyIndividualDocument);
        }

        private LegalEntityInfo GetPrincipalWithoutPowerOfAttorneyLegalEntityInfo()
        {
            var princOrg = userCard.PrincipalWithoutPowerOfAttorneyOrganization;

            if (princOrg == null)
                return null;

            var name = princOrg.Name;
            var inn = princOrg.INN;
            var kpp = princOrg.KPP;
            var ogrn = princOrg.OGRN;
            var constituentDocument = userCard.PrincipalWithoutPowerOfAttorneyOrganizationConstituentDocument;
            var phone = princOrg.Phone;

            var legalAddress = GetAddressInfo(userCard.PrincipalWithoutPowerOfAttorneyOrganizationLegalAddressSubjectRfCode, userCard.PrincipalWithoutPowerOfAttorneyOrganizationLegalAddress);

            var actualAddress = GetAddressInfo(userCard.PrincipalWithoutPowerOfAttorneyOrganizationAddressSubjectRfCode, userCard.PrincipalWithoutPowerOfAttorneyOrganizationAddress);

            return new LegalEntityInfo(name, inn, kpp, ogrn, constituentDocument, phone, registrationAddress: legalAddress, actualAddress: actualAddress);
        }

        private IndividualInfo GetPrincipalWithoutPowerOfAttorneyIndividual1Info(StaffEmployee emlpoyee)
        {
            var birthDate = emlpoyee.BirthDate;
            var citizenshipType = userCard.PrincipalWithoutPowerOfAttorneyIndividualCitizenship;
            var birthPlace = userCard.PrincipalWithoutPowerOfAttorneyIndividualBirthPlace;
            var phone = emlpoyee.Phone;
            var gender = GetGender(emlpoyee);
            AddressInfo residenceAddress = GetAddressInfo(userCard.PrincipalWithoutPowerOfAttorneyIndividualAddressSubjectRfCode, userCard.PrincipalWithoutPowerOfAttorneyIndividualAddress);

            var identityCard = GetPrincipalWithoutPowerOfAttorneyIdentityCard();

            return new IndividualInfo(birthDate, citizenshipType, birthPlace, phone, gender, residenceAddress: residenceAddress, identityCard: identityCard);
        }

        private IdentityCardOfIndividual GetPrincipalWithoutPowerOfAttorneyIdentityCard()
        {
            if (userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentKindCode == null)
                return null;

            var kind = Convert(userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentKindCode.Value);
            var seriesNumber = userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentSeries;
            var issueDate = userCard.IssueDateOfDocumentProvingIdentityOfIAWPOA;

            var issuer = userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentIssuer;
            var issuerCode = userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentIssuerCode;

            return new IdentityCardOfIndividual(kind, seriesNumber, issueDate, issuer, issuerCode);
        }

        private RussianEntityInfo GetRussianEntityInfo()
        {
            var princOrg = userCard.PrincipalOrganization;
            var name = princOrg.Name;
            var inn = princOrg.INN;
            var kpp = princOrg.KPP;
            var ogrn = princOrg.OGRN;
            var legalAddress = princOrg.Addresses.FirstOrDefault(t => t.AddressType == StaffAddresseAddressType.LegalAddress)?.Address ?? throw new Exception("Не найден юридический адрес");
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