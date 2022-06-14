using Application.Core;
using Application.interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
 public class Create
 {
  public class Command : IRequest<Result<Unit>> // command did not return anything
  {
   public Activity Activity { get; set; }
  }

  public class CommandValidator : AbstractValidator<Command>
  {
   public CommandValidator()
   {
    RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
   }
  }
  public class Handler : IRequestHandler<Command, Result<Unit>>
  {
   private readonly DataContext context;
   private readonly IUserAccessor userAccessor;

   public Handler(DataContext context, IUserAccessor userAccessor) // inject data context
   {
    this.userAccessor = userAccessor;
    this.context = context;
   }
   /// <summary>
   /// 
   /// </summary>
   /// <param name="request"></param>
   /// <param name="cancellationToken"></param>
   /// <returns>value of unit, letting API know we finish this method</returns>
   public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
   {
    var user = await this.context.Users.FirstOrDefaultAsync(x => x.UserName == this.userAccessor.GetUsername());

    var attendee = new ActivityAttendee
    {
     AppUser = user,
     Activity = request.Activity,
     IsHost = true
    };

    request.Activity.Attendees.Add(attendee);

    this.context.Activities.Add(request.Activity); // adding the activity in memory, not database so does not need asynchronous version

    var result = await this.context.SaveChangesAsync() > 0; // if save changes is successful, result will be true

    if (!result) return Result<Unit>.Failure("Failed to create activity");

    return Result<Unit>.Success(Unit.Value);
   }
  }
 }
}
