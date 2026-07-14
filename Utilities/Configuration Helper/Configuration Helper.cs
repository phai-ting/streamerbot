using System;
using System.IO;
using System.Collections.Generic;

public class CPHInline
{
    string CONFIG_PREFIX = "config.";
    string configFile;
    string configDir = "config";
    string configFullPath;
    string[] ignoredConfigs = { "config.loaded", "config.file", "config.dir" };

	public bool Execute()
	{
        bool success = true;
        CPH.TryGetArg("actionName", out string actionName);
        // Check for configuration directory override
        if (CPH.TryGetArg("config_dir", out string configDirArg))
        {
            configDir = configDirArg;
        }

        // Check for configuration filename override
        if (!CPH.TryGetArg("config_file", out string configFileArg))
        {
            CPH.LogInfo("Config::No config_file defined. Using the default.");
            configFile = $"{actionName}.config";
        }
        else
        {
            configFile = configFileArg;
        }
        configFullPath = $"{configDir}\\{configFile}";

        // Check to see if the Configuration Helper has already be executed
        CPH.TryGetArg("config_loaded", out bool configLoaded);
        if (configLoaded || !File.Exists(configFullPath))
        {
            // The Configuration Helper was run before or there is no config file created yet so save the values to disk
            WriteConfig();
        }
        else
        {
            LoadConfig();
        }
        // Set up arguments in case this code runs again and make the values visible in the Action history
        CPH.SetArgument("config_loaded", true);
        CPH.SetArgument("config_dir", configDir);
        CPH.SetArgument("config_file", configFile);
        return success;
	}

    public bool LoadConfig()
    {
        string trimmedLine;
        string key;
        string value;
        int splitIndex;
        foreach (string line in File.ReadLines(configFullPath))
        {
            trimmedLine = line.Trim();
            // Only load parameters that start with "config."
            if (trimmedLine.StartsWith(CONFIG_PREFIX))
            {
                // The key and value are expected to be separated by "="
                splitIndex = line.IndexOf("=");
                if (splitIndex > 0)
                {
                    // Keys in the config file have "." converted to "_" to conform with Streamer.bot standards
                    key = line.Substring(0, splitIndex).Trim().Replace('.', '_');
                    value = line.Substring(splitIndex + 1);
                    CPH.SetArgument(key, value);
                }
            }
            CPH.LogInfo($"Config::Loaded config from {configFile}");
        }
        return true;
    }

    public bool WriteConfig()
    {
        DateTime now = DateTime.Now;
        string timestampString = now.ToString();
        string trimmedKey;

        // Create the config directory just in case
        Directory.CreateDirectory(configDir);
        // Write a header line to the config file
        File.WriteAllText(configFullPath, $"## Configuration file - {timestampString} \n");
        // All of the available Streamer.bot variables are exposed as "args"
        foreach (var arg in args)
        {
            // Convert from the Streamer.bot standard of "_" in variable names to "." to follow more of a standard config file convention
            trimmedKey = arg.Key.Trim().Replace('_', '.');
            // Only save configs that are not in the list of configs to ignore
            if (trimmedKey.StartsWith(CONFIG_PREFIX) && Array.IndexOf(ignoredConfigs, trimmedKey) == -1)
            {
                CPH.LogInfo($"Config::{trimmedKey} = {arg.Value}");
                File.AppendAllText(configFullPath, $"{trimmedKey}={arg.Value}\n");
            }
        }
         CPH.LogInfo($"Config::Wrote config to {configFile}");
        return true;
    }
}