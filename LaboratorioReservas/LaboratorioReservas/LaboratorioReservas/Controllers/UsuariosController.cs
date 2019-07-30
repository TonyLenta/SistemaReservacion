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
    public class UsuariosController : Controller
    {
        private LaboratorioReservaEntities db = new LaboratorioReservaEntities();

        // GET: Usuarios
        public ActionResult Index(string search)
        {
            if (Session["UserID"] != null)
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
                return View(uSUARIOs.ToList().OrderByDescending(m => m.UsuarioId));
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Usuario usuario = db.Usuarios.Find(id);
                if (usuario == null)
                {
                    return HttpNotFound();
                }
                return View(usuario);
            }
            return RedirectToAction("Login", "Home");
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.RolesId = new SelectList(db.Roles, "RolesId", "Rol");
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UsuarioId,RolesId,Nombres,Cedula,Token,ChatId,NombreUsuario,Contrasena,Restablecer,EstadoSesion,EstadoUsuario,Creacion,Modificacion")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Usuario nuevo = new Usuario();
                    nuevo.RolesId = Convert.ToInt32(Request.Form["RolesId"]);
                    nuevo.Nombres = Request.Form["Nombres"];
                    nuevo.Cedula = Request.Form["Cedula"];
                    nuevo.Token = Request.Form["Token"];
                    nuevo.ChatId = Request.Form["ChatId"];
                    nuevo.NombreUsuario = Request.Form["NombreUsuario"];
                    nuevo.Contrasena = Request.Form["Contrasena"];
                    nuevo.Restablecer = 0;
                    nuevo.EstadoSesion = 0;
                    nuevo.EstadoUsuario = 1;
                    nuevo.Creacion = DateTime.Now;
                    nuevo.Modificacion = DateTime.Now;
                    db.Usuarios.Add(nuevo);
                    db.SaveChanges();
                    this.RegistrarAuditoria("Creación", "Se ha creado un usuario " + nuevo.Nombres, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            ViewBag.RolesId = new SelectList(db.Roles, "RolesId", "Rol", usuario.RolesId);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Usuario usuario = db.Usuarios.Find(id);
                if (usuario == null)
                {
                    return HttpNotFound();
                }
                ViewBag.RolesId = new SelectList(db.Roles, "RolesId", "Rol", usuario.RolesId);
                return View(usuario);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UsuarioId,RolesId,Nombres,Cedula,Token,ChatId,NombreUsuario,Contrasena,Restablecer,EstadoSesion,EstadoUsuario,Creacion,Modificacion")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Usuario nuevo = db.Usuarios.Find(Convert.ToInt32(Request.Form["UsuarioId"]));
                    nuevo.RolesId = Convert.ToInt32(Request.Form["RolesId"]);
                    nuevo.Nombres = Request.Form["Nombres"];
                    nuevo.Cedula = Request.Form["Cedula"];
                    nuevo.Token = Request.Form["Token"];
                    nuevo.ChatId = Request.Form["ChatId"];
                    nuevo.NombreUsuario = Request.Form["NombreUsuario"];
                    nuevo.Contrasena = Request.Form["Contrasena"];
                    nuevo.Modificacion = DateTime.Now;
                    db.SaveChanges();
                    this.RegistrarAuditoria("Modificación", "Se ha modificado un usuario " + nuevo.Nombres, Convert.ToInt32(Session["UserID"]));
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                }
            }
            ViewBag.RolesId = new SelectList(db.Roles, "RolesId", "Rol", usuario.RolesId);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Usuario usuario = db.Usuarios.Find(id);
                if (usuario == null)
                {
                    return HttpNotFound();
                }
                return View(usuario);
            }
            return RedirectToAction("Login", "Home");
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Usuario nuevo = db.Usuarios.Find(id);
                nuevo.EstadoUsuario = 0;
                nuevo.Modificacion = DateTime.Now;
                db.SaveChanges();
                this.RegistrarAuditoria("Eliminacón", "Se ha eliminado un usuario " + nuevo.Nombres, Convert.ToInt32(Session["UserID"]));
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
        public ActionResult Cambiar(int? id)
        {
            if (Session["UserID"] != null)
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
                return View("Cambiar", uSUARIO);
            }
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cambiar()
        {
            try
            {
                Usuario cambio = db.Usuarios.Find(Convert.ToInt32(Request.Form["UsuarioId"]));
                cambio.Contrasena = Request.Form["Contrasena"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                this.RegistrarAuditoria("Error", ex.Message, -1);
            }
            return View("Cambiar");
        }

        public ActionResult Perfil(int? id)
        {
            if (Session["UserID"] != null)
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
                return View(uSUARIO);
            }
            return RedirectToAction("Login", "Home");

        }
    }
}
