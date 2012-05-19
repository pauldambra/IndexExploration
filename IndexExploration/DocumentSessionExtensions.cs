using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Raven.Client;

namespace IndexExploration
{
    public static class DocumentSessionExtensions
    {
        public static void ClearStaleIndexes(this IDocumentSession documentSession)
        {
            while (documentSession.Advanced.DatabaseCommands.GetStatistics().StaleIndexes.Length != 0)
            {
                Thread.Sleep(10);
            }
        }
    }
}
