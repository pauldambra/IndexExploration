using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexExploration
{
    public class Person
    {
        public string MembershipNumber { get; set; }
        public List<Body> Bodies { get; set; } 
    }

    public class Body
    {
        public DateTime MailingDate { get; set; }
        public string BodyText { get; set; }
    }

    public class DateCount
    {
        public DateTime MailingDate { get; set; }
        public int RecipientCount { get; set; }
    }

    public class PersonMailing
    {
        public string MembershipNumber { get; set; }
        public string MailingBody { get; set; }
        public DateTime MailingDate { get; set; }
    }
}
