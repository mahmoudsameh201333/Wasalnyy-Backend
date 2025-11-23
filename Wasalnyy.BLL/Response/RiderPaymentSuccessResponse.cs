using System;

namespace Wasalnyy.BLL.Response
{
    public class RiderPaymentSuccessResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        


        public RiderPaymentSuccessResponse(bool isSuccess , string message = "")
        {
            IsSuccess = isSuccess;
            Message = message;
           
        }
    }
}
