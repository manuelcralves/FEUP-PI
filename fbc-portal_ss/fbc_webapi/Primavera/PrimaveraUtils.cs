using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace fbc_webapi.Primavera
{
    public static class PrimaveraUtils
    {
        public static string FindPrimaveraV10Path()
        {
            string pathPrimavera = "";
            string primaveraPathSource = "";
            string errorsFindingPrimaveraV10 = "";

            try
            {
                if (GetPrimaveraV10PathFromEnvironmentVariable("PERCURSOSGE100", EnvironmentVariableTarget.User, ref errorsFindingPrimaveraV10, ref pathPrimavera, ref primaveraPathSource))
                    return pathPrimavera;

                if (GetPrimaveraV10PathFromEnvironmentVariable("PERCURSOSGE100", EnvironmentVariableTarget.Machine, ref errorsFindingPrimaveraV10, ref pathPrimavera, ref primaveraPathSource))
                    return pathPrimavera;

                if (GetPrimaveraV10PathFromEnvironmentVariable("PERCURSOSGP100", EnvironmentVariableTarget.User, ref errorsFindingPrimaveraV10, ref pathPrimavera, ref primaveraPathSource))
                    return pathPrimavera;

                if (GetPrimaveraV10PathFromEnvironmentVariable("PERCURSOSGP100", EnvironmentVariableTarget.Machine, ref errorsFindingPrimaveraV10, ref pathPrimavera, ref primaveraPathSource))
                    return pathPrimavera;

                if (Environment.Is64BitOperatingSystem)
                {
                    if (GetPrimaveraV10PathFromRegistryKey(RegistryView.Registry64, @"SOFTWARE\PRIMAVERA\SGE100\Default\ERP", ref errorsFindingPrimaveraV10, ref pathPrimavera, ref primaveraPathSource))
                        return pathPrimavera;

                    if (GetPrimaveraV10PathFromRegistryKey(RegistryView.Registry64, @"SOFTWARE\PRIMAVERA\SGP100\Default\ERP", ref errorsFindingPrimaveraV10, ref pathPrimavera, ref primaveraPathSource))
                        return pathPrimavera;
                }

                if (GetPrimaveraV10PathFromRegistryKey(RegistryView.Registry32, @"SOFTWARE\PRIMAVERA\SGE100\Default\ERP", ref errorsFindingPrimaveraV10, ref pathPrimavera, ref primaveraPathSource))
                    return pathPrimavera;

                if (GetPrimaveraV10PathFromRegistryKey(RegistryView.Registry32, @"SOFTWARE\PRIMAVERA\SGP100\Default\ERP", ref errorsFindingPrimaveraV10, ref pathPrimavera, ref primaveraPathSource))
                    return pathPrimavera;

                string testPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PRIMAVERA", "SG100", "Apl");

                if (Directory.Exists(testPath))
                {
                    pathPrimavera = testPath;
                    primaveraPathSource = "Fixed value/default value for 64 bits, as it was not found by other methods";
                    return pathPrimavera;
                }

                testPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "PRIMAVERA", "SG100", "Apl");

                if (Directory.Exists(testPath))
                {
                    pathPrimavera = testPath;
                    primaveraPathSource = "Fixed value/default value for 32 bits, as it was not found by other methods";
                    return pathPrimavera;
                }

                errorsFindingPrimaveraV10 += $"Primavera 10 was not found. Attempts were made to find it through the 'PERCURSOSGE100' and 'PERCURSOSGP100' environment variables, through the 64 bits and 32 bits registry keys for local machine software and through the default directories, but it was not found in any. Please install it first.{Environment.NewLine}";

                return pathPrimavera;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errorsFindingPrimaveraV10))
                    Log.Warning("Errors finding Primavera V10: {@ErrorsFindingPrimaveraV10}", errorsFindingPrimaveraV10);

                if (!string.IsNullOrEmpty(pathPrimavera))
                    Log.Information("Using Primavera V10 path '{@PathPrimavera}' from {@PrimaveraPathSource}", pathPrimavera, primaveraPathSource);
            }
        }

        private static bool GetPrimaveraV10PathFromEnvironmentVariable(string environmentVariableName, EnvironmentVariableTarget environmentVariableTarget, ref string errorsFindingPrimaveraV10, ref string pathPrimavera, ref string primaveraPathSource)
        {
            string envVarTargetName = "";

            if (environmentVariableTarget == EnvironmentVariableTarget.Machine)
                envVarTargetName = "local machine";
            else if (environmentVariableTarget == EnvironmentVariableTarget.User)
                envVarTargetName = "local user";

            string testPath;
            try
            {
                testPath = Environment.GetEnvironmentVariable(environmentVariableName, environmentVariableTarget);
            }
            catch (Exception ex)
            {
                errorsFindingPrimaveraV10 += $"Error getting {envVarTargetName} environment variable '{environmentVariableName}': {ex}{Environment.NewLine}{Environment.NewLine}";
                return false;
            }

            if (string.IsNullOrEmpty(testPath))
                return false;

            if (!Directory.Exists(testPath))
            {
                errorsFindingPrimaveraV10 += $"Environment variable '{environmentVariableName}' exists in {envVarTargetName} but the directory does not exist: '{testPath}'{Environment.NewLine}{Environment.NewLine}";
                return false;
            }

            pathPrimavera = testPath;
            primaveraPathSource = $"Environment variable '{environmentVariableName}' in {envVarTargetName}";

            return true;
        }

        private static bool GetPrimaveraV10PathFromRegistryKey(RegistryView registryView, string registryPath, ref string errorsFindingPrimaveraV10, ref string pathPrimavera, ref string primaveraPathSource)
        {
            string registryViewName = "";

            if (registryView == RegistryView.Registry64)
                registryViewName = "64 bits";
            else if (registryView == RegistryView.Registry32)
                registryViewName = "32 bits";

            try
            {
                using (RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
                {
                    using (RegistryKey registryKey = localMachine.OpenSubKey(registryPath))
                    {
                        if (registryKey == null)
                            return false;

                        string percursoApl = (string)registryKey.GetValue("PERCURSOAPL");

                        if (string.IsNullOrEmpty(percursoApl))
                            return false;

                        if (!Directory.Exists(percursoApl))
                        {
                            errorsFindingPrimaveraV10 += $"Value of 'PERCURSOAPL' exists in {registryViewName} windows registry '{registryKey}' but the directory does not exist: '{percursoApl}'{Environment.NewLine}{Environment.NewLine}";
                            return false;
                        }

                        pathPrimavera = percursoApl;
                        primaveraPathSource = $"Value of 'PERCURSOAPL' in {registryViewName} windows registry '{registryKey}'";

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                errorsFindingPrimaveraV10 += $"Error getting windows registry key value '{registryPath}\\PERCURSOAPL' in {registryViewName} windows registry: {ex}{Environment.NewLine}{Environment.NewLine}";
                return false;
            }
        }
    }
}