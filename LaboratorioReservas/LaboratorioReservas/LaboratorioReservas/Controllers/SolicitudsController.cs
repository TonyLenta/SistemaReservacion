using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Modelos;

namespace LaboratorioReservas.Controllers
{
    public class SolicitudsController : Controller
    {
        private LaboratorioReservaEntities db = new LaboratorioReservaEntities();

        // GET: Solicituds
        public ActionResult Index(String search)
        {
            if (Session["UserID"] != null)
            {
                this.ActualizacionEstados();
                var solicituds = db.Solicituds
                    .Where(s => (s.Sala.Salas.Contains(search)
                    || search == null
                    || s.Materia.Escuela.NombreEscuela.Contains(search)
                    || s.Observaciones.Contains(search)
                    || s.Materia.Usuario.Nombres.Contains(search)) && s.EstadoSolicitud == 1)
                    .Include(s => s.Materia).Include(s => s.Sala);
                return View(solicituds.ToList());
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Solicituds/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Solicitud solicitud = db.Solicituds.Find(id);
                if (solicitud == null)
                {
                    return HttpNotFound();
                }
                return View(solicitud);
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Solicituds/Create
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.MateriaId = new SelectList(db.Materias, "MateriaId", "NombreMateria");
                ViewBag.SalasId = new SelectList(db.Salas, "SalasId", "Salas");
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Solicituds/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SolicitudId,SalasId,MateriaId,FechaInicio,FechaFinal,HoraInicio,HoraFinal,Observaciones,Confirmacion,EstadoSolicitud,Creacion,Modificacion")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                String mensaje = "";
                try
                {
                    if (DateTime.Compare(Convert.ToDateTime(Request.Form["FechaInicio"]), Convert.ToDateTime(Request.Form["FechaFinal"])) < 0)
                    {
                        if (TimeSpan.Compare(TimeSpan.Parse(Request.Form["HoraInicio"]), TimeSpan.Parse(Request.Form["HoraFinal"])) < 0)
                        {
                            Solicitud nuevo = new Solicitud();
                            nuevo.SalasId = Convert.ToInt32(Request.Form["SalasId"]);
                            nuevo.MateriaId = Convert.ToInt32(Request.Form["MateriaId"]);
                            nuevo.FechaInicio = Convert.ToDateTime(Request.Form["FechaInicio"]);
                            nuevo.FechaFinal = Convert.ToDateTime(Request.Form["FechaFinal"]);
                            nuevo.HoraInicio = TimeSpan.Parse(Request.Form["HoraInicio"]);
                            nuevo.HoraFinal = TimeSpan.Parse(Request.Form["HoraFinal"]);
                            nuevo.Observaciones = Request.Form["Observaciones"];
                            nuevo.Confirmacion = "EN ESPERA";
                            nuevo.EstadoSolicitud = 1;
                            nuevo.Creacion = DateTime.Now;
                            nuevo.Modificacion = DateTime.Now;
                            db.Solicituds.Add(nuevo);
                            db.SaveChanges();
                            this.RegistrarAuditoria("Creación", "Se ha creado una solicitud desde " + Request.Form["FechaInicio"] + " Hasta: " + Request.Form["FechaFinal"], Convert.ToInt32(Session["UserID"]));
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            mensaje = "Horas mal ingresadas";
                        }
                    }
                    else
                    {
                        mensaje = "Fechas mal ingresadas";
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
                ModelState.AddModelError("", mensaje);
            }
            ViewBag.MateriaId = new SelectList(db.Materias.Where(s => s.EstadoMateria == 1), "MateriaId", "NombreMateria", solicitud.MateriaId);
            ViewBag.SalasId = new SelectList(db.Salas.Where(s => s.EstadoSalas == 1), "SalasId", "Salas", solicitud.SalasId);
            return View(solicitud);
        }

        // GET: Solicituds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Solicitud solicitud = db.Solicituds.Find(id);
                if (solicitud == null)
                {
                    return HttpNotFound();
                }
                ViewBag.MateriaId = new SelectList(db.Materias.Where(s => s.EstadoMateria == 1), "MateriaId", "NombreMateria", solicitud.MateriaId);
                ViewBag.SalasId = new SelectList(db.Salas.Where(s => s.EstadoSalas == 1), "SalasId", "Salas", solicitud.SalasId);
                return View(solicitud);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Solicituds/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SolicitudId,SalasId,MateriaId,FechaInicio,FechaFinal,HoraInicio,HoraFinal,Observaciones,Confirmacion,EstadoSolicitud,Creacion,Modificacion")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Solicitud nuevo = db.Solicituds.Find(Convert.ToInt32(Request.Form["SalasId"]));
                    nuevo.SalasId = Convert.ToInt32(Request.Form["SalasId"]);
                    nuevo.MateriaId = Convert.ToInt32(Request.Form["MateriaId"]);
                    nuevo.FechaInicio = Convert.ToDateTime(Request.Form["FechaInicio"]);
                    nuevo.FechaFinal = Convert.ToDateTime(Request.Form["FechaFinal"]);
                    nuevo.HoraInicio = TimeSpan.Parse(Request.Form["HoraInicio"]);
                    nuevo.HoraFinal = TimeSpan.Parse(Request.Form["HoraFinal"]);
                    nuevo.Observaciones = Request.Form["Observaciones"];
                    nuevo.Modificacion = DateTime.Now;
                    db.SaveChanges();
                    this.RegistrarAuditoria("Modificación", "Se ha modificado una solicitud de la  " + nuevo.Sala.Salas + " desde: " + nuevo.FechaInicio + " Hasta: " + nuevo.FechaFinal, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            ViewBag.MateriaId = new SelectList(db.Materias.Where(s => s.EstadoMateria == 1), "MateriaId", "NombreMateria", solicitud.MateriaId);
            ViewBag.SalasId = new SelectList(db.Salas.Where(s => s.EstadoSalas == 1), "SalasId", "Salas", solicitud.SalasId);
            return View(solicitud);
        }

        // GET: Solicituds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Solicitud solicitud = db.Solicituds.Find(id);
                if (solicitud == null)
                {
                    return HttpNotFound();
                }
                return View(solicitud);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Solicituds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Solicitud nuevo = db.Solicituds.Find(id);
                nuevo.EstadoSolicitud = 0;
                nuevo.Modificacion = DateTime.Now;
                db.SaveChanges();
                this.RegistrarAuditoria("Eliminación", "Se ha eliminado una solicitud de la  " + nuevo.Sala.Salas + " desde: " + nuevo.FechaInicio + " Hasta: " + nuevo.FechaFinal, Convert.ToInt32(Session["UserID"]));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                this.RegistrarAuditoria("Error", ex.Message, -1);
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult SolEliminadas(String search)
        {
            this.ActualizacionEstados();
            if (Session["UserID"] != null)
            {
                var sOLICITUDs = db.Solicituds
                    .Where(s => (s.Sala.Salas.Contains(search)
                    || search == null
                    || s.Materia.Escuela.NombreEscuela.Contains(search)
                    || s.Observaciones.Contains(search) || s.Materia.Usuario.Nombres.Contains(search)) && s.EstadoSolicitud == 0)
                    .Include(s => s.Materia).Include(s => s.Sala);
                return View(sOLICITUDs.ToList());
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult SolTerminadas(String search)
        {
            this.ActualizacionEstados();
            if (Session["UserID"] != null)
            {
                var solicituds = db.Solicituds
                    .Where(s => (s.Sala.Salas.Contains(search)
                    || search == null
                    || s.Materia.Escuela.NombreEscuela.Contains(search)
                    || s.Observaciones.Contains(search) || s.Materia.Usuario.Nombres.Contains(search)) && s.EstadoSolicitud == 1 && s.Confirmacion.Equals("TERMINADO"))
                    .Include(s => s.Materia).Include(s => s.Sala);
                return View(solicituds.ToList());
            }
            return RedirectToAction("Login", "Home");
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
        private void ActualizacionEstados()
        {
            DateTime hoy = DateTime.Now;
            var sol = db.Solicituds.Where(s => (hoy >= s.FechaInicio && hoy <= s.FechaFinal) && s.EstadoSolicitud == 1).ToList();
            sol.ForEach(s => {
                s.Confirmacion = "EN EJECUCION";
            });
            db.SaveChanges();

            var sol1 = db.Solicituds.Where(s => (hoy < s.FechaInicio) && s.EstadoSolicitud == 1).ToList();
            sol1.ForEach(s => {
                s.Confirmacion = "EN ESPERA";
            });
            db.SaveChanges();

            var sol2 = db.Solicituds.Where(s => (hoy > s.FechaFinal) && s.EstadoSolicitud == 1).ToList();
            sol2.ForEach(s => {
                s.Confirmacion = "TERMINADO";
            });
            db.SaveChanges();

        }

        public ActionResult Horario()
        {
            if (Session["UserID"] != null)
            {
                DateTime hoy = DateTime.Now;
                var sol = db.Solicituds.Where(s => ((hoy >= s.FechaInicio && hoy <= s.FechaFinal) || hoy < s.FechaInicio) && s.EstadoSolicitud == 1).ToList();
                String datos = "[";
                foreach (var item in sol)
                {

                    datos += "{";
                    datos += " title: " + "'" + item.Sala.Salas + "',";
                    datos += " start: " + "'" + item.FechaInicio.Date.Year + "-" + item.FechaInicio.Date.Month + "-" + item.FechaInicio.Date.Day + "',";
                    datos += " end: " + "'" + item.FechaFinal.Date.Year + "-" + item.FechaFinal.Date.Month + "-" + item.FechaFinal.Date.Day + "',";
                    datos += "},";

                }
                datos += "]";
                ViewBag.calendario = datos;
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Horarios()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return RedirectToAction("Login", "Home");

        }
    }
}
