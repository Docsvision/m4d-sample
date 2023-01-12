using Autofac;

using DocsVision.WebClient.Extensibility;

using PowersOfAttorneyServerExtension.Services;

using System;
using System.Diagnostics;
using System.Reflection;

namespace PowersOfAttorneyServerExtension
{
    /// <summary>
    /// Задаёт описание расширения для WebClient, которое задано в текущей сборке
    /// </summary>
    public class PowersOfAttorneyDemoWebClientExtension : WebClientExtension
    {
        /// <summary>
        /// Создаёт новый экземпляр <see cref="LayoutWebClientExtension" />
        /// </summary>
        /// <param name="serviceProvider">Сервис-провайдер</param>
        public PowersOfAttorneyDemoWebClientExtension(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Получить название расширения
        /// </summary>
        public override string ExtensionName
        {
            get { return Assembly.GetAssembly(typeof(PowersOfAttorneyDemoWebClientExtension)).GetName().Name; }
        }

        /// <summary>
        /// Получить версию расширения
        /// </summary>
        public override Version ExtensionVersion
        {
            get { return new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion); }
        }

        #region WebClientExtension Overrides

        public override void InitializeContainer(global::Autofac.ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<PowersOfAttorneyDemoService>().As<IPowersOfAttorneyDemoService>().SingleInstance();
        }

        #endregion
    }
}