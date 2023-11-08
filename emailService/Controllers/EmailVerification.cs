using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Data.SqlClient;
//using Newtonsoft.Json.Linq;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailVerification : Controller
    {

        [HttpGet("VerifyEmail")]
        public /*IActionResult*/async Task<string> VerifyEmail(string email, string code /*[FromBody] string email*/)
        {
            //string email = "zapaspas99@gmail.com";
            //Генерация уникальной ссылки для подтверждения
            string verificationLink = GenerateVerificationLink();

            // Отправка электронного письма с ссылкой для подтверждения
            bool isEmailSent = SendVerificationEmail(email, code);

            if (isEmailSent)
            {
                return "1";
            }
            else
            {
                return "Не удалось отправить письмо с подтверждением";
            }
        }
        [HttpGet("GenerateVerificationLink")]
        private string GenerateVerificationLink()
        {
            // Генерация уникальной ссылки, например, с использованием GUID
            string verificationCode = Guid.NewGuid().ToString();

            // TODO: Сохранение verificationCode в базе данных или хранилище данных

            // Формирование ссылки для подтверждения
            string verificationLink = "https://example.com/verify?verificationCode=" + verificationCode;

            return verificationLink;
        }
        [HttpGet("SendVerificationEmail")]
        private bool SendVerificationEmail(string email, string verificationLink)
        {            
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

                mail.From = new MailAddress("karaskaras3278@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Подтверждение адреса электронной почты";
                mail.Body = "Пожалуйста, введите этот код для подтверждения вашего адреса электронной почты: " + verificationLink;

                smtpClient.Credentials = new NetworkCredential("karaskaras3278@gmail.com", "iqtr jxri jkna puru");
                smtpClient.EnableSsl = true;
                smtpClient.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                // Обработка ошибки отправки электронной почты
                return false;
            }
        }
        [HttpGet("VerifyEmailConfirmation")]
        private bool VerifyEmailConfirmation(string email, string verificationCode)
        {
            // TODO: Проверка, существует ли verificationCode в базе данных или хранилище данных

            // В данном примере сравниваем verificationCode с email, но в реальном приложении используйте более надежный алгоритм проверки
            if (verificationCode == email)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}