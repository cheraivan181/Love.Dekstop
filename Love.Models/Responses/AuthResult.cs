using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Love.Models.Responses
{
    public class AuthResult
    {
		[JsonProperty("userName")]
		public string UserName { get; set; }
		[JsonProperty("roles")]
		public List<string> Roles { get; set; }
		[JsonProperty("accessToken")]
		public string AccessToken { get; set; }
		[JsonProperty("refreshToken")]
		public string RefreshToken { get; set; }
		[JsonProperty("phoneToken")]
		public string PhoneToken { get; set; }
		[JsonProperty("userId")]
		public string UserId { get; set; }
	}
}
