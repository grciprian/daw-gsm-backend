using AutoMapper;
using GSM.Dtos;
using GSM.Models;

namespace GSM
{
    public class MappingProfile : Profile
    {
        public void CreateDualMapping<TSource, TDestination>()
        {
            CreateMap<TSource, TDestination>()
                .ForAllMembers(opt => opt.Condition((source, dest, sourceMember, destMember) =>
                    (sourceMember != null)));
            CreateMap<TDestination, TSource>()
                .ForAllMembers(opt => opt.Condition((source, dest, sourceMember, destMember) =>
                    (sourceMember != null)));
        }
        public MappingProfile()
        {
            CreateDualMapping<ApplicationUser, ApplicationUserDto>();
            CreateDualMapping<Gadget, GadgetDto>();
            CreateDualMapping<Contract, ContractDto>();
            CreateDualMapping<Contract, UpdateContractDto>();
        }
    }
}
