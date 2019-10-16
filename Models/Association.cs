namespace qwerty.Models
{
    public class Association
    {
        public int AssociationId { get; set; }
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public User User { get; set; }
        public DojoActivity Activity { get; set; }
    }
}