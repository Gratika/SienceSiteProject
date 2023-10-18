using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Data.SqlClient;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailVerification : Controller
    {

        [HttpGet("VerifyEmail")]
        public IActionResult VerifyEmail(/*string email*/)
        {
            string email = "zapaspas99@gmail.com";
            // Генерация уникальной ссылки для подтверждения
            string verificationLink = GenerateVerificationLink();

            // Отправка электронного письма с ссылкой для подтверждения
            bool isEmailSent = SendVerificationEmail(email, verificationLink);

            if (isEmailSent)
            {
                return Ok("Письмо с подтверждением отправлено");
            }
            else
            {
                return BadRequest("Не удалось отправить письмо с подтверждением");
            }
        }

        [HttpGet("ConfirmEmail")]
        public IActionResult ConfirmEmail(string email, string verificationCode)
        {
            // Проверка ссылки подтверждения
            bool isEmailConfirmed = VerifyEmailConfirmation(email, verificationCode);

            if (isEmailConfirmed)
            {
                // Обновление статуса проверки почты в базе данных
                UpdateEmailVerificationStatus(email);

                return Ok("Адрес электронной почты успешно подтвержден");
            }
            else
            {
                return BadRequest("Недействительная ссылка подтверждения");
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
                mail.Body = "Пожалуйста, перейдите по ссылке для подтверждения вашего адреса электронной почты: " + verificationLink;

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
        [HttpGet("UpdateEmailVerificationStatus")]
        private void UpdateEmailVerificationStatus(string email)
        {
            // Подключение к базе данных
            using (var connection = new SqlConnection($"server=mysql_db;port=3306;database=archivist;user=root;password=root_password;"))
            {
                connection.Open();

                // Создание SQL-запроса для обновления статуса проверки почты
                string query = "UPDATE Users SET email_is_checked = 1 WHERE email = @email";

                // Создание команды с параметром
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    // Выполнение команды обновления
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}