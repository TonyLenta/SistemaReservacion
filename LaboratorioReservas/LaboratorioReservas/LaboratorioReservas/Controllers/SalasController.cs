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
    public class SalasController : Controller
    {
        private LaboratorioReservaEntities db = new LaboratorioReservaEntities();

        // GET: Salas
        public ActionResult Index(String search)
        {
            if (Session["UserID"] != null)
            {
                var sala = db.Salas.Where(s => (s.Salas.Contains(search) || search == null) && s.EstadoSalas == 1);
                return View(sala.ToList().OrderByDescending(m => m.SalasId));
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Salas/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Sala sala = db.Salas.Find(id);
                if (sala == null)
                {
                    return HttpNotFound();
                }
                return View(sala);
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Salas/Create
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Salas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SalasId,Salas,EstadoSalas,Creacion,Modificacion")] Sala sala)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Sala nuevo = new Sala();
                    nuevo.Salas = Request.Form["Salas"];
                    nuevo.EstadoSalas = 1;
                    nuevo.Creacion = DateTime.Now;
                    nuevo.Modificacion = DateTime.Now;
                    db.Salas.Add(nuevo);
                    db.SaveChanges();
                    this.RegistrarAuditoria("Creación", "Se ha creado una sala " + nuevo.Salas, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            return View(sala);
        }

        // GET: Salas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Sala sala = db.Salas.Find(id);
                if (sala == null)
                {
                    return HttpNotFound();
                }
                return View(sala);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Salas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SalasId,Salas,EstadoSalas,Creacion,Modificacion")] Sala sala)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Sala nuevo = db.Salas.Find(Convert.ToInt32(Request.Form["SalasId"]));
                    nuevo.Salas = Request.Form["Salas"];
                    nuevo.Modificacion = DateTime.Now;
                    db.SaveChanges();
                    this.RegistrarAuditoria("Actualización", "Se ha actualizado una sala " + nuevo.Salas, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            return View(sala);
        }

        // GET: Salas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Sala sala = db.Salas.Find(id);
                if (sala == null)
                {
                    return HttpNotFound();
                }
                return View(sala);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Salas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Sala nuevo = db.Salas.Find(id);
                nuevo.EstadoSalas = 0;
                nuevo.Modificacion = DateTime.Now;
                db.SaveChanges();
                this.RegistrarAuditoria("Eliminación", "Se ha actualizado una sala " + nuevo.Salas, Convert.ToInt32(Session["UserID"]));
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
