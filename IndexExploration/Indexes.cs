using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace IndexExploration
{
    public class Indexes
    {

        public class RavenPeopleByMembershipNo : AbstractIndexCreationTask<Person>
        {
            public RavenPeopleByMembershipNo()
            {
                Map = people => from person in people
                                select new { person.MembershipNumber };
            }
        }

        public class RecipientsByDate : AbstractIndexCreationTask<Person, PersonMailing>
        {
            public RecipientsByDate()
            {
                Map = people =>
                      people.SelectMany(person => person.Bodies,
                                        (person, body) =>
                                        new PersonMailing
                                        {
                                            MembershipNumber = person.MembershipNumber,
                                            MailingBody = body.BodyText,
                                            MailingDate = body.MailingDate
                                        });

                Indexes.Add(pm => pm.MailingDate.Date, FieldIndexing.Default);
            }
        }

        public class RecipientsCountByDate : AbstractIndexCreationTask<Person, DateCount>
        {
            public RecipientsCountByDate()
            {
                Map = people =>
                      people.SelectMany(person => person.Bodies,
                                        (person, body) =>
                                        new DateCount
                                            {
                                                RecipientCount = 1, 
                                                MailingDate = body.MailingDate
                                            });

                Reduce = results => results.GroupBy(result => result.MailingDate)
                                           .Select(g => new DateCount
                                              {
                                                  MailingDate = g.Key, 
                                                  RecipientCount = g.Sum(x => x.RecipientCount)
                                              });
                
                Indexes.Add(pm => pm.MailingDate.Date, FieldIndexing.Default);
            }
        }
    }
}
