﻿using System;

namespace Data.Interfaces
{
    public interface ITerminalDO : IBaseDO
    {
        int Id { get; set; }
        string Name { get; set; }
        int TerminalStatus { get; set; }
        string Endpoint { get; set; }

        // TODO: remove this, DO-1397
        // bool RequiresAuthentication { get; set; }

        string Description { get; set; }
    }
}