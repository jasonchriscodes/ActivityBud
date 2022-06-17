using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
 public class Create
 {
  public class Command : IRequest<Result<CommentDto>>
  {
   public string Body { get; set; }
   public Guid ActivityId { get; set; }
  }

  public class CommanValidator : AbstractValidator<Command>
  {
   public CommanValidator()
   {
    RuleFor(x => x.Body).NotEmpty();
   }
  }

  public class Handler : IRequestHandler<Command, Result<CommentDto>>
  {
   private readonly IUserAccessor userAccessor;
   private readonly IMapper mapper;
   private readonly DataContext context;
   public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
   {
    this.context = context;
    this.mapper = mapper;
    this.userAccessor = userAccessor;
   }

   public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
   {
    var activity = await this.context.Activities.FindAsync(request.ActivityId);

    if (activity == null) return null;

    var user = await this.context.Users
    .Include(p => p.Photos)
    .SingleOrDefaultAsync(u => u.UserName == this.userAccessor.GetUsername());

    var comment = new Comment
    {
     Author = user,
     Activity = activity,
     Body = request.Body,
    };

    activity.Comments.Add(comment);

    var success = await this.context.SaveChangesAsync() > 0;

    if (success) return Result<CommentDto>.Success(this.mapper.Map<CommentDto>(comment));
    
    return Result<CommentDto>.Failure("Failed to add comment");
   }
  }
 }
}