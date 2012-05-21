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

        protected readonly Person First = new Person
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

        protected readonly Person Second = new Person
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

        protected EmbeddableDocumentStore Store;

        [SetUp]
        public void Setup()
        {
            Store = new EmbeddableDocumentStore
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

            Store.Initialize();

            IndexCreation.CreateIndexes(typeof (IndexesHolder.RavenPeopleByMembershipNo).Assembly, Store);
            IndexCreation.CreateIndexes(typeof (IndexesHolder.RecipientsCountByDate).Assembly, Store);
            IndexCreation.CreateIndexes(typeof (IndexesHolder.RecipientsByDate).Assembly, Store);
        }

        [TearDown]
        public void Teardown()
        {
            Store.Dispose();
        }
    }
}