﻿using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
 public class Edit
 {
  public class Command : IRequest
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

  public class Handler : IRequestHandler<Command>
  {
   private readonly DataContext context;
   private readonly IMapper mapper;

   public Handler(DataContext context, IMapper mapper)
   {
    this.context = context;
    this.mapper = mapper;
   }

   public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
   {
    var activity = await this.context.Activities.FindAsync(request.Activity.Id);

    this.mapper.Map(request.Activity, activity); // map each property inside request activity to the activity in our database
    await this.context.SaveChangesAsync();
    return Unit.Value;
   }
  }
 }
}
