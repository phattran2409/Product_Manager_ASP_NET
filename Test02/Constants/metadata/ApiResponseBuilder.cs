

using Test02.Payload.Response;

namespace Test02.Constants.metadata
{
    public class ApiResponseBuilder
    {
        // This method is used to build a response object for single data
        public static ApiResponse<T> BuildResponse<T>(int statusCode, string message, T data, string reason = null)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Message = message,
                Data = data,
                IsSuccess = statusCode >= 200 && statusCode < 300,
                Reason = reason
            };
        }

        // This method is used to build a response object for error response
        public static ApiResponse<T> BuildErrorResponse<T>(T data, int statusCode, string message, string reason)
        {
            return new ApiResponse<T>
            {
                Data = data,
                StatusCode = statusCode,
                Message = message,
                Reason = reason,
                IsSuccess = false
                //StatusCode = statusCode,
                //Message = message,
                //Data = null,
                //IsSuccess = false,
                //Reason = reason
            };
        }



        public static ApiResponse<PagingReponse<T>> BuildPageResponse<T>(
            IEnumerable<T> items,
            int totalPages,
            int currentPage,
            int pageSize,
            long totalItems,
            string message,
            bool hasNext ,
            bool hasPrevious)
        {
            var pagedResponse = new PagingReponse<T>
            {
                Items = items,
                meta = new PaginationMeta
                {
                    TotalPages = totalPages,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    hasNext = hasNext, 
                    hasPrevious = hasPrevious
                }
            };

            return new ApiResponse<PagingReponse<T>>
            {
                Data = pagedResponse,
                Message = message,
                StatusCode = 200,
                IsSuccess = true,
                Reason = null
            };

        }
    }
}
