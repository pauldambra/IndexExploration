using System;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Linq;

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
                                        .AsProjection<PersonMailing>().ToList();
                Assert.True(!actual.Any());
            }
        }

        [Test]
        public void ShouldGetEmptyListWhenNoMailingIsPresentInMemoryMethod()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(_first);
                session.Store(_second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var unrealisticDate = new DateTime(3014, 4, 1);

                var candidates = from person in session.Query<Person>().ToList()
                                  from body in person.Bodies
                                  where body.MailingDate.Date == unrealisticDate.Date
                                  select
                                      new PersonMailing
                                          {
                                              MailingBody = body.BodyText,
                                              MailingDate = body.MailingDate,
                                              MembershipNumber = person.MembershipNumber
                                          };

                Assert.True(!candidates.Any());
            }
        }

        [Test]
        public void ShouldGetTwoInListWithExpectedValueInMemoryMethod()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(_first);
                session.Store(_second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var unrealisticDate = new DateTime(3014, 4, 1);

                var candidates = from person in session.Query<Person>().ToList()
                                 from body in person.Bodies
                                 where body.MailingDate.Date == TargetDate.Date
                                 select
                                     new PersonMailing
                                     {
                                         MailingBody = body.BodyText,
                                         MailingDate = body.MailingDate,
                                         MembershipNumber = person.MembershipNumber
                                     };

                Assert.AreEqual(2,candidates.Count());
            }
        }

        [Test]
        public void ShouldGetTwoInListWithExpectedValue()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(_first);
                session.Store(_second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var actual = session.Query<PersonMailing, Indexes.RecipientsByDate>()
                                    .Where(p => p.MailingDate.Date == TargetDate.Date);
                Assert.IsNotNull(actual);
                Assert.AreEqual(2, actual.Count());
            }

        }
    }
}
