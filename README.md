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

This was indeed my problem and not Raven's. Building the index using SelectMany was the issue that was throwing the InvalidCastException.

rewriting the Index creation using the linq query form as [suggested on twitter](https://twitter.com/#!/pauldambra/status/204644424116998144) was the key here:
``` c#
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
```
Above is also an example of fixing the indexing issue.

For this there were two steps removing the class declaration so ```select new PersonMailing {}``` became ```select new``` which alters how the results are stored in Raven.
This also meant that the index had to be on the DateTime object and not on the results of a date call for that object.

Both of which I managed to get to after seeing [this answer on SO](http://stackoverflow.com/a/7566061/222163)