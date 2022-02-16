using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prospecto.Data;
using Prospecto.Models;
using Prospecto.Repositorio;
using static Prospecto.Enums.Enums;

namespace Prospecto.Controllers
{
    [Authorize]
    public class CProspectoController : BaseController
    {
        private readonly CRepository<ProspectoModel> gRepository;
        private readonly CRepository<ColoniaModel> cRepository;
        private readonly CRepository<CalleModel> lRepository;

        IHttpContextAccessor _httpContextAccessor;

        string[] lParam = { };
        string[] lVar = { };
        string[] lParametros = { "@Opc", "@Id","@Nombre","@PrimerApellido","@SegundoApellido","Calle",
                                "@Numero","@Colonia","@Telefono","@RFC" };

        public CProspectoController(ConnectionConfiguration _configuration, IHttpContextAccessor httpContextAccessor)
        {
            gRepository = new CRepository<ProspectoModel>(_configuration);
            cRepository = new CRepository<ColoniaModel>(_configuration);
            lRepository = new CRepository<CalleModel>(_configuration);
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index(string searchString = "@")
        {
            lParam = new string[] { "@Opc", "@Nombre" };
            lVar = new string[] { "2", searchString };
            IEnumerable<ProspectoModel> Lista = await gRepository.BDLista("c_spProspecto", lParam, lVar);

            return View("Index", Lista);
        }


        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            ProspectoModel model = new ProspectoModel();
            ViewBag.Id = null;

            if (id.HasValue) //cuando se va editar uno existente
            {
                lParam = new string[] { "@Opc", "@Id" };
                lVar = new string[] { "3", id.Value.ToString() };
                model = await gRepository.BDBuscarReg("c_spProspecto", lParam, lVar);

                ViewBag.Id = model.ID;
                model.SCol1 = model.ColoniaNom;

                string NomRol = _httpContextAccessor.HttpContext.User.FindFirst("Role").Value.ToString().Trim();

                ViewBag.Status = model.Estatus == "Autorizado" || model.Estatus == "Rechazado" || NomRol=="PROMOTOR" ? "T" : "";
                if (ViewBag.Status == "T")
                {
                    ViewBag.StatusText = model.Estatus == "Autorizado" ? "- A U T O R I Z A D O -" : model.Estatus == "Rechazado"?"- R E C H A Z A D O - : " + model.Comentario.Trim():"";
                }
 
                GetCalle(model, model.Colonia);
            }
            return View("~/Views/CProspecto/AddEdit.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit2(int? id)
        {
            ProspectoModel model = new ProspectoModel();
            ViewBag.Id = null;

            if (id.HasValue) //cuando se va editar uno existente
            {
                lParam = new string[] { "@Opc", "@Id" };
                lVar = new string[] { "3", id.Value.ToString() };
                model = await gRepository.BDBuscarReg("c_spProspecto", lParam, lVar);

                ViewBag.Id = model.ID;
                ViewBag.ColoniaNom = model.SCol1.Trim();
                ViewBag.CalleNom = model.CalleNom.Trim();

                string NomRol = _httpContextAccessor.HttpContext.User.FindFirst("Role").Value.ToString().Trim();

                ViewBag.Status = model.Estatus == "Autorizado" || model.Estatus == "Rechazado" || NomRol == "PROMOTOR" ? "T" : "";
                if (ViewBag.Status == "T")
                {
                    ViewBag.StatusText = model.Estatus == "Autorizado" ? "- A U T O R I Z A D O -" : model.Estatus == "Rechazado" ? "- R E C H A Z A D O - : " + model.Comentario.Trim() : "";
                }

                GetCalle(model, model.Colonia);
            }
            return View("~/Views/CProspecto/Autorizar.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(ProspectoModel cust, List<IFormFile> files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    lVar = LlenarAray(cust);
                    int idl=await gRepository.BDAddAsync("c_spProspecto", lParametros, lVar);//Guarda Dato nuevo o Modificado
                    if (idl != 0)
                    {
                        await GuardarArchivo(files,idl);
                    }
                    Alert("El Dato Fue Guardado", NotificationType.success);
                    return RedirectToAction("Index");
                }
                return View("~/Views/CProspecto/AddEdit.cshtml", cust);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred...",ex);
            }
        }
        private async Task GuardarArchivo(List<IFormFile> files,int idx)
        {
            try
            {
                foreach (IFormFile aformFile in files)
                {
                    var formFile = aformFile;
                    if (formFile.Length > 0)
                    {
                        //Subir archivo a la base de datos
                        //Obtener nombre de archivo
                        var filename = Path.GetFileName(formFile.FileName);
                        //Obtener extensión de archivo  
                        var fileextension = Path.GetExtension(filename);
                        // concatenar nombre de archivo y extensión de archivo   
                        var newfilename = filename;
                            //String.Concat(Convert.ToString(Guid.NewGuid()), fileextension);
                        var documentViewmodel = new DocumentModel()
                        {
                            Id = 0,
                            FileName = newfilename,
                            FileType = fileextension,
                            Created = DateTime.Now,
                            Modified = DateTime.Now,
                            IdProspecto = idx
                        };
                        using (var target = new MemoryStream())
                        {
                            formFile.CopyTo(target);
                            documentViewmodel.FileData = target.ToArray();
                        }

                        await gRepository.BDAddFileAsync(documentViewmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred...", ex);
            }
        }
        public async Task<IActionResult> Autorizado(int id)
        {
            lParam = new string[] { "@Opc","@Id", };
            lVar = new string[] { "4",id.ToString() };
            await gRepository.BDAddAsync("c_spProspecto", lParam, lVar);
            var result = new { param1 = "Ok" };
            return Json(result);
        }

        public IActionResult Rechazar(int id)
        {
            ProspectoModel model = new ProspectoModel();
            model.ID = id;
            return PartialView("~/Views/CProspecto/_ProsRechazo.cshtml", model);
        }

        [HttpPost]
        public ActionResult AutoComplete(string Prefix)
        {
            ProspectoModel R = new ProspectoModel();
            if (!String.IsNullOrEmpty(Prefix))
            {
                return Json(GetAuto(R, Prefix, 2));
            }
            else
            {
                return Json(GetAuto(R, "", 2));
            }
        }
        public async Task<IActionResult> GuardarComR_Accion(string ID, string comentario)
        {
            lParam = new string[] { "@Opc","@Id","@Nombre" };
            lVar = new string[] { "5", ID.Trim(), comentario.ToUpper() };
            await gRepository.BDAddAsync("c_spProspecto", lParam, lVar);
            return Json("SAVE");
        }

        List<ColoniaModel> GetAuto(ProspectoModel model, string ParamFijo, int opc)
        {
            string[] lParametros = { "@Opc", "@Nom" };
            string[] lVariables = { opc.ToString(), ParamFijo.Trim() };
            var util = cRepository.BDListaCombo("c_spColonia", lParametros, lVariables);

            return util.ToList();
        }

        public ActionResult Llenar_Calle(string col, int opc)
        {
            ProspectoModel R = new ProspectoModel();
            return Json(GetCalle(R, col, opc));
        }
        IEnumerable<SelectListItem> GetCalle(ProspectoModel model, string lxcol, int Opc = 1)
        {
            var Combol = new List<SelectListItem>();

            if (lxcol != null)
            {
                string[] lParametros = { "@Opc", "@CodigoCol" };
                string[] lVariables = { "2", lxcol.Trim() };
                var cal = lRepository.BDListaCombo("c_spCalle", lParametros, lVariables);

                var listacal = cal.ToList();

                foreach (var Data in listacal)
                {
                    model.Cal1.Add(new SelectListItem()
                    {
                        Value = Data.CodigoCalle,
                        Text = Data.Nombre
                    });
                }
            }
            return model.Cal1;
        }
        //------------------------------------------
        //--------------------Nueva Calle
        public IActionResult AddCalle()
        {
            CalleModel model = new CalleModel();
            //ViewBag.idCol = id;
            return PartialView("~/Views/CProspecto/_NewCalle.cshtml", model);
        }
        public async Task<ActionResult> NewCalleAjax(string lcol, string lcalle)
        {
            lParam = new string[] { "@Opc", "@Cod", "@Nom", "@CodigoCol" };
            lVar = new string[] { "3", "0", lcalle, lcol.PadLeft(5,'0') };
            await gRepository.BDAddAsync("c_spCalle", lParam, lVar);//Guarda Dato nuevo o Modificado
            return Json("SAVE");
        }
        string[] LlenarAray(ProspectoModel item)
        {
            string[] lcampos = {
            "1",item.ID.ToString(),item.Nombre,item.PrimerApellido,item.SegundoApellido is null?"":item.SegundoApellido,
            item.Calle,item.Numero,item.Colonia.PadLeft(5,'0'),item.Telefono,item.RFC
            };
            return lcampos;
        }

        //---------------Tarabajo con Archivos
        [HttpPost]
        public IActionResult UploadToFileSystem(IFormFile files, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            string Ruta = $"{webHostEnvironment.WebRootPath}\\files\\";
            bool basePathExists = System.IO.Directory.Exists(Ruta);
            if (!basePathExists) Directory.CreateDirectory(Ruta);
            string filename = Path.Combine(Ruta, files.FileName);

            using (FileStream fileStream = System.IO.File.Create(filename))
            {
                files.CopyTo(fileStream);
                fileStream.Flush();
            }

            //List<SolicitudMov> movExcel = GetMovEx(files.FileName);

            //return View("~/Areas/CreditoYCobranza/Views/CobranzaC/Nomina.cshtml", movExcel);
            return View("Index");
        }
        //private List<SolicitudMov> GetMovEx(string fname)
        //{
        //    List<SolicitudMov> mov = new List<SolicitudMov>();
        //    var filename = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fname;
        //    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //    using (var stream = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read))
        //    {
        //        using (var reader = ExcelReaderFactory.CreateReader(stream))
        //        {
        //            var conf = new ExcelDataSetConfiguration
        //            {
        //                ConfigureDataTable = _ => new ExcelDataTableConfiguration
        //                {
        //                    UseHeaderRow = true
        //                }
        //            };
        //            var dataSet = reader.AsDataSet(conf);
        //            var dataTable = dataSet.Tables[0];
        //            var lReng = 0;
        //            for (var i = 1; i < dataTable.Rows.Count; i++)
        //            {
        //                if (dataTable.Rows[i][0].ToString().Length > 0)
        //                {
        //                    mov.Insert(lReng, new SolicitudMov
        //                    {
        //                        Sucursal = dataTable.Rows[i][0].ToString().Trim(),
        //                        Serie = dataTable.Rows[i][1].ToString().Trim(),
        //                        Folio = dataTable.Rows[i][2].ToString().Substring(1, 8).Trim(),
        //                        Titular = dataTable.Rows[i][3].ToString().Trim(),
        //                        Importe = Convert.ToDecimal(dataTable.Rows[i][4].ToString().Trim())
        //                    });
        //                    lReng = +1;
        //                }
        //            }
        //            reader.Close();
        //        }

        //    }
        //    return mov;
        //}

    }
}
