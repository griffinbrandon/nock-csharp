using System;

namespace Nock.CSharp.Test
{
    public class Person
    {
        public int Id { get; set; } = (new Random()).Next();
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}