using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace HUSauth
{
    class Program
    {
        private static string User { get; set; } = null;
        private static string Pass { get; set; } = null;
        private static bool IsVerboseOutput { get; set; } = false;
        private static bool IsCheckOnly { get; set; } = false;
        private static bool IsOutputHelpOnly { get; set; } = false;
        private static bool IsOutputVersionOnly { get; set; } = false;
        private static bool IsEncryptedPass { get; set; } = false;
		private static bool IsForceAuthentication{ get; set; } = false;

        static void Main(string[] args)
        {
            ParseArgs(args);

            if (IsOutputVersionOnly)
            {
                OutputVersion();
                return;
            }

            if (IsOutputHelpOnly)
            {
                OutputHelp();
                return;
            }

            if (IsCheckOnly)
            {
                OutputAvailable();
                return;
            }

            if (User == null || Pass == null)
            {
                OutputHelp();
                return;
            }

            Authentication();
        }

        private static void ParseArgs(string[] args)
        {
            var argsNormalizer = new Func<string, string>((string arg) => arg.Replace("/", "-").Replace("--", "-"));

            for (var i = 0; i < args.Length; i++)
            {
                switch (argsNormalizer(args[i]))
                {
                    case "-u":
                    case "-user":
                        User = args[++i];
                        break;

                    case "-p":
                    case "-password":
                        Pass = args[++i];
                        break;

                    case "-e":
                    case "-encrypted":
                        IsEncryptedPass = true;
                        break;

					case "-f":
					case "-force":
						IsForceAuthentication = true;
						break;

                    case "-v":
                    case "-verbose":
                        IsVerboseOutput = true;
                        break;

                    case "-c":
                    case "-check":
                        IsCheckOnly = true;
                        break;

                    case "-h":
                    case "-help":
                        IsOutputHelpOnly = true;
                        break;

                    case "-version":
                        IsOutputVersionOnly = true;
                        break;
                }
            }
        }

        private static void OutputHelp()
        {
            using (var sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("HUSauth.Help.txt")))
            {
                Console.WriteLine(sr.ReadToEnd());
            }
        }

        private static void OutputVersion()
        {
            var sb = new StringBuilder();
            sb.Append(@"HUSauth Core  ");
            sb.AppendLine(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine(sb);
            using (var sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("HUSauth.Version.txt")))
            {
                Console.WriteLine(sr.ReadToEnd());
            }
        }

        private static bool OutputAvailable()
        {
            var result = Network.IsAvailable(IsVerboseOutput);
            if (result)
            {
                Console.WriteLine(@"internet connection is available");
            }
            else
            {
                Console.WriteLine(@"internet connection is not available");
            }

            return result;
        }

        private static void Authentication()
        {
            if (IsEncryptedPass)
            {
                Pass = Encrypt.DecryptString(Pass);
            }

			if (IsForceAuthentication || !OutputAvailable())
            {
                Network.Authentication(User, Pass, IsVerboseOutput);
                OutputAvailable();
            }

            if (!IsEncryptedPass)
            {
                Console.WriteLine();
                Console.WriteLine(@"generated encrypted password: " + Encrypt.EncryptString(Pass));
                Console.WriteLine(@"you should use encrypted password in shellscript, cron, systemd unit, and others");
            }
        }
    }
}
