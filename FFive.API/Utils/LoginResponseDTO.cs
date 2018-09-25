namespace FFive.API.Utils
{
    public class LoginResponseDTO
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string user_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public bool is_admin { get; set; }
    }
}