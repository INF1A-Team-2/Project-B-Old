using System.Net;
using Newtonsoft.Json;

namespace RestaurantManager;

class DatabaseCredentials
{
    public string Adress;
    public string Token;
}

static class DatabaseConnection
{
    private const string _credentialsFileName = "DatabaseCredentials.json";
    private static readonly DatabaseCredentials _credentials = GetCredentialsFromJson(_credentialsFileName);
    private static readonly HttpClient _httpClient = new HttpClient();

    public static List<List<object>> Execute(string query, params object[] values)
    {
        StringContent body = new StringContent(
            JsonConvert.SerializeObject(new { Query = query, Values = values }),
            System.Text.Encoding.UTF8,
            "application/json");
        
        _httpClient.DefaultRequestHeaders.Add("Token", _credentials.Token);
        
        HttpResponseMessage res = _httpClient.PostAsync(_credentials.Adress, body).Result;
        string responseBody = res.Content.ReadAsStringAsync().Result;

        switch (res.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                throw new Exception("The provided database token is invalid");
            
            case HttpStatusCode.BadRequest:
                throw new Exception("No query was provided");
            
            case HttpStatusCode.InternalServerError:
                throw new Exception($"An internal database error occured: {responseBody}");
        }
        
        return JsonConvert.DeserializeObject<List<List<object>>>(responseBody);
    }

    private static DatabaseCredentials GetCredentialsFromJson(string fileName)
    {
        using (StreamReader stream = new StreamReader(fileName))
        {
            return JsonConvert.DeserializeObject<DatabaseCredentials>(stream.ReadToEnd());
        }
    }
}