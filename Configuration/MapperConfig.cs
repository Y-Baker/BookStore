using AutoMapper;
using BookStore.DTOs.Author;
using BookStore.DTOs.Book;
using BookStore.DTOs.Category;
using BookStore.DTOs.Order;
using BookStore.DTOs.User;
using BookStore.Models;

namespace BookStore.Configuration;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<AddAdminDTO, Admin>();
        CreateMap<Admin, AdminViewDTO>();

        CreateMap<AddCustomerDTO, Customer>();
        CreateMap<Customer, CustomerViewDTO>();
        CreateMap<EditCustomerDTO, Customer>()
            .ForAllMembers(opt =>
            opt.Condition((src, des, srcMember) => srcMember != null));

        CreateMap<Book, BookViewDTO>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.Name : "Unknown"))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Unknown"))
            .AfterMap((src, dest, context) => dest.PhotoUri = new Uri($"{context.Items["BaseUrl"]}/{src.PhotoId}"));
        CreateMap<AddBookDTO, Book>();
        CreateMap<EditBookDTO, Book>()
            .ForAllMembers(opt =>
            opt.Condition((src, des, srcMember) => srcMember != null));
        
        CreateMap<Author, AuthorViewDTO>();
        CreateMap<AddAuthorDTO, Author>();
        CreateMap<EditAuthorDTO, Author>()
            .ForAllMembers(opt =>
            opt.Condition((src, des, srcMember) => srcMember != null));

        CreateMap<Category, CategoryViewDTO>();
        CreateMap<AddCategoryDTO, Category>();
        CreateMap<EditCategoryDTO, Category>()
            .ForAllMembers(opt =>
            opt.Condition((src, des, srcMember) => srcMember != null));

        CreateMap<Order, OrderViewDTO>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : "Unknown"))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Details.Sum(od => od.Quantity * od.Book!.Price)));
        CreateMap<OrderDetails, OrderDetailViewDTO>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : "Unknown"));       
    }
}
