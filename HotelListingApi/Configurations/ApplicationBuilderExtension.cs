using Microsoft.AspNetCore.Builder;

namespace HotelListingApi.Configurations
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder  AddGlobalErrorHandlingMiddleWare(this IApplicationBuilder applicationBuilder) 
            => applicationBuilder.UseMiddleware<GlobalExceptionHandlingMiddleWare>();
    }
}
