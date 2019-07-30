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
    public class MateriasController : Controller
    {
        private LaboratorioReservaEntities db = new LaboratorioReservaEntities();

        // GET: Materias
        public ActionResult Index(string search)
        {
            if (Session["UserID"] != null)
            {
                var mATERIAs = db.Materias.Where(s => (
                s.NombreMateria.Contains(search) ||
                s.Escuela.NombreEscuela.Contains(search) ||
                s.Usuario.Nombres.Contains(search) ||
                s.Usuario.Cedula.Contains(search) ||
                s.Usuario.NombreUsuario.Contains(search) ||
                search == null) && s.EstadoMateria == 1).Include(m => m.Escuela).OrderByDescending(m => m.EscuelaId).Include(m => m.Nivel).OrderByDescending(m => m.NivelId).Include(m => m.Usuario).OrderByDescending(m => m.UsuarioId);
                return View(mATERIAs.ToList().OrderByDescending(m => m.MateriaId));
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Materias/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Materia materia = db.Materias.Find(id);
                if (materia == null)
                {
                    return HttpNotFound();
                }
                return View(materia);
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Materias/Create
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.EscuelaId = new SelectList(db.Escuelas.Where(s => s.EstadoEscuela == 1), "EscuelaId", "NombreEscuela");
                ViewBag.NivelId = new SelectList(db.Nivels.Where(s => s.EstadoNivel == 1), "NivelId", "NivelId");
                ViewBag.UsuarioId = new SelectList(db.Usuarios.Where(s => s.EstadoUsuario == 1 && s.RolesId == 3), "UsuarioId", "Nombres");
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Materias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MateriaId,NivelId,EscuelaId,UsuarioId,NombreMateria,EstadoMateria,Creacion,Modificacion")] Materia materia)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Materia nuevo = new Materia();
                    nuevo.NombreMateria = Request.Form["NombreMateria"];
                    nuevo.NivelId = Convert.ToInt32(Request.Form["NivelId"]);
                    nuevo.EscuelaId = Convert.ToInt32(Request.Form["EscuelaId"]);
                    nuevo.UsuarioId = Convert.ToInt32(Request.Form["UsuarioId"]);
                    nuevo.EstadoMateria = 1;
                    nuevo.Creacion = DateTime.Now;
                    nuevo.Modificacion = DateTime.Now;
                    db.Materias.Add(nuevo);
                    db.SaveChanges();
                    this.RegistrarAuditoria("Inserción", "Se ha insertado una materia " + nuevo.NombreMateria, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            ViewBag.EscuelaId = new SelectList(db.Escuelas.Where(s => s.EstadoEscuela == 1), "EscuelaId", "NombreEscuela");
            ViewBag.NivelId = new SelectList(db.Nivels.Where(s => s.EstadoNivel == 1), "NivelId", "NivelId");
            ViewBag.UsuarioId = new SelectList(db.Usuarios.Where(s => s.EstadoUsuario == 1 && s.RolesId == 3), "UsuarioId", "Nombres");
            return View(materia);
        }

        // GET: Materias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Materia materia = db.Materias.Find(id);
                if (materia == null)
                {
                    return HttpNotFound();
                }
                ViewBag.EscuelaId = new SelectList(db.Escuelas.Where(s => s.EstadoEscuela == 1), "EscuelaId", "NombreEscuela");
                ViewBag.NivelId = new SelectList(db.Nivels.Where(s => s.EstadoNivel == 1), "NivelId", "NivelId");
                ViewBag.UsuarioId = new SelectList(db.Usuarios.Where(s => s.EstadoUsuario == 1 && s.RolesId == 3), "UsuarioId", "Nombres");
                return View(materia);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Materias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MateriaId,NivelId,EscuelaId,UsuarioId,NombreMateria,EstadoMateria,Creacion,Modificacion")] Materia materia)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Materia nuevo = db.Materias.Find(Convert.ToInt32(Request.Form["MateriaId"]));
                    nuevo.NombreMateria = Request.Form["NombreMateria"];
                    nuevo.NivelId = Convert.ToInt32(Request.Form["NivelId"]);
                    nuevo.EscuelaId = Convert.ToInt32(Request.Form["EscuelaId"]);
                    nuevo.UsuarioId = Convert.ToInt32(Request.Form["UsuarioId"]);
                    nuevo.Modificacion = DateTime.Now;
                    db.SaveChanges();
                    this.RegistrarAuditoria("Actualización", "Se ha actualizado una materia " + nuevo.NombreMateria, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            ViewBag.EscuelaId = new SelectList(db.Escuelas.Where(s => s.EstadoEscuela == 1), "EscuelaId", "NombreEscuela");
            ViewBag.NivelId = new SelectList(db.Nivels.Where(s => s.EstadoNivel == 1), "NivelId", "NivelId");
            ViewBag.UsuarioId = new SelectList(db.Usuarios.Where(s => s.EstadoUsuario == 1 && s.RolesId == 3), "UsuarioId", "Nombres");
            return View(materia);
        }

        // GET: Materias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Materia materia = db.Materias.Find(id);
                if (materia == null)
                {
                    return HttpNotFound();
                }
                return View(materia);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Materias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Materia nuevo = db.Materias.Find(id);
                nuevo.EstadoMateria = 0;
                nuevo.Modificacion = DateTime.Now;
                db.SaveChanges();
                this.RegistrarAuditoria("Eliminación", "Se ha eliminado una materia " + nuevo.NombreMateria, Convert.ToInt32(Session["UserID"]));
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
