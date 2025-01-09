namespace ProductsApI.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }



    }
}
