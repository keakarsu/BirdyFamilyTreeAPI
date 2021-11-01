using BirdyAPI.Database;
using BirdyModel;
using BirdyModel.Database.Birdy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BirdyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirdController : ControllerBase
    {
        [HttpGet("{pId}")]
        public ActionResult<GenericResponseModel> GetAll(string pId)
        {
            // var loUserId = HelperMethods.GetApiUserIdFromToken(HttpContext.User.Identity);
            var loMyData = new List<BirdDto>();
            var loGenericResponse = new GenericResponseModel
            {
                Code = -1,
                Status = "Fail"
            };

            var loResult = GetData.GetAllBirdsByUserId(pId);

            if (!loResult.Any())
            {
                loGenericResponse.Message = "Kayıtlı kuş bulunamadı";
                return loGenericResponse;
            }

            loGenericResponse.Code = 200;
            loGenericResponse.Status = "OK";
            loGenericResponse.Data = loMyData;

            return loGenericResponse;
        }

        [HttpGet("{pId}/FamilyTree")]
        public ActionResult<GenericResponseModel> GetFamilyTreeOfBird(string pId)
        {
            // var loUserId = HelperMethods.GetApiUserIdFromToken(HttpContext.User.Identity);
            var loMyData = new List<BirdDto>();
            var loGenericResponse = new GenericResponseModel
            {
                Code = -1,
                Status = "Fail"
            };

            var loResult = GetData.GetBirdById(pId);

            if (loResult == null)
            {
                loGenericResponse.Message = "Kayıtlı kuş bulunamadı";
                return loGenericResponse;
            }

            var loAllBirds = GetData.GetAllBirdsByUserId(loResult.user_uuid.ToString());
            loResult.children = loAllBirds.FindAll(x => x.father_uuid == loResult.row_guid || x.mother_uuid == loResult.row_guid);
            loResult.parent = loAllBirds.FindAll(x => x.row_guid == loResult.father_uuid || x.row_guid == loResult.mother_uuid);

            foreach(var loMyParent in loResult.parent)
            {
                loMyParent.parent = loAllBirds.FindAll(x => x.row_guid == loMyParent.father_uuid || x.row_guid == loMyParent.mother_uuid);
            }



            loGenericResponse.Code = 200;
            loGenericResponse.Status = "OK";
            loGenericResponse.Data = loMyData;

            return loGenericResponse;
        }

        [HttpPut]
        public ActionResult<GenericResponseModel> Update([FromBody] Bird pObject)
        {
            var loUserId = HelperMethods.GetApiUserIdFromToken(HttpContext.User.Identity);
            var loGenericResponse = new GenericResponseModel
            {
                Code = -1,
                Status = "Fail",
                Message="Güncelleme Başarısız"
            };

            var loObj = GetData.GetBirdById(pObject.row_guid.ToString());

            if (loObj == null)
            {
                loGenericResponse.Message = "Kuş bulunamadı!";
                return loGenericResponse;
            }

            //var loOldObj = (Bird)loObj.Clone();

           
            
            loObj.identity_no = pObject.identity_no ?? loObj.identity_no;
            loObj.poultry_identity = pObject.poultry_identity ?? loObj.poultry_identity;
            loObj.identity_color = pObject.identity_color ?? loObj.identity_color;
            loObj.gender = pObject.gender ?? loObj.gender;
            loObj.physical_attribute = pObject.physical_attribute ?? loObj.physical_attribute;
            loObj.physical_property = pObject.physical_property ?? loObj.physical_property;
            loObj.flying_attribute = pObject.flying_attribute ?? loObj.flying_attribute;
            loObj.old_owner_name = pObject.old_owner_name ?? loObj.old_owner_name;
            loObj.story = pObject.story ?? loObj.story;
            loObj.mother_uuid = pObject.mother_uuid ?? loObj.mother_uuid;
            loObj.father_uuid = pObject.father_uuid ?? loObj.father_uuid;
            loObj.nickname = pObject.nickname ?? loObj.nickname;
            loObj.birth_date = pObject.birth_date ?? loObj.birth_date;
            loObj.is_active = pObject.is_active ?? loObj.is_active;
            loObj.is_deleted = pObject.is_deleted ?? loObj.is_deleted;
            loObj.row_update_date = DateTime.Now;
            loObj.row_update_user = loUserId;

            if (!Crud<Bird>.Update(loObj, out _, null)) return loGenericResponse;

            loGenericResponse.Code = 200;
            loGenericResponse.Status = "OK";
            loGenericResponse.Data = loObj;

            return loGenericResponse;
        }

        [HttpPost]
        public ActionResult<GenericResponseModel> Insert([FromBody] Bird pObject)
        {
            var loUserId = HelperMethods.GetApiUserIdFromToken(HttpContext.User.Identity);
            var loGenericResponse = new GenericResponseModel
            {
                Status = "Fail",
                Code = -1
            };
            pObject.row_create_date = DateTime.Now;
            pObject.row_create_user = loUserId;
            pObject.row_guid = Guid.NewGuid();
            pObject.is_deleted = false;
            pObject.is_active = true;
            var loResult = Crud<Bird>.Insert(pObject, out _);


            if (loResult > 0)
            {
                pObject.id = (int)loResult;
                loGenericResponse.Data = pObject;
                loGenericResponse.Status = "Ok";
                loGenericResponse.Code = 200;
            }
            else
            {
                loGenericResponse.Status = "Fail";
                loGenericResponse.Code = -1;
                loGenericResponse.Message = "Kullanıcı kaydedilemedi.";
            }

            return loGenericResponse;
        }
    }
}
