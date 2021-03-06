﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Ddd.Template.Contracts.Commands.Visitor;
using Ddd.Template.Web.Scaffolding;
using Ddd.Template.Web.Scaffolding.Configuration;
using NServiceBus;

namespace Ddd.Template.Web
{
	public class MvcApplication : HttpApplication
	{
		protected static IWindsorContainer Container;

		protected void Application_Start()
		{
			BootstrapContainer();

			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();
		}

		protected void Application_End()
		{
			Container.Dispose();
		}

		private static void BootstrapContainer()
		{
			Container = new WindsorContainer().Install(FromAssembly.This());

			// Let windsor handle all our IoC, things'll just work! :]
			//DependencyResolver.SetResolver(new WindsorDependepcyResolver(_container));
			var controllerFactory = new WindsorControllerFactory(Container.Kernel);
			ControllerBuilder.Current.SetControllerFactory(controllerFactory);
		}

		protected void Session_Start(object sender, EventArgs e)
		{
			var visitor = CreateVisitor(GetHttpRequest());

			var session = GetSession();
			session.Add("VisitorId", visitor.AggregateId);
			session.Add("VisitorVersion", visitor.OriginalVersion);

			var bus = Container.Resolve<IBus>();
			bus.Send(visitor);

			// DebugCreateVisitors();
		}

		/*
		private void DebugCreateVisitors()
		{
			var request = GetHttpRequest();
			var bus = Container.Resolve<IBus>();
			for (var i = 0; i < 10; i++)
			{
				var visitors = new List<AddVisitor>();
				for (var j = 0; j < 100; j++)
					visitors.Add(CreateVisitor(request));
				bus.Send(visitors.ToArray());
			}
		}
		*/

		private AddVisitor CreateVisitor(HttpRequestBase request)
		{
			var userLanguages = request.UserLanguages;

			return new AddVisitor
					{
						AggregateId = Guid.NewGuid(),
						CommandId = Guid.NewGuid(),
						Created = DateTime.UtcNow,
						OriginalVersion = 0,
						UserAgent = request.UserAgent,
						UserHostAddress = request.UserHostAddress,
						UserHostName = request.UserHostName,
						Platform = request.Browser.Platform,
						UserLanguages = userLanguages == null ? new List<string>() : userLanguages.ToList()
					};
		}

		protected virtual HttpSessionStateBase GetSession()
		{
			return new HttpSessionStateWrapper(Session);
		}

		protected virtual HttpRequestBase GetHttpRequest()
		{
			return new HttpRequestWrapper(Request);
		}

		protected virtual HttpApplicationStateBase GetHttpApplication()
		{
			return new HttpApplicationStateWrapper(Application);
		}
	}
}
