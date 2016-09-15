using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Amazon.DynamoDBv2;
using Example.DataAccess;

namespace Example.Web.Controllers
{
    [RoutePrefix("food")]
    public class FoodController : ApiController
    {
        public TreeRepository TreeRepository { get; set; }
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();

        public FoodController()
        {
            TreeRepository = new TreeRepository(client);
        }

        [HttpGet]
        [Route("{foodName}")]
        public IHttpActionResult GetTreeList(string foodName)
        {
            return Json(TreeRepository.GetTreesByFoodNames("foodName"));
        }
    }
}
