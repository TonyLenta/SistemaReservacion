using Modelos;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LaboratorioReservas.Controllers
{
    public class PdfsController : Controller
    {
      
            private LaboratorioReservaEntities db = new LaboratorioReservaEntities();

            // GET: PDFs
            public ActionResult Index()
            {
                if (Session["UserID"] != null)
                {
                    return View();
                }
                return RedirectToAction("Login", "Home");

            }

            public ActionResult ExportPDFAuditorias()
            {
                return new ActionAsPdf("Auditoria")
                {
                    FileName = Server.MapPath("~/Content/Auditorias.pdf")
                };
            }

            public ActionResult ExportPDFMaterias()
            {

                return new ActionAsPdf("Materia")
                {
                    FileName = Server.MapPath("~/Content/Materias.pdf")
                };
            }

            public ActionResult ExportPDFEscuelas()
            {

                return new ActionAsPdf("Escuela")
                {
                    FileName = Server.MapPath("~/Content/Escuelas.pdf")
                };
            }


            public ActionResult ExportPDFNiveles()
            {
                return new ActionAsPdf("Nivel")
                {
                    FileName = Server.MapPath("~/Content/Niveles.pdf")
                };
            }

            public ActionResult ExportPDFSalas()
            {
                return new ActionAsPdf("Sala")
                {
                    FileName = Server.MapPath("~/Content/Salas.pdf")
                };
            }

            public ActionResult ExportPDFSolicitudes()
            {
                return new ActionAsPdf("Solicitud")
                {
                    FileName = Server.MapPath("~/Content/Solicitudes.pdf")
                };
            }

            public ActionResult ExportPDFUsuarios()
            {
                return new ActionAsPdf("Usuario")
                {
                    FileName = Server.MapPath("~/Content/Usuarios.pdf")
                };
            }
            public ActionResult ExportPDFUnUsuarios(int id)
            {
                return new ActionAsPdf("UnUsuario/" + id)
                {
                    FileName = Server.MapPath("~/Content/Usuario.pdf")
                };
            }

            public ActionResult Auditoria(string search)
            {
                var auditoria = db.Auditorias.Where(s =>
                    s.Accion.Contains(search) ||
                    s.Detalle.Contains(search) ||
                    s.Usuario.Nombres.Contains(search) ||
                    search == null).Include(m => m.Usuario);
                return View("Auditoria", auditoria.ToList());
            }


            public ActionResult Escuela(string search)
            {
                var escuelas = db.Escuelas.Where(s =>
                   (s.NombreEscuela.Contains(search) || search == null) && s.EstadoEscuela == 1);
                return View("Escuela", escuelas.ToList());
            }

            public ActionResult Materia(string search)
            {
                var mATERIAs = db.Materias.Where(s => (
                      s.NombreMateria.Contains(search) ||
                      s.Escuela.NombreEscuela.Contains(search) ||
                      s.Usuario.Nombres.Contains(search) ||
                      s.Usuario.Cedula.Contains(search) ||
                      s.Usuario.NombreUsuario.Contains(search) ||
                      search == null) && s.EstadoMateria == 1).Include(m => m.Escuela).Include(m => m.Nivel).Include(m => m.Usuario);
                return View("Materia", mATERIAs.ToList());
            }

            public ActionResult Nivel(string search)
            {
                var nivel = db.Nivels.Where(s => (s.Nivel1.Contains(search) ||
                    search == null) && s.EstadoNivel == 1);
                return View("Nivele", nivel.ToList());
            }

            public ActionResult Sala(string search)
            {
                var sala = db.Salas.Where(s => (s.Salas.Contains(search) || search == null) && s.EstadoSalas == 1);
                return View("Sala", sala.ToList());
            }

            public ActionResult Solicitud(string search)
            {
                var sOLICITUDs = db.Solicituds.Include(s => s.Materia).Include(s => s.Sala);
                return View("Solicitud", sOLICITUDs.ToList());
            }

            public ActionResult Usuario(string search)
            {
                var uSUARIOs = db.Usuarios
                             .Where(s => (
                             s.Nombres.Contains(search) ||
                             s.NombreUsuario.Contains(search) ||
                             s.Token.Contains(search) ||
                             s.ChatId.Contains(search) ||
                             s.Role.Rol.Contains(search) ||
                             search == null) && s.EstadoUsuario == 1)
                             .Include(u => u.Role);
                return View("Usuario", uSUARIOs.ToList());
            }

            public ActionResult UnUsuario(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Usuario uSUARIO = db.Usuarios.Find(id);
                if (uSUARIO == null)
                {
                    return HttpNotFound();
                }
                return View("UnUsuario", uSUARIO);
            }

        }
    }
    