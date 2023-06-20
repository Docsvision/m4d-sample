using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.WebClient.Diagnostics;

using System;
using System.Collections.Generic;
using System.Linq;

using static PowersOfAttorneyServerExtension.Helpers.UserCardPowerOfAttorney;

namespace PowersOfAttorneyServerExtension.Helpers
{
    internal static class UserCardPowerOfAttorneyEMCHDExtensions
    {
        public static PowerOfAttorneyData ConvertToPowerOfAttorneyEMHCDData(this UserCardPowerOfAttorney userCard)
        {
            return Converter.Convert(userCard);
        }

        class Converter
        {
            private readonly UserCardPowerOfAttorney userCard;

            private Converter(UserCardPowerOfAttorney userCard)
            {
                this.userCard = userCard ?? throw new ArgumentNullException(nameof(userCard));
            }

            public static PowerOfAttorneyEMHCDData Convert(UserCardPowerOfAttorney userCard)
            {
                return new Converter(userCard).Convert();
            }

            private PowerOfAttorneyEMHCDData Convert()
            {
                return new PowerOfAttorneyEMHCDData(withEsia: false, withNotary: false, withTax: true)
                {
                    Document = CreateDocumentPart(),
                    SenderID = $"{userCard.GenEntityPrinINN}{userCard.GenEntityPrinKPP}",
                    RecipientID = userCard.GenTaxAuthPOASubmit,
                    FinalRecipientID = userCard.GenFinalRecipientTaxID,
                };
            }

            private PowerOfAttorneyEMHCDData.PowerOfAttorneyDocument CreateDocumentPart()
            {
                return new PowerOfAttorneyEMHCDData.PowerOfAttorneyDocument
                {
                    KND = userCard.GenKnd,
                    PowerOfAttorneyData = CreatePowerOfAttorneyDataPart()
                };
            }

            private PowerOfAttorneyEMHCDData.PowerOfAttorneyDocumentData CreatePowerOfAttorneyDataPart()
            {
                return new PowerOfAttorneyEMHCDData.PowerOfAttorneyDocumentData
                {
                    //NotaryCertificateInfo = CreateNotaryCertificateInfoPart(),
                    PowerOfAttorney = CreatePowerOfAttorneyPart(),
                    Principal = CreatePrincipalsPart(),
                    Representative = CreateRepresentativesPart(),
                    RepresentativePowers = CreateRepresentativePowersPart()
                };
            }

            private PowerOfAttorneyEMHCDData.RepresentativePowersInfo CreateRepresentativePowersPart()
            {
                var powersType = userCard.GenPowersType ?? throw new ApplicationException(Resources.Error_PowersTypeIsEmpty);

                var part = new PowerOfAttorneyEMHCDData.RepresentativePowersInfo
                {
                    AuthorityType = powersType,
                    JointRepresentationType = userCard.GenJointExerPowers ?? throw new ApplicationException(Resources.Error_JointExerIsEmpty),
                };

                if (userCard.GenLossPowersTransfer != null)
                    part.LossOfAuthorityType = userCard.GenLossPowersTransfer;

                if (powersType == PowerOfAttorneyEMHCDData.AuthorityType.Code)
                {
                    part.MachineReadablePowersInfo = userCard.GetPowersCodes().Select(power => new PowerOfAttorneyEMHCDData.MachineReadablePowersInfo
                    {
                        PowersCode = power
                    }).ToList();
                }
                else
                {
                    part.PowersTextContent = userCard.GenPowerTextContent;
                }

                return part;
            }

            private PowerOfAttorneyEMHCDData.RepresentativesInfo CreateRepresentativesPart()
            {
                return new PowerOfAttorneyEMHCDData.RepresentativesInfo
                {
                    RepresentativeType = Convert(userCard.GenRepresentativeType ?? throw new ApplicationException(Resources.Error_GenRepresentativeIsEmpty)),
                    Representative = CreateRepresentativePart(userCard.GenRepresentativeType.Value)
                };
            }

            private PowerOfAttorneyEMHCDData.RepresentativeInfo CreateRepresentativePart(GenRepresentativeTypes representativeType)
            {
                // Представитель-физлицо
                if (representativeType == GenRepresentativeTypes.individual)
                    return new PowerOfAttorneyEMHCDData.RepresentativeInfo
                    {
                        Individual = new PowerOfAttorneyEMHCDData.SoleExecutiveIndividualInfo
                        {
                            Inn = userCard.GenRepresentativeINN,
                            IndividualInfo = CreateRepresentativeIndividualInfoPart(),
                            Position = userCard.GenRepresentativePosition ?? userCard.GenRepresentative.PositionName,
                            Snils = userCard.GenRepresentativeSNILS
                        }
                    };

                throw new ArgumentOutOfRangeException(nameof(representativeType));
            }

            private PowerOfAttorneyEMHCDData.IndividualInfo CreateRepresentativeIndividualInfoPart()
            {
                return new PowerOfAttorneyEMHCDData.IndividualInfo
                {
                    BirthDate = userCard.GenReprDateOfBirth,
                    BirthPlace = userCard.GenReprPlaceOfBirth,
                    Citizenship = userCard.GenReprCitizenship,
                    CitizenshipType = userCard.GenReprCitizenshipSign,
                    ContactPhone = userCard.GenReprPhoneNum,
                    EMail = userCard.GenReprEmail,
                    Fio = new PowerOfAttorneyEMHCDData.FIO
                    {
                        FirstName = userCard.GenRepresentative.FirstName,
                        LastName = userCard.GenRepresentative.LastName,
                        MiddleName = userCard.GenRepresentative.MiddleName
                    },
                    Gender = userCard.GenGenderOfRepresentative,
                    IdentityCard = new PowerOfAttorneyEMHCDData.IdentityCardOfIndividual
                    {
                        DocumentKindCode = userCard.GenTypeCodeReprIDDoc?.ToString(),
                        DocumentSerialNumber = userCard.GenSerNumReprIDDoc,
                        ExpDate = userCard.GenDateExpReprIDDoc,
                        IssueDate = userCard.GenDateIssReprIDDoc ?? throw new ApplicationException(Resources.Error_DateIssReprIDDocIsEmpty),
                        Issuer = userCard.GenAuthIssReprIDDoc,
                        IssuerCode = userCard.GenCodeAuthDivIssReprIDDoc
                    }
                };
            }

            private PowerOfAttorneyEMHCDData.PrincipalsInfo CreatePrincipalsPart()
            {
                return new PowerOfAttorneyEMHCDData.PrincipalsInfo
                {
                    PrincipalType = Convert(userCard.GenPrincipalType ?? throw new ApplicationException(Resources.Error_PrincipalTypeIsEmpty)),
                    PrincipalInfo = CreatePrincipalPart(userCard.GenPrincipalType.Value)
                };
            }

            private PowerOfAttorneyEMHCDData.PrincipalInfo CreatePrincipalPart(GenPrincipalTypes principalType)
            {
                if (principalType == GenPrincipalTypes.entity)
                    return new PowerOfAttorneyEMHCDData.PrincipalInfo
                    {
                        RussianEntity = new PowerOfAttorneyEMHCDData.RussianLegalEntityPrincipalInfo
                        {
                            EntityInfo = CreatePrincipalEntityInfoPart(),
                            PrincipalsWithoutPowerOfAttorneyInfo = new List<PowerOfAttorneyEMHCDData.PrincipalWithoutPowerOfAttorneyInfo> { CreatePrincipalsWithoutPowerOfAttorneyInfoPart() },
                            SoleExecutiveIsIndividual = userCard.GenIndSEB == true,
                            SoleExecutiveIsManagementCompany = userCard.GenMngtCompanySEB == true,
                            SoleExecutiveIsSoleProprietor = userCard.GenSolePropSEB == true
                        }
                    };

                throw new ArgumentOutOfRangeException(nameof(principalType));
            }

            private PowerOfAttorneyEMHCDData.PrincipalWithoutPowerOfAttorneyInfo CreatePrincipalsWithoutPowerOfAttorneyInfoPart()
            {
                return new PowerOfAttorneyEMHCDData.PrincipalWithoutPowerOfAttorneyInfo
                {
                    AuthorityType = Convert(userCard.GenPowersTypeOfSEB ?? throw new ApplicationException(Resources.Error_PowersTypeOfSEBIsEmpty)),
                    IndividualInfo = CreateSeoIndividualInfoPart()
                };
            }

            private PowerOfAttorneyEMHCDData.SoleExecutiveIndividualInfo CreateSeoIndividualInfoPart()
            {
                return new PowerOfAttorneyEMHCDData.SoleExecutiveIndividualInfo
                {
                    ConfirmationDocument = new PowerOfAttorneyEMHCDData.ConfirmationOfAuthorityDocument
                    {
                        DocumentName = userCard.GenDocConfAuthCEO,
                        IdentityOfDocument = userCard.GenSerNumCEOIDDoc,
                        IssueDate = userCard.GenDateIssCEOIDDoc,
                        Issuer = userCard.GenAuthIssCEOIDDoc
                    },
                    IndividualInfo = new PowerOfAttorneyEMHCDData.IndividualInfo
                    {
                        BirthDate = userCard.GenCeoDateOfBirth,
                        BirthPlace = userCard.GenCeoPlaceOfBirth,
                        Citizenship = userCard.GenCeoCitizenship,
                        CitizenshipType = userCard.GenCeoCitizenshipSign,
                        ContactPhone = userCard.GenCeoPhoneNum,
                        EMail = userCard.GenCeoPhoneNum,
                        Fio = new PowerOfAttorneyEMHCDData.FIO
                        {
                            FirstName = userCard.GenCeo.FirstName,
                            LastName = userCard.GenCeo.LastName,
                            MiddleName = userCard.GenCeo.MiddleName
                        },
                        Gender = userCard.GenCeoGender,
                        IdentityCard = new PowerOfAttorneyEMHCDData.IdentityCardOfIndividual
                        {
                            DocumentKindCode = userCard.GenTypeCodeCEOIDDoc?.ToString(),
                            DocumentSerialNumber = userCard.GenSerNumCEOIDDoc,
                            ExpDate = userCard.GenDateExpCEOIDDoc,
                            IssueDate = userCard.GenDateIssCEOIDDoc ?? throw new ApplicationException(Resources.Error_DateIssCEOIDDocIsEmpty),
                            Issuer = userCard.GenAuthIssCEOIDDoc,
                            IssuerCode = userCard.GenCodeAuthDivIssCEOIDDoc
                        },
                        ResidenceAddress = new PowerOfAttorneyEMHCDData.AddressInfo
                        {
                            Address = userCard.GenCeoAddrRussia,
                            SubjectOfRussia = userCard.GenCeoAddrSubRussia,
                            FiasAddress = userCard.GenFiasCEOAddrRussia,
                            FiasCode = userCard.GenCeoFIASAddrID
                        }
                    },
                    Inn = userCard.GenCeoIIN,
                    ParticipantStatus = userCard.GenNotarStatOfSoleExBody,
                    Position = userCard.GenCeoPosition,
                    Snils = userCard.GenCeoSNILS
                };
            }

            private PowerOfAttorneyEMHCDData.LegalEntityInfo CreatePrincipalEntityInfoPart()
            {
                return new PowerOfAttorneyEMHCDData.LegalEntityInfo
                {
                    ConfirmationOfAuthorityDocument = new PowerOfAttorneyEMHCDData.ConfirmationOfAuthorityDocument
                    {
                        DocumentName = userCard.GenDocConfAuthCEO,
                        IdentityOfDocument = userCard.GenSerNumCEOIDDoc,
                        IssueDate = userCard.GenDateIssCEOIDDoc,
                        Issuer = userCard.GenAuthIssCEOIDDoc
                    },
                    ConstituentDocument = userCard.GenConstDocumentEntPrin,
                    EMail = userCard.GenEmailEntPrin,
                    Inn = userCard.GenEntityPrinINN ?? userCard.GenEntityPrincipal.INN,
                    Kpp = userCard.GenEntityPrinKPP ?? userCard.GenEntityPrincipal.KPP,
                    LegalAddress = new PowerOfAttorneyEMHCDData.AddressInfo
                    {
                        FiasAddress = userCard.GenFiasEntAddrRussia,
                        FiasCode = userCard.GenEntFIASAddrID,
                        Address = userCard.GenEntAddrRussia,
                        SubjectOfRussia = userCard.GenEntAddrSubRussia,
                    },
                    Name = userCard.GenEntityPrincipal.Name,
                    Ogrn = userCard.GenEntPrinOGRN ?? userCard.GenEntityPrincipal.OGRN,
                    ParticipantStatus = userCard.GenNotarStatusOfEntPrin,
                    Phone = userCard.GenEntityPrincipal.Phone,
                    RegistrationNumber = null
                };
            }

            private PowerOfAttorneyEMHCDData.PowerOfAttorneyInfo CreatePowerOfAttorneyPart()
            {
                return new PowerOfAttorneyEMHCDData.PowerOfAttorneyInfo
                {
                    InformationSystemName = userCard.GenInfsysGenPOA,
                    IrrevocablePowerOfAttorneyInfo = userCard.GenPoaKind == GenPoaKindTypes.irrecovablePOA ? CreateIrrevocablePowerOfAttorneyInfoPart() : null,
                    PowerOfAttorneyAdditionalNumber = userCard.GenAddPOAID,
                    PowerOfAttorneyEndDate = userCard.GenPoaExpirationDate ?? throw new ApplicationException(Resources.Error_EmptyPowerOfAttorneyEndDate),
                    PowerOfAttorneyInternalNumber = userCard.GenInternalPOANumber,
                    PowerOfAttorneyInternalRegistrationDate = userCard.GenPoaInternRegDate,
                    PowerOfAttorneyKind = Convert(userCard.GenPoaKind ?? throw new ApplicationException(Resources.Error_PoaKindIsEmpty)),
                    PowerOfAttorneyNotaryNumber = userCard.GenPoaRegNumNotarRegistry,
                    PowerOfAttorneyNumber = userCard.GenSinglePOAregnumber ?? throw new ApplicationException(Resources.Error_SinglePOAregnumberIsEmpty),
                    PowerOfAttorneyStartDate = userCard.GenPoaDateOfIssue ?? throw new ApplicationException(Resources.Error_PoaDateOfIssueIsEmpty),
                    RetrustType = Convert(userCard.GenPossibilityOfSubstitution ?? throw new ApplicationException(Resources.Error_PossibilityOfSubstitutionIsEmpty)),
                    SubmittedPowerOfAttorneyTaxCode = userCard.GenTaxAuthPOASubmit,
                    TaxCode = new List<string> { userCard.GenTaxAuthPOAValid }
                };
            }

            private PowerOfAttorneyEMHCDData.IrrevocablePowerOfAttorneyInfo CreateIrrevocablePowerOfAttorneyInfoPart()
            {
                return new PowerOfAttorneyEMHCDData.IrrevocablePowerOfAttorneyInfo
                {
                    RevocationCondition = userCard.GenRevocIrrevPOA ?? throw new ApplicationException(Resources.Error_RevocIrrevPOAIsEmpty),
                    RevocationConditionDescription = userCard.RevocationConditionDescription,
                    RevocationPossibleType = userCard.GenSignTransferIrrevPOA ?? throw new ApplicationException(Resources.Error_SignTransferIrrevPOAIsEmpty)
                };
            }


            private PowerOfAttorneyEMHCDData.NotaryCertificateInfo CreateNotaryCertificateInfoPart()
            {
                return new PowerOfAttorneyEMHCDData.NotaryCertificateInfo
                {
                    AdditionalInformation = userCard.GenAddInfo,
                    ElectronicDocumentTransferMethod = userCard.GenMethOfIssElNotarDoc,
                    FromAttorneyToEPGU = userCard.GenSendDocReprEPGU,
                    FromAttorneyToFNP = userCard.GenSendDocReprFNP,
                    FromDeclarantToFNP = userCard.GenSendDocApplFNP,
                    FromPrincipalToEPGU = userCard.GenSendDocPrinEPGU,
                    HandwrittenSignature = new List<PowerOfAttorneyEMHCDData.HandwrittenSignature> { CreateHandwrittenSignaturePart() },
                    NotaryDeputyInfo = CreateNotaryDeputyInfoPart(),
                    NotaryInfo = CreateNotaryInfoPart(),
                    NotaryPaymentDiscount = userCard.GenAmountOfBenefGranted,
                    NotaryPaymentPaid = userCard.GenPayNotarAction ?? throw new ApplicationException(Resources.Error_PayNotarActionIsEmpty),
                    OtherDocumentTransferMethod = userCard.GenAnotherMethOfIss,
                    OtherInformationOfAuthenticationInscription = userCard.GenOtherInfoOfCertInscr,
                    Place = userCard.GenPoaIssuePlace
                };
            }

            private PowerOfAttorneyEMHCDData.NotaryInfo CreateNotaryInfoPart()
            {
                return new PowerOfAttorneyEMHCDData.NotaryInfo
                {
                    Fio = new PowerOfAttorneyEMHCDData.FIO
                    {
                        FirstName = userCard.GenNotaryName,
                        LastName = userCard.GenNotayLastName,
                        MiddleName = userCard.GenInterimNotaryMiddleName
                    },
                    Position = userCard.GenNotaryPosition,
                    RegistrationNumber = userCard.GenRegNumOfNotaryInMoJ

                };
            }

            private PowerOfAttorneyEMHCDData.NotaryDeputyInfo CreateNotaryDeputyInfoPart()
            {
                return new PowerOfAttorneyEMHCDData.NotaryDeputyInfo
                {
                    Fio = new PowerOfAttorneyEMHCDData.FIO
                    {
                        FirstName = userCard.GenInterimNotarytName,
                        LastName = userCard.GenInterimNotaryLastName,
                        MiddleName = userCard.GenInterimNotaryMiddleName
                    },
                    Position = userCard.GenPositionOfInterimNotary,
                    RegistrationNumber = userCard.GenRegNumOfIntNotarInMoJ
                };
            }

            private PowerOfAttorneyEMHCDData.HandwrittenSignature CreateHandwrittenSignaturePart()
            {
                return new PowerOfAttorneyEMHCDData.HandwrittenSignature
                {
                    Fio = new PowerOfAttorneyEMHCDData.FIO
                    {
                        FirstName = userCard.GenPoaSignerName,
                        LastName = userCard.GenPoaSignerLastName,
                        MiddleName = userCard.GenPoaSignerMiddleName
                    },
                    PdfDocumentHash = System.Convert.FromBase64String(userCard.GenPdfDocHash),
                    SignatureDate = userCard.GenDateTimeOfSignature ?? throw new ArgumentOutOfRangeException(Resources.Error_DateTimeOfSignatureIsEmpty),
                    SignatureHash = System.Convert.FromBase64String(userCard.GenSignatureHash),
                    SignatureImage = System.Convert.FromBase64String(userCard.GenSignatureImage)
                };
            }

            private PowerOfAttorneyEMHCDData.SoleExecutiveAuthorityType Convert(GenPowersTypeOfSEBTypes authorityType)
            {
                switch (authorityType)
                {
                    case GenPowersTypeOfSEBTypes.individual:
                        return PowerOfAttorneyEMHCDData.SoleExecutiveAuthorityType.Individual;
                    case GenPowersTypeOfSEBTypes.joint:
                        return PowerOfAttorneyEMHCDData.SoleExecutiveAuthorityType.Joint;
                }
                throw new ArgumentOutOfRangeException(nameof(authorityType));
            }


            private PowerOfAttorneyEMHCDData.PowerOfAttorneyLossOfAuthorityType? Convert(LossPowersSubstTypes? lossPowers)
            {
                if (lossPowers == null)
                    return null;

                switch (lossPowers.Value)
                {
                    case LossPowersSubstTypes.notLost:
                        return PowerOfAttorneyEMHCDData.PowerOfAttorneyLossOfAuthorityType.NotLost;
                    case LossPowersSubstTypes.lost:
                        return PowerOfAttorneyEMHCDData.PowerOfAttorneyLossOfAuthorityType.Lost;
                }

                throw new ArgumentOutOfRangeException(nameof(lossPowers));
            }

            private PowerOfAttorneyEMHCDData.JointRepresentationType Convert(JointExerPowersTypes jointExer)
            {
                switch (jointExer)
                {
                    case JointExerPowersTypes.individual:
                        return PowerOfAttorneyEMHCDData.JointRepresentationType.Individual;
                    case JointExerPowersTypes.joint:
                        return PowerOfAttorneyEMHCDData.JointRepresentationType.Joint;
                }
                throw new ArgumentOutOfRangeException(nameof(jointExer));
            }

            private PowerOfAttorneyEMHCDData.EntityType Convert(GenRepresentativeTypes entity)
            {
                switch (entity)
                {
                    case GenRepresentativeTypes.entity:
                        return PowerOfAttorneyEMHCDData.EntityType.RussianEntity;
                    case GenRepresentativeTypes.soleProprietor:
                        return PowerOfAttorneyEMHCDData.EntityType.SoleProprietor;
                    case GenRepresentativeTypes.individual:
                        return PowerOfAttorneyEMHCDData.EntityType.Individual;
                    case GenRepresentativeTypes.rusEntityBranch:
                        return PowerOfAttorneyEMHCDData.EntityType.BranchOfEntity;
                    case GenRepresentativeTypes.foreignEntBranch:
                        return PowerOfAttorneyEMHCDData.EntityType.BranchOfForeignEntity;
                }
                throw new ArgumentOutOfRangeException(nameof(entity));
            }

            private PowerOfAttorneyEMHCDData.PrincipalType Convert(GenPrincipalTypes entity)
            {
                switch (entity)
                {
                    case GenPrincipalTypes.entity:
                        return PowerOfAttorneyEMHCDData.PrincipalType.RussianEntity;
                    case GenPrincipalTypes.foreignEntity:
                        return PowerOfAttorneyEMHCDData.PrincipalType.ForeignEntity;
                    case GenPrincipalTypes.soleProprietor:
                        return PowerOfAttorneyEMHCDData.PrincipalType.SoleProprietor;
                    case GenPrincipalTypes.individual:
                        return PowerOfAttorneyEMHCDData.PrincipalType.Individual;
                }
                throw new ArgumentOutOfRangeException(nameof(entity));
            }

            private PowerOfAttorneyRetrustType Convert(GenPossibilityOfSubstitutionTypes substitution)
            {
                switch (substitution)
                {
                    case GenPossibilityOfSubstitutionTypes.withoutSubstitution:
                        return PowerOfAttorneyRetrustType.None;
                    case GenPossibilityOfSubstitutionTypes.onetimeSubstitution:
                        return PowerOfAttorneyRetrustType.OneTime;
                    case GenPossibilityOfSubstitutionTypes.subsequentSubstitution:
                        return PowerOfAttorneyRetrustType.Followed;
                }
                throw new ArgumentOutOfRangeException(nameof(substitution));
            }


            private PowerOfAttorneyEMHCDData.PowerOfAttorneyKind Convert(GenPoaKindTypes poaKind)
            {
                switch (poaKind)
                {
                    case GenPoaKindTypes.ordinaryPOA:
                        return PowerOfAttorneyEMHCDData.PowerOfAttorneyKind.Regular;
                    case GenPoaKindTypes.irrecovablePOA:
                        return PowerOfAttorneyEMHCDData.PowerOfAttorneyKind.Irrevocable;
                }
                throw new ArgumentOutOfRangeException(nameof(poaKind));
            }
        }
    }
}
