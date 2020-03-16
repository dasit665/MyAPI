using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

using MyAPI.Controllers.SimpliFile.Models;

namespace MyAPI.Controllers.SimpliFile
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimpleFileController : ControllerBase
    {
        #region Controller an DB context initialization
        public sfsContext DB { get; set; }

        public SimpleFileController(sfsContext DB)
        {
            this.DB = DB;
        }
        #endregion


        #region Common API Testing
        //Common API Testing
        [HttpGet]
        public IActionResult OnGet()
        {
            return StatusCode(StatusCodes.Status200OK, "SimpliFileController is work, Method OnGet...");
        }
        #endregion


        #region AddFileWithProp
        [HttpPost]
        [Route("AddFileWithProp")]
        public async Task<long> AddFileWithProp([FromBody]byte[] file)
        {
            var propModel = new PropModel();
            var Props = typeof(PropModel).GetProperties().Select(s => s.Name).ToArray();

            foreach (var i in HttpContext.Request.Headers)
            {
                if (Props.Contains(i.Key) == true)
                {
                    var prop = typeof(PropModel).GetProperty(i.Key);

                    if (prop.PropertyType == typeof(long))
                    {
                        prop.SetValue(propModel, Convert.ToInt64(i.Value.ToString()));
                    }
                    else if (prop.PropertyType == typeof(int))
                    {
                        prop.SetValue(propModel, Convert.ToInt32(i.Value.ToString()));
                    }
                    else
                    {
                        prop.SetValue(propModel, i.Value.ToString());
                    }
                }
            }

            var extension = propModel.FileName.Split('.').Last();
            var isCompresed = DB.NotCompressionExtentions.Where(w => w.Extention == extension).FirstOrDefault() != null ? true : false;


            if (isCompresed==true)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    new GZipStream(ms, CompressionMode.Compress).Read(file);
                    DB.Files.Add(new Files()
                    {
                        Id = propModel.FileId,
                        CreateDate = DateTime.Now,
                        FileData = ms.GetBuffer(),
                        IsCompress = true,
                        SystemId = propModel.SystemId,
                    });
                    await DB.SaveChangesAsync();
                }
            }

            else
            {
                DB.Files.Add(new Files()
                {
                    Id = propModel.FileId,
                    FileData = file,
                    CreateDate = DateTime.Now,
                    IsCompress = false,
                    SystemId = propModel.SystemId
                });
                DB.SaveChanges();
            }

            DB.FilesProp.Add(new FilesProp()
            {
                FileId = propModel.FileId,
                UserName = propModel.UserName,
                FileName = propModel.FileName,
                Prop1 = propModel.Prop1,
                Prop2 = propModel.Prop2,
                Prop3 = propModel.Prop3,
                Prop4 = propModel.Prop4,
                Prop5 = propModel.Prop5,
                Prop6 = propModel.Prop6,
                Prop7 = propModel.Prop7,
                Prop8 = propModel.Prop8,
                Prop9 = propModel.Prop9,
                Prop10 = propModel.Prop10
            });
            DB.SaveChanges();

            return propModel.FileId;
        }
        #endregion


        #region GetFileById/{id}
        [HttpGet]
        [Route("GetFileById/{id}")]
        public IActionResult GetFileById(long id)
        {
            var file = DB.Files.Where(w => w.Id == id).FirstOrDefault();

            if (file is null)
            {
                return NotFound($"file id:{id} not found");
            }
            else
            {
                var fileName = DB.FilesProp.Where(w => w.FileId == id).FirstOrDefault().FileName;

                if (file.IsCompress.Value == true)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        new GZipStream(ms, CompressionMode.Decompress).Read(file.FileData);
                        return File(ms.GetBuffer(), "application/octet-stream", fileName);
                    }
                }
                else
                {
                    return File(file.FileData, "application/octet-stream", fileName);
                }
            }
        }
        #endregion


        #region GetPropById/{id}
        [HttpGet]
        [Route("GetPropById/{id}")]
        public PropModel GetPropById(long id)
        {
            HttpContext.Response.Headers.Add("Content-Type", "application/json");

            var res = (from pMod in DB.FilesProp
                       where pMod.FileId == id
                       select new PropModel
                       {
                           FileId = pMod.FileId,
                           FileName = pMod.FileName,
                           SystemId = default,
                           UserName = pMod.UserName,
                           Prop1 = pMod.Prop1,
                           Prop2 = pMod.Prop2,
                           Prop3 = pMod.Prop3,
                           Prop4 = pMod.Prop4,
                           Prop5 = pMod.Prop5,
                           Prop6 = pMod.Prop6,
                           Prop7 = pMod.Prop7,
                           Prop8 = pMod.Prop8,
                           Prop9 = pMod.Prop9,
                           Prop10 = pMod.Prop10,
                       }).FirstOrDefault();

            return res;
        }
        #endregion


        #region GetSystemInfo
        [HttpGet]
        [Route("GetSystemInfo")]
        public List<RSystem> GetSystemInfo()
        {
            HttpContext.Response.Headers.Add("contenttype", "application/json");

            return DB.RSystem.Select(s => s).ToList();
        }
        #endregion


        #region UpdateFileWithProp/{id}
        [HttpPut]
        [Route("UpdateFileWithProp/{id}")]

        public IActionResult UpdateFileWithProp(long id, [FromBody]byte[] file)
        {
            var propModel = new PropModel();
            var props = typeof(PropModel).GetProperties().Select(s => s.Name).ToArray();

            foreach (var i in HttpContext.Request.Headers)
            {
                if (props.Contains(i.Key))
                {
                    var Prop = typeof(PropModel).GetProperty(i.Key);

                    if (Prop.PropertyType == typeof(int))
                    {
                        Prop.SetValue(propModel, Convert.ToInt32(i.Value.ToString()));
                    }
                    else if (Prop.PropertyType == typeof(long))
                    {
                        Prop.SetValue(propModel, Convert.ToInt64(i.Value.ToString()));
                    }
                    else
                    {
                        Prop.SetValue(propModel, i.Value.ToString());
                    }
                }
            }

            var UpdateProp = DB.FilesProp.Where(w => w.FileId == id).FirstOrDefault();
            if (UpdateProp != null)
            {
                UpdateProp.FileName = propModel.FileName;
                UpdateProp.UserName = propModel.UserName;
                UpdateProp.Prop1 = propModel.Prop1;
                UpdateProp.Prop2 = propModel.Prop2;
                UpdateProp.Prop3 = propModel.Prop3;
                UpdateProp.Prop4 = propModel.Prop4;
                UpdateProp.Prop5 = propModel.Prop5;
                UpdateProp.Prop6 = propModel.Prop6;
                UpdateProp.Prop7 = propModel.Prop7;
                UpdateProp.Prop8 = propModel.Prop8;
                UpdateProp.Prop9 = propModel.Prop9;
                UpdateProp.Prop10 = propModel.Prop10;
                DB.SaveChanges();
            }

            var UpdateFile = DB.Files.Where(w => w.Id == id).FirstOrDefault();
            if (UpdateFile != null)
            {
                var extension = propModel.FileName.Split('.').Last();
                var isCompresed = DB.NotCompressionExtentions.Where(w => w.Extention == extension).FirstOrDefault() != null ? true : false;

                UpdateFile.SystemId = propModel.SystemId;
                UpdateFile.CreateDate = DateTime.Now;
                UpdateFile.IsCompress = isCompresed;

                if (isCompresed)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        new GZipStream(ms, CompressionMode.Compress).Read(file);
                        UpdateFile.FileData = ms.GetBuffer();
                    }
                }
                else
                {
                    UpdateFile.FileData = file;
                }

                DB.SaveChanges();

            }

            return Ok(id.ToString());
        }
        #endregion


        #region "DeleteFile/{id}"
        [HttpDelete]
        [Route("DeleteFile/{id}")]
        public IActionResult DeleteFile(long id)
        {
            var file = DB.Files.Where(w => w.Id == id).FirstOrDefault();
            var props = DB.FilesProp.Where(w => w.FileId == id).FirstOrDefault();

            if (file != null && props != null)
            {
                DB.Remove(file);
                DB.Remove(props);

                DB.SaveChanges();

                return Ok($"id:{id} has been deleted...");
            }
            else
            {
                return BadRequest($"id:{id} was not deleted...");
            }

        }
        #endregion
    }
}