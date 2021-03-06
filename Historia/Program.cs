﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Historia
{
    class Program
    {
        public static object writerLock = new object();
        public static HTMLWriter writer = new HTMLWriter();

        static void Main(string[] args)
        { 
            var options = new Options();
            try
            {
                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    Endpoint.LocalBarrack = Endpoint.ConvertToIPEndPoint(options.barrack);
                    Endpoint.LocalZone = Endpoint.ConvertToIPEndPoint(options.zone);
                    Endpoint.LocalWeb = Endpoint.ConvertToIPEndPoint(options.web);

                    var config = new ConfigServer(Endpoint.LocalBarrack, Endpoint.LocalWeb, options.TOSPath);
                    var destination = Endpoint.ConvertToIPEndPoint(config.Init());

                    new Proxy().StartAsync(Endpoint.LocalBarrack, destination);

                    StartGame(options.TOSPath);
                }

                ParseCommands();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void ParseCommands()
        {
            List<string> valid = new List<string>() { "exit", "logsplit" };

            while (true)
            {
                Console.WriteLine("\nAvailable commands: \nlogsplit: Starts a new packet log.\nexit: Stops program execution.\n");
                Console.Write("Command: ");
                var cmd = Console.ReadLine();

                if (!valid.Contains(cmd))
                {
                    Console.WriteLine("Invalid command.");
                    continue;
                }

                if (cmd == "exit")
                    break;

                if (cmd == "logsplit")
                {
                    lock (writerLock)
                        Program.writer = new HTMLWriter();
                    continue;
                }
            }
        }

        private static void StartGame(string path, string arguments = "-SERVICE")
        {
            try
            {
                Console.WriteLine("[Program] Starting Tree of Savior.");
                Process.Start(new ProcessStartInfo()
                {
                    FileName = GetClientExe(path),
                    Arguments = arguments
                });
            } catch (Exception e)
            {
                Console.WriteLine("[Program] Error. Unable to start Tree of Savior.");
                Console.WriteLine(e.Message);
            }
        }

        public static string GetClientExe(string argument)
        {
            var clientExe = Path.Combine(argument, "release\\Client_tos.exe");
            if (!File.Exists(clientExe))
                throw new FileNotFoundException(string.Format("Error. The file '{0}' could not be located.", clientExe));
            return clientExe;
        }
    }
}
