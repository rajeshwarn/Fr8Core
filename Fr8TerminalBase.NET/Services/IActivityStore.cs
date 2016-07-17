using System.Collections.Generic;
using Fr8.Infrastructure.Data.DataTransferObjects;
using Fr8.TerminalBase.Interfaces;

namespace Fr8.TerminalBase.Services
{
    public interface IActivityStore
    {
        TerminalDTO Terminal { get; }

        void RegisterActivity(ActivityTemplateDTO activityTemplate, IActivityFactory activityFactory);

        /// <summary>
        /// Registers activity with default Activity Factory
        /// </summary>
        /// <typeparam name="T">Type of activity</typeparam>
        /// <param name="activityTemplate"></param>
        void RegisterActivity<T>(ActivityTemplateDTO activityTemplate) where T : IActivity;

        IActivityFactory GetFactory(ActivityTemplateDTO activityTemplate);
        
        List<ActivityTemplateDTO> GetAllTemplates();
    }
}