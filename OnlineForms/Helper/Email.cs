using System;
using System.Configuration;
using System.Net.Mail;
using System.Diagnostics;

namespace OnlineForms.Helper
{
    public class Email
    {
		public static void SendEmail(string[] toAddress, string[] ccAddress, string subject, string body)
		{
			SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"].ToString());
			string fromAddress = ConfigurationManager.AppSettings["FromEmailAddress"];
			MailMessage message = new MailMessage
			{
				Subject = $"[{ConfigurationManager.AppSettings["CurrentEnvironment"]}] {subject}",
				Body = body,
				IsBodyHtml = true,
				From = new MailAddress(fromAddress),
				
			};
			string[] array = toAddress;
			foreach (string address in array)
			{
				try
				{
					message.To.Add(new MailAddress(address));
				}
				catch (Exception ex)
				{
					throw new Exception("Failed to add " + address + " to To field of email.", ex);
				}
			}
			if ((ccAddress == null || ccAddress.Length != 0) && ccAddress != null)
			{
				array = ccAddress;
				foreach (string address2 in array)
				{
					try
					{
						message.CC.Add(new MailAddress(address2));
					}
					catch (Exception ex3)
					{
						throw new Exception("Failed to add " + address2 + " to CC field of email.", ex3);
					}
				}
			}
			try
			{
				smtpClient.Send(message);
			}
			catch (Exception ex2)
			{
				throw new Exception("Failed to send email message", ex2);
			}
		}
	}
}