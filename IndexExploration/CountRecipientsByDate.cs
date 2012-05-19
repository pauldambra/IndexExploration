using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace IndexExploration
{
    [TestFixture]
    public class CountRecipientsByDate : RavenTestBase
    {
        [Test]
        public void ShouldGetZeroWhenNoMailingIsPresent()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(_first);
                session.Store(_second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var unrealisticDate = new DateTime(3014, 4, 1);
                var actual = session.Query<DateCount, Indexes.RecipientsCountByDate>()
                                     .Where(dc => dc.MailingDate.Date == unrealisticDate);
                Assert.True(!actual.Any());   
            }
        }

        [Test]
        public void ShouldGetTwoWithExpectedValue()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(_first);
                session.Store(_second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var results = session.Query<DateCount, Indexes.RecipientsCountByDate>()
                                            .ToList();
                Assert.AreEqual(1, results.Count);
                DateCount actual = null;
                try
                {
                    actual = results.Single(dc => dc.MailingDate.Date == TargetDate);
                }
                catch (Exception ex)
                {
                    Assert.Fail("this definitely shouldn't throw: " + ex);
                }
                Assert.IsNotNull(actual);
                Assert.AreEqual(2, actual.RecipientCount); 
            }
            
        }
    }
}
