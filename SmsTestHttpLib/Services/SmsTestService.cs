using Newtonsoft.Json;
using SmsTestHttpLib.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SmsTestHttpLib.Services
{
    public class SmsTestService
    {
        private readonly HttpClient _httpClient;

        public SmsTestService(string baseAddress, string username, string password)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task<GetMenuResponse<List<MenuItem>>> GetMenuAsync(bool commandParam)
        {
            var command = "GetMenu";

            var response = new GetMenuResponse<List<MenuItem>>() 
            { 
                Command = command,
                Success = false
            };

            var requestBody = new
            {
                Command = command,
                CommandParameters = new
                {
                    WithPrice = commandParam
                }
            };

            using (_httpClient)
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                try
                {
                    var httpResponse = await _httpClient.PostAsync("", content);

                    var responseBody = await httpResponse.Content.ReadAsStringAsync();

                    var tempRespose = JsonConvert.DeserializeObject<GetMenuResponse<string>>(responseBody);

                    if (tempRespose == null)
                    {
                        response.ErrorMessage = "Error parsing response";
                        return response;
                    }

                    if (string.IsNullOrEmpty(tempRespose.Data))
                    {
                        response.ErrorMessage = "Data is null";
                        return response;
                    }

                    if (tempRespose.Success == false || !string.IsNullOrEmpty(tempRespose.ErrorMessage))
                    {
                        response.ErrorMessage = tempRespose.ErrorMessage;
                        return response;
                    }

                    response.Data = JsonConvert.DeserializeObject<List<MenuItem>>(tempRespose.Data);
                    response.Success = true;

                    return response;
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    return response;
                }
            }
        }

        public async Task<BaseResponse> PostOrderAsync(Order order)
        {
            var command = "SendOrder";
            var response = new BaseResponse()
            {
                Command = command,
                Success = false
            };

            var requestBody = new
            {
                response.Command,
                CommandParameters = new
                {
                    order
                }
            };

            using (_httpClient)
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var httpResponse = await _httpClient.PostAsync("", content);

                    var responseBody = await httpResponse.Content.ReadAsStringAsync();

                    var tempRespose = JsonConvert.DeserializeObject<BaseResponse>(responseBody);

                    if (tempRespose == null)
                    {
                        response.ErrorMessage = "Error parsing response";
                        return response;
                    }

                    if (tempRespose.Success == false || !string.IsNullOrEmpty(tempRespose.ErrorMessage))
                    {
                        response.ErrorMessage = tempRespose.ErrorMessage;
                        return response;
                    }

                    response.Success = true;

                    return response;
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.Message;
                    return response;
                }
            }
        }
    }
}
