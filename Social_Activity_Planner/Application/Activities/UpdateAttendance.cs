using Application.Core;
using Application.interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
 public class UpdateAttendance
 {
  public class Command : IRequest<Result<Unit>>
  {
   public Guid Id { get; set; }
  }

  public class Handler : IRequestHandler<Command, Result<Unit>>
  {
   private readonly DataContext context;
   private readonly IUserAccessor userAccessor;
   public Handler(DataContext context, IUserAccessor userAccessor)
   {
    this.userAccessor = userAccessor;
    this.context = context;
   }

   public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
   {
    var activity = await this.context.Activities
    .Include(x => x.Attendees).ThenInclude(u => u.AppUser)
    .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

    if (activity == null) return null;

    var user = await this.context.Users.FirstOrDefaultAsync(
        u => u.UserName == this.userAccessor.GetUsername());

    if (user == null) return null;

    var hostUsername = activity.Attendees.FirstOrDefault(
        a => a.IsHost)?.AppUser?.UserName;

    var attendance = activity.Attendees.FirstOrDefault(a => a.AppUser.UserName == user.UserName);

    if (attendance != null && hostUsername == user.UserName)
    {
     activity.isCancelled = !activity.isCancelled;
    }
    if (attendance != null && hostUsername != user.UserName)
    {
     activity.Attendees.Remove(attendance);
    }
    if (attendance == null)
    {
     attendance = new ActivityAttendee
     {
      AppUser = user,
      Activity = activity,
      IsHost = false
     };
     activity.Attendees.Add(attendance);
    }

    var result = await this.context.SaveChangesAsync(cancellationToken) > 0;

    return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating attendance");
   }
  }
 }
}