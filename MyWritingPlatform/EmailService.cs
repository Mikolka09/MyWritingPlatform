using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyWritingPlatform.ViewModels
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            DownloadPassword dp = new DownloadPassword();
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта \"My Writing Platform\"", dp.DeobfuscateSmtp()[0]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.rambler.ru", 465, true);
                await client.AuthenticateAsync(dp.DeobfuscateSmtp()[0], dp.DeobfuscateSmtp()[1]);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
           
        }
    }
}
