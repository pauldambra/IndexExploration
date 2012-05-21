using System;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Linq;

namespace IndexExploration
{
    [TestFixture]
    public class ListRecipientsByDate : RavenTestBase
    {
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void ShouldGetEmptyListWhenNoMailingIsPresent()
        {
            using (var session = Store.OpenSession())
            {
                session.Store(First);
                session.Store(Second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var unrealisticDate = new DateTime(3014, 4, 1);
                var actual =
                    session.Query<PersonMailing, IndexesHolder.RecipientsByDate>()
                           .Where(pm => pm.MailingDate == unrealisticDate);
                Assert.True(!actual.Any());
            }
        }

        [Test]
        public void ShouldGetEmptyListWhenNoMailingIsPresentInMemoryMethod()
        {
            using (var session = Store.OpenSession())
            {
                session.Store(First);
                session.Store(Second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var unrealisticDate = new DateTime(3014, 4, 1);

                var people = session.Query<Person>().ToList();
                var candidates = from person in people
                                  from body in person.Bodies
                                  where body.MailingDate == unrealisticDate
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
            using (var session = Store.OpenSession())
            {
                session.Store(First);
                session.Store(Second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var people = session.Query<Person>().ToList();
                var candidates = from person in people
                                 from body in person.Bodies
                                 where body.MailingDate == TargetDate
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
            using (var session = Store.OpenSession())
            {
                session.Store(First);
                session.Store(Second);
                session.SaveChanges();
                session.ClearStaleIndexes();

                var actual = session.Query<PersonMailing, IndexesHolder.RecipientsByDate>()
                                    .Where(p => p.MailingDate == TargetDate);
                Assert.IsNotNull(actual);
                Assert.AreEqual(2, actual.Count());
            }

        }
    }
}
