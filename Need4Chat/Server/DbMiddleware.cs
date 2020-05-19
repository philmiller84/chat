using Microsoft.EntityFrameworkCore.Internal;
using Need4Chat.Server.Models;
using Need4Chat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Need4Chat.Server
{
    public class DbMiddleware
    {

        public DbMiddleware()
        {
            //InitializeData();
        }

        private string GetEncodedHash(string password, string salt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
            return base64digest.Substring(0, base64digest.Length - 2);
        }

        public bool TryLoginAttempt(User t)
        {
            try
            {
                if (t.Name.Length > 20 || t.Password.Length > 20)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            database1Context db = new database1Context();
            IQueryable<string> q = from u in db.User where u.Name == t.Name select u.Password;

            string salt = "WHAT IS THE NAME OF THE ";
            string encodedPassword = GetEncodedHash(t.Password, salt);

            if (q.Count() > 0)
            {
                return q.Single() == encodedPassword;
            }
            else
            {
                t.Password = encodedPassword;
                db.User.Add(t);
                // Console.WriteLine("Sending to DB: {0}", t.ToString());
                db.SaveChanges();
                return true;
            }


        }
        public IEnumerable<User> GetRegisteredUsers()
        {
            database1Context db = new database1Context();
            IOrderedQueryable<User> q = from r in db.User orderby r.Name ascending select r;
            return q.ToList<User>();
        }


        public bool AddMessage(ChatMessage msg)
        {
            if (msg.Body.Length > 200)
            {
                return false;
            }

            database1Context db = new database1Context();
            IQueryable<Guid> q = from r in db.User where r.Name == msg.Username select r.Id;

            if (q.Count() < 1)
            {
                return false;
            }

            Guid userID = q.FirstOrDefault();

            Message t = new Message() { User = userID, Text = msg.Body };
            db.Message.Add(t);
            //Console.WriteLine("Sending to DB: {0}", t.ToString());
            db.SaveChanges();

            return true;
        }

        public IEnumerable<ChatMessage> GetMessages()
        {
            database1Context db = new database1Context();
            IQueryable<ChatMessage> q = from r in db.Message
                                        join u in db.User on r.User equals u.Id
                                        orderby r.Timestamp ascending
                                        select new ChatMessage { Username = u.Name, Body = r.Text, DateAndTime = r.Timestamp };
            return q.ToList<ChatMessage>();
        }

        //private void InitializeData()
        //{
        //	var messageRecords = db.Message;
        //	db.Message.RemoveRange(messageRecords);
        //}
    }
}
