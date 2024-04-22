using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;

using static PowersOfAttorney.UserCard.Common.Helpers.UserCardPowerOfAttorney;

namespace PowersOfAttorney.UserCard.Common.Helpers
{
    public static class UserCardPowerOfAttorneyEMCHDExtensions
    {
        public static PowerOfAttorneyData ConvertToPowerOfAttorneyEMCHDData(this UserCardPowerOfAttorney userCard, ObjectContext context)
        {
            return Converter.Convert(userCard, context);
        }

        class Converter
        {
            private readonly UserCardPowerOfAttorney userCard;
            private readonly ObjectContext objectContext;

            private Converter(UserCardPowerOfAttorney userCard, ObjectContext objectContext)
            {
                this.userCard = userCard ?? throw new ArgumentNullException(nameof(userCard));
                this.objectContext = objectContext;
            }

            public static PowerOfAttorneyEMCHDData Convert(UserCardPowerOfAttorney userCard, ObjectContext objectContext)
            {
                return new Converter(userCard, objectContext).Convert();
            }

            private PowerOfAttorneyEMCHDData Convert()
            {
                var data = userCard.IsB2BScopeOnly() ? new PowerOfAttorneyEMCHDData(withEsia: false, withNotary: false)
                    : new PowerOfAttorneyEMCHDData(withEsia: false, withNotary: false, withTax: true)
                    {
                        RecipientID = userCard.GenTaxAuthPOASubmit,
                        FinalRecipientID = userCard.GenFinalRecipientTaxID,
                    };

                data.Document = CreateDocumentPart();

                // Если конечный получатель не назначен - берём получаетеля
                if (string.IsNullOrEmpty(data.FinalRecipientID))
                    data.FinalRecipientID = data.RecipientID;

                if (userCard.IsRetrusted())
                {
                    var originalPowerOfAttorneyUserCard = GetOriginalPowerOfAttorneyUserCard();
                    data.SenderID = $"{originalPowerOfAttorneyUserCard.GenEntityPrinINN}{originalPowerOfAttorneyUserCard.GenEntityPrinKPP}";
                }
                else
                    data.SenderID = $"{userCard.GenEntityPrinINN}{userCard.GenEntityPrinKPP}";

                return data;
            }

            private PowerOfAttorneyEMCHDData.PowerOfAttorneyDocument CreateDocumentPart()
            {
                var document = new PowerOfAttorneyEMCHDData.PowerOfAttorneyDocument();

                if (userCard.PoaScope != PoaScopeType.B2B)
                {
                    document.KND = userCard.GenKnd;
                }               

                if (userCard.IsRetrusted())
                    document.RetrustPowerOfAttorneyData = CreatePowerOfAttorneyDataRetrustPart();
                else
                    document.PowerOfAttorneyData = CreatePowerOfAttorneyDataPart();

                return document;
            }

            private PowerOfAttorneyEMCHDData.RetrustPowerOfAttorneyDocumentData CreatePowerOfAttorneyDataRetrustPart()
            {
                var ceo = userCard.GenCeo.GetValueOrThrow(Resources.Error_EmptyCeo);
                var part = new PowerOfAttorneyEMCHDData.RetrustPowerOfAttorneyDocumentData
                {
                    DelegatedAuthority = new PowerOfAttorneyEMCHDData.DelegatedAuthorityPrincipalsInfo
                    {
                        PrincipalType = PowerOfAttorneyEMCHDData.EntityType.Individual,
                        PrincipalInfo = new PowerOfAttorneyEMCHDData.DelegatedPowerOfAttorneyPrincipal
                        {
                            Individual = new PowerOfAttorneyEMCHDData.IndividualPrincipalInfo1
                            {
                                Inn = userCard.GenCeoIIN,
                                Snils = userCard.GenCeoSNILS,
                                IndividualInfo = new PowerOfAttorneyEMCHDData.IndividualInfo
                                {
                                    BirthDate = userCard.GenCeoDateOfBirth,
                                    BirthPlace = userCard.GenCeoPlaceOfBirth,
                                    Citizenship = userCard.GenCeoCitizenship,
                                    CitizenshipType = userCard.IsB2BScopeOnly() ? null : userCard.GenCeoCitizenshipSign,
                                    ContactPhone = userCard.GenCeoPhoneNum,
                                    EMail = userCard.GenCeoEmail,
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
                                    ResidenceAddress = userCard.IsB2BScopeOnly() ? null : new PowerOfAttorneyEMCHDData.AddressInfo
                                    {
                                        Address = userCard.GenCeoAddrRussia,
                                        SubjectOfRussia = userCard.GenCeoAddrSubRussia,
                                        FiasAddress = userCard.GenFiasCEOAddrRussia,
                                        FiasCode = userCard.GenCeoFIASAddrID
                                    }
                                },
                            }
                        }
                    },
                    PrimaryPowerOfAttorney = CreatePrimaryPowerOfAttorneyInfoPart(GetOriginalPowerOfAttorneyUserCard()),
                    PowerOfAttorney = CreatePowerOfAttorneyRetrustPart(),
                    Representative = CreateRepresentativesRetrustPart(),
                    RepresentativePowers = CreateRepresentativePowersRetrustPart()
                };

                // Если доверенность в рамках передоверия второго уровня
                if (userCard.GenParentalPowerOfAttorneyUserCard.HasValue)
                    part.ParentPowerOfAttorney = CreateParentPowerOfAttorneyInfoPart(GetParentalPowerOfAttorneyUserCard());

                return part;
            }

            private PowerOfAttorneyEMCHDData.EntityType Convert(int genDelegatorType)
            {
                return (PowerOfAttorneyEMCHDData.EntityType)(genDelegatorType + 1);
            }            

            private UserCardPowerOfAttorney GetOriginalPowerOfAttorneyUserCard()
            {
                return GetUserCardPowerOfAttorney(userCard.GenOriginalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_UnableToGetOriginalPowerOfAttorneyUserCard));
            }

            private UserCardPowerOfAttorney GetParentalPowerOfAttorneyUserCard()
            {
                return GetUserCardPowerOfAttorney(userCard.GenParentalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_UnableToGetParentalPowerOfAttorneyUserCard));
            }

            private PowerOfAttorneyEMCHDData.RepresentativesInfo CreateRepresentativesRetrustPart()
            {
                return CreateRepresentativesPart();
            }

            private PowerOfAttorneyEMCHDData.PowerOfAttorneyInfo CreatePowerOfAttorneyRetrustPart()
            {
                return CreatePowerOfAttorneyPart();
            }

            private PowerOfAttorneyEMCHDData.RepresentativePowersInfo CreateRepresentativePowersRetrustPart()
            {
                return CreateRepresentativePowersPart();
            }

            private PowerOfAttorneyEMCHDData.PrimaryPowerOfAttorneyInfo CreateParentPowerOfAttorneyInfoPart(UserCardPowerOfAttorney parentUserCardPowerOfAttorney)
            {
                // Это родительская доверенность, а значит у неё есть OriginalPowerOfAttorney
                // Далее заполняем как родительскую, выданную в рамках передоверия
                return new PowerOfAttorneyEMCHDData.PrimaryPowerOfAttorneyInfo
                {
                    PowerOfAttorneyOption = PowerOfAttorneyEMCHDData.PowerOfAttorneyOption.Parent,
                    PowerOfAttorneyForm = PowerOfAttorneyEMCHDData.PowerOfAttorneyForm.Electronic,
                    PowerOfAttorneyStartDate = parentUserCardPowerOfAttorney.GenPoaDateOfIssue ?? throw new ApplicationException(Resources.Error_PoaDateOfIssueIsEmpty),
                    PowerOfAttorneyEndDate = parentUserCardPowerOfAttorney.GenPoaExpirationDate ?? throw new ApplicationException(Resources.Error_EmptyPowerOfAttorneyEndDate),
                    ParentPowerOfAttorneyInternalNumber = parentUserCardPowerOfAttorney.GenDocumentRegNumber,
                    ParentPowerOfAttorneyNumber = parentUserCardPowerOfAttorney.GenSinglePOAregnumber
                };
            }

            private PowerOfAttorneyEMCHDData.PrimaryPowerOfAttorneyInfo CreatePrimaryPowerOfAttorneyInfoPart(UserCardPowerOfAttorney originaUserCardPowerOfAttorney)
            {
                // Это первоначальная доверенность, т.к. пришла из OriginalPowerOfAttorney
                // Далее заполняем как первоначальную
                var data = new PowerOfAttorneyEMCHDData.PrimaryPowerOfAttorneyInfo
                {
                    PowerOfAttorneyOption = PowerOfAttorneyEMCHDData.PowerOfAttorneyOption.Primary,
                    PowerOfAttorneyForm = PowerOfAttorneyEMCHDData.PowerOfAttorneyForm.Electronic,
                    PowerOfAttorneyStartDate = originaUserCardPowerOfAttorney.GenPoaDateOfIssue ?? throw new ApplicationException(Resources.Error_PoaDateOfIssueIsEmpty),
                    PowerOfAttorneyEndDate = originaUserCardPowerOfAttorney.GenPoaExpirationDate ?? throw new ApplicationException(Resources.Error_EmptyPowerOfAttorneyEndDate),
                    PrimaryPowerOfAttorneyInternalNumber = originaUserCardPowerOfAttorney.GenDocumentRegNumber,
                    PrimaryPowerOfAttorneyNumber = originaUserCardPowerOfAttorney.GenSinglePOAregnumber,
                    PrimaryPowerOfAttorneyPrincipals = new List<PowerOfAttorneyEMCHDData.PrimaryPowerOfAttorneyPrincipalInfo> { GetPrimaryPowerOfAttorneyPrincipals(originaUserCardPowerOfAttorney) }
                };

                return data;
            }

            private PowerOfAttorneyEMCHDData.PrimaryPowerOfAttorneyPrincipalInfo GetPrimaryPowerOfAttorneyPrincipals(UserCardPowerOfAttorney originaUserCardPowerOfAttorney)
            {
                var entityPrincipal = originaUserCardPowerOfAttorney.GenEntityPrincipal.GetValueOrThrow(Resources.Error_EmptyPrincipalOrganization);

                return new PowerOfAttorneyEMCHDData.PrimaryPowerOfAttorneyPrincipalInfo
                {
                    // В данном примере первоначальная доверенность может быть выдана только для юрлица
                    PrincipalType = Convert(originaUserCardPowerOfAttorney.GenPrincipalType ?? throw new ApplicationException(Resources.Error_PrincipalTypeIsEmpty)),
                    PrimaryPrincipalInfo = new PowerOfAttorneyEMCHDData.PrimaryPowerOfAttorneyPrincipal
                    {
                        RussianEntity = new PowerOfAttorneyEMCHDData.LegalEntityInfo
                        {
                            ConfirmationOfAuthorityDocument = new PowerOfAttorneyEMCHDData.ConfirmationOfAuthorityDocument
                            {
                                DocumentName = originaUserCardPowerOfAttorney.GenDocConfAuthCEO,
                                IdentityOfDocument = originaUserCardPowerOfAttorney.GenSerNumCEOIDDoc,
                                IssueDate = originaUserCardPowerOfAttorney.GenDateIssCEOIDDoc,
                                Issuer = originaUserCardPowerOfAttorney.GenAuthIssCEOIDDoc
                            },
                            ConstituentDocument = originaUserCardPowerOfAttorney.GenConstDocumentEntPrin,
                            EMail = originaUserCardPowerOfAttorney.GenEmailEntPrin,
                            Inn = originaUserCardPowerOfAttorney.GenEntityPrinINN ?? entityPrincipal.INN.AsNullable(),
                            Kpp = originaUserCardPowerOfAttorney.GenEntityPrinKPP ?? entityPrincipal.KPP.AsNullable(),
                            LegalAddress = new PowerOfAttorneyEMCHDData.AddressInfo
                            {
                                FiasAddress = originaUserCardPowerOfAttorney.GenFiasEntAddrRussia,
                                FiasCode = originaUserCardPowerOfAttorney.GenEntFIASAddrID,
                                Address = originaUserCardPowerOfAttorney.IsB2BScopeOnly() ? null : originaUserCardPowerOfAttorney.GenEntAddrRussia,
                                SubjectOfRussia = originaUserCardPowerOfAttorney.GenEntAddrSubRussia,
                            },
                            Name = entityPrincipal.Name.AsNullable(),
                            Ogrn = originaUserCardPowerOfAttorney.GenEntPrinOGRN ?? entityPrincipal.OGRN.AsNullable(),
                            ParticipantStatus = originaUserCardPowerOfAttorney.GenNotarStatusOfEntPrin,
                            Phone = entityPrincipal.Phone.AsNullable()
                        }
                    }
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

                if (userCard.GenPossibilityOfSubstitution != null &&
                    userCard.GenPossibilityOfSubstitution != GenPossibilityOfSubstitutionTypes.withoutSubstitution
                    && userCard.GenLossPowersTransfer != null)
                    part.LossOfAuthorityType = Convert(userCard.GenLossPowersTransfer);

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
                    Representative = CreateRepresentativePart()
                };
            }

            private PowerOfAttorneyEMCHDData.RepresentativeInfo CreateRepresentativePart()
            {
                var representative = userCard.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual);

                // Представитель-физлицо
                if (userCard.GenRepresentativeType.Value == GenRepresentativeTypes.individual)
                    return new PowerOfAttorneyEMCHDData.RepresentativeInfo
                    {
                        Individual = new PowerOfAttorneyEMCHDData.SoleExecutiveIndividualInfo
                        {
                            Inn = userCard.GenRepresentativeINN,
                            IndividualInfo = CreateRepresentativeIndividualInfoPart(userCard),
                            Position = userCard.GenRepresentativePosition ?? representative.PositionName.AsNullable(),
                            Snils = userCard.GenRepresentativeSNILS
                        }
                    };

                throw new ArgumentOutOfRangeException(nameof(userCard.GenRepresentativeType));
            }

            private PowerOfAttorneyEMCHDData.IndividualInfo CreateRepresentativeIndividualInfoPart(UserCardPowerOfAttorney userCard)
            {
                var representative = userCard.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual);

                return new PowerOfAttorneyEMCHDData.IndividualInfo
                {
                    BirthDate = userCard.GenReprDateOfBirth,
                    BirthPlace = userCard.GenReprPlaceOfBirth,
                    Citizenship = userCard.GenReprCitizenship,
                    CitizenshipType = userCard.IsB2BScopeOnly() ? null : userCard.GenReprCitizenshipSign,
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
                    PrincipalInfo = CreatePrincipalPart()
                };
            }

            private PowerOfAttorneyEMCHDData.PrincipalInfo CreatePrincipalPart()
            {
                if (userCard.GenPrincipalType.Value == GenPrincipalTypes.entity)
                    return new PowerOfAttorneyEMCHDData.PrincipalInfo
                    {
                        RussianEntity = new PowerOfAttorneyEMCHDData.RussianLegalEntityPrincipalInfo
                        {
                            EntityInfo = CreatePrincipalEntityInfoPart(isRussianLegalEntity: true),
                            PrincipalsWithoutPowerOfAttorneyInfo = new List<PowerOfAttorneyEMCHDData.PrincipalWithoutPowerOfAttorneyInfo> { CreatePrincipalsWithoutPowerOfAttorneyInfoPart() },
                            SoleExecutiveIsIndividual = userCard.GenIndSEB == true,
                            SoleExecutiveIsManagementCompany = userCard.GenMngtCompanySEB == true,
                            SoleExecutiveIsSoleProprietor = userCard.GenSolePropSEB == true
                        }
                    };

                throw new ArgumentOutOfRangeException(nameof(userCard.GenPrincipalType));
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
                    IndividualInfo = new PowerOfAttorneyEMCHDData.IndividualInfo
                    {                        
                        BirthPlace = userCard.GenCeoPlaceOfBirth,
                        Citizenship = userCard.GenCeoCitizenship,
                        CitizenshipType = userCard.IsB2BScopeOnly() ? null : userCard.GenCeoCitizenshipSign,
                        ContactPhone = userCard.GenCeoPhoneNum,
                        EMail = userCard.GenCeoEmail,
                        Fio = new PowerOfAttorneyEMCHDData.FIO
                        {
                            FirstName = ceo.FirstName.AsNullable(),
                            LastName = ceo.LastName.AsNullable(),
                            MiddleName = ceo.MiddleName.AsNullable()
                        },
                        Gender = userCard.GenCeoGender,                        
                        ResidenceAddress = userCard.IsB2BScopeOnly() ? null : new PowerOfAttorneyEMCHDData.AddressInfo
                        {
                            Address = userCard.GenCeoAddrRussia,
                            SubjectOfRussia = userCard.GenCeoAddrSubRussia,
                            FiasAddress = userCard.GenFiasCEOAddrRussia,
                            FiasCode = userCard.GenCeoFIASAddrID
                        }
                    },                    
                    ParticipantStatus = userCard.GenNotarStatOfSoleExBody,
                    Position = userCard.GenCeoPosition,
                    Snils = userCard.GenCeoSNILS
                };
            }

            private PowerOfAttorneyEMCHDData.LegalEntityInfo CreatePrincipalEntityInfoPart(bool isRussianLegalEntity = false)
            {
                var entityPrincipal = userCard.GenEntityPrincipal.GetValueOrThrow(Resources.Error_EmptyPrincipalOrganization);
                return new PowerOfAttorneyEMCHDData.LegalEntityInfo
                {
                    ConfirmationOfAuthorityDocument = isRussianLegalEntity ? null : new PowerOfAttorneyEMCHDData.ConfirmationOfAuthorityDocument
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
                        Address = userCard.IsB2BScopeOnly() ? null : userCard.GenEntAddrRussia,
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
                var part = new PowerOfAttorneyEMCHDData.PowerOfAttorneyInfo
                {
                    InformationSystemName = userCard.GenInfsysGenPOA,
                    IrrevocablePowerOfAttorneyInfo = userCard.GenPoaKind == GenPoaKindTypes.irrecovablePOA ? CreateIrrevocablePowerOfAttorneyInfoPart() : null,
                    PowerOfAttorneyAdditionalNumber = userCard.GenAddPOAID,
                    PowerOfAttorneyEndDate = userCard.GenPoaExpirationDate ?? throw new ApplicationException(Resources.Error_EmptyPowerOfAttorneyEndDate),
                    PowerOfAttorneyInternalNumber = userCard.GenDocumentRegNumber,
                    PowerOfAttorneyInternalRegistrationDate = userCard.IsB2BScopeOnly() ? null : userCard.GenPoaInternRegDate,
                    PowerOfAttorneyKind = Convert(userCard.GenPoaKind ?? throw new ApplicationException(Resources.Error_PoaKindIsEmpty)),
                    PowerOfAttorneyNotaryNumber = userCard.GenPoaRegNumNotarRegistry,
                    PowerOfAttorneyNumber = userCard.GenSinglePOAregnumber ?? throw new ApplicationException(Resources.Error_SinglePOAregnumberIsEmpty),
                    PowerOfAttorneyStartDate = userCard.GenPoaDateOfIssue ?? throw new ApplicationException(Resources.Error_PoaDateOfIssueIsEmpty),
                    RetrustType = Convert(userCard.GenPossibilityOfSubstitution ?? throw new ApplicationException(Resources.Error_PossibilityOfSubstitutionIsEmpty))                    
                };

                if (!userCard.IsB2BScopeOnly()) {
                    part.SubmittedPowerOfAttorneyTaxCode = userCard.GenTaxAuthPOASubmit;
                    if (!string.IsNullOrEmpty(userCard.GenTaxAuthPOAValid))
                        part.TaxCode = new List<string> { userCard.GenTaxAuthPOAValid };
                }

                return part;
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

            private UserCardPowerOfAttorney GetUserCardPowerOfAttorney(Document document)
            {
                if (document is null)
                    throw new ArgumentNullException(nameof(document));
                return new UserCardPowerOfAttorney(document, objectContext);
            }
        }
    }
}
