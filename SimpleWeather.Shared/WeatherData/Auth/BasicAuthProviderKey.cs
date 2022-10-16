using SimpleWeather.Utils;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SimpleWeather.WeatherData.Auth
{
    public class BasicAuthProviderKey : ProviderKey
    {
        [DataMember(Name = "username")]
        public string UserName { get; set; } = string.Empty;
        [DataMember(Name = "password")]
        public string Password { get; set; } = string.Empty;

        public BasicAuthProviderKey() : base() { }
        public BasicAuthProviderKey(string username, string password) : base()
        {
            this.UserName = username;
            this.Password = password;
        }

        public override void FromString(string input)
        {
            input?.ReplaceFirst("Basic", "")?.Let((auth) =>
            {
                try
                {
                    var bytes = Convert.FromBase64String(auth);
                    Encoding.GetEncoding("ISO-8859-1").GetString(bytes)?.Let(it =>
                    {
                        it.Split(':').Let(split =>
                        {
                            UserName = split.First();
                            Password = split.Last();
                        });
                    });
                }
                catch { }
            });
        }

        public override string ToString()
        {
            var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{UserName}:{Password}"));
            return $"Basic {encoded}";
        }
    }
}
