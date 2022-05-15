﻿using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
 public class Edit
 {
  public class Command : IRequest<Result<Unit>>
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
   private readonly IMapper mapper;

   public Handler(DataContext context, IMapper mapper)
   {
    this.context = context;
    this.mapper = mapper;
   }

   public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
   {
    var activity = await this.context.Activities.FindAsync(request.Activity.Id);
    if (activity == null)
    {
     return null;
    }
    this.mapper.Map(request.Activity, activity); // map each property inside request activity to the activity in our database
    var result = await this.context.SaveChangesAsync() > 0;
    if (!result)
    {
     return Result<Unit>.Failure("Failed to update activity");
    }
    return Result<Unit>.Success(Unit.Value);
   }
  }
 }
}
