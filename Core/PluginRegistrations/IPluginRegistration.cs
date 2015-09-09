﻿using Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Core.PluginRegistrations
{
    public interface IPluginRegistration
    {
        string BaseUrl { get; set; }

        void RegisterActions();

        string CallPluginRegistrationByString(string typeName, string methodName, ActionDO curActionDO);

        IEnumerable<ActivityTemplateDO> AvailableActions { get; }
		
      //  JObject GetConfigurationSettings();

        string AssembleName(ActivityTemplateDO curActionTemplateDo);
        Task<IEnumerable<string>> GetFieldMappingTargets(ActionDO curAction);
    }
}
