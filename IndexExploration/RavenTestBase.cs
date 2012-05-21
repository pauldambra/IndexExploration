using System;
using System.Collections.Generic;
using NUnit.Framework;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace IndexExploration
{
    public class RavenTestBase
    {
        private const string SecondMemNo = "B12345";
        private const string FirstMemNo = "A12345";
        protected static readonly DateTime TargetDate = new DateTime(2011, 4, 1);

        protected readonly Person _first = new Person
                                             {
                                                 Bodies = new List<Body>
                                                              {
                                                                  new Body
                                                                      {
                                                                          BodyText = "the first text",
                                                                          MailingDate = TargetDate
                                                                      }
                                                              },
                                                 MembershipNumber = FirstMemNo
                                             };

        protected readonly Person _second = new Person
                                              {
                                                  Bodies = new List<Body>
                                                               {
                                                                   new Body
                                                                       {
                                                                           BodyText = "the second text",
                                                                           MailingDate = TargetDate
                                                                       }
                                                               },
                                                  MembershipNumber = SecondMemNo
                                              };

        protected EmbeddableDocumentStore _store;

        [SetUp]
        public void Setup()
        {
            _store = new EmbeddableDocumentStore
                         {
                             Configuration =
                                 {
                                     RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                                     RunInMemory = true
                                 },
                             Conventions =
                                 {
                                     FindIdentityProperty = p => (p.DeclaringType == typeof (Person))
                                                                     ? (p.Name == "MembershipNumber")
                                                                     : (p.Name == "Id")
                                 }
                         };

            _store.Initialize();

            IndexCreation.CreateIndexes(typeof (Indexes.RavenPeopleByMembershipNo).Assembly, _store);
            IndexCreation.CreateIndexes(typeof (Indexes.RecipientsCountByDate).Assembly, _store);
            IndexCreation.CreateIndexes(typeof (Indexes.RecipientsByDate).Assembly, _store);
        }

        [TearDown]
        public void Teardown()
        {
            
        }
    }
}