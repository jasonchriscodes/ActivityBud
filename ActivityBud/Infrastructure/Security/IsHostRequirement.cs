using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
 public class IsHostRequirement : IAuthorizationRequirement
 {
 }

 public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
 {
  private readonly DataContext dbcontext;
  private readonly IHttpContextAccessor httpContextAccessor;
  public IsHostRequirementHandler(DataContext dbcontext, IHttpContextAccessor httpContextAccessor)
  {
   this.httpContextAccessor = httpContextAccessor;
   this.dbcontext = dbcontext;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
  {
   var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier); // query is more efficient if find activity by its PK

   if (userId == null) return Task.CompletedTask; // user not meet the authorization requirement

   var activityId = Guid.Parse(this.httpContextAccessor.HttpContext?.Request.RouteValues
   .SingleOrDefault(x => x.Key == "id").Value?.ToString()); // get the activity id from the resource

   var attendee = this.dbcontext.ActivityAttendees
   .AsNoTracking()
   .SingleOrDefaultAsync(x => x.AppUserId == userId && x.ActivityId == activityId)
   .Result; // get the attendee from the database

   if (attendee == null) return Task.CompletedTask;

   if (attendee.IsHost) context.Succeed(requirement); // if the user is the host, then allow the user to access the resource

   return Task.CompletedTask;
  }
 }
}