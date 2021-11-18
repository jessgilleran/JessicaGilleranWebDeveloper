using JessicaGilleranWebDeveloper.Models;
using JessicaGilleranWebDeveloper.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JessicaGilleranWebDeveloper.Controllers
{
    public class ContactController : Controller
    {
        public AppKeyConfig AppConfigs { get; }

        public ContactController(IOptions<AppKeyConfig> appkeys)
        {
            AppConfigs = appkeys.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitForm(ContactFormViewModel model)
        {
            if (model.Email == null && model.PhoneNumber == null)
            {
                ModelState.AddModelError(string.Empty, "Either an email or phone number must be entered.");

                return View("Index",model);
            }

            if (ModelState.IsValid)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(AppConfigs.InquiryEmail));
                email.To.Add(MailboxAddress.Parse("jess.gilleran@gmail.com"));
                email.Subject = "Website inquiry";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text =
                    $"<h3>You have a new inquiry!</h3>" +
                    $"<p>From: {model.Name}<p>" +
                    $"<p>Email: {model.Email}<p>" +
                    $"<p>Phone: {model.PhoneNumber}<p>" +
                    $"<p>Message: {model.Message}<p>"
                };

                //send email
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(AppConfigs.InquiryEmail, AppConfigs.InquiryEmailPassword);
                smtp.Send(email);
                smtp.Disconnect(true);

                return View("InquiryConfirmation");
            }
            else
            {
                return View("Index", model);
            }
        }
    }
}
