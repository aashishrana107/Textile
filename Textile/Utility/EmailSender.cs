using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Textile.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public MailJetSettings _mailJetSettings { get; set; }
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            _mailJetSettings = _configuration.GetSection("Mailjet").Get<MailJetSettings>();
            //MailjetClient client = new MailjetClient("e4395a697d7c2a8669a4e5a96d959d1b", "afa389cee333c9eecc1d2c7a6d8fbe66");
            MailjetClient client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.SecretKey);
            MailjetRequest request = new MailjetRequest
            {
                Resource = SendV31.Resource,
            }
             .Property(Send.Messages, new JArray {
             new JObject {
              {
               "From",
               new JObject {
                {"Email", "aashishrana109@gmail.com"},
                {"Name", "Ashish"}
               }
                  }, {
                   "To",
                       new JArray {
                            new JObject {
                                 {
                                  "Email",
                                  email
                                 }, 
                                {
                                  "Name",
                                  "Textile pvt Ltd."
                                 }
                            }
                       }
                  },
              {
               "Subject",
               subject
              },
              {
               "HTMLPart",
               body
              }
                 //, {
              // "CustomID",
              // "AppGettingStartedTest"
              //}
             }
             });
            //await client.PostAsync(request);
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
            }

        }
    }
}
