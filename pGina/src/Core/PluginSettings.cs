/*
	Copyright (c) 2017, pGina Team
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

namespace pGina.Core
{
    public static class PluginSettings
    {
        public static void SetMask(Guid pluginUuid, bool authenticateEnabled, bool authorizeEnabled, bool gatewayEnabled, bool notificationEnabled, bool changePasswordEnabled)
        {
            int mask = 0;
            if (authenticateEnabled)
                mask |= (int)Core.PluginLoader.State.AuthenticateEnabled;
            if (authorizeEnabled)
                mask |= (int)Core.PluginLoader.State.AuthorizeEnabled;
            if (gatewayEnabled)
                mask |= (int)Core.PluginLoader.State.GatewayEnabled;
            if (notificationEnabled)
                mask |= (int)Core.PluginLoader.State.NotificationEnabled;
            if (changePasswordEnabled)
                mask |= (int)Core.PluginLoader.State.ChangePasswordEnabled;

            Core.Settings.Get.SetSetting(pluginUuid.ToString(), mask);
        }

        public static void SavePluginOrder(List<Guid> listOfPluginUuid, Type pluginType)
        {
            string setting = pluginType.Name + "_Order";
            List<string> orderedList = new List<string>();
            foreach (var pluginUuid in listOfPluginUuid)
            {
                orderedList.Add(pluginUuid.ToString());
            }
            Settings.Get.SetSetting(setting, orderedList.ToArray());
        }
    }
}
