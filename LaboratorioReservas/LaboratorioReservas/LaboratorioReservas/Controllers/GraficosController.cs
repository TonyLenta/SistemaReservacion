using Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LaboratorioReservas.Controllers
{
    public class GraficosController : Controller
    {
        private LaboratorioReservaEntities db = new LaboratorioReservaEntities();

        // GET: Graficos
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                String StrDatospastel = "[['Salas','Cantidad'],";
                String StrDatospastel1 = "[['Estados','Cantidad'],";
                String StrDatoslinea = "[['Anos','Aceptados'],";
                String StrDatoslinea2 = "[['Anos','Rechazados'],";
                String StrDatoslinea3 = "[['Anos','Sin Aprobar'],";

                String StrDatoslineatiempo = "[";
                DataTable datospasteles = new DataTable();
                DataTable datospasteles1 = new DataTable();
                DataTable datoslieandetiempo = new DataTable();
                DataTable datosliea = new DataTable();
                DataTable datosliea2 = new DataTable();
                DataTable datosliea3 = new DataTable();

                SqlConnection conn = new SqlConnection("Data source=.;initial catalog=LaboratorioReserva;MultipleActiveResultSets=True;Integrated Security=true;");
                try
                {
                    conn.Open();
                    //pastel
                    SqlCommand cmd = new SqlCommand("select SALAS.Salas,COUNT(SOLICITUD.SolicitudId) as total from SOLICITUD, SALAS where SALAS.SalasId = SOLICITUD.SalasId group by (SALAS.Salas)", conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    datospasteles.Load(rdr);
                    foreach (DataRow recorrer in datospasteles.Rows)
                    {
                        StrDatospastel += "[";
                        StrDatospastel += "'" + recorrer[0] + "'," + recorrer[1];
                        StrDatospastel += "],";
                    }
                    StrDatospastel += "]";
                    rdr.Close();

                    //pastel1
                    SqlCommand cmd3 = new SqlCommand("SELECT SOLICITUD.Confirmacion,COUNT(SOLICITUD.SolicitudId) FROM SOLICITUD GROUP BY(SOLICITUD.Confirmacion)", conn);
                    SqlDataReader rdr3 = cmd3.ExecuteReader();
                    datospasteles1.Load(rdr3);
                    foreach (DataRow recorrer in datospasteles1.Rows)
                    {
                        StrDatospastel1 += "[";
                        StrDatospastel1 += "'" + recorrer[0] + "'," + recorrer[1];
                        StrDatospastel1 += "],";
                    }
                    StrDatospastel1 += "]";
                    rdr3.Close();

                    //linea de tiempo
                    SqlCommand cmd1 = new SqlCommand("select SALAS.Salas,SOLICITUD.FechaInicio,SOLICITUD.FechaFinal FROM SOLICITUD,SALAS WHERE SOLICITUD.SalasId=SALAS.SalasId AND SOLICITUD.Confirmacion='EN EJECUCION' ", conn);
                    SqlDataReader rdr1 = cmd1.ExecuteReader();
                    datoslieandetiempo.Load(rdr1);
                    foreach (DataRow recorrer in datoslieandetiempo.Rows)
                    {
                        StrDatoslineatiempo += "[";
                        StrDatoslineatiempo += "'" + recorrer[0] + "',new Date(" + Convert.ToDateTime(recorrer[1]).Year + "," + Convert.ToDateTime(recorrer[1]).Month + "," + Convert.ToDateTime(recorrer[1]).Day + "),new Date(" + Convert.ToDateTime(recorrer[2]).Year + "," + Convert.ToDateTime(recorrer[2]).Month + "," + Convert.ToDateTime(recorrer[2]).Day + ")";
                        StrDatoslineatiempo += "],";
                    }
                    StrDatoslineatiempo += "]";
                    rdr1.Close();



                    //LINEA
                    SqlCommand cmd2 = new SqlCommand("select SOLICITUD.FechaFinal, count(SOLICITUD.SolicitudId) as confirmacion from SOLICITUD where SOLICITUD.Confirmacion = 'EN EJECUCION' group by SOLICITUD.FechaFinal, SOLICITUD.Confirmacion ", conn);
                    SqlDataReader rdr2 = cmd2.ExecuteReader();
                    datosliea.Load(rdr2);
                    foreach (DataRow recorrer in datosliea.Rows)
                    {
                        StrDatoslinea += "[";
                        StrDatoslinea += "'" + Convert.ToDateTime(recorrer[0]).Year + "'," + recorrer[1];
                        StrDatoslinea += "],";
                    }
                    StrDatoslinea += "]";
                    rdr2.Close();

                    //LINEA2
                    SqlCommand cmd4 = new SqlCommand("select SOLICITUD.FechaFinal, count(SOLICITUD.SolicitudId) as confirmacion from SOLICITUD where SOLICITUD.Confirmacion = 'EN ESPERA' group by SOLICITUD.FechaFinal, SOLICITUD.Confirmacion ", conn);
                    SqlDataReader rdr4 = cmd4.ExecuteReader();
                    datosliea2.Load(rdr4);
                    foreach (DataRow recorrer in datosliea2.Rows)
                    {
                        StrDatoslinea2 += "[";
                        StrDatoslinea2 += "'" + Convert.ToDateTime(recorrer[0]).Year + "'," + recorrer[1];
                        StrDatoslinea2 += "],";
                    }
                    StrDatoslinea2 += "]";
                    rdr4.Close();

                    //LINEA3
                    SqlCommand cmd5 = new SqlCommand("select SOLICITUD.FechaFinal, count(SOLICITUD.SolicitudId) as confirmacion from SOLICITUD where SOLICITUD.Confirmacion = 'TERMINADO' group by SOLICITUD.FechaFinal, SOLICITUD.Confirmacion ", conn);
                    SqlDataReader rdr5 = cmd5.ExecuteReader();
                    datosliea3.Load(rdr5);
                    foreach (DataRow recorrer in datosliea3.Rows)
                    {
                        StrDatoslinea3 += "[";
                        StrDatoslinea3 += "'" + Convert.ToDateTime(recorrer[0]).Year + "'," + recorrer[1];
                        StrDatoslinea3 += "],";
                    }
                    StrDatoslinea3 += "]";
                    rdr4.Close();

                    conn.Close();

                    // return Json(Convert.ToChar(39), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    this.RegistrarAuditoria("Error", ex.Message, -1);
                    return RedirectToAction("UserDashBoard", "Home");
                }
                ViewBag.pastel = StrDatospastel;
                ViewBag.lineatiempo = StrDatoslineatiempo;
                ViewBag.linea = StrDatoslinea;
                ViewBag.linea2 = StrDatoslinea2;
                ViewBag.linea3 = StrDatoslinea3;
                ViewBag.pastel1 = StrDatospastel1;
                return View();
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


    }
}