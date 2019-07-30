using Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LaboratorioReservas.Controllers
{
    public class HomeController : Controller
    {
        private LaboratorioReservaEntities db = new LaboratorioReservaEntities();
        // GET: Home
        public ActionResult Login()
        {
            if (Session["UserID"] != null)
            {
                return RedirectToAction("UserDashBoard");

            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuario objUser)
        {
            if (ModelState.IsValid)
            {
                using (LaboratorioReservaEntities db = new LaboratorioReservaEntities())
                {
                    var obj = db.Usuarios.Where(a => a.NombreUsuario.Equals(objUser.NombreUsuario) && a.Contrasena.Equals(objUser.Contrasena) && a.RolesId != 3).FirstOrDefault();
                    if (obj != null)
                    {
                        if (obj.EstadoSesion == 0)
                        {
                            obj.EstadoSesion = 1;
                            db.SaveChanges();
                            Session["UserID"] = obj.UsuarioId.ToString();
                            Session["UserName"] = obj.NombreUsuario.ToString();
                            Session["UserNames"] = obj.Nombres.ToString();
                            Session["UserRol"] = obj.RolesId;
                            Session["UserToken"] = obj.Token.ToString();
                            Session["UserChatId"] = obj.ChatId.ToString();
                            return RedirectToAction("UserDashBoard");
                        }
                        ViewBag.Message = "Usuario logueado anteriormente";
                    }
                    else
                    {
                        var obj1 = db.Usuarios.Where(a => a.NombreUsuario.Equals(objUser.NombreUsuario) && a.RolesId != 3).FirstOrDefault();
                        if (obj1 != null)
                        {
                            var restablecido = db.Restablecers.Where(a => a.UsuarioId == obj1.UsuarioId && a.NuevaPassword.Equals(objUser.Contrasena)).FirstOrDefault();
                            if (restablecido != null)
                            {
                                if (this.MinutosTranscurridos(DateTime.Now, Convert.ToDateTime(restablecido.Creacion)) < 2)
                                {
                                    obj1.EstadoSesion = 1;
                                    db.SaveChanges();
                                    Session["UserID"] = obj1.UsuarioId.ToString();
                                    Session["UserName"] = obj1.NombreUsuario.ToString();
                                    Session["UserNames"] = obj1.Nombres.ToString();
                                    Session["UserRol"] = obj1.RolesId;
                                    Session["UserToken"] = obj1.Token.ToString();
                                    Session["UserChatId"] = obj1.ChatId.ToString();
                                    return RedirectToAction("UserDashBoard");
                                }
                                else
                                {
                                    ViewBag.Message = "Clave temporal ha expirado";
                                    return View(objUser);
                                }

                            }
                        }
                        ViewBag.Message = "Error de usuario o contraseña";
                    }
                }
            }
            return View(objUser);
        }

        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Restablecer()
        {
            if (Session["UserID"] != null)
            {
                return RedirectToAction("UserDashBoard");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Restablecer([Bind(Include = "RestablecerId,UsuarioId,NuevaPassword")] Restablecer rESTABLECER)
        {
            try
            {
                string user = Request.Form["NombreUsuario"].ToString();
                Usuario usuario = db.Usuarios.Where(s => s.NombreUsuario.Equals(user) && s.RolesId != 3).FirstOrDefault();
                if (usuario != null)
                {
                    String clave = this.CrearPassword(10);
                    Restablecer nuevo = new Restablecer();
                    nuevo.NuevaPassword = clave;
                    nuevo.UsuarioId = usuario.UsuarioId;
                    nuevo.Creacion = DateTime.Now;
                    nuevo.Modificacion = DateTime.Now;
                    usuario.EstadoSesion = 0;
                    //usuario.Contrasena = clave;
                    db.Restablecers.Add(nuevo);
                    db.SaveChanges();

                    if (Request.Form["optradio"].ToString().Equals("telegram"))
                    {
                         Telegram.bot.token = usuario.Token;
                         Telegram.bot.sendMessage.send(usuario.ChatId, "Clave generada con exito: " + clave);
                    }
                    else
                    {
                        this.EnviarMensajeEmail(usuario.NombreUsuario, "Restablecimiento de contraseña", "Clave generada con exito: " + clave);
                    }
                    ViewBag.Message = "Clave generada satisfactoriamente, enviado a " + Request.Form["optradio"].ToString();
                    ViewBag.Tipo = "Satisfactorio!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.RegistrarAuditoria("Error", ex.Message, -1);
            }
            ViewBag.Tipo = "Error!";
            ViewBag.Message = "Usuario no encontrado en la base de datos";
            return View();
        }

        private string CrearPassword(int longitud)
        {
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return res.ToString();
        }

        private void RegistrarAuditoria(String accion, String detalle, int usuario)
        {
            Auditoria auditoria = new Auditoria();
            auditoria.Accion = accion;
            auditoria.Detalle = detalle;
            auditoria.Creacion = DateTime.Now;
            if (usuario != -1)
            {
                auditoria.UsuarioId = usuario;
            }
            db.Auditorias.Add(auditoria);
            db.SaveChanges();
        }

        private Boolean EnviarMensajeEmail(string receiver, string subject, string message)
        {
            try
            {
                var senderEmail = new MailAddress("jjessikaryh@gmail.com", "No_responder");
                var receiverEmail = new MailAddress(receiver, "Receiver");
                var password = "elkindaniel17";
                var sub = subject;
                var body = message;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(mess);
                }
                return true;

            }
            catch (Exception ex)
            {
                this.RegistrarAuditoria("Error", ex.Message, -1);
            }
            return false;
        }

        private Double MinutosTranscurridos(DateTime fecha_actual, DateTime fecha_ingreso)
        {
            TimeSpan ts = fecha_actual - fecha_ingreso;
            return Convert.ToDouble(ts.TotalMinutes);
        }

        public ActionResult CerrarSession()
        {
            Usuario cerrar = db.Usuarios.Find(Convert.ToInt32(Session["UserID"]));
            cerrar.EstadoSesion = 0;
            db.SaveChanges();
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session["UserNames"] = null;
            Session["UserRol"] = null;
            Session["UserToken"] = null;
            Session["UserChatId"] = null;
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }
    }
}