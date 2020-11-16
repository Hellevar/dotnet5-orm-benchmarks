namespace Runner.Models
{
    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int BlogId { get; set; }

        public Blog Blog { get; set; }
    }
}
