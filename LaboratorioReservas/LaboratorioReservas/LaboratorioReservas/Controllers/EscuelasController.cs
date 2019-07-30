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
    public class EscuelasController : Controller
    {
        private LaboratorioReservaEntities db = new LaboratorioReservaEntities();

        // GET: Escuelas
        public ActionResult Index(string search)
        {
            if (Session["UserID"] != null)
            {
                var escuelas = db.Escuelas.Where(s =>
                (s.NombreEscuela.Contains(search) || search == null) && s.EstadoEscuela == 1);
                return View(escuelas.ToList().OrderByDescending(m => m.EscuelaId));
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Escuelas/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Escuela escuela = db.Escuelas.Find(id);
                if (escuela == null)
                {
                    return HttpNotFound();
                }
                return View(escuela);
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Escuelas/Create
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Escuelas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EscuelaId,NombreEscuela,EstadoEscuela,Creacion,Modificacion")] Escuela escuela)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Escuela nuevo = new Escuela();
                    nuevo.NombreEscuela = Request.Form["NombreEscuela"];
                    nuevo.EstadoEscuela = 1;
                    nuevo.Creacion = DateTime.Now;
                    nuevo.Modificacion = DateTime.Now;
                    db.Escuelas.Add(nuevo);
                    db.SaveChanges();
                    this.RegistrarAuditoria("Inserción", "Se ha insertado una escuela " + nuevo.NombreEscuela, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            return View(escuela);
        }

        // GET: Escuelas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Escuela escuela = db.Escuelas.Find(id);
                if (escuela == null)
                {
                    return HttpNotFound();
                }
                return View(escuela);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Escuelas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EscuelaId,NombreEscuela,EstadoEscuela,Creacion,Modificacion")] Escuela escuela)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Escuela nuevo = db.Escuelas.Find(Convert.ToInt32(Request.Form["EscuelaId"]));
                    nuevo.NombreEscuela = Request.Form["NombreEscuela"];
                    nuevo.Modificacion = DateTime.Now;
                    db.SaveChanges();
                    this.RegistrarAuditoria("Actualización", "Se ha actualizado una escuela " + escuela.NombreEscuela, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            return View(escuela);
        }

        // GET: Escuelas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Escuela escuela = db.Escuelas.Find(id);
                if (escuela == null)
                {
                    return HttpNotFound();
                }
                return View(escuela);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Escuelas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Escuela escuela = db.Escuelas.Find(id);
                escuela.EstadoEscuela = 0;
                escuela.Modificacion = DateTime.Now;
                db.SaveChanges();
                this.RegistrarAuditoria("Eliminación", "Se ha eliminado una escuela " + escuela.NombreEscuela, Convert.ToInt32(Session["UserID"]));
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
    }
}
