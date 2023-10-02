using ReadingClub.Domain;
using ReadingClub.Infrastructure.DTO.User;

namespace ReadingClub.Infrastructure.Profile
{
    public class UserProfile : AutoMapper.Profile
    {
        public UserProfile() 
        { 
            CreateMap<User, UserDto>();
            CreateMap<UserLoginDto, User>();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
        }
    }
}
