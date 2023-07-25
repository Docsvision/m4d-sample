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
        public static PowerOfAttorneyData ConvertToPowerOfAttorneyEMCHDData(this UserCardPowerOfAttorney userCard)
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

            public static PowerOfAttorneyEMCHDData Convert(UserCardPowerOfAttorney userCard)
            {
                return new Converter(userCard).Convert();
            }

            private PowerOfAttorneyEMCHDData Convert()
            {
                return new PowerOfAttorneyEMCHDData(withEsia: false, withNotary: false, withTax: true)
                {
                    Document = CreateDocumentPart(),
                    SenderID = $"{userCard.GenEntityPrinINN}{userCard.GenEntityPrinKPP}",
                    RecipientID = userCard.GenTaxAuthPOASubmit,
                    FinalRecipientID = userCard.GenFinalRecipientTaxID,
                };
            }

            private PowerOfAttorneyEMCHDData.PowerOfAttorneyDocument CreateDocumentPart()
            {
                return new PowerOfAttorneyEMCHDData.PowerOfAttorneyDocument
                {
                    KND = userCard.GenKnd,
                    PowerOfAttorneyData = CreatePowerOfAttorneyDataPart()
                };
            }

            private PowerOfAttorneyEMCHDData.PowerOfAttorneyDocumentData CreatePowerOfAttorneyDataPart()
            {
                return new PowerOfAttorneyEMCHDData.PowerOfAttorneyDocumentData
                {
                    //NotaryCertificateInfo = CreateNotaryCertificateInfoPart(),
                    PowerOfAttorney = CreatePowerOfAttorneyPart(),
                    Principal = CreatePrincipalsPart(),
                    Representative = CreateRepresentativesPart(),
                    RepresentativePowers = CreateRepresentativePowersPart()
                };
            }

            private PowerOfAttorneyEMCHDData.RepresentativePowersInfo CreateRepresentativePowersPart()
            {
                var powersType = userCard.GenPowersType ?? throw new ApplicationException(Resources.Error_PowersTypeIsEmpty);

                var part = new PowerOfAttorneyEMCHDData.RepresentativePowersInfo
                {
                    AuthorityType = powersType,
                    JointRepresentationType = userCard.GenJointExerPowers ?? throw new ApplicationException(Resources.Error_JointExerIsEmpty),
                };

                if (userCard.GenLossPowersTransfer != null)
                    part.LossOfAuthorityType = userCard.GenLossPowersTransfer;

                if (powersType == PowerOfAttorneyEMCHDData.AuthorityType.Code)
                {
                    part.MachineReadablePowersInfo = userCard.GetPowersCodes().Select(power => new PowerOfAttorneyEMCHDData.MachineReadablePowersInfo
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

            private PowerOfAttorneyEMCHDData.RepresentativesInfo CreateRepresentativesPart()
            {
                return new PowerOfAttorneyEMCHDData.RepresentativesInfo
                {
                    RepresentativeType = Convert(userCard.GenRepresentativeType ?? throw new ApplicationException(Resources.Error_GenRepresentativeIsEmpty)),
                    Representative = CreateRepresentativePart(userCard.GenRepresentativeType.Value)
                };
            }

            private PowerOfAttorneyEMCHDData.RepresentativeInfo CreateRepresentativePart(GenRepresentativeTypes representativeType)
            {
                var representative = userCard.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual);

                // Представитель-физлицо
                if (representativeType == GenRepresentativeTypes.individual)
                    return new PowerOfAttorneyEMCHDData.RepresentativeInfo
                    {
                        Individual = new PowerOfAttorneyEMCHDData.SoleExecutiveIndividualInfo
                        {
                            Inn = userCard.GenRepresentativeINN,
                            IndividualInfo = CreateRepresentativeIndividualInfoPart(),
                            Position = userCard.GenRepresentativePosition ?? representative.PositionName.AsNullable(),
                            Snils = userCard.GenRepresentativeSNILS
                        }
                    };

                throw new ArgumentOutOfRangeException(nameof(representativeType));
            }

            private PowerOfAttorneyEMCHDData.IndividualInfo CreateRepresentativeIndividualInfoPart()
            {
                var representative = userCard.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual);

                return new PowerOfAttorneyEMCHDData.IndividualInfo
                {
                    BirthDate = userCard.GenReprDateOfBirth,
                    BirthPlace = userCard.GenReprPlaceOfBirth,
                    Citizenship = userCard.GenReprCitizenship,
                    CitizenshipType = userCard.GenReprCitizenshipSign,
                    ContactPhone = userCard.GenReprPhoneNum,
                    EMail = userCard.GenReprEmail,
                    Fio = new PowerOfAttorneyEMCHDData.FIO
                    {
                        FirstName = representative.FirstName.AsNullable(),
                        LastName = representative.LastName.AsNullable(),
                        MiddleName = representative.MiddleName.AsNullable()
                    },
                    Gender = userCard.GenGenderOfRepresentative,
                    IdentityCard = new PowerOfAttorneyEMCHDData.IdentityCardOfIndividual
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

            private PowerOfAttorneyEMCHDData.PrincipalsInfo CreatePrincipalsPart()
            {
                return new PowerOfAttorneyEMCHDData.PrincipalsInfo
                {
                    PrincipalType = Convert(userCard.GenPrincipalType ?? throw new ApplicationException(Resources.Error_PrincipalTypeIsEmpty)),
                    PrincipalInfo = CreatePrincipalPart(userCard.GenPrincipalType.Value)
                };
            }

            private PowerOfAttorneyEMCHDData.PrincipalInfo CreatePrincipalPart(GenPrincipalTypes principalType)
            {
                if (principalType == GenPrincipalTypes.entity)
                    return new PowerOfAttorneyEMCHDData.PrincipalInfo
                    {
                        RussianEntity = new PowerOfAttorneyEMCHDData.RussianLegalEntityPrincipalInfo
                        {
                            EntityInfo = CreatePrincipalEntityInfoPart(),
                            PrincipalsWithoutPowerOfAttorneyInfo = new List<PowerOfAttorneyEMCHDData.PrincipalWithoutPowerOfAttorneyInfo> { CreatePrincipalsWithoutPowerOfAttorneyInfoPart() },
                            SoleExecutiveIsIndividual = userCard.GenIndSEB == true,
                            SoleExecutiveIsManagementCompany = userCard.GenMngtCompanySEB == true,
                            SoleExecutiveIsSoleProprietor = userCard.GenSolePropSEB == true
                        }
                    };

                throw new ArgumentOutOfRangeException(nameof(principalType));
            }

            private PowerOfAttorneyEMCHDData.PrincipalWithoutPowerOfAttorneyInfo CreatePrincipalsWithoutPowerOfAttorneyInfoPart()
            {
                return new PowerOfAttorneyEMCHDData.PrincipalWithoutPowerOfAttorneyInfo
                {
                    AuthorityType = Convert(userCard.GenPowersTypeOfSEB ?? throw new ApplicationException(Resources.Error_PowersTypeOfSEBIsEmpty)),
                    IndividualInfo = CreateSeoIndividualInfoPart()
                };
            }

            private PowerOfAttorneyEMCHDData.SoleExecutiveIndividualInfo CreateSeoIndividualInfoPart()
            {
                var ceo = userCard.GenCeo.GetValueOrThrow(Resources.Error_EmptyCeo);
                return new PowerOfAttorneyEMCHDData.SoleExecutiveIndividualInfo
                {
                    ConfirmationDocument = new PowerOfAttorneyEMCHDData.ConfirmationOfAuthorityDocument
                    {
                        DocumentName = userCard.GenDocConfAuthCEO,
                        IdentityOfDocument = userCard.GenSerNumCEOIDDoc,
                        IssueDate = userCard.GenDateIssCEOIDDoc,
                        Issuer = userCard.GenAuthIssCEOIDDoc
                    },
                    IndividualInfo = new PowerOfAttorneyEMCHDData.IndividualInfo
                    {
                        BirthDate = userCard.GenCeoDateOfBirth,
                        BirthPlace = userCard.GenCeoPlaceOfBirth,
                        Citizenship = userCard.GenCeoCitizenship,
                        CitizenshipType = userCard.GenCeoCitizenshipSign,
                        ContactPhone = userCard.GenCeoPhoneNum,
                        EMail = userCard.GenCeoPhoneNum,
                        Fio = new PowerOfAttorneyEMCHDData.FIO
                        {
                            FirstName = ceo.FirstName.AsNullable(),
                            LastName = ceo.LastName.AsNullable(),
                            MiddleName = ceo.MiddleName.AsNullable()
                        },
                        Gender = userCard.GenCeoGender,
                        IdentityCard = new PowerOfAttorneyEMCHDData.IdentityCardOfIndividual
                        {
                            DocumentKindCode = userCard.GenTypeCodeCEOIDDoc?.ToString(),
                            DocumentSerialNumber = userCard.GenSerNumCEOIDDoc,
                            ExpDate = userCard.GenDateExpCEOIDDoc,
                            IssueDate = userCard.GenDateIssCEOIDDoc ?? throw new ApplicationException(Resources.Error_DateIssCEOIDDocIsEmpty),
                            Issuer = userCard.GenAuthIssCEOIDDoc,
                            IssuerCode = userCard.GenCodeAuthDivIssCEOIDDoc
                        },
                        ResidenceAddress = new PowerOfAttorneyEMCHDData.AddressInfo
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

            private PowerOfAttorneyEMCHDData.LegalEntityInfo CreatePrincipalEntityInfoPart()
            {
                var entityPrincipal = userCard.GenEntityPrincipal.GetValueOrThrow(Resources.Error_EmptyPrincipalOrganization);
                return new PowerOfAttorneyEMCHDData.LegalEntityInfo
                {
                    ConfirmationOfAuthorityDocument = new PowerOfAttorneyEMCHDData.ConfirmationOfAuthorityDocument
                    {
                        DocumentName = userCard.GenDocConfAuthCEO,
                        IdentityOfDocument = userCard.GenSerNumCEOIDDoc,
                        IssueDate = userCard.GenDateIssCEOIDDoc,
                        Issuer = userCard.GenAuthIssCEOIDDoc
                    },
                    ConstituentDocument = userCard.GenConstDocumentEntPrin,
                    EMail = userCard.GenEmailEntPrin,
                    Inn = userCard.GenEntityPrinINN ?? entityPrincipal.INN.AsNullable(),
                    Kpp = userCard.GenEntityPrinKPP ?? entityPrincipal.KPP.AsNullable(),
                    LegalAddress = new PowerOfAttorneyEMCHDData.AddressInfo
                    {
                        FiasAddress = userCard.GenFiasEntAddrRussia,
                        FiasCode = userCard.GenEntFIASAddrID,
                        Address = userCard.GenEntAddrRussia,
                        SubjectOfRussia = userCard.GenEntAddrSubRussia,
                    },
                    Name = entityPrincipal.Name.AsNullable(),
                    Ogrn = userCard.GenEntPrinOGRN ?? entityPrincipal.OGRN.AsNullable(),
                    ParticipantStatus = userCard.GenNotarStatusOfEntPrin,
                    Phone = entityPrincipal.Phone.AsNullable(),
                    RegistrationNumber = null
                };
            }

            private PowerOfAttorneyEMCHDData.PowerOfAttorneyInfo CreatePowerOfAttorneyPart()
            {
                return new PowerOfAttorneyEMCHDData.PowerOfAttorneyInfo
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

            private PowerOfAttorneyEMCHDData.IrrevocablePowerOfAttorneyInfo CreateIrrevocablePowerOfAttorneyInfoPart()
            {
                return new PowerOfAttorneyEMCHDData.IrrevocablePowerOfAttorneyInfo
                {
                    RevocationCondition = userCard.GenRevocIrrevPOA ?? throw new ApplicationException(Resources.Error_RevocIrrevPOAIsEmpty),
                    RevocationConditionDescription = userCard.RevocationConditionDescription,
                    RevocationPossibleType = userCard.GenSignTransferIrrevPOA ?? throw new ApplicationException(Resources.Error_SignTransferIrrevPOAIsEmpty)
                };
            }


            private PowerOfAttorneyEMCHDData.NotaryCertificateInfo CreateNotaryCertificateInfoPart()
            {
                return new PowerOfAttorneyEMCHDData.NotaryCertificateInfo
                {
                    AdditionalInformation = userCard.GenAddInfo,
                    ElectronicDocumentTransferMethod = userCard.GenMethOfIssElNotarDoc,
                    FromAttorneyToEPGU = userCard.GenSendDocReprEPGU,
                    FromAttorneyToFNP = userCard.GenSendDocReprFNP,
                    FromDeclarantToFNP = userCard.GenSendDocApplFNP,
                    FromPrincipalToEPGU = userCard.GenSendDocPrinEPGU,
                    HandwrittenSignature = new List<PowerOfAttorneyEMCHDData.HandwrittenSignature> { CreateHandwrittenSignaturePart() },
                    NotaryDeputyInfo = CreateNotaryDeputyInfoPart(),
                    NotaryInfo = CreateNotaryInfoPart(),
                    NotaryPaymentDiscount = userCard.GenAmountOfBenefGranted,
                    NotaryPaymentPaid = userCard.GenPayNotarAction ?? throw new ApplicationException(Resources.Error_PayNotarActionIsEmpty),
                    OtherDocumentTransferMethod = userCard.GenAnotherMethOfIss,
                    OtherInformationOfAuthenticationInscription = userCard.GenOtherInfoOfCertInscr,
                    Place = userCard.GenPoaIssuePlace
                };
            }

            private PowerOfAttorneyEMCHDData.NotaryInfo CreateNotaryInfoPart()
            {
                return new PowerOfAttorneyEMCHDData.NotaryInfo
                {
                    Fio = new PowerOfAttorneyEMCHDData.FIO
                    {
                        FirstName = userCard.GenNotaryName,
                        LastName = userCard.GenNotayLastName,
                        MiddleName = userCard.GenInterimNotaryMiddleName
                    },
                    Position = userCard.GenNotaryPosition,
                    RegistrationNumber = userCard.GenRegNumOfNotaryInMoJ

                };
            }

            private PowerOfAttorneyEMCHDData.NotaryDeputyInfo CreateNotaryDeputyInfoPart()
            {
                return new PowerOfAttorneyEMCHDData.NotaryDeputyInfo
                {
                    Fio = new PowerOfAttorneyEMCHDData.FIO
                    {
                        FirstName = userCard.GenInterimNotarytName,
                        LastName = userCard.GenInterimNotaryLastName,
                        MiddleName = userCard.GenInterimNotaryMiddleName
                    },
                    Position = userCard.GenPositionOfInterimNotary,
                    RegistrationNumber = userCard.GenRegNumOfIntNotarInMoJ
                };
            }

            private PowerOfAttorneyEMCHDData.HandwrittenSignature CreateHandwrittenSignaturePart()
            {
                return new PowerOfAttorneyEMCHDData.HandwrittenSignature
                {
                    Fio = new PowerOfAttorneyEMCHDData.FIO
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

            private PowerOfAttorneyEMCHDData.SoleExecutiveAuthorityType Convert(GenPowersTypeOfSEBTypes authorityType)
            {
                switch (authorityType)
                {
                    case GenPowersTypeOfSEBTypes.individual:
                        return PowerOfAttorneyEMCHDData.SoleExecutiveAuthorityType.Individual;
                    case GenPowersTypeOfSEBTypes.joint:
                        return PowerOfAttorneyEMCHDData.SoleExecutiveAuthorityType.Joint;
                }
                throw new ArgumentOutOfRangeException(nameof(authorityType));
            }


            private PowerOfAttorneyEMCHDData.PowerOfAttorneyLossOfAuthorityType? Convert(LossPowersSubstTypes? lossPowers)
            {
                if (lossPowers == null)
                    return null;

                switch (lossPowers.Value)
                {
                    case LossPowersSubstTypes.notLost:
                        return PowerOfAttorneyEMCHDData.PowerOfAttorneyLossOfAuthorityType.NotLost;
                    case LossPowersSubstTypes.lost:
                        return PowerOfAttorneyEMCHDData.PowerOfAttorneyLossOfAuthorityType.Lost;
                }

                throw new ArgumentOutOfRangeException(nameof(lossPowers));
            }

            private PowerOfAttorneyEMCHDData.JointRepresentationType Convert(JointExerPowersTypes jointExer)
            {
                switch (jointExer)
                {
                    case JointExerPowersTypes.individual:
                        return PowerOfAttorneyEMCHDData.JointRepresentationType.Individual;
                    case JointExerPowersTypes.joint:
                        return PowerOfAttorneyEMCHDData.JointRepresentationType.Joint;
                }
                throw new ArgumentOutOfRangeException(nameof(jointExer));
            }

            private PowerOfAttorneyEMCHDData.EntityType Convert(GenRepresentativeTypes entity)
            {
                switch (entity)
                {
                    case GenRepresentativeTypes.entity:
                        return PowerOfAttorneyEMCHDData.EntityType.RussianEntity;
                    case GenRepresentativeTypes.soleProprietor:
                        return PowerOfAttorneyEMCHDData.EntityType.SoleProprietor;
                    case GenRepresentativeTypes.individual:
                        return PowerOfAttorneyEMCHDData.EntityType.Individual;
                    case GenRepresentativeTypes.rusEntityBranch:
                        return PowerOfAttorneyEMCHDData.EntityType.BranchOfEntity;
                    case GenRepresentativeTypes.foreignEntBranch:
                        return PowerOfAttorneyEMCHDData.EntityType.BranchOfForeignEntity;
                }
                throw new ArgumentOutOfRangeException(nameof(entity));
            }

            private PowerOfAttorneyEMCHDData.PrincipalType Convert(GenPrincipalTypes entity)
            {
                switch (entity)
                {
                    case GenPrincipalTypes.entity:
                        return PowerOfAttorneyEMCHDData.PrincipalType.RussianEntity;
                    case GenPrincipalTypes.foreignEntity:
                        return PowerOfAttorneyEMCHDData.PrincipalType.ForeignEntity;
                    case GenPrincipalTypes.soleProprietor:
                        return PowerOfAttorneyEMCHDData.PrincipalType.SoleProprietor;
                    case GenPrincipalTypes.individual:
                        return PowerOfAttorneyEMCHDData.PrincipalType.Individual;
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


            private PowerOfAttorneyEMCHDData.PowerOfAttorneyKind Convert(GenPoaKindTypes poaKind)
            {
                switch (poaKind)
                {
                    case GenPoaKindTypes.ordinaryPOA:
                        return PowerOfAttorneyEMCHDData.PowerOfAttorneyKind.Regular;
                    case GenPoaKindTypes.irrecovablePOA:
                        return PowerOfAttorneyEMCHDData.PowerOfAttorneyKind.Irrevocable;
                }
                throw new ArgumentOutOfRangeException(nameof(poaKind));
            }
        }
    }
}
