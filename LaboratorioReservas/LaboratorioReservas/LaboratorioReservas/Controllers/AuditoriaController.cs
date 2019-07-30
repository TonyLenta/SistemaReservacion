using Modelos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LaboratorioReservas.Controllers
{
    public class AuditoriaController : Controller
    {
        private LaboratorioReservaEntities db = new LaboratorioReservaEntities();

        public ActionResult Index(string search)
        {
            if (Session["UserID"] != null)
            {
                var auditoria = db.Auditorias.Where(s =>
             s.Accion.Contains(search) ||
             s.Detalle.Contains(search) ||
             s.Usuario.Nombres.Contains(search) ||
             search == null).Include(m => m.Usuario).OrderByDescending(m => m.AuditoriId);

                return View("index", auditoria.ToList());
            }
            return RedirectToAction("Login", "Home");

        }
    }
}