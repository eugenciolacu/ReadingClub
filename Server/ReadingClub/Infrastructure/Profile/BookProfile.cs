using ReadingClub.Domain;
using ReadingClub.Domain.Alternative;
using ReadingClub.Infrastructure.DTO.Book;

namespace ReadingClub.Infrastructure.Profile
{
    public class BookProfile : AutoMapper.Profile
    {
        public BookProfile() 
        {
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.Cover, opt => opt.MapFrom(src => src.Cover != null ? Convert.ToBase64String(src.Cover) : null));
            //.ForMember(dest => dest.File, opt => opt.MapFrom(src => Convert.ToBase64String(src.File)));

            CreateMap<BookExtra, BookDto>()
                .ForMember(dest => dest.Cover, opt => opt.MapFrom(src => src.Cover != null ? Convert.ToBase64String(src.Cover) : null));
                //.ForMember(dest => dest.File, opt => opt.MapFrom(src => Convert.ToBase64String(src.File)));

            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Cover, opt => opt.MapFrom(src => ConvertStringToByteArray(src.Cover)))
                .ForMember(dest => dest.CoverMime, opt => opt.MapFrom(src => GetMime(src.Cover)))
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => ConvertStringToByteArray(src.File)));

            CreateMap<UpdateBookDto, Book>()
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Cover, opt => opt.MapFrom(src => ConvertStringToByteArray(src.Cover)))
                .ForMember(dest => dest.CoverMime, opt => opt.MapFrom(src => GetMime(src.Cover)))
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => ConvertStringToByteArray(src.File)));
        }

        private static byte[]? ConvertStringToByteArray(string? value)
        {
            if(value != null)
            {
                string[] parts = value.Split(',');
                if (parts.Length == 2)
                {
                    return Convert.FromBase64String(parts[1]);
                }
            }

            return null;
        }

        private static string? GetMime(string? value)
        {
            if(value != null)
            {
                string[] parts = value.Split(',');
                return parts[0];
            }
            return null;
        }
    }
}
