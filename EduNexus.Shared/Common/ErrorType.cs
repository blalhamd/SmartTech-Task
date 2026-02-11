namespace EduNexus.Shared
{
    public enum ErrorType
    {
        None = 0, 
        Validation = 1,           // 400 Bad Request
        NotFound = 2,             // 404 Not Found
        Conflict = 3,             // 409 Already exist
        UnAuthorized = 4,             // 401 UnAuthorized
        InternalServerError = 5,  // 500 Serrver error
    }
}
