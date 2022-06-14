namespace Domain
{
 public class Activity
 {
  public Guid Id { get; set; }
  public string Title { get; set; }
  public DateTime Date { get; set; }
  public string Description { get; set; }
  public string Category { get; set; }
  public string City { get; set; }
  public string Venue { get; set; }
  public bool IsCancelled { get; set; }
  public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>();//initialize attendee inside the entity, so we don't get a null reference when try and add something in collection (get empty array back to attendee instead of null)
 }
}
