So I have a person model (which in the real application is more complex) which is being logged and I want to consume it for reporting.

``` c#
    public class Person
    {
        public string MembershipNumber { get; set; }
        public List<Body> Bodies { get; set; } 
    }
```

Each Body in the list has it's own date

``` c#
	public class Body
	{
		public DateTime MailingDate { get; set; }
		public string BodyText { get; set; }
	}
```

And I want to count and grab people by date.

So I did some googling and stackoverflowing (not a yet an actual verb) and built myself a couple of indexes...

I can count things but I'll be jiggered if I can index and find people by date...

I'm trying this
``` c#
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
```

with this model
``` c#
    public class PersonMailing
    {
        public string MembershipNumber { get; set; }
        public string MailingBody { get; set; }
        public DateTime MailingDate { get; set; }
    }
```

If I try to query by date I'm told it isn't indexed... 

``` c#
	var actual = session.Query<PersonMailing, Indexes.RecipientsByDate>()
						.Where(p => p.MailingDate.Date == TargetDate.Date);
```

if I try to grab the PersonMailing objects I'm told Raven can't cast.

``` c#
	var actual = session.Query<PersonMailing, Indexes.RecipientsByDate>()
							.As<PersonMailing>().ToList();
```

I am sure this is my problem and not Raven's.

This example project expects Raven version 1.0.888
