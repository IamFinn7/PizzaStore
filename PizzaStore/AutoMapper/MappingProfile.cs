namespace PizzaStore.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Products, ProductVM>().ReverseMap();
        }
    }
}
