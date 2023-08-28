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


//using CardDocumentМЧДScript = DocsVision.BackOffice.WinForms.ScriptClassBase;
using System.Diagnostics;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.WinForms.Controls;
using System.Security.Cryptography.X509Certificates;

namespace BackOffice
{
    public class CardDocumentДоверенность__версия_EMHCD_1_Script : CardDocumentМЧДScript
    {
        #region Nested Classes
        private class ScriptHelper
        {
            private readonly POAScriptHelper scriptHelper;
            private readonly BaseCardControl cardControl;
      
            public ScriptHelper(BaseCardControl cardControl, POAScriptHelper scriptHelper) 
            {
                this.cardControl = cardControl; 
                this.scriptHelper = scriptHelper;
            }

            public void CreateEMCHDPowerOfAttorney()
            {
                Try(() =>
                {
                    scriptHelper.CreateEMCHDPowerOfAttorney();
                    ChangeState("Create");
                });
            }

            public void CreateEMCHDRetrustPowerOfAttorney()
            {
                Try(() =>
                {
                    scriptHelper.CreateEMCHDRetrustPowerOfAttorney();
                    ChangeState("Create");
                });
            }

            public void MarkAsRevokedPowerOfAttorney(bool withChildrenPowerOfAttorney)
            {
                Try(() =>
                {
                    scriptHelper.MarkAsRevokedPowerOfAttorney(withChildrenPowerOfAttorney);
                    ChangeState("To revoke");
                });
            }

            public void SignPowerOfAttorney()
            {
                Try(() =>
                {
                    WithCertificate(cert =>
                    {
                        scriptHelper.SignPowerOfAttorney(cert);
                        ChangeState("Sign");
                    });
                });
            }

            public void Export(bool withSignature)
            {
                Try(() =>
                {
                    WithFolder(folder =>
                    {
                        scriptHelper.Export(folder, withSignature);
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
                X509Certificate2 certificate =
                    SelectCertificateForm.SelectCertificate(ref cancel, cardControl.ObjectContext, true, out IKeyContainer keyContainer);

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
                cardControl.ObjectContext.GetService<IUIService>().ShowMessage(ex.ToString());
            }

            private void ChangeState(string operationAlias)
            {
                var state = cardControl.BaseObject.SystemInfo.State;
                StatesStateMachineBranch stateBranch = cardControl.AvailableBranches.FirstOrDefault(item =>
                        string.Equals(item.Operation?.DefaultName, operationAlias, StringComparison.OrdinalIgnoreCase) &&
                        (item.BranchType == StatesStateMachineBranchBranchType.Line) &&
                        (item.StartState?.GetObjectId() == state.GetObjectId()));
                if (stateBranch == null)
                {
                    cardControl.ObjectContext.GetService<IUIService>().ShowMessage($"Could not find branch for {operationAlias} operation");
                    return;
                }

                cardControl.ChangeState(stateBranch);
            }
        }
        #endregion
        private ScriptHelper scriptHelper;
        private ScriptHelper POAScriptHelper
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
            POAScriptHelper.CreateEMCHDPowerOfAttorney();
        }

        /// <summary>
        /// Retrust
        /// </summary>
        public virtual void CreateEMCHDRetrustPowerOfAttorney_ItemClick()
        {
            POAScriptHelper.CreateEMCHDRetrustPowerOfAttorney();
        }

        /// <summary>
        /// Sign
        /// </summary>
        public virtual void SignPowerOfAttorney_ItemClick()
        {
            POAScriptHelper.SignPowerOfAttorney();
        }

        /// <summary>
        /// Export with signature
        /// </summary>
        public virtual void ExportWithSignaturePowerOfAttorney_ItemClick()
        {
            POAScriptHelper.Export(withSignature: true);
        }

        /// <summary>
        /// Export without signature
        /// </summary>
        public virtual void ExportWithoutSignaturePowerOfAttorney_ItemClick()
        {
            POAScriptHelper.Export(withSignature: false);
        }

        /// <summary>
        /// Mark as revoked
        /// </summary>
        public virtual void MarkAsRevokedPowerOfAttorney_ItemClick()
        {
            POAScriptHelper.MarkAsRevokedPowerOfAttorney(withChildrenPowerOfAttorney: true);
        } 
    }
}
