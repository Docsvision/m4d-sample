using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using PowersOfAttorney.UserCard.Common;
using PowersOfAttorney.UserCard.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSData;
using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSDOVBBData;
using static PowersOfAttorney.UserCard.Common.Helpers.UserCardPowerOfAttorney;

public static class UserCardPowerOfAttorneyFNSDOVBBExtensions
{
    public static PowerOfAttorneyData ConvertToPowerOfAttorneyFNSDOVBBData(this UserCardPowerOfAttorney userCard, IPowerOfAttorneyService powerOfAttorneyService)
    {
        try
        {
            return Converter.Convert(userCard, powerOfAttorneyService);
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex.ToString());
            throw new Exception(String.Format(Resources.Error_IncorrectUserCardData, ex.Message));
        }
    }

    class Converter
    {
        private readonly UserCardPowerOfAttorney userCard;
        private readonly IPowerOfAttorneyService powerOfAttorneyService;
        private const int LossOfPowersUponSubstitutionLost = 1;
        private readonly bool forNotary = false;

        private Converter(UserCardPowerOfAttorney userCard, IPowerOfAttorneyService powerOfAttorneyService)
        {
            this.userCard = userCard ?? throw new ArgumentNullException(nameof(userCard));
            this.powerOfAttorneyService = powerOfAttorneyService;

            var revocationPossibleType = ConverToEnumWithFixIndex<RevocationPossibleType>(userCard.RevocationPossibleType);
            this.forNotary = revocationPossibleType == RevocationPossibleType.Impossible;
            
            // для передоверия нотариальное заверение также требуется для ФЛ и ИП доверителя
            if (!forNotary && userCard.IsRetrusted() && (userCard.PrincipalType == GenPrincipalTypes.soleProprietor || userCard.PrincipalType == GenPrincipalTypes.individual))
                this.forNotary = true;

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
            var organizationName = userCard.Signer.GetValueOrThrow(Resources.Error_EmptySigner).Unit.Name;
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
            var powers = new List<RepresentativePowerInfo>();

            var pCodes = userCard.RepresentativePowersCodes?.ToList();
            if (pCodes?.Any() == true)
                powers.AddRange(pCodes.Select(pow => new RepresentativePowerInfo(pow)));
           
            var pTexts = userCard.RepresentativePowersText?.ToList();
            if (pTexts?.Any() == true)
                powers.AddRange(pTexts.Select(pow => new RepresentativePowerInfo(pow)));

            return powers;
        }

        private RepresentativeInfo GetRepresentativeInfo()
        {
            var individualInfo = GetIndividualPrincipalInfo();

            return new RepresentativeInfo(individualInfo);
        }

        private IndividualInfo2 GetIndividualPrincipalInfo()
        {
            var employee = userCard.RepresentativeIndividual.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual);
            var inn = userCard.RepresentativeIndividualInn;
            var snils = userCard.RepresentativeIndividualSnils;
            var individualInfo = GetRepresentativeIndividualInfo(employee);
            var fio = GetFio(employee);

            return new IndividualInfo2(inn, snils, individualInfo, fio);
        }

        private FIO GetFio(StaffEmployee employee)
        {
            return new FIO(employee.LastName.AsNullable(), employee.FirstName.AsNullable(), employee.MiddleName.AsNullable());
        }

        private IndividualInfo GetRepresentativeIndividualInfo(StaffEmployee employee)
        {
            var birthDate = employee.BirthDate;
            var birthPlace = userCard.RepresentativeIndividualBirthPlace;

            if (!forNotary)
                return new IndividualInfo(birthDate, userCard.RepresentativeCitizenshipType);


            var phone = employee.Phone.AsNullable();
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

            return new DelegatedAuthorityPrincipalInfo(GetIndividualDelegatedAuthorityInfo(), GetFio(userCard.Signer.GetValueOrThrow(Resources.Error_EmptySigner)));
        }

        private IndividualDelegatedAuthorityInfo GetIndividualDelegatedAuthorityInfo()
        {
            var ceo = userCard.GenCeo.GetValueOrThrow(Resources.Error_EmptyCeo);
            return new IndividualDelegatedAuthorityInfo(userCard.GenCeoIIN, userCard.GenCeoSNILS, GetIndividualDelegatedAuthorityIndividualInfo(),
            new FIO(ceo.LastName, ceo.FirstName, ceo.MiddleName.AsNullable()));

        }

        private IndividualInfo GetIndividualDelegatedAuthorityIndividualInfo()
        {
            var ceo = userCard.GenCeo.GetValueOrThrow(Resources.Error_EmptyCeo);
            return new IndividualInfo(ceo.BirthDate, Convert(userCard.GenCeoCitizenshipSign), citizenship: userCard.GenCeoCitizenship);
        }

        private CitizenshipType Convert(PowerOfAttorneyEMCHDData.CitizenshipType? genCeoCitizenshipSign)
        {
            if (genCeoCitizenshipSign == null)
                throw new ArgumentNullException(Resources.Error_GenCeoCitizenshipSignIsEmpty);

            return (CitizenshipType)(int)genCeoCitizenshipSign.Value;
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

            return new PrincipalInfo(GetRussianLegalEntityPrincipalInfo(), GetFio(userCard.Signer.GetValueOrThrow(Resources.Error_EmptySigner)));
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
            var employee = userCard.PrincipalWithoutPowerOfAttorneyIndividual.GetValueOrThrow(Resources.Error_EmptyPrincipalWithoutPowerOfAttorneyIndividual);
            var snils = userCard.PrincipalWithoutPowerOfAttorneyIndividualSnils;
            var individualInfo = GetPrincipalWithoutPowerOfAttorneyIndividual1Info(employee);
            var position = employee.PositionName.AsNullable() ?? throw new Exception(String.Format(Resources.Error_PositionNotSpecified, employee.DisplayName));
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
            var princOrg = userCard.PrincipalWithoutPowerOfAttorneyOrganization.Value;
            if (princOrg == null)
                return null;

            var name = princOrg.Name.AsNullable();
            var inn = princOrg.INN.AsNullable();
            var kpp = princOrg.KPP.AsNullable();
            var ogrn = princOrg.OGRN.AsNullable();
            var phone = princOrg.Phone.AsNullable();
            var legalAddress = GetAddressInfo(userCard.PrincipalWithoutPowerOfAttorneyOrganizationLegalAddressSubjectRfCode, userCard.PrincipalWithoutPowerOfAttorneyOrganizationLegalAddress);

            if (!forNotary)
                return new LegalEntityInfo(name, inn, kpp, ogrn, phone:phone, registrationAddress: legalAddress);

            var constituentDocument = userCard.PrincipalWithoutPowerOfAttorneyOrganizationConstituentDocument;
            var actualAddress = GetAddressInfo(userCard.PrincipalWithoutPowerOfAttorneyOrganizationAddressSubjectRfCode, userCard.PrincipalWithoutPowerOfAttorneyOrganizationAddress);

            return new LegalEntityInfo(name, inn, kpp, ogrn, constituentDocument, phone, registrationAddress: legalAddress, actualAddress: actualAddress);
        }

        private IndividualInfo GetPrincipalWithoutPowerOfAttorneyIndividual1Info(StaffEmployee emlpoyee)
        {
            if (emlpoyee is null)
                throw new ArgumentNullException(nameof(emlpoyee));

            var birthDate = emlpoyee.BirthDate;
            var citizenshipType = userCard.PrincipalWithoutPowerOfAttorneyIndividualCitizenship;
            
            if(!forNotary)
                return new IndividualInfo(birthDate, citizenshipType);

            var birthPlace = userCard.PrincipalWithoutPowerOfAttorneyIndividualBirthPlace;
            var phone = emlpoyee.Phone.AsNullable();
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
            var princOrg = userCard.PrincipalOrganization.GetValueOrThrow(Resources.Error_EmptyPrincipalOrganization);

            string errors = String.Empty;
            var name = princOrg.Name.AsNullable();
            if (name == null)
                errors += String.Format(Resources.Error_OrgNameNotSpecified, princOrg.GetObjectId()) + Environment.NewLine;
            var inn = princOrg.INN.AsNullable();
            if (inn == null)
                errors += String.Format(Resources.Error_InnNotSpecified, princOrg.Name) + Environment.NewLine;
            var kpp = princOrg.KPP.AsNullable();
            if (kpp == null)
                errors += String.Format(Resources.Error_KppNotSpecified, princOrg.Name) + Environment.NewLine;
            var ogrn = princOrg.OGRN.AsNullable();
            if (ogrn == null)
                errors += String.Format(Resources.Error_OgrnNotSpecified, princOrg.Name) + Environment.NewLine;
            var legalAddress = princOrg.Addresses.FirstOrDefault(t => t.AddressType == StaffAddresseAddressType.LegalAddress)?.Address;
            if (legalAddress == null)
                errors += String.Format(Resources.Error_LegalAddressNotSpecified, princOrg.Name) + Environment.NewLine;

            if (!String.IsNullOrEmpty(errors))
            {
                throw new Exception(errors);
            }

            if(!forNotary)
                return new RussianEntityInfo(name, ogrn, inn, kpp, legalAddress);


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