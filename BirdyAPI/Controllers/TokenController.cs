using BirdyAPI.Database;
using BirdyModel;
using Microsoft.AspNetCore.Cors;
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
    [EnableCors("AllowOrigin")]
    public class TokenController : ControllerBase
    {
        [HttpPost]
        public ActionResult<GenericResponseModel> Token([FromBody] ApiUser user)
        {
            var loUserId = GetData.CheckApiKey(user.api_key);

            if (loUserId <= 0) return Unauthorized();

            if (string.IsNullOrEmpty(user.language))
            {
                user.language = "tr";
            }

            var loToken = HelperMethods.GenerateToken(loUserId.ToString(), 0);
            var loGeneratedToken = new GeneratedToken
            {
                token = loToken,
                generation_time = DateTime.Now,
                api_user_id = loUserId
            };

            //Crud<GeneratedToken>.Insert(loGeneratedToken);
            return Ok(new { token = loGeneratedToken });
        }
    }
}
