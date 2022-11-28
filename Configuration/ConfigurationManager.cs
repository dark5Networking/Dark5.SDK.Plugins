using System;
using System.IO;
using JetBrains.Annotations;
using Dark5.SDK.Core.Controllers;
using Dark5.SDK.Core.Plugins;

namespace Dark5.SDK.Plugins.Configuration
{
	/// <summary>
	/// Utility helpers for generating and serializing plugin configuration files.
	/// </summary>
	[PublicAPI]
	public static class ConfigurationManager
	{
		/// <summary>
		/// The default name of the NFive definition file.
		/// </summary>
		public const string DefinitionFile = "dark5.yml";

		/// <summary>
		/// The default name of the NFive lock file
		/// </summary>
		public const string LockFile = "dark5.lock";

		/// <summary>
		/// The default name of the FX resource file
		/// </summary>
		public const string ResourceFile = "fxmanifest.lua";

		/// <summary>
		/// The default path to the NFive plugins directory
		/// </summary>
		public const string PluginPath = "plugins";

		/// <summary>
		/// The default path to the NFive configuration directory
		/// </summary>
		public const string ConfigurationPath = "config";

		/// <summary>
		/// Deserializes the specified configuration file into the specified type.
		/// </summary>
		/// <param name="path">The configuration file to deserialize.</param>
		/// <param name="type">The type to deserialize as.</param>
		/// <returns>The deserialized object.</returns>
		public static object Load(string path, Type type)
		{
			path = Path.Combine(ConfigurationPath, path);

			if (!File.Exists(path)) InitializeType(path, type);

			return Yaml.Deserialize(File.ReadAllText(path), type);
		}

		/// <summary>
		/// Deserializes the specified configuration file into the specified type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize as.</typeparam>
		/// <param name="path">The configuration file to deserialize.</param>
		/// <returns>The deserialized object.</returns>
		public static T Load<T>(string path) => (T)Load(path, typeof(T));

		/// <summary>
		/// Deserializes the specified file, owned by the specified plugin, into the specified type.
		/// </summary>
		/// <param name="name">The name of the plugin to look under.</param>
		/// <param name="file">The file to deserialize under the plugin.</param>
		/// <param name="type">The type to deserialize as.</param>
		/// <returns>The deserialized object.</returns>
		public static object Load(Name name, string file, Type type) => Load(Path.Combine(name.Vendor, name.Project, $"{file}.yml"), type);

		/// <summary>
		/// Generates and saves an initial configuration for the specified plugin.
		/// </summary>
		/// <param name="plugin">Name of the plugin to generate configuration for.</param>
		/// <param name="type">The type of the configuration object.</param>
		/// <returns>The default configuration object.</returns>
		public static object InitializeConfig(Name plugin, Type type)
		{
			// Create new instance of type
			var configuration = (IControllerConfiguration)Activator.CreateInstance(type);

			// Generate default configuration if necessary
			// ReSharper disable once InvertIf
			if (!File.Exists(Path.Combine(ConfigurationPath, plugin.Vendor, plugin.Project, $"{configuration.FileName}.yml")))
			{
				// Create configuration directory if necessary
				Directory.CreateDirectory(Path.Combine(ConfigurationPath, plugin.Vendor, plugin.Project));

				InitializeType(Path.Combine(ConfigurationPath, plugin.Vendor, plugin.Project, $"{configuration.FileName}.yml"), type);
			}

			// Load configuration
			return Load(plugin, configuration.FileName, type);
		}

		/// <summary>
		/// Generates and saves an initial configuration for the specified type.
		/// </summary>
		/// <param name="file">The configuration file to deserialize.</param>
		/// <param name="type">The type of the configuration object.</param>
		public static void InitializeType(string file, Type type)
		{
			// Create new instance of type
			var configuration = (IControllerConfiguration)Activator.CreateInstance(type);

			// Write Yaml to file
			File.WriteAllText(file, Yaml.Serialize(configuration));
		}
	}
}
