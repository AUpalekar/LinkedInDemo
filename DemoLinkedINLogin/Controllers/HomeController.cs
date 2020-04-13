using DemoLinkedINLogin.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace DemoLinkedINLogin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult LinkedIN()
        {
            //test comments
            //Need to install below library
            //Install-Package Restsharp
            
            return View();
        }

        public ActionResult LinkedINAuth(string code, string state)
        {
            try
            {
                //Get Accedd Token
                 var client = new RestClient("https://www.linkedin.com/oauth/v2/accessToken");
                var request = new RestRequest(Method.POST);
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("code", code);
                request.AddParameter("redirect_uri", "http://localhost:57633/Home/LinkedINAuth");

                //request.AddParameter("client_id", "client id");
                request.AddParameter("client_id", "7797u563ns17ca");

                //request.AddParameter("client_secret", "client secret");
                request.AddParameter("client_secret", "MfvyVy3PYGxlNqC9");
                IRestResponse response = client.Execute(request);
                var content = response.Content;

                //Fetch AccessToken
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                LinkedINVM linkedINVM = jsonSerializer.Deserialize<LinkedINVM>(content);

                //Get Profile Details
                client = new RestClient("https://api.linkedin.com/v1/people/~?oauth2_access_token=" + linkedINVM.access_token + "&format=json");
                request = new RestRequest(Method.GET);
                response = client.Execute(request);
                content = response.Content;

                jsonSerializer = new JavaScriptSerializer();
                LinkedINResVM linkedINResVM = jsonSerializer.Deserialize<LinkedINResVM>(content);

                //Code for PDF generation

                //GeneratePDF(linkedINResVM);

                //PDF generation code ends
                return View(linkedINResVM);

            }
            catch
            {
                throw;
            }
        }

        public void GeneratePDF(LinkedINResVM linkedINResVM)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10, 10, 10, 10);

                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                string pdfName = linkedINResVM.headline;
                //string path = Server.MapPath("../Resumes") +"//"+ pdfName + ".pdf";
                //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));


                document.Open();

                Chunk chunk = new Chunk("This is from chunk. ");
                document.Add(chunk);

                Phrase phrase = new Phrase("This is from Phrase.");
                document.Add(phrase);

                Paragraph para = new Paragraph("This is from paragraph.");
                document.Add(para);

                string text = linkedINResVM.firstName + "\r\n" + linkedINResVM.lastName + "\r\n";
                Paragraph paragraph = new Paragraph();
                paragraph.SpacingBefore = 10;
                paragraph.SpacingAfter = 10;
                paragraph.Alignment = Element.ALIGN_LEFT;
                paragraph.Font = FontFactory.GetFont(FontFactory.HELVETICA, 12f, BaseColor.GREEN);
                paragraph.Add(text);
                document.Add(paragraph);

                document.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                Response.Clear();
                Response.ContentType = "application/pdf";


                Response.AddHeader("Content-Disposition", "attachment; filename=" + pdfName + ".pdf");
                Response.ContentType = "application/pdf";
                Response.Buffer = true;
                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);



                Response.End();
                Response.Close();
            }
        }
    }
}