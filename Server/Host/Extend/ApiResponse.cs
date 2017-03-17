using System;

namespace Host
{
    [Serializable]
    public class ApiResponse
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public  string Message { get; set; }
        
        public static ApiResponse SuccessResponse(object data)
        {
            return new ApiResponse
            {
                Success = true,
                Data = data
            };
        }

        public static ApiResponse FailResponse(string message)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message
            };
        }
    }
}
