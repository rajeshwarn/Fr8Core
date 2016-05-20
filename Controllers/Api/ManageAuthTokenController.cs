﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using StructureMap;
using Data.Interfaces;
using Fr8Data.DataTransferObjects;
using Hub.Infrastructure;
using Hub.Interfaces;
using HubWeb.Infrastructure;

namespace HubWeb.Controllers.Api
{
    [Fr8ApiAuthorize]
    public class ManageAuthTokenController : ApiController
    {
        private readonly IActivityTemplate _activityTemplate;

        public IActivity Activity { get; set; }
        public IAuthorization Authorization { get; set; }
        public ITerminal Terminal { get; set; }
        
        public ManageAuthTokenController()
        {
            _activityTemplate = ObjectFactory.GetInstance<IActivityTemplate>();
            Activity = ObjectFactory.GetInstance<IActivity>();
            Authorization = ObjectFactory.GetInstance<IAuthorization>();
            Terminal = ObjectFactory.GetInstance<ITerminal>();
        }

        /// <summary>
        /// Extract user's auth-tokens and parent terminals.
        /// </summary>
        public IHttpActionResult Get()
        {
            var terminals = Terminal.GetAll();
            var authTokens = Authorization.GetAllTokens(User.Identity.GetUserId());

            var groupedTerminals = terminals
                .Where(x => authTokens.Any(y => y.TerminalID == x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new ManageAuthToken_Terminal
                {
                    Id = x.Id,
                    Name = x.Name,
                    Label = x.Label,
                    AuthTokens = authTokens
                        .Where(y => y.TerminalID == x.Id && !string.IsNullOrEmpty(y.ExternalAccountId))
                        .Select(y => new ManageAuthToken_AuthToken
                        {
                            Id = y.Id,
                            ExternalAccountName = y.DisplayName,
                            IsMain = y.IsMain
                        })
                        .OrderBy(y => y.ExternalAccountName)
                        .ToList()
                })
                .ToList();

            return Ok(groupedTerminals);
        }

        /// <summary>
        /// Revoke token.
        /// </summary>
        [HttpPost]
        public IHttpActionResult Revoke(Guid id)
        {
            var accountId = User.Identity.GetUserId();
            Authorization.RevokeToken(accountId, id);

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult AuthenticateTerminalsByActivities(IEnumerable<Guid> activityIds)
        {
            var result = new List<ManageAuthToken_Terminal_Activity>();

            using (var uow = ObjectFactory.GetInstance<IUnitOfWork>())
            {
                var accountId = User.Identity.GetUserId();
                var authTokens = Authorization.GetAllTokens(accountId).ToArray();

                foreach (Guid activityId in activityIds)
                {
                    var activity =  uow.PlanRepository.GetActivityQueryUncached().FirstOrDefault(x => x.Id == activityId);

                    if (activity == null)
                    {
                        continue;
                    }

                    var template = _activityTemplate.GetByKey(activity.ActivityTemplateId);
                    result.Add(
                        new ManageAuthToken_Terminal_Activity
                        {
                            ActivityId = activityId,
                            Terminal = new ManageAuthToken_Terminal
                            {
                                Id = template.Terminal.Id,
                                Name = template.Terminal.Name,
                                Version = template.Terminal.Version,
                                Label = template.Terminal.Label,
                                AuthenticationType = template.Terminal.AuthenticationType,
                                AuthTokens = authTokens
                                    .Where(x => x.TerminalID == template.Terminal.Id)
                                    .Where(x => !string.IsNullOrEmpty(x.ExternalAccountId))
                                    .Select(x => new ManageAuthToken_AuthToken
                                    {
                                         Id = x.Id,
                                         ExternalAccountName = x.DisplayName,
                                         IsMain = x.IsMain,
                                         IsSelected = (x.Id == activity.AuthorizationTokenId)
                                    })
                                    .OrderBy(x => x.ExternalAccountName)
                                    .ToList()
                            }
                        }
                    );
                }
            }

            result = result.OrderBy(x => x.Terminal.Name).ToList();

            return Ok(result);
        }

        [Fr8HubWebHMACAuthenticate]
        [HttpPost]
        public IHttpActionResult Apply(IEnumerable<ManageAuthToken_Apply> authTokenList)
        {
            var userId = User.Identity.GetUserId();

            foreach (var applyItem in authTokenList)
            {
                Authorization.GrantToken(applyItem.ActivityId, applyItem.AuthTokenId);

                if (applyItem.IsMain)
                {
                    Authorization.SetMainToken(userId, applyItem.AuthTokenId);
                }
            }

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SetDefault(Guid id)
        {
            var userId = User.Identity.GetUserId();
            Authorization.SetMainToken(userId, id);

            return Ok();
        }
    }
}