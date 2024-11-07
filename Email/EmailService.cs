using System;
using System.Configuration;
using System.Net.Mail;

namespace OnlineForms.Email
{
    public class EmailService
    {
        public static void SendEmail(string fromAddress, string[] toAddress, string[] ccAddress, string subject, string body, bool htmlIndicator)
        {
            if(string.IsNullOrWhiteSpace(fromAddress))
            {
                fromAddress = ConfigurationManager.AppSettings["SystemEmail"].ToString();
            }
            

            string mailServer = ConfigurationManager.AppSettings["SmtpServer"].ToString();
            SmtpClient smtpClient = new SmtpClient(mailServer);
            MailMessage message = new MailMessage()
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = htmlIndicator,
                From = new MailAddress(fromAddress)
            };
            foreach(string address in toAddress)
            {
                try
                {
                    message.To.Add(new MailAddress(address));
                }catch (Exception ex)
                {
                    Exception e = new Exception("Failed to add " + address + " to To field of email.", ex);
                    throw e;
                }
            }
            if (!(ccAddress?.Length == 0 || ccAddress == null))
            {
                foreach (string address in ccAddress)
                {
                    try
                    {
                        message.CC.Add(new MailAddress(address));
                    }
                    catch (Exception ex)
                    {
                        Exception e = new Exception("Failed to add " + address + " to CC field of email.", ex);
                        throw e;
                    }
                }
            }
            try
            {
                smtpClient.Send(message);
            }catch (Exception ex)
            {
                //Exception e = new Exception("Failed to send email message", ex);
                //throw e;
            }
        }
    }
}