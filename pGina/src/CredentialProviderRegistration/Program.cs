﻿/*
	Copyright (c) 2011, pGina Team
	All rights reserved.

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions are met:
		* Redistributions of source code must retain the above copyright
		  notice, this list of conditions and the following disclaimer.
		* Redistributions in binary form must reproduce the above copyright
		  notice, this list of conditions and the following disclaimer in the
		  documentation and/or other materials provided with the distribution.
		* Neither the name of the pGina Team nor the names of its contributors 
		  may be used to endorse or promote products derived from this software without 
		  specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
	ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
	DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY
	DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
	(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
	LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
	ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
	(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

using log4net;

namespace pGina.CredentialProvider.Registration
{
    class Program
    {
        public static readonly string EXE_NAME = "pGina.CredentialProvider.Registration.exe";

        static Program()
        {
            // Init logging
            pGina.Shared.Logging.Logging.Init();
        }

        static ILog m_logger = LogManager.GetLogger("Program");

        static int Main(string[] args)
        {
            // Default settings
            Settings settings = new Settings();
            try
            {
                // Parse command line arguments
                settings = ParseClArgs(args);
            }
            catch (Exception e)
            {
                m_logger.ErrorFormat("{0}" + Environment.NewLine + Environment.NewLine + "{1}",
                    e.Message, UsageText());
                return 1;
            }

            // Check path for sanity
            DirectoryInfo pathInfo = new DirectoryInfo(settings.Path);
            if (! pathInfo.Exists )
            {
                m_logger.ErrorFormat("Path {0} doesn't exist or is not a directory.", settings.Path);
                return 1;
            }
               
            // Do the work...
            try
            {
                CredProviderManager manager = CredProviderManager.GetManager();
                manager.CpInfo = settings;
                manager.ExecuteDefaultAction();
            }
            catch (Exception e)
            {
                m_logger.ErrorFormat("Error: {0}" + Environment.NewLine, e);
                return 1;
            }
            
            return 0;
        }

        public static Settings ParseClArgs(string[] args)
        {
            Settings settings = new Settings();

            int nArgs = args.Count();

            // Process options
            int idx = 0;
            while (idx < nArgs)
            {
                // Long form args
                if (args[idx].StartsWith("--"))
                {
                    string opt = args[idx++].Substring(2);
                    string value = null;
                    if (idx < nArgs)
                    {
                        value = args[idx++];
                    }

                    switch (opt)
                    {
                        case "guid":
                            settings.ProviderGuid = new Guid(value);
                            break;
                        case "path":
                            settings.Path = value;
                            break;
                        default:
                            throw new Exception("Unknown option: " + opt);
                    }
                }
                // Short form arguments
                else if (args[idx].StartsWith("-"))
                {
                    string opt = args[idx++].Substring(1);
                    switch (opt)
                    {
                        case "h":
                        case "?":
                            throw new Exception("pGina Registration App Help");
                        default:
                            throw new Exception("Unknown option: " + opt);
                    }
                }
                else
                {
                    break;
                }
            }

            if (idx >= nArgs)
                throw new Exception("Must provide a short name.");

            // There should be at most 2 arguments left
            if (idx >= nArgs - 2 && idx < nArgs)
            {
                if (idx == nArgs - 2)
                {
                    string mode = args[idx++];
                    if (mode.Equals("install", StringComparison.CurrentCultureIgnoreCase))
                        settings.OpMode = OperationMode.INSTALL;
                    else if (mode.Equals("uninstall", StringComparison.CurrentCultureIgnoreCase))
                        settings.OpMode = OperationMode.UNINSTALL;
                    else if (mode.Equals("disable", StringComparison.CurrentCultureIgnoreCase))
                        settings.OpMode = OperationMode.DISABLE;
                    else if (mode.Equals("enable", StringComparison.CurrentCultureIgnoreCase))
                        settings.OpMode = OperationMode.ENABLE;
                    else
                        throw new Exception("Unrecognized operation mode: " + mode);
                }

                settings.ShortName = args[idx++];
            }
            else
            {
                throw new Exception("Must provide the <short_name> argument.");
            }

            return settings;
        }

        public static string UsageText()
        {
            return String.Format(
                "Usage: {0} [options] [Mode] <short_name>" + Environment.NewLine +
                "-------------------------------------------------------------------------------" + Environment.NewLine +
                "  Mode         The operating mode.  One of install, uninstall, enable or " + Environment.NewLine +
                "               disable. " + Environment.NewLine +
                "               Default: INSTALL." + Environment.NewLine +
                "  short_name   The short name of the credential provider DLL." + Environment.NewLine +
                Environment.NewLine +
                "  Options" + Environment.NewLine +
                "     --guid <guid>     The Guid of the credential provider to be installed." + Environment.NewLine +
                "                       Default is the pGina default Guid." + Environment.NewLine +
                "     --path <dir>      Full or relative path to a directory containing the " + Environment.NewLine +
                "                       CP DLL.  The DLL may be located within a" + Environment.NewLine +
                "                       subdirectory named x64 or Win32 (64 or 32 bit)." + Environment.NewLine +
                "                       Defaults to the current directory." + Environment.NewLine +
                "     -h, -?            Show this help text." + Environment.NewLine
                , EXE_NAME);
        }

    }
}
