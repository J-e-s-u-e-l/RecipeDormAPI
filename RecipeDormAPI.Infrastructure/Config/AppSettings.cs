using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeDormAPI.Infrastructure.Config
{
    public class AppSettings
    {
        public string[] AllowedOrigins { get; set; }
        public string ProcessingError { get; set; }
        public string RegistrationSuccessfully { get; set; }
        public string UserWithEmailNotFound { get; set; }
        public string AccountLocked { get; set; }
        public string SignInSuccessful { get; set; }
        public string ExternalLoginError { get; set; }
        public string HomePage { get; set; }
        public string SpoonacularApiKey { get; set; }
    }
}
