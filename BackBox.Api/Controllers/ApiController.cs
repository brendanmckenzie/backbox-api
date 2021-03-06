﻿using Dapper;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace BackBox.Api.Controllers
{
    public class ApiController : Controller
    {
        static Random random = new Random();

        static Guid? testSessionId;
        public static void TestOverrideSessionId(Guid id)
        {
            testSessionId = id;
        }

        static DateTime? testLastCheck;
        public static void TestOverrideLastCheck(DateTime lastCheck)
        {
            testLastCheck = lastCheck;
        }

        static SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["Content"].ConnectionString);
        }

        Guid GetId()
        {
            if (testSessionId.HasValue)
            {
                return testSessionId.Value;
            }

            if (Session["id"] == null) { throw new InvalidOperationException(); }

            return (Guid)Session["id"];
        }

        public Guid Connect()
        {
            if (testSessionId.HasValue)
            {
                GetConnection().Execute("delete from [Message] where [UserId] = @id; delete from [User] where [Id] = @id; insert into [User] ( Id, Connected ) values ( @id, getdate() );", new { id = testSessionId.Value });

                return testSessionId.Value;
            }

            if (Session["id"] == null)
            {
                var id = Guid.NewGuid();

                Session["id"] = id;

                var name = "Guest-" + random.Next(1000, 10000).ToString();

                GetConnection().Execute("insert into [User] ( Id, Connected, [Name] ) values ( @id, getdate(), @name );", new { id = id, name = name });
            }

            return (Guid)Session["id"];
        }

        public string SetName(string name)
        {
            GetConnection().Execute("update [User] set [Name] = @name where [Id] = @id", new { id = GetId(), name = name });

            return "ok, " + name;
        }

        public string SetBounds(double lat, double lng, int radius)
        {
            GetConnection().Execute(string.Format("update [User] set [Radius] = @radius, [Location] = geography::STGeomFromText('POINT({1} {0})', 4326) where [Id] = @id", lat, lng), new { id = GetId(), radius = radius });

            return string.Format("ok, i'll let you know about messages {2}km around ({0}, {1})", lat, lng, radius);
        }

        public Guid Send(double lat, double lng, string message)
        {
            var id = Guid.NewGuid();

            GetConnection().Execute(string.Format("insert into [Message] ( [Id], [UserId], [Timestamp], [Location], [Content] ) values ( @id, @userId, getdate(), geography::STGeomFromText('POINT({0} {1})', 4326), @content ) ", lng, lat), new { id, userId = GetId(), content = message });

            return id;
        }

        public string GetLatest()
        {
            const string Sql = @"
declare @location geography;
declare @radius int;

select
    @location = [Location],
    @radius = [Radius]
from
    [User]
where
    [Id] = @userId;

select
    M.[Id],
    M.[Timestamp],
    M.[Content],
    M.[Location].Lat as [Lat],
    M.[Location].Long as [Lng],
    U.Name as [User]
from
    [Message] M
    inner join [User] U on M.[UserId] = U.[Id] 
where
    (@timestamp is null or M.[Timestamp] > @timestamp)
    and abs(M.[Location].STDistance(@location)) < @radius
order by
    M.[Timestamp]";

            var ret = GetConnection().Query<Message>(Sql, new { timestamp = testLastCheck ?? Session["last_check"], userId = GetId() });

            Session["last_check"] = DateTime.Now;

            return Newtonsoft.Json.JsonConvert.SerializeObject(ret);
        }

        public HttpStatusCodeResult Invalid()
        {
            return new HttpStatusCodeResult(418, "I'm a teapot.");
        }

        public class Message
        {
            public Guid Id { get; set; }
            public string User { get; set; }
            public DateTime Timestamp { get; set; }
            public string Content { get; set; }
            public double Lat { get; set; }
            public double Lng { get; set; }
        }
    }
}
