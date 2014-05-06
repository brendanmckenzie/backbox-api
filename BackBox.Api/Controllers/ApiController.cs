using Dapper;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace BackBox.Api.Controllers
{
    public class ApiController : Controller
    {
        static SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["Content"].ConnectionString);
        }

        Guid GetId()
        {
            if (Session["id"] == null) { throw new InvalidOperationException(); }

            return (Guid)Session["id"];
        }

        public Guid Connect()
        {
            if (Session["id"] == null)
            {
                var id = Guid.NewGuid();

                Session["id"] = id;

                GetConnection().Execute("insert into [User] ( Id, Connected ) values ( @id, getdate() );", new { id = id });
            }

            Response.ContentType = "text/plain";

            return (Guid)Session["id"];
        }

        public string SetName(string name)
        {
            Response.ContentType = "text/plain";

            GetConnection().Execute("update [User] set [Name] = @name where [Id] = @id", new { id = GetId(), name = name });

            return "ok, " + name;
        }

        public string SetBounds(double lat, double lng, int radius)
        {
            Response.ContentType = "text/plain";

            GetConnection().Execute("update [User] set [Radius] = @radius, [Location] = (@lat, @lng) where [Id] = @id", new { id = GetId(), radius = radius, lat = lat, lng = lng });

            return string.Format("ok, i'll let you know about messages {2}km around ({0}, {1})", lat, lng, radius);
        }

        public Guid Send(string message, double lat, double lng)
        {
            Response.ContentType = "text/plain";

            var id = Guid.NewGuid();

            GetConnection().Execute("insert into [Message] ( [Id], [UserId], [Timestamp], [Content] ) values ( @id, @userId, getdate(), @content ) ", new { id, userId = GetId(), content = message });

            return id;
        }

        public string GetLatest()
        {
            var user = GetConnection().Query<User>("select [Id], [Name], [Location] from [User] where [Id] = @id", new { id = GetId() }).First();

            var ret = GetConnection().Query<Message>("select [Id], [Timestamp], [Content] from [Message] where @timestamp is null or [Timestamp] > @timestamp", new { timestamp = Session["last_check"] });

            Response.ContentType = "text/plain";

            return Newtonsoft.Json.JsonConvert.SerializeObject(ret);
        }

        public HttpStatusCodeResult Invalid()
        {
            return new HttpStatusCodeResult(418, "I'm a teapot.");
        }

        public class User
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Message
        {
            public Guid Id { get; set; }
            public User User { get; set; }
            public DateTime Timestamp { get; set; }
            public string Content { get; set; }
        }
    }
}
