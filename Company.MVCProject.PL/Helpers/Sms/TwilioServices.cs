using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Company.MVCProject.PL.Helpers.Sms
{
    public class TwilioServices(IOptions<TwilioSettings> _options) : ITwilioServices
    {
        public async Task<MessageResource> SendSms(Sms sms)
        {
            // Intialize Connection
            TwilioClient.Init(_options.Value.AccountSID, _options.Value.AuthToken);

            // Build Message
            var message = await MessageResource.CreateAsync
                (
                    body: sms.Body,
                    to: sms.To,
                    from: new Twilio.Types.PhoneNumber(_options.Value.PhoneNumber)
                );
            // Return Message
            return message;
        }
    }
}
