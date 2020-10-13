using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wikibot.DataAccess;

namespace Wikibot.App.Controllers
{
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "BotAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private RequestData _requestData;
        public RequestController(IDataAccess dataAccess)
        {
            _requestData = new RequestData(dataAccess);
        }
        //Get Requests
        [HttpGet("requests")]
        public IActionResult GetRequests()
        {
            var list = _requestData.GetWikiJobRequestsWithPages(1, 100, "ASC", "ID");
            return new OkObjectResult(list);
        }

        //Pre Approve Request
        [HttpPost("preapprove")]
        public IActionResult PreApproveRequest(int requestId)
        {
            _requestData.UpdateStatus(requestId, JobStatus.PreApproved);
            return new OkObjectResult("Request status successfully updated");
        }

        //Approve Request
        [HttpPost("approve")]
        public IActionResult ApproveRequest(int requestId)
        {
            _requestData.UpdateStatus(requestId, JobStatus.Approved);
            return new OkObjectResult("Request status successfully updated");   
        }
        //Reject Request
        [HttpPost("reject")]
        public IActionResult RejectRequest(int requestId)
        {
            _requestData.UpdateStatus(requestId, JobStatus.Rejected);
            return new OkObjectResult("Request status successfully updated");
        }
    }
}
