﻿using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace Hub.Interfaces
{
    public interface IFact
    {
        IList<FactDO> GetByObjectId(IUnitOfWork unitOfWork, string id);
        IList<FactDO> GetAll(IUnitOfWork unitOfWork, ICollection<IdentityUserRole> roles = null);
    }
}
