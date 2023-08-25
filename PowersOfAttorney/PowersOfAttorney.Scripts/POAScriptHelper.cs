using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectModel;
using System;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using DocsVision.BackOffice.WinForms.Controls;
using System.Diagnostics;

namespace PowersOfAttorney.Scripts
{
    public class POAScriptHelper
    {
        private readonly Guid powerOfAttorneyUserCardId;
        public POAScriptHelper(ObjectContext context, Guid powerOfAttorneyUserCardId)
        {
            this.Context = context;
            this.powerOfAttorneyUserCardId = powerOfAttorneyUserCardId;
        }

        private ObjectContext Context { get; }

        private IPowerOfAttorneyService PowerOfAttorneyService => this.Context.GetService<IPowerOfAttorneyService>();

        public void CreateEMCHDPowerOfAttorney()
        {
            Try(() =>
            {
                var userCardPOA = GetUserCard();

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
            });
        }

        public void CreateEMCHDRetrustPowerOfAttorney()
        {
            Try(() =>
            {
                var userCardPOA = GetUserCard();

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
            });
        }

        public void SignPowerOfAttorney()
        {
            Try(() =>
            {
                WithCertificate(cert =>
                {
                   PowerOfAttorney powerOfAttorney = GetPowerOfAttorneyCard();

                   this.PowerOfAttorneyService.SignPowerOfAttorney(powerOfAttorney, cert, PowerOfAttorneySignatureFormat.CADES);
                   this.Context.AcceptChanges();
                });
            });
        }

        public void Export(bool withSignature)
        {
            Try(() =>
            {
                WithFolder(folder =>
                {
                   var powerOfAttorney = GetPowerOfAttorneyCard();
                   this.PowerOfAttorneyService.ExportMachineReadablePowerOfAttorney(powerOfAttorney, folder, withSignature);
                });
            });
        }

        public void MarkAsRevokedPowerOfAttorney(bool withChildrenPowerOfAttorney)
        {
            Try(() =>
            {
                PowerOfAttorney powerOfAttorney = GetPowerOfAttorneyCard();

                this.PowerOfAttorneyService.MarkAsRevoked(powerOfAttorney, withChildrenPowerOfAttorney);
                this.Context.AcceptChanges();
            });
        }

        private UserCardEMCHDPowerOfAttorney GetUserCard()
        {
            return UserCardEMCHDPowerOfAttorney.GetUserCard(this.Context, powerOfAttorneyUserCardId);
        }

        private PowerOfAttorney GetPowerOfAttorneyCard()
        {
            return UserCardEMCHDPowerOfAttorney.GetPowerOfAttorneyCard(this.Context, powerOfAttorneyUserCardId);
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

        private void Try(Action handler)
        {
            try
            {
                handler.Invoke();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        private void ProcessException(Exception ex)
        {
            Trace.WriteLine(ex.ToString());
            MessageBox.Show(ex.ToString());
        }
    }
}
