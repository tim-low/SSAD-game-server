using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Game_Server.Controller.Database.Tables;
using Game_Server.Database;
using Game_Server.Model;
using Game_Server.Network;
using Game_Server.Util;
using Game_Server.Util.Json;
using Microsoft.EntityFrameworkCore;

namespace Game_Server.Web
{
    /// <summary>
    /// HttpServer class that listen on port 8080
    ///     - API Base Gateway = http://domain:8000/api/v1/{0}/{1}/{2}
    ///     - {0} refer to the object
    ///     - {1} refer to the action
    ///     - {2} refer to the id
    /// </summary>
    public class ApiServer
    {
        private const int MAX_NUM_OF_WORKER = 10;
        private HttpListener HttpServer;
        private string ipAddr;
        private bool _running = false;
        private string _apiGateway;
        private Thread[] HttpListenerThread = new Thread[MAX_NUM_OF_WORKER];

        private Mutex Mutex;

        public static ApiServer Create(IPConf conf)
        {
            if (!HttpListener.IsSupported)
            {
#if DEBUG
                Log.Error("HttpListener is not supported in your current operating system.");
#endif
                return null;
            }
            else
            {
                return new ApiServer(conf);
            }
        }

        private ApiServer(IPConf conf)
        {
            HttpServer = new HttpListener();
            ipAddr = conf.ServerIp;
            _apiGateway = String.Format("http://{0}:8000/", ipAddr);
            HttpServer.Prefixes.Add(_apiGateway);
            _running = false;
            Mutex = new Mutex(false, "Api");
        }

        public void Toggle()
        {
            if (_running)
            {
                Log.Info("Web Server already started.");
            }
            else
            {
                Start();
            }
        }

        public void Start()
        {
            HttpServer.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            HttpServer.Start();
            HttpServer.BeginGetContext(ProcessRequest, HttpServer);
            /*
            Task listenTask = StartListening();
            listenTask.GetAwaiter().GetResult();
            */
            // HttpServer.Close();

        }

        private void ProcessRequest(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            HttpServer.BeginGetContext(ProcessRequest, HttpServer);
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            response.KeepAlive = false;
            string methodType = request.HttpMethod;
            // Get the url of the request page/what api is called.
            Uri requestUri = request.Url;
            Log.Debug("{0} {1}", methodType, requestUri.AbsolutePath);
            if (requestUri.AbsolutePath.IndexOf("/api/v1") != 0)
            {
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.Close();
            }
            else
            {
                var bodyContent = HttpUtility.UrlDecode(new StreamReader(request.InputStream).ReadToEnd());
                
                string jsonOutput = "";
                // Process the url and method type and return a correspond object in json (refer to #database channel in Discord)
                try
                {
                    //Log.Debug("Content: {0}", bodyContent);
                    jsonOutput = ProcessHttpRequest(methodType, requestUri, bodyContent);
                }
                catch (Exception e)
                {
                    Log.Error("{0}", e.StackTrace);
                    jsonOutput = Newtonsoft.Json.JsonConvert.SerializeObject(new { responseMsg = "Error" });
                }
                finally
                {

                    if (jsonOutput == "")
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                    }
                    else if (jsonOutput == "invalid token")
                    {
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                    else // valid json data
                    {

                        // Encoding the data into bytestream
                        byte[] data = Encoding.UTF8.GetBytes(jsonOutput);

                        // Set the response header
                        response.ContentType = "application/json";
                        response.ContentEncoding = Encoding.UTF8;
                        response.ContentLength64 = data.LongLength;

                        response.OutputStream.Write(data, 0, data.Length);
                    }

                    response.OutputStream.Flush();
                    response.OutputStream.Close();
                    response.Close();
                }
            }
        }

        private string ProcessHttpRequest(string methodType, Uri requestUri, string bodyContent)
        {
            string[] args = requestUri.AbsolutePath.Split('/').Skip(3).ToArray();
            GameClient client;
            if (args.Length < 3)
                return "";
            else if ((client = ServerMain.Instance.Server.GetClient(args[2])) == null)
                return "invalid token";
            else if (client?.Character?.CharacterDb?.Account?.Permission == 0)
                return "invalid token";
            switch (args[0].ToLower())
            {
                case "question":
                    return QuestionApi(methodType, args.Skip(1).ToArray(), bodyContent, client.Character.CharacterDb.AccountId);
                case "quiz":
                    return QuizApi(methodType, args.Skip(1).ToArray(), bodyContent, client.Character.CharacterDb.AccountId);
                case "analytic":
                    return AnalyticApi(methodType, args.Skip(1).ToArray(), bodyContent, client.Character.CharacterDb.AccountId);
                default:
                    return "";
            }
        }

        /// <summary>
        /// Question API Processor
        /// </summary>
        /// <param name="methodType"></param>
        /// <param name="args"></param>
        /// <param name="bodyContent"></param>
        /// <returns></returns>
        private string QuestionApi(string methodType, string[] args, string bodyContent, string accountId)
        {
            if (methodType == "POST" && args[0] == "post")
            {
                // Deserialize json string, bodyContent, into object
                var question = Newtonsoft.Json.JsonConvert.DeserializeObject<QuestionJson>(bodyContent);
                var newQuestion = ServerMain.Instance.Database.AddTeacherQuestion(question, accountId);

                if (newQuestion == null)
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { responseMsg = "Error" });
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { responseMsg = "New Question of QuestionId: " + newQuestion.Id + " has been added" });
                }
            }
            else if(methodType == "GET" && args[0] == "get") // GET
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { questions = ServerMain.Instance.Database.GetTeacherQuestions(accountId) });
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { responseMsg = "You are a lost sheep. You are probably using wrong HTTP method or wrong endpoint!" });
            }
        }


        /// <summary>
        /// Quiz API Processor
        /// </summary>
        /// <param name="methodType"></param>
        /// <param name="args"></param>
        /// <param name="bodyContent"></param>
        /// <returns></returns>
        private string QuizApi(string methodType, string[] args, string bodyContent, string accountId)
        {
            if (methodType == "POST" && args[0] == "post")
            {

                // deserialize the json string, bodyContent into Quiz object
                var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<QuizJson>(bodyContent);
                var newQuiz = ServerMain.Instance.Database.AddTeacherQuizzes(jsonObj, accountId);
                if(newQuiz == null || newQuiz.Id == Int32.MinValue)
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { responseMsg = "Error" });
                }
                else
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { responseMsg = "New Quiz of QuizId: " + newQuiz.Id + " has been added" });
                }
            }
            else if (methodType == "GET" && args[0] == "get")
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { quiz = ServerMain.Instance.Database.GetTeacherQuizzes(accountId) });
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { responseMsg = "You are a lost sheep. You are probably using wrong HTTP method or wrong endpoint!" });
            }
        }

        private string AnalyticApi(string methodType, string[] args, string bodyContent, string accountId)
        {
            if(methodType == "GET")
            {
                if (args[0] == "quiz")
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { quizStat = ServerMain.Instance.Database.GetQuizStats() });
                }
                else if (args[0] == "topic")
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { topicMasteryStats = ServerMain.Instance.Database.GetTopicMasteryStats() });
                }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { responseMsg = "You are a lost sheep. You are probably using wrong HTTP method or wrong endpoint!" });
        }

    }
}