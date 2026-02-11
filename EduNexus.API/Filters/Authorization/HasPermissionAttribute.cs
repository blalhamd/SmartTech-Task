using Microsoft.AspNetCore.Authorization;

namespace EduNexus.API.Filters.Authorization
{
    // [Authorize(Policy = "Product:View")] => HasPermission("Product:View")
    public class HasPermissionAttribute(string policy) : AuthorizeAttribute(policy)
    {
       
    }
}


