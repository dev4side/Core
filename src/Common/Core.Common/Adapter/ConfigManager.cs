using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Core.Log;
using Core.Common.Mappers;
using Ninject;

namespace Core.Common.Adapters
{
	public class ConfigManager
	{
        [Inject]
        public ILog<ConfigManager> Log { get; set; }

		private readonly string _configPath;
		private readonly XmlDocument _appConfig = new XmlDocument();
		private readonly XPathNavigator _navigator = null;

		public ConfigManager(string configPath)
		{
            Kernel.ObjectFactory.ResolveDependencies(this);

			if (configPath == null)
				throw new ArgumentNullException("configPath", "path of App.config file cannot be null..");

			_configPath = configPath;

			Log.Info("Started using App.config filename: " + _configPath);

			try
			{
				_appConfig.Load(_configPath);
				_navigator = _appConfig.CreateNavigator();
			}
			catch (FileNotFoundException ex)
			{
				Log.Error(ex, "File " + _configPath + " could not be found");
				throw;
			}
			catch(IOException ex)
			{
				Log.Error(ex, "File " + _configPath + " could not be read or correctly parsed");
				throw;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unknown error while loading " + _configPath + ", see exception details");
				throw;
			}
		}

		public string GetConfigValue(string xpathExpr)
		{
			var node = _navigator.SelectSingleNode(xpathExpr);
			if (node != null)
				return node.Value;

			Log.Error("Failed attempt to read XML node " + xpathExpr + " in file " + _configPath);
			return null;
		}
	
		public void SetConfigValue(string xpathExpr, string value)
		{
			if (value != null)
			{
				var node = _navigator.SelectSingleNode(xpathExpr);
				if (node != null)
				{
					node.SetValue(value);
					Log.Debug("Updated XML node " + xpathExpr + " in file " + _configPath + " with value " + value);
				}
				else
				{
					Log.Error("Failed attempt to write XML node " + xpathExpr + " in file " + _configPath + " with value " + value);
					throw new InvalidOperationException("ConfigManager could not build XML subtrees, just update pre-existing nodes. Node " + xpathExpr + " does not exists in file " + _configPath);
				}
			}
		}

		public void ApplyChanges()
		{
			try
			{
				_appConfig.Save(_configPath);
			}
			catch (IOException ex)
			{
				Log.Error(ex, "IO error writing file " + _configPath);
				throw;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unknown error while writeing " + _configPath + ", see exception details");
				throw;
			}
		}

		public void WriteConfigObject<TConfigObj>(TConfigObj configObj) where TConfigObj : class, new()
		{
			foreach (var prop in typeof(TConfigObj).GetProperties())
			{
				if (prop.PropertyType == typeof(string))
				{
					MapToConfigSection attr = prop.GetCustomAttributes(typeof(MapToConfigSection), false).AsQueryable().FirstOrDefault() as MapToConfigSection;
					if (attr != null)
					{
						var value = prop.GetValue(configObj, null) as string;
						if (value != null)
						{
							this.SetConfigValue(attr.XPathExpr, value);
						}
					}
				}
			}
		}

		public TConfigObj ReadConfigObject<TConfigObj>() where TConfigObj : class, new()
		{
			TConfigObj result = new TConfigObj();

			foreach (var prop in typeof(TConfigObj).GetProperties())
			{
				if (prop.PropertyType == typeof(string))
				{
					MapToConfigSection attr = prop.GetCustomAttributes(typeof(MapToConfigSection), false).AsQueryable().FirstOrDefault() as MapToConfigSection;
					if (attr != null)
					{
						prop.SetValue(result,this.GetConfigValue(attr.XPathExpr),null);
					}
				}
			}
			return result;
		}
	}
}
