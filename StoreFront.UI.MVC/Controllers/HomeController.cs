using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StoreFront.UI.MVC.Models;//gives me access to ContactViewModel
using System.Net;//Network credentials
using System.Net.Mail;// access to most email functionality

namespace StoreFront.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Shop()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Detail()
        {
            return View();
        }

        public ActionResult Contact()//add view...selected type Create with contactViewModel as the model...
        {
            return View();
        }//end Action

        //post Contact Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactViewModel cvm)
        {
            //check to see if the info they gave us matches the model...
            //when a class has validation attributes, that validation should be checked BEFORE attempting to process any data
            if (!ModelState.IsValid)//reverse logic
            {
                //send them back to the form, passing their inputs back to the form with the HTML form
                return View(cvm);//cvm object in this return populates the form with what the user inputed the first time. Shows them what the wrong information was in the field.
            }//end if
            //build the message - what we see when we recieve the email
            string message = $"You have received an email from {cvm.Name} with a subject of {cvm.Subject}. Please respond to {cvm.Email} with your response to the following message: <br/>{cvm.Message}";

            //MailMessage object (what send the email from ASP.NET application - ADD USING STATEMENT FOR SYSTEM.NET.MAIL
            MailMessage mm = new MailMessage(
                //FROM
                "administration@example.com",
                //To - this assumes forwarding from the host
                "sample@mail.com", //hardcoded forward to this email address
                                             //SUBJECT
                cvm.Subject,
                //BODY
                message
                );
            //Allow HTML formatting in the email message
            mm.IsBodyHtml = true;//allows html
            //if you want to mark the emails with high priority
            mm.Priority = MailPriority.High;//the default is normal
            //respond to the sender's email instead of our own SmarterAsp email address
            mm.ReplyToList.Add(cvm.Email);

            //StmpClient - the information from the HOST(SmarterASP.net) that allows the email to actually be sent.
            SmtpClient client = new SmtpClient("mail.example.com");

            //client Credentials
            client.Credentials = new NetworkCredential("administration@example.com", "P@ssw0rd");

            //client.Port = 8889;//sets a different port incase ISP blocks a certain port.
            //it is possible that the mailserver is down or we mayu have configuration issues, so we want to encapsulate our code in a try/catch structure.
            try
            {
                //attempt to send the email
                client.Send(mm);
            }//end try
            catch (Exception ex)//passing through otf what is happing here
            {
                ViewBag.CustomerMessage = $"We're sorry your request could not be sent at this time. Please try again later.<br/>Error Message:<br/>{ex.StackTrace}";
                return View(cvm);//returns the view with the entire message for that user's can copy and paste it for ltr.
            }//end catch
            //if all goes well, we will return the user to a view that confirms their message has been sent.
            return View("EmailConfirmation", cvm);//here's all your stuff that was sent, where it was sent and so on...

        }//end Action
    }
}
