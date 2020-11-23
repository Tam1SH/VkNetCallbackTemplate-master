using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Utils;
namespace VkBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        public delegate void VKHandler(Message msg);

        static public event VKHandler VKMessageAction_New;

        private readonly IConfiguration _configuration;

        private readonly IVkApi _vkApi;

        public CallbackController(IVkApi vkApi, IConfiguration configuration)
        {
            _vkApi = vkApi;
            _configuration = configuration;
            MainLogic.GetInstance(vkApi);

        }

        [HttpPost]
        public IActionResult Callback([FromBody] Updates updates)
        {
            // Тип события
            switch (updates.Type)
            {
                case "confirmation":
                    return Ok(_configuration["Config:Confirmation"]);


                case "message_new":
                    VKMessageAction_New(Message.FromJson(new VkResponse(updates.Object)));
                    break;

            }

            return Ok("ok");
        }

    }
    
}
