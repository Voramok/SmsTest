namespace SmsTestHttpLib.Models
{
    public sealed class GetMenuResponse<T> : BaseResponse
    {
        public T? Data { get; set; }
    }
}
