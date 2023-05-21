using Autofac;
using ScanEventLibrary.ApiClient;
using ScanEventLibrary.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventLibrary
{
	public static class DependencyManager
	{
		//todo: change this to fluent style
		public static ContainerBuilder RegisterDependencies(ContainerBuilder builder)
		{
			//todo:  move this into the class that it's located it
			builder.RegisterType<FileDataSource>().As<IDataSource>();
			builder.RegisterType<ScanEventClient>().As<IScanEventClient>();
			builder.Register(t => t.Resolve<IHttpClientFactory>().CreateClient()).As<HttpClient>();

			return builder;
		}
	}
}
