
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;


namespace ImageMongoDb.Controllers
{
    public class ImageController : Controller
    {

   
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetImages()
        {
            return View();
        }

       
        [HttpGet]
        public JsonResult GetData()
        {

            //var res = from mod in _genericRepository.GetAll()
            //          select mod;
            //res.AsEnumerable<ImageToDBW>()
            //  return Json(_genericRepository.GetAll(), JsonRequestBehavior.AllowGet);

            var collection = GetCollection();
            var imgdoc = collection.Find(new BsonDocument()).ToList();
            //List<Image> imageList = new List<Image>();
            //foreach (var img in imagelist)
            //{
            //    imageList.Add(new Image
            //    {
            //        id = img.id,
            //        ImageName = img.ImageName,
            //        show = img.show

            //    });
            //}

            return Json(imgdoc.ToJson());
           
        }

        [HttpGet]
        public ActionResult GetImage()
        {
            return View();
        }

            public async Task<ActionResult> RetrieveImageFile(int id)
        {


            var filter = Builders<BsonDocument>.Filter.Eq("id", id);
            var collection = GetCollection();
           
         
            
           
            var imageDoc = await collection.FindAsync<ImageDocument>(filter).ConfigureAwait(false);
            byte[] cover = imageDoc.FirstOrDefault<ImageDocument>().ContentImage;
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }

        [DataContract]
        public class ChunkMetaData
        {
            [DataMember(Name = "uploadUid")]
            public string UploadUid { get; set; }
            [DataMember(Name = "fileName")]
            public string FileName { get; set; }
            [DataMember(Name = "contentType")]
            public string ContentType { get; set; }
            [DataMember(Name = "chunkIndex")]
            public long ChunkIndex { get; set; }
            [DataMember(Name = "totalChunks")]
            public long TotalChunks { get; set; }
            [DataMember(Name = "totalFileSize")]
            public long TotalFileSize { get; set; }
        }

        public class FileResult
        {
            public bool uploaded { get; set; }
            public string fileUid { get; set; }
        }

        [HttpPost]
        public ActionResult Submit(List<IFormFile>  files)
        {
            if (files != null)
            {
                TempData["UploadedFiles"] = GetFileInfo(files);
            }

            return RedirectToRoute("Demo", new { section = "upload", example = "result" });
        }

        public async Task<ActionResult> Save(List<IFormFile>  files)
        {
            // The Name of the Upload component is "files"
               if (files != null)
            {
                foreach (var file in files)
                {
                    string theFileName = Path.GetFileName(file.FileName);

                    //get the bytes from the content stream of the file
                    byte[] thePictureAsBytes = new byte[file.Length];
                   ImageDocument imagedocument = new ImageDocument();
                 //   imagedocument.ContentImage = thePictureAsBytes;
                    // Some browsers send file names with full path. This needs to be stripped.
                    //var fileName = Path.GetFileName(file.FileName);
                    //var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                 //   _imageRepository.Add(imagedocument);

                    //// it will be null
                    //var testProduct = await _imageRepository.GetById(imagedocument.Id);

                    // If everything is ok then:
              //      await _uow.Commit();

                    // The product will be added only after commit
                 //   testProduct = await _productRepository.GetById(product.Id);

                  //  return Ok(testProduct);
                    // The files are not actually saved in this demo
                    // file.SaveAs(physicalPath);
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    //  var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    var physicalPath = Path.Combine("", fileName);
                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public void AppendToFile(string fullPath, Stream content)
        {
            try
            {
                using (FileStream stream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (content)
                    {
                        content.CopyTo(stream);
                    }
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> ChunkSave(List<IFormFile>  files, string metaData)
        {
            if (metaData == null)
            {
                return await Save(files);
            }

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(metaData));
            var serializer = new DataContractJsonSerializer(typeof(ChunkMetaData));
            ChunkMetaData somemetaData = serializer.ReadObject(ms) as ChunkMetaData;
            string path = String.Empty;
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    //get the bytes from the content stream of the file
                    byte[] thePictureAsBytes = new byte[file.Length];
                    ImageDocument imagedocument = new ImageDocument();
                    imagedocument.ContentImage = thePictureAsBytes;

                    // Some browsers send file names with full path. This needs to be stripped.
                    //var fileName = Path.GetFileName(file.FileName);
                    //var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    //     _imageRepository.Add(imagedocument);

                    //// it will be null
                    //var testProduct = await _imageRepository.GetById(imagedocument.Id);

                    // If everything is ok then:
                    ///  await _uow.Commit();
                    ///  
                    var id = new ObjectId();
                    imagedocument.Id = Convert.ToString(id);
                    var collection = GetCollection();
                    collection.InsertOne(imagedocument.ToBsonDocument());
                }
            }

            FileResult fileBlob = new FileResult();
            fileBlob.uploaded = somemetaData.TotalChunks - 1 <= somemetaData.ChunkIndex;
            fileBlob.fileUid = somemetaData.UploadUid;

            return Json(fileBlob);
        }

        private IEnumerable<string> GetFileInfo(List<IFormFile>  files)
        {
            return
                from a in files
                where a != null
                select string.Format("{0} ({1} bytes)", Path.GetFileName(a.FileName), a.Length);
        }

        private IMongoCollection<BsonDocument> GetCollection()

        {
            MongoClient dbClient = new MongoClient("mongodb://api_user:api1234@localhost:27017/api_prod_db");
            var database = dbClient.GetDatabase("imageMongoDb");
            var collection = database.GetCollection<BsonDocument>("Images");
            return collection;
        }
    }
}
