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
            using (var session = Store.OpenSession())
            {
                session.Store(First);
                session.Store(Second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var unrealisticDate = new DateTime(3014, 4, 1);
                var actual = session.Query<DateCount, IndexesHolder.RecipientsCountByDate>()
                                     .Where(dc => dc.MailingDate == unrealisticDate);
                Assert.True(!actual.Any());   
            }
        }

        [Test]
        public void ShouldGetTwoWithExpectedValue()
        {
            using (var session = Store.OpenSession())
            {
                session.Store(First);
                session.Store(Second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var results = session.Query<DateCount, IndexesHolder.RecipientsCountByDate>()
                                     .Where(dc => dc.MailingDate == TargetDate)
                                     .ToList();
                Assert.AreEqual(1, results.Count);
                var actual = results.First();
                Assert.IsNotNull(actual);
                Assert.AreEqual(2, actual.RecipientCount);
            }

        }

        [Test]
        public void ShouldGetZeroWhenNoMailingIsPresentInMemoryMethod()
        {
            using (var session = Store.OpenSession())
            {
                session.Store(First);
                session.Store(Second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var unrealisticDate = new DateTime(3014, 4, 1);
                var actual = session.Query<DateCount, IndexesHolder.RecipientsCountByDate>()
                                    .ToList()
                                    .Where(dc => dc.MailingDate == unrealisticDate);
                Assert.True(!actual.Any());
            }
        }

        [Test]
        public void ShouldGetTwoWithExpectedValueInMemoryMethod()
        {
            using (var session = Store.OpenSession())
            {
                session.Store(First);
                session.Store(Second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var results = session.Query<DateCount, IndexesHolder.RecipientsCountByDate>()
                                            .ToList();
                Assert.AreEqual(1, results.Count);
                DateCount actual = null;
                try
                {
                    actual = results.Single(dc => dc.MailingDate == TargetDate);
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
