using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using HipChat;
using HipChat.Entities;

namespace HipChatMessenger
{
    class Program
    {
        private static string _token;
        private static string _from;
        private static string _roomName;
        private static string _message;
        private static bool _notify;
        private static string _bgcolor;
        private static int _roomId;

        private static bool _continue;

        //can either pass in a token or use the one in the config
        static void Main(string[] args)
        {
            SetDefaults(args);
            _continue = true;

            //enter interactive mode
            if (args.Length == 0)
            {
                while (_continue)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Enter your command, ? for help, or exit");
                    Console.Write("hipchat: ");
                    args = Console.ReadLine().Split(' ');    
                    SetDefaults(args);

                    ProcessCommands(args);
                }
            }
            else
            {
                ProcessCommands(args);
            }
        }

        public static void ProcessCommands(string[] args)
        {
            //requires a token
            if (!string.IsNullOrEmpty(_token) || args.Count(a => a.ToLower().StartsWith("token")) == 1)
            {
                if (args.Length > 0)
                {
                    string cmd = args[0].Trim().ToLower();

                    switch (cmd)
                    {
                        case "?":
                            ShowHelp();
                            break;

                        case "getrooms":
                            GetRooms();
                            break;

                        case "send":
                            Send();
                            break;

                        case "exit":
                            _continue = false;
                            break;
                    }
                }
            }
            else
            {
                throw new ApplicationException("In order to use this tool you need to supply a hip chat token!");
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("getrooms");
            Console.WriteLine("\tGets a list of all the rooms you have access too.");
            Console.WriteLine("\tExample: getrooms");
            Console.WriteLine();

            Console.WriteLine("send");
            Console.WriteLine("\tSends a message to the specified room.");
            Console.WriteLine("\tExample: send [from:\"{friendly name}\"] [roomid:{numeric room id}] [notify:{true/false}] message:{your message here}");
        }

        private static void SetDefaults(string[] args)
        {
            _token = Contains("token", args)
                ? GetValue("token", args)
                : ConfigurationManager.AppSettings["token"];

            _from = Contains("from", args)
                ? GetValue("from", args)
                : ConfigurationManager.AppSettings["defaultFrom"];

            _roomName = Contains("roomname", args)
                ? GetValue("roomname", args)
                : "";
            _message = Contains("message", args)
                ? GetValue("message", args)
                : "";

            _notify = Contains("notify", args)
                ? Convert.ToBoolean(GetValue("notify", args))
                : Convert.ToBoolean(ConfigurationManager.AppSettings["notify"]);

            _bgcolor = Contains("bgcolor", args)
                ? GetValue("bgcolor", args)
                : "";

            _roomId = Contains("roomid", args)
                ? Convert.ToInt32(GetValue("roomid", args))
                : 0;
        }

        static int GetRoomId()
        {
            //asked for room by name - get id
            if (Convert.ToInt32(_roomId) == 0 && !string.IsNullOrEmpty(_roomName))
                _roomId =
                    GetRooms().FirstOrDefault(r => r.Name.ToLower() == _roomName.Replace("\"", "").ToLower()).Id;

            //can't be 0!!
            if (_roomId == 0)
                throw new ApplicationException("The room you asked for doesn't exist");

            return Convert.ToInt32(_roomId);
        }

        static void Send()
        {
            HipChatClient client = new HipChatClient(_token, _roomId, _from);
            int roomid = GetRoomId();
            client.SendMessage(_message, _from, _notify);
        }

        static List<Room> GetRooms()
        {
            List<Room> rooms = new List<Room>();
            var client = new HipChatClient(_token);
            rooms = client.ListRoomsAsNativeObjects();

            foreach (Room room in rooms)
            {
                Console.WriteLine(room.Name + " - " + room.Id);
            }

            return rooms;
        }

        static string GetValue(string key, string[] args)
        {
            if (key.ToLower() == "message")
            {
                string msg = "";
                bool found = false;

                //get everything after the message key and create the message
                for (int i = 0; i < args.Length; i++)
                {
                    if (!found && args[i].Contains("message"))
                    {
                        found = true;
                        msg = args[i].ToLower().Replace("message:", "");
                    }
                    else if (found)
                    {
                        msg += " " + args[i];
                    }
                }

                return msg;
            }
            
            string value = args.Where(a => a.Contains(key)).FirstOrDefault().Split(':')[1] ?? "";

            return value;
        }

        static bool Contains(string key, string[] args)
        {
            if (args.Count(a => a.ToLower().StartsWith(key.ToLower())) == 1)
            {
                return true;
            }
            return false;
        }
    }
}
