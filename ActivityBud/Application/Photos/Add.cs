using Application.Core;
using Application.interfaces;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
 public class Add
 {
  public class Command : IRequest<Result<Photo>>
  {
   public IFormFile File { get; set; }
  }

  public class Handler : IRequestHandler<Command, Result<Photo>>
  {
   private readonly IUserAccessor userAccessor;
   private readonly IPhotoAccessor photoAccessor;
   private readonly DataContext context;
   public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
   {
    this.context = context;
    this.photoAccessor = photoAccessor;
    this.userAccessor = userAccessor;
   }

   public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
   {
    var user = await this.context.Users.Include(p => p.Photos)
    .FirstOrDefaultAsync(p => p.UserName == this.userAccessor.GetUsername());

    if (user == null) return null;

    var photoUploadResult = await this.photoAccessor.AddPhoto(request.File);

    var photo = new Photo
    {
     Url = photoUploadResult.Url,
     Id = photoUploadResult.PublicId,
    };

    if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

    user.Photos.Add(photo);

    var result = await this.context.SaveChangesAsync() > 0;

    if (result) return Result<Photo>.Success(photo);

    return Result<Photo>.Failure("Problem adding photo");
   }
  }
 }
}