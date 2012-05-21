using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace IndexExploration
{
    public class IndexesHolder
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
                Map = people => from person in people
                                from body in person.Bodies
                                select new {
                                               person.MembershipNumber,
                                               MailingBody = body.BodyText,
                                               body.MailingDate
                                           };

                Indexes.Add(pm => pm.MailingDate, FieldIndexing.Default);
            }
        }

        public class RecipientsCountByDate : AbstractIndexCreationTask<Person, DateCount>
        {
            public RecipientsCountByDate()
            {
                Map = people => from person in people
                                from body in person.Bodies
                                select new {
                                               RecipientCount = 1,
                                               body.MailingDate
                                           };

                Indexes.Add(dc => dc.MailingDate, FieldIndexing.Default);
                
                Reduce = results => results.GroupBy(result => result.MailingDate)
                                           .Select(g => new DateCount
                                              {
                                                  MailingDate = g.Key, 
                                                  RecipientCount = g.Sum(x => x.RecipientCount)
                                              });
                
            }
        }
    }
}
