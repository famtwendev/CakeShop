using AutoMapper;
using CakeShop.Areas.Admin.Data;
using CakeShop.Data;
using CakeShop.ModelsView;
using CakeShop.ModelsView.ForgotPassword;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace CakeShop.Helpers
{
    public class AutoMapperProfile : Profile
    {
       public AutoMapperProfile() {

            CreateMap<RegisterVM, KhachHang>();
            /*                .ForMember(kh => kh.HoTen, option => option.MapFrom(RegisterVM => RegisterVM.HoTen));
                            .ReverseMap();*/
            CreateMap<ThayDoiThongTinVM, KhachHang>();
            CreateMap<AdminLoginVM, NhanVien>()
               .ForMember(nv => nv.MaNv, opt => opt.MapFrom(AdminLoginVM => AdminLoginVM.UserName))
               .ForMember(nv => nv.MatKhau, opt => opt.MapFrom(AdminLoginVM => AdminLoginVM.Password))
               .ReverseMap();
        }
    }
}  
