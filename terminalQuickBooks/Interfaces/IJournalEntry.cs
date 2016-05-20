﻿using System;
using Data.Entities;
using Fr8Data.DataTransferObjects;
using TerminalBase.Infrastructure;

namespace terminalQuickBooks.Interfaces
{
    public interface IJournalEntry
    {
        StandardAccountingTransactionDTO GetAccountingTransactionData(Intuit.Ipp.Data.JournalEntry journalEntry);
        Intuit.Ipp.Data.JournalEntry CreateQbJournalEntry(StandardAccountingTransactionDTO crate);
        void Create(
            StandardAccountingTransactionDTO curStandardAccountingTransactionCM,
            AuthorizationTokenDO authTokenDO,
            string userId,
            IHubCommunicator hubCommunicator);
    }
}