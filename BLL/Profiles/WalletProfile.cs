using AutoMapper;
using DAL.Models;
using BLL.DTOs;

namespace BLL.Profiles
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<User, UserDto>(); 
            CreateMap<Transaction, TransactionDto>(); 
        }
    }
}