using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;

namespace LaserWar.Global
{
	static class AppAttributes
	{
		static readonly Assembly m_Assembly = null;

		static readonly AssemblyTitleAttribute m_Title = null;
		static readonly AssemblyCompanyAttribute m_Company = null;
		static readonly AssemblyCopyrightAttribute m_Copyright = null;
		static readonly AssemblyProductAttribute m_Product = null;
		static readonly Icon m_Icon = null;

		public static string Title { get; private set; }
		public static string CompanyName { get; private set; }
		public static string Copyright { get; private set; }
		public static string ProductName { get; private set; }
		public static Icon AppIcon { get; private set; }

		static Version m_Version = null;
		public static string Version
		{
			get { return m_Version == null ? "" : m_Version.ToString(); }
		}

		static AppAttributes()
		{
			try
			{
				Title = "";
				CompanyName = "";
				Copyright = "";
				ProductName = "";
				m_Version = null;
				AppIcon = null;

				m_Assembly = Assembly.GetEntryAssembly();

				if (m_Assembly != null)
				{
					object[] attributes = m_Assembly.GetCustomAttributes(false);

					foreach (object attribute in attributes)
					{
						Type type = attribute.GetType();

						if (type == typeof(AssemblyTitleAttribute)) m_Title = (AssemblyTitleAttribute)attribute;
						if (type == typeof(AssemblyCompanyAttribute)) m_Company = (AssemblyCompanyAttribute)attribute;
						if (type == typeof(AssemblyCopyrightAttribute)) m_Copyright = (AssemblyCopyrightAttribute)attribute;
						if (type == typeof(AssemblyProductAttribute)) m_Product = (AssemblyProductAttribute)attribute;
					}

					m_Version = m_Assembly.GetName().Version;

					m_Icon = System.Drawing.Icon.ExtractAssociatedIcon(m_Assembly.ManifestModule.FullyQualifiedName);
				}

				if (m_Title != null)
					Title = m_Title.Title;
				if (m_Company != null)
					CompanyName = m_Company.Company;
				if (m_Copyright != null)
					Copyright = m_Copyright.Copyright;
				if (m_Product != null)
					ProductName = m_Product.Product;

				if (m_Icon != null)
					AppIcon = m_Icon;
			}
			catch
			{
			}
		}
	}
}
