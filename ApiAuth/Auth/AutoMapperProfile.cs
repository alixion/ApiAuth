using ApiAuth.Auth.Endpoints;
using ApiAuth.Auth.Model;
using ApiAuth.Endpoints;
using AutoMapper;

namespace ApiAuth.Auth;

public class AutoMapperProfile:Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Account, MyAccountResult>();
    }
}