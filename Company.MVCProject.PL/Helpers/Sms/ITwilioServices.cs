using Twilio.Rest.Api.V2010.Account;

namespace Company.MVCProject.PL.Helpers.Sms
{
    public interface ITwilioServices
    {
        Task<MessageResource> SendSms(Sms sms);
    }
}
