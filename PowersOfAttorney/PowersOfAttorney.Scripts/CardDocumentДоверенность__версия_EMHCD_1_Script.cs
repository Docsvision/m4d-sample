
using System;
using System.Windows.Forms;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.ObjectModel;
using DocsVision.BackOffice.WinForms;
using DocsVision.BackOffice.ObjectModel;
using PowersOfAttorney.Scripts;
using DevExpress.XtraBars;
using DocsVision.Platform.WinForms;
using System.Linq;
using System.Windows.Forms.Design;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using CardDocumentМЧДScript = DocsVision.BackOffice.WinForms.ScriptClassBase; // эту строчку надо закомментировать
using System.Diagnostics;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.WinForms.Controls;
using System.Security.Cryptography.X509Certificates;
using System.CodeDom;

namespace BackOffice
{
    
    public class CardDocumentДоверенность__версия_EMHCD_1_Script : CardDocumentМЧДScript // эту строчку надо закомментировать для вида передоверия
  //public class CardDocumentПередоверие__версия_EMHCD_1_Script : CardDocumentМЧДScript  // эту строчку надо раскомментировать для вида передоверия
    {
        #region Nested Classes
        public class POAScriptHelper
        {
            private readonly Guid powerOfAttorneyUserCardId;
            public POAScriptHelper(ObjectContext context, Guid powerOfAttorneyUserCardId)
            {
                this.Context = context;
                this.powerOfAttorneyUserCardId = powerOfAttorneyUserCardId;
            }

            private ObjectContext Context { get; set; }

            private IPowerOfAttorneyService PowerOfAttorneyService { get { return this.Context.GetService<IPowerOfAttorneyService>(); } }

            public void CreateEMCHDPowerOfAttorney()
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
            }

            public void CreateEMCHDRetrustPowerOfAttorney()
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
            }

            public void SignPowerOfAttorney(X509Certificate2 cert)
            {
                PowerOfAttorney powerOfAttorney = GetPowerOfAttorneyCard();

                this.PowerOfAttorneyService.SignPowerOfAttorney(powerOfAttorney, cert, PowerOfAttorneySignatureFormat.CADES);
                this.Context.AcceptChanges();
            }

            public void Export(string folder, bool withSignature)
            {
                var powerOfAttorney = GetPowerOfAttorneyCard();
                this.PowerOfAttorneyService.ExportMachineReadablePowerOfAttorney(powerOfAttorney, folder, withSignature);
            }

            public void MarkAsRevokedPowerOfAttorney(bool withChildrenPowerOfAttorney)
            {
                PowerOfAttorney powerOfAttorney = GetPowerOfAttorneyCard();

                this.PowerOfAttorneyService.MarkAsRevoked(powerOfAttorney, withChildrenPowerOfAttorney);
                this.Context.AcceptChanges();
            }

            private UserCardEMCHDPowerOfAttorney GetUserCard()
            {
                return UserCardEMCHDPowerOfAttorney.GetUserCard(this.Context, powerOfAttorneyUserCardId);
            }

            private PowerOfAttorney GetPowerOfAttorneyCard()
            {
                return UserCardEMCHDPowerOfAttorney.GetPowerOfAttorneyCard(this.Context, powerOfAttorneyUserCardId);
            }
        }
        private class ScriptHelper
        {
            private readonly POAScriptHelper scriptHelper;
            private readonly BaseCardControl cardControl;

            private static class Operations
            {
                public const string CreateOperation = "Create";
                public const string RevokeOperation = "To revoke";
                public const string SignOperation = "Sign";
            }

            public ScriptHelper(BaseCardControl cardControl, POAScriptHelper scriptHelper)
            {
                this.cardControl = cardControl;
                this.scriptHelper = scriptHelper;
            }

            public void CreateEMCHDPowerOfAttorney()
            {
                Try(() =>
                {
                    if (TryGetBranch(Operations.CreateOperation, out var branch))
                    {
                        scriptHelper.CreateEMCHDPowerOfAttorney();
                        ChangeState(branch);
                        ShowMessage("Доверенность сформирована");
                    }
                });
            }

            public void CreateEMCHDRetrustPowerOfAttorney()
            {
                Try(() =>
                {
                    if (TryGetBranch(Operations.CreateOperation, out var branch))
                    {
                        scriptHelper.CreateEMCHDRetrustPowerOfAttorney();
                        ChangeState(branch);
                        ShowMessage("Доверенность сформирована");
                    }
                });
            }

            public void MarkAsRevokedPowerOfAttorney(bool withChildrenPowerOfAttorney)
            {
                Try(() =>
                {
                    if (TryGetBranch(Operations.RevokeOperation, out var branch))
                    {
                        scriptHelper.MarkAsRevokedPowerOfAttorney(withChildrenPowerOfAttorney);
                        ChangeState(branch);
                        ShowMessage("Доверенность отозвана");
                    }
                });
            }

            public void SignPowerOfAttorney()
            {
                Try(() =>
                {
                    if (TryGetBranch(Operations.SignOperation, out var branch))
                    {
                        WithCertificate(cert =>
                        {
                            scriptHelper.SignPowerOfAttorney(cert);
                            ChangeState(branch);
                            ShowMessage("Доверенность подписана");
                        });
                    }
                });
            }

            public void Export(bool withSignature)
            {
                Try(() =>
                {
                    WithFolder(folder =>
                    {
                        scriptHelper.Export(folder, withSignature);
                        ShowMessage("Доверенность экспортирована");
                    });
                });
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
                IKeyContainer keyContainer;
                X509Certificate2 certificate =
                    SelectCertificateForm.SelectCertificate(ref cancel, cardControl.ObjectContext, true, out keyContainer);

                try
                {
                    if (!cancel)
                    {
                        action(certificate);
                    }
                }
                finally
                {
                    if (keyContainer != null)
                        keyContainer.Dispose();
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

            private void ShowMessage(string message)
            {
                cardControl.ObjectContext.GetService<IUIService>().ShowMessage(message);
            }

            private void ProcessException(Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                ShowMessage(ex.ToString());
            }

            private bool TryGetBranch(string operationAlias, out StatesStateMachineBranch stateBranch)
            {
                var state = cardControl.BaseObject.SystemInfo.State;
                stateBranch = cardControl.AvailableBranches.FirstOrDefault(item =>
                        string.Equals(item.Operation.DefaultName, operationAlias, StringComparison.OrdinalIgnoreCase) &&
                        (item.BranchType == StatesStateMachineBranchBranchType.Line) &&
                        (item.StartState.GetObjectId() == state.GetObjectId()));
                if (stateBranch == null)
                {
                    ShowMessage("Не найден переход для операции " + operationAlias + ", состояние " + state.DefaultName);               
                }
                return stateBranch != null;
            }

            private void ChangeState(StatesStateMachineBranch stateBranch)
            {
                cardControl.ChangeState(stateBranch);
            }
        }
        #endregion
        private ScriptHelper scriptHelper;
        private ScriptHelper Script
        {
            get
            {
                if (scriptHelper == null)
                {
                    scriptHelper = new ScriptHelper(this.CardControl, new POAScriptHelper(this.CardControl.ObjectContext, this.CardData.Id));
                }
                return scriptHelper;
            }
        }

        private void CreateEMCHD_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            CreateEMCHDPowerOfAttorney_ItemClick();
        }

        private void CreateEMCHDRetrust_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            CreateEMCHDRetrustPowerOfAttorney_ItemClick();
        }

        private void Sign_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            SignPowerOfAttorney_ItemClick();
        }

        private void ExportWithSignature_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            ExportWithSignaturePowerOfAttorney_ItemClick();
        }

        private void ExportWithoutSignature_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            ExportWithoutSignaturePowerOfAttorney_ItemClick();
        }

        private void MarkAsRevoked_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            MarkAsRevokedPowerOfAttorney_ItemClick();
        }

        /// <summary>
        /// Create
        /// </summary>
        public virtual void CreateEMCHDPowerOfAttorney_ItemClick()
        {
            Script.CreateEMCHDPowerOfAttorney();
        }

        /// <summary>
        /// Retrust
        /// </summary>
        public virtual void CreateEMCHDRetrustPowerOfAttorney_ItemClick()
        {
            Script.CreateEMCHDRetrustPowerOfAttorney();
        }

        /// <summary>
        /// Sign
        /// </summary>
        public virtual void SignPowerOfAttorney_ItemClick()
        {
            Script.SignPowerOfAttorney();
        }

        /// <summary>
        /// Export with signature
        /// </summary>
        public virtual void ExportWithSignaturePowerOfAttorney_ItemClick()
        {
            Script.Export(withSignature: true);
        }

        /// <summary>
        /// Export without signature
        /// </summary>
        public virtual void ExportWithoutSignaturePowerOfAttorney_ItemClick()
        {
            Script.Export(withSignature: false);
        }

        /// <summary>
        /// Mark as revoked
        /// </summary>
        public virtual void MarkAsRevokedPowerOfAttorney_ItemClick()
        {
            Script.MarkAsRevokedPowerOfAttorney(withChildrenPowerOfAttorney: true);
        }
    }
}