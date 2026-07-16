using System;
using System.IO;
using System.Collections.Generic;

public class CPHInline
{
    string CONFIG_PREFIX = "config.";
    int prefixLength;
    string configFile;
    string configDir = "config";
    string configFullPath;
    string[] ignoredConfigs = { "config.loaded", "config.file", "config.dir" };
    int loadedConfigCount = 0;

	public bool Execute()
	{
        bool success = true;
        prefixLength = CONFIG_PREFIX.Length;
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

        bool needWrite = false;
        // Check to see if the Configuration Helper has already be executed
        CPH.TryGetArg("config_loaded", out bool configLoaded);
        // If the config was already loaded and the Configuration Helper is called again, force a write
        needWrite = configLoaded;
        if (!configLoaded && File.Exists(configFullPath))
        {
            LoadConfig();
            // If the count of configs before reading the file is different than the count after, there were probably new configs added in the code.
            // Need to write out the new configs.
            if (loadedConfigCount != ConfigCount())
            {
                CPH.LogInfo("Config::New configs have been addedd to the code. Forcing a write of the config file.");
                needWrite = true;
            }
        }

        if (needWrite)
        {
            WriteConfig();
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

        loadedConfigCount = 0;
        foreach (string line in File.ReadLines(configFullPath))
        {
            trimmedLine = line.Trim();
            if (trimmedLine.Length > 0 && !trimmedLine.StartsWith("#"))
            {
                // The key and value are expected to be separated by "="
                splitIndex = line.IndexOf("=");
                if (splitIndex > 0)
                {
                    // Keys in the config file have "." converted to "_" to conform with Streamer.bot standards
                    key = line.Substring(0, splitIndex).Trim();
                    // If the line is a legacy config entry that starts with "config.", remove it
                    if (key.StartsWith(CONFIG_PREFIX))
                    {
                        key = key.Substring(prefixLength);
                    }
                    key = key.Replace('.', '_');
                    value = line.Substring(splitIndex + 1);
                    CPH.SetArgument($"config_{key}", value);
                    loadedConfigCount++;
                }
            }
        }
        CPH.LogInfo($"Config::Loaded config from {configFile}");
        return true;
    }

    public bool WriteConfig()
    {
        DateTime now = DateTime.Now;
        string timestampString = now.ToString();
        string trimmedKey;
        string hintKey;

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
                trimmedKey = trimmedKey.Substring(prefixLength);
                hintKey = "hint_" + trimmedKey;
                if (CPH.TryGetArg(hintKey, out string hintStr))
                {
                    File.AppendAllText(configFullPath, $"# {hintStr}\n");
                }
                CPH.LogDebug($"Config::{trimmedKey} = {arg.Value}");
                File.AppendAllText(configFullPath, $"{trimmedKey}={arg.Value}\n");
            }
        }
         CPH.LogInfo($"Config::Wrote config to {configFile}");
        return true;
    }

    public int ConfigCount()
    {
        int count = 0;
        foreach (var arg in args)
        {
            if (arg.Key.StartsWith("config_"))
            {
                count++;
            }
        }
        return count;
    }
}