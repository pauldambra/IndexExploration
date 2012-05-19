using System;
using System.Linq;
using NUnit.Framework;

namespace IndexExploration
{
    [TestFixture]
    public class ListRecipientsByDate : RavenTestBase
    {
        [Test]
        public void ShouldGetEmptyListWhenNoMailingIsPresent()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(_first);
                session.Store(_second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var unrealisticDate = new DateTime(3014, 4, 1);
                var actual = session.Query<PersonMailing, Indexes.RecipientsByDate>()
                                        .ToList();
                Assert.True(!actual.Any());
            }
        }
    }
}
