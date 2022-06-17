using System;

namespace Domain
{
 public class Comment
 {
  public int Id { get; set; }
  public string body { get; set; }
  public AppUser Author { get; set; }
  public Activity Activity { get; set; }
  public DateTime CreateAt { get; set; } = DateTime.UtcNow;
 }
}