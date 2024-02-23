using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Deltabyte.Mail
{
    public class Field
    {
        #region Properties
        public String User { get; set; }
        public String Host { get; set; }
        public String Date { get; set; }
        public String Title { get; set; }
        public String Errno { get; set; }
        public String Append { get; set; }
        public String System { get; set; }
        public String Domain { get; set; }
        public String Content { get; set; }
        public List<String> IpV4 { get; set; }
        public List<String> IpV6 { get; set; }
        #endregion

        #region Constructor
        public Field()
        {
            Init();
        }

        public Field(String s)
        {
            Init();
            Content = s;
        }

        public Field(String s, String t)
        {
            Init();
            Title = t;
            Content = s;
        }

        public Field(String s, String t, String a)
        {
            Init();
            Title = t;
            Content = s;
            Append = a;
        }

        private void Init()
        {
            User = Environment.UserName;
            Host = Environment.MachineName;
            Date = DateTime.Now.ToString();
            System = Environment.OSVersion.ToString();
            Domain = Environment.UserDomainName;
            Title = null;
            Errno = null;
            Append = null;
            Content = null;
            IpV4 = new List<string>();
            IpV6 = new List<string>();
        }
        #endregion

        #region Methods
        #endregion
    }

    public class Data : IDisposable
    {
        #region Properties
        #region Public
        /// <summary>
        /// error occured
        /// </summary>
        public Boolean Error { get; private set; } = false;
        /// <summary>
        /// error message
        /// </summary>
        public String Text { get; private set; } = String.Empty;
        /// <summary>
        /// mail server name
        /// </summary>
        public String Server { get; set; } = String.Empty;
        /// <summary>
        /// mail server login user
        /// </summary>
        public String User { get; set; } = String.Empty;
        /// <summary>
        /// mail server password
        /// </summary>
        public String Password { get; set; } = String.Empty;
        /// <summary>
        /// mail receiver
        /// </summary>
        public String To { get; set; } = String.Empty;
        /// <summary>
        /// mail sender
        /// </summary>
        public String From { get; set; } = String.Empty;
        /// <summary>
        /// mail subject
        /// </summary>
        public String Subject { get; set; } = String.Empty;
        /// <summary>
        /// mail body
        /// </summary>
        public String Body { get; set; } = String.Empty;
        /// <summary>
        /// mail server port, default=587
        /// </summary>
        public int Port { get; set; } = 587;
        /// <summary>
        /// list of image paths
        /// </summary>
        public List<String> Images { get; set; } = new List<string>();
        /// <summary>
        /// mail server connection timeout, default=10000
        /// </summary>
        public int Timeout { get; set; } = 10000;
        public Field Fields { get; set; }
        #endregion

        #region Private
        private MailMessage Message { get; set; } = default(MailMessage);
        private SmtpClient Client { get; set; } = default(SmtpClient);
        #endregion
        #endregion

        #region Constructor
        public Data()
        {
            global::System.Net.ServicePointManager.SecurityProtocol = global::System.Net.SecurityProtocolType.Tls12;
            Fields = new Field();
        }
        public Data(Data d)
        {
            global::System.Net.ServicePointManager.SecurityProtocol = global::System.Net.SecurityProtocolType.Tls12;
           // global::System.Net.ServicePointManager.SecurityProtocol = global::System.Net.SecurityProtocolType.
            Error = d.Error;
            Text = d.Text;
            Server = d.Server;
            User = d.User;
            Password = d.Password;
            To = d.To;
            From = d.From;
            Subject = d.Subject;
            Body = d.Body;
            Port = d.Port;
            Images = new List<string>(d.Images);
            Timeout = d.Timeout;
            Fields = d.Fields;
        }
        #endregion

        #region Send
        /// <summary>
        /// send message
        /// </summary>
        /// <returns>true=success, false=failed</returns>
        public Boolean Send()
        {
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.Subject = Subject;
                    Message.Body = Body;
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    Client.EnableSsl = true;
                  
                    try
                    {
                        Client.Send(Message);
                        return true;
                    }
                    catch (Exception e)
                    {
                        try 
                        {
                            Client.Credentials = new NetworkCredential(User, Password);
                            Client.Send(Message);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Error = true;
                            Text = ex.ToString();
                        }
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public Boolean Send(Boolean html)
        {
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.IsBodyHtml = html;
                    Message.Subject = Subject;
                    Message.Body = Body;
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    
                    try
                    {
                        Text = Client.Port.ToString() + "don't need NetworkCredential";
                        Client.EnableSsl = false;
                        Client.Credentials = CredentialCache.DefaultNetworkCredentials;
                        Client.Send(Message);
                        return true;
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            Text = "need NetworkCredential";
                            Client.EnableSsl = true;
                            Client.Credentials = new NetworkCredential(User, Password);
                            Client.Send(Message);
                            Text += e.ToString();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Error = true;
                            Text += ex.ToString();
                        }
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// send message
        /// </summary>
        /// <param name="s">mail subject</param>
        /// <param name="b">mail body</param>
        /// <returns>true=success, false=failed</returns>
        public Boolean Send(String s, String b)
        {
            Subject = s;
            Body = b;
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.Subject = Subject;
                    Message.Body = Body;
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    Client.EnableSsl = true;
                    Client.Credentials = new NetworkCredential(User, Password);
                    try
                    {
                        Client.Send(Message);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error = true;
                        Text = e.ToString();
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// send message with attachment
        /// </summary>
        /// <param name="s">attachment file</param>
        /// <returns>true=success, false=failed</returns>
        public Boolean Send(String s)
        {
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.Subject = Subject;
                    Message.Body = Body;
                    if (!String.IsNullOrEmpty(s))
                    {
                        Message.Attachments.Add(Attach(s));
                    }
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    Client.EnableSsl = true;
                    Client.Credentials = new NetworkCredential(User, Password);
                    try
                    {
                        Client.Send(Message);
                        Message.Attachments.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error = true;
                        Text = e.ToString();
                        Message.Attachments.Dispose();
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// send message with attachment
        /// </summary>
        /// <param name="html">html body</param>
        /// <param name="s">attachment file</param>
        /// <returns>true=success, false=failed</returns>
        public Boolean Send(Boolean html, String s)
        {
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.IsBodyHtml = html;
                    Message.Subject = Subject;
                    Message.Body = Body;
                    if (!String.IsNullOrEmpty(s))
                    {
                        Message.Attachments.Add(Attach(s));
                    }
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    Client.EnableSsl = true;
                    Client.Credentials = new NetworkCredential(User, Password);
                    try
                    {
                        Client.Send(Message);
                        Message.Attachments.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error = true;
                        Text = e.ToString();
                        Message.Attachments.Dispose();
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// send message with attachment
        /// </summary>
        /// <param name="sl">attachment file list</param>
        /// <returns>true=success, false=failed</returns>
        public Boolean Send(List<String> sl)
        {
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.Subject = Subject;
                    Message.Body = Body;
                    foreach (String s in sl)
                    {
                        Message.Attachments.Add(Attach(s));
                    }
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    Client.EnableSsl = true;
                    Client.Credentials = new NetworkCredential(User, Password);
                    try
                    {
                        Client.Send(Message);
                        Message.Attachments.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error = true;
                        Text = e.ToString();
                        Message.Attachments.Dispose();
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// send message with attachment
        /// </summary>
        /// <param name="html">html body</param>
        /// <param name="sl">attachment file list</param>
        /// <returns>true=success, false=failed</returns>
        public Boolean Send(Boolean html, List<String> sl)
        {
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.IsBodyHtml = html;
                    Message.Subject = Subject;
                    Message.Body = Body;
                    foreach (String s in sl)
                    {
                        Message.Attachments.Add(Attach(s));
                    }
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    Client.EnableSsl = true;
                    Client.Credentials = new NetworkCredential(User, Password);
                    try
                    {
                        Client.Send(Message);
                        Message.Attachments.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error = true;
                        Text = e.ToString();
                        Message.Attachments.Dispose();
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// send message with attachment
        /// </summary>
        /// <param name="s">mail subject</param>
        /// <param name="b">mail body</param>
        /// <param name="sl">attachment file list</param>
        /// <returns>true=success, false=failed</returns>
        public Boolean Send(String s, String b, List<String> sl)
        {
            Subject = s;
            Body = b;
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.Subject = Subject;
                    Message.Body = Body;
                    foreach (String f in sl)
                    {
                        Message.Attachments.Add(Attach(f));
                    }
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    Client.EnableSsl = true;
                    Client.Credentials = new NetworkCredential(User, Password);
                    try
                    {
                        Client.Send(Message);
                        Message.Attachments.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error = true;
                        Text = e.ToString();
                        Message.Attachments.Dispose();
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// send message with attachment
        /// </summary>
        /// <param name="s">mail subject</param>
        /// <param name="b">mail body</param>
        /// <param name="f">attachment file</param>
        /// <returns>true=success, false=failed</returns>
        public Boolean Send(String s, String b, String f)
        {
            Subject = s;
            Body = b;
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.Subject = Subject;
                    Message.Body = Body;
                    if (!String.IsNullOrEmpty(f))
                    {
                        Message.Attachments.Add(Attach(f));
                    }
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    Client.EnableSsl = true;
                    Client.Credentials = new NetworkCredential(User, Password);
                    try
                    {
                        Client.Send(Message);
                        Message.Attachments.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error = true;
                        Text = e.ToString();
                        Message.Attachments.Dispose();
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// send message with attachment and image
        /// </summary>
        /// <param name="s">subject</param>
        /// <param name="b">body</param>
        /// <param name="f">attachment file</param>
        /// <param name="p">image file</param>
        /// <returns></returns>
        public Boolean Send(String s, String b, String f, String p)
        {
            Subject = s;
            Body = b;
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.Subject = Subject;
                    Message.Body = Body;
                    if (!String.IsNullOrEmpty(f))
                    {
                        Message.Attachments.Add(Attach(f));
                    }
                    if (!String.IsNullOrWhiteSpace(p))
                    {
                        Message.AlternateViews.Add(Image(p));
                    }
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    Client.EnableSsl = true;
                    Client.Credentials = new NetworkCredential(User, Password);
                    try
                    {
                        Client.Send(Message);
                        Message.Attachments.Dispose();
                        Message.AlternateViews.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error = true;
                        Text = e.ToString();
                        Message.Attachments.Dispose();
                        Message.AlternateViews.Dispose();
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// send message with attachments and images
        /// </summary>
        /// <param name="s">subject</param>
        /// <param name="b">body</param>
        /// <param name="sl">list of attachment files</param>
        /// <param name="il">list of image files</param>
        /// <returns></returns>
        public Boolean Send(String s, String b, List<String> sl, List<String> il)
        {
            Subject = s;
            Body = b;
            if (Validate() == false) return false;
            using (Message = new MailMessage(From, To))
            {
                using (Client = new SmtpClient(Server))
                {
                    Message.Subject = Subject;
                    Message.Body = Body;
                    foreach (String f in sl)
                    {
                        Message.Attachments.Add(Attach(f));
                    }
                    foreach (String i in il)
                    {
                        Message.AlternateViews.Add(Image(i));
                    }
                    Client.UseDefaultCredentials = false;
                    Client.Port = Port;
                    Client.Timeout = Timeout;
                    Client.EnableSsl = true;
                    Client.Credentials = new NetworkCredential(User, Password);
                    try
                    {
                        Client.Send(Message);
                        Message.Attachments.Dispose();
                        Message.AlternateViews.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error = true;
                        Text = e.ToString();
                        Message.Attachments.Dispose();
                        Message.AlternateViews.Dispose();
                    }
                    return false;
                }
            }
        }
        #endregion

        #region Get
        #endregion

        #region Dispose
        public void Dispose()
        {
            Client.Dispose();
            Message.Dispose();
        }
        #endregion

        #region Private Methods
        private Boolean Validate()
        {
            if (Server == String.Empty) return false;
            if (User == String.Empty) return false;
            if (Password == String.Empty) return false;
            if (To == String.Empty) return false;
            if (From == String.Empty) return false;
            if (Subject == String.Empty) return false;
            //if (Body == String.Empty) return false;
            return true;
        }

        private Attachment Attach(String s)
        {
            Attachment a = new Attachment(s);
            a.ContentDisposition.CreationDate = File.GetCreationTime(s);
            a.ContentDisposition.ModificationDate = File.GetLastWriteTime(s);
            a.ContentDisposition.ReadDate = File.GetLastAccessTime(s);
            a.ContentDisposition.FileName = Path.GetFileName(s);
            a.ContentDisposition.Size = new FileInfo(s).Length;
            a.ContentDisposition.DispositionType = DispositionTypeNames.Attachment;
            return a;
        }

        private AlternateView Image(String s)
        {
            LinkedResource inline = new LinkedResource(s, MediaTypeNames.Image.Jpeg);
            inline.ContentId = Guid.NewGuid().ToString();
            String body = @"<img src='cid:" + inline.ContentId + @"'/>";
            AlternateView av = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
            av.LinkedResources.Add(inline);
            return av;
        }

        private AlternateView Image(String s, String i)
        {
            LinkedResource inline = new LinkedResource(s, MediaTypeNames.Image.Jpeg);
            inline.ContentId = i;
            AlternateView av = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html);
            av.LinkedResources.Add(inline);
            return av;
        }

        private AlternateView Image(Tuple<String, String> t)
        {
            LinkedResource inline = new LinkedResource(t.Item2, MediaTypeNames.Image.Jpeg);
            inline.ContentId = t.Item1;
            AlternateView av = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html);
            av.LinkedResources.Add(inline);
            return av;
        }

        private Boolean Validate(String s)
        {
            return new global::System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(s);
        }
        #endregion
    }
}
