using Autofac;
using log4net.Config;
using NLog;
using ScanEventLibrary;
using ScanEventLibrary.ApiClient;
using ScanEventLibrary.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventService
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			XmlConfigurator.Configure();

			//it's been years since i've built a windows service so I've never dependency injected one so this is googled.
			ContainerBuilder containerBuilder = new ContainerBuilder();

			//todo: Ilogger registration

			containerBuilder.RegisterType<Service1>().AsSelf().InstancePerLifetimeScope();

			//todo: convert to fluent style function call
			containerBuilder = DependencyManager.RegisterDependencies(containerBuilder);

			IContainer container = containerBuilder.Build();

			ServiceBase.Run(container.Resolve<Service1>());

		}
	}
}
