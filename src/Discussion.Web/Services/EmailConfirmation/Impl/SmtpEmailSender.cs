﻿using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Discussion.Web.Services.EmailConfirmation.Impl
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSendingOptions _emailSendingOptions;
        public SmtpEmailSender(IOptions<EmailSendingOptions> emailSendingOptions)
        {
            _emailSendingOptions = emailSendingOptions.Value;
        }
        
        public async Task SendEmailAsync(string emailTo, string subject, string message)
        {
            var mimeMessage = new MimeMessage ();
            mimeMessage.From.Add (new MailboxAddress ("dotnet club", _emailSendingOptions.MailFrom));
            mimeMessage.To.Add (new MailboxAddress (emailTo));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart (TextFormat.Html) { Text = message };


            using (var client = new SmtpClient())
            {
                client.Connect(_emailSendingOptions.ServerHost, _emailSendingOptions.ServerSslPort, useSsl: true);
                if (!string.IsNullOrEmpty(_emailSendingOptions.LoginName)
                    && !string.IsNullOrEmpty(_emailSendingOptions.Password))
                {
                    client.Authenticate(_emailSendingOptions.LoginName, _emailSendingOptions.Password);
                }

                await client.SendAsync(mimeMessage);
                client.Disconnect(true);
            }
        }
    }
}
