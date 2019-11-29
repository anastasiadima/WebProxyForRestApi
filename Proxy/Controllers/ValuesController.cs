using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Proxy.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     public class ValuesController : ControllerBase
     {
          static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

          static readonly IDatabase db = redis.GetDatabase();

          // GET api/values
          [HttpGet]
          public ActionResult<string> Get()
          {
               var isUpToDate = db.StringGet("isUpToDate");
               if (isUpToDate.HasValue && isUpToDate.ToString() == "yes")
               { 
                    //luam din redis
                    return db.StringGet("book").ToString();
               }
                //returnam valoarea din mongo
               return db.StringGet("book").ToString();
          }

          // GET api/values/5
          [HttpGet("{id}")]
          public ActionResult<string> Get(int id)
          {
               db.StringSet("isUpToDate", "no");
               db.StringSet("book", "Inima de aur 1");
               return db.StringGet("book").ToString();
          }

          // POST api/values
          [HttpPost]
          public void Post([FromBody] string value)
          {
          }

          // PUT api/values/5
          [HttpPut("{id}")]
          public void Put(int id, [FromBody] string value)
          {
          }

          // DELETE api/values/5
          [HttpDelete("{id}")]
          public void Delete(int id)
          {
          }
     }
}
