using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectModel;
using System;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using DocsVision.BackOffice.WinForms.Controls;

namespace PowersOfAttorney.Scripts
{
    internal class POAScriptHelper
    {
        private readonly Guid powerOfAttorneyUserCardId;
        public POAScriptHelper(ObjectContext context, Guid powerOfAttorneyUserCardId)
        {
            this.Context = context;
            this.powerOfAttorneyUserCardId = powerOfAttorneyUserCardId;
        }

        private ObjectContext Context { get; }

        private IPowerOfAttorneyService PowerOfAttorneyService => this.Context.GetService<IPowerOfAttorneyService>();

        public Guid CreateEMCHDPowerOfAttorney()
        {
            IUserCardPOA userCardPOA = GetUserCardPOA();

            var powerOfAttorneyFormat = this.Context.GetObject<PowersPowerOfAttorneyFormat>(PowerOfAttorneyEMCHDData.FormatId);

            var powerOfAttorney = this.PowerOfAttorneyService.CreatePowerOfAttorney(userCardPOA.PowerOfAttorneyData,
                                                                                    userCardPOA.Representative,
                                                                                    userCardPOA.Signer,
                                                                                    powerOfAttorneyFormat);
            powerOfAttorney.MainInfo.UserCard = userCardPOA.Id;
            powerOfAttorney.MainInfo.PrincipalINN = userCardPOA.PrincipalInn;
            this.Context.SaveObject(powerOfAttorney);

            var powerOfAttorneyId = powerOfAttorney.GetObjectId();

            // Сохраним ИД созданной СКД в ПКД
            userCardPOA.PowerOfAttorneyCardId = powerOfAttorneyId;
            this.Context.AcceptChanges();

            return powerOfAttorneyId;
        }

        public Guid CreateEMCHDRetrustPowerOfAttorney()
        {
            IUserCardPOA userCardPOA = GetUserCardPOA();

            var powerOfAttorney = this.PowerOfAttorneyService.RetrustPowerOfAttorney(userCardPOA.PowerOfAttorneyData,
                                                                                    userCardPOA.Representative,
                                                                                    userCardPOA.Signer,
                                                                                    userCardPOA.ParentalPowerOfAttorney);
            powerOfAttorney.MainInfo.UserCard = userCardPOA.Id;
            powerOfAttorney.MainInfo.PrincipalINN = userCardPOA.PrincipalInn;
            this.Context.SaveObject(powerOfAttorney);

            var powerOfAttorneyId = powerOfAttorney.GetObjectId();

            // Сохраним ИД созданной СКД в ПКД
            userCardPOA.PowerOfAttorneyCardId = powerOfAttorneyId;
            this.Context.AcceptChanges();

            return powerOfAttorneyId;
        }

        public void SignPowerOfAttorney()
        {
            WithCertificate(cert =>
            {
                PowerOfAttorney powerOfAttorney = GetPowerOfAttorneyCard();

                this.PowerOfAttorneyService.SignPowerOfAttorney(powerOfAttorney, cert, PowerOfAttorneySignatureFormat.CADES);
                this.Context.AcceptChanges();
            });
        }

        public void Export(bool withSignature)
        {
            WithFolder(folder =>
            {
                var powerOfAttorney = GetPowerOfAttorneyCard();
                this.PowerOfAttorneyService.ExportMachineReadablePowerOfAttorney(powerOfAttorney, folder, withSignature);
            });
        }

        public void MarkAsRevokedPowerOfAttorney(bool withChildrenPowerOfAttorney)
        {
            PowerOfAttorney powerOfAttorney = GetPowerOfAttorneyCard();

            this.PowerOfAttorneyService.MarkAsRevoked(powerOfAttorney, withChildrenPowerOfAttorney);
            this.Context.AcceptChanges();
        }

        private IUserCardPOA GetUserCardPOA()
        {
            var document = this.Context.GetObject<Document>(powerOfAttorneyUserCardId);

            if (document == null)
            {
                throw new Exception("User card not found " + powerOfAttorneyUserCardId);
            }

            return new UserCardEMCHDPOA(this.Context, document);
        } 

        private PowerOfAttorney GetPowerOfAttorneyCard()
        {
            var powerOfAttorneyId = GetPowerOfAttorneyCardId();
            return this.Context.GetObject<PowerOfAttorney>(powerOfAttorneyId);
        }

        private Guid GetPowerOfAttorneyCardId()
        {
            var userCardPowerOfAttorney = GetUserCardPOA();

            if (userCardPowerOfAttorney.PowerOfAttorneyCardId.GetValueOrDefault() == Guid.Empty)
            {
                throw new Exception("Poa ID not found in user card");
            }

            return userCardPowerOfAttorney.PowerOfAttorneyCardId.Value;
        }

        private void WithFolder(Action<string> action)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {              
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    action(dlg.SelectedPath);
                }
            }
        }

        private void WithCertificate(Action<X509Certificate2> action)
        {
            bool cancel = false;
            X509Certificate2 certificate = 
                SelectCertificateForm.SelectCertificate(ref cancel, this.Context, true, out IKeyContainer keyContainer);

            try
            {
                if (!cancel)
                {
                    action(certificate);
                }
            }
            finally
            {
                keyContainer?.Dispose();
            }
        }
    }
}
