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
    public class NivelsController : Controller
    {
        private LaboratorioReservaEntities db = new LaboratorioReservaEntities();

        // GET: Nivels
        public ActionResult Index(String search)
        {
            if (Session["UserID"] != null)
            {
                var nivel = db.Nivels.Where(s => (s.Nivel1.Contains(search) ||
                search == null) && s.EstadoNivel == 1);
                return View(nivel.ToList().OrderByDescending(m => m.NivelId));
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Nivels/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Nivel nivel = db.Nivels.Find(id);
                if (nivel == null)
                {
                    return HttpNotFound();
                }
                return View(nivel);
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Nivels/Create
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Nivels/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NivelId,Nivel1,EstadoNivel,Creacion,Modificacion")] Nivel nivel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Nivel nuevo = new Nivel();
                    nuevo.Nivel1 = Request.Form["Nivel1"];
                    nuevo.EstadoNivel = 1;
                    nuevo.Creacion = DateTime.Now;
                    nuevo.Modificacion = DateTime.Now;
                    db.Nivels.Add(nuevo);
                    db.SaveChanges();
                    this.RegistrarAuditoria("Inserción", "Se ha insertado el nivel " + nuevo.Nivel1, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            return View(nivel);
        }

        // GET: Nivels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Nivel nivel = db.Nivels.Find(id);
                if (nivel == null)
                {
                    return HttpNotFound();
                }
                return View(nivel);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Nivels/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NivelId,Nivel1,EstadoNivel,Creacion,Modificacion")] Nivel nivel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Nivel nivel1 = db.Nivels.Find(Convert.ToInt32(Request.Form["NivelId"]));
                    nivel1.Nivel1 = Request.Form["Nivel1"];
                    nivel1.Modificacion = DateTime.Now;
                    db.SaveChanges();
                    this.RegistrarAuditoria("Actualización", "Se ha editado el nivel " + nivel1.Nivel1, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            return View(nivel);
        }

        // GET: Nivels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Nivel nivel = db.Nivels.Find(id);
                if (nivel == null)
                {
                    return HttpNotFound();
                }
                return View(nivel);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Nivels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Nivel nivel1 = db.Nivels.Find(id);
                nivel1.EstadoNivel = 0;
                nivel1.Modificacion = DateTime.Now;
                db.SaveChanges();
                this.RegistrarAuditoria("Eliminación", "Se ha eliminado el nivel " + nivel1.Nivel1, Convert.ToInt32(Session["UserID"]));
                return RedirectToAction("Index");
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
