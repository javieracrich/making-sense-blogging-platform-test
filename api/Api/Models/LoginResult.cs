namespace Api.Models
{
	public class LoginResult
	{
		public string Userid { get; set; }
		public string Username { get; set; }
		public string Token { get; set; }
		public int Expiresin { get; set; }
	}
}
