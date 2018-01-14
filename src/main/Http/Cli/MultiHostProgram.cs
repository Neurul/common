using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using org.neurul.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common.Http.Cli
{
    public static class MultiHostProgram
    {
        public static void Start(IConsoleWrapper console, string appName, string[] uriStrings, string[] uriNames, params INancyBootstrapper[] bootStrappers)
        {
            console.WriteLine();
            console.WriteLine($"Initializing {appName}...");
            console.WriteLine();

            AssertionConcern.AssertArgumentNotNull(appName, nameof(appName));
            AssertionConcern.AssertArgumentNotNull(uriStrings, nameof(uriStrings));
            AssertionConcern.AssertArgumentNotNull(uriNames, nameof(uriNames));
            AssertionConcern.AssertArgumentNotNull(bootStrappers, nameof(bootStrappers));
            AssertionConcern.AssertArgumentNotEmpty(appName, "AppName cannot be empty.", nameof(appName));
            AssertionConcern.AssertArgumentValid(a => a.Length == bootStrappers.Length, uriStrings, $"{bootStrappers.Length - uriStrings.Length} URI values of the required {bootStrappers.Length} URIs was/were not specified.", nameof(uriStrings));
            AssertionConcern.AssertArgumentValid(a => a.Length == bootStrappers.Length, uriNames, $"{bootStrappers.Length - uriNames.Length} URI names of the required {bootStrappers.Length} URIs was/were not specified.", nameof(uriNames));

            for (int i = 0; i < uriStrings.Length; i++)
                AssertionConcern.AssertArgumentValid(a => Uri.IsWellFormedUriString(a[i], UriKind.Absolute), uriStrings, $"Must specify valid '{uriNames[i]}' URI", nameof(uriStrings));

            for (int i = 0; i < uriStrings.Length; i++)
                new NancyHost(bootStrappers[i], new Uri(uriStrings[i])).Start();
                
            var response = string.Empty;

            while (response.ToUpper() != "Y")
            {
                response = string.Empty;

                console.Clear();
                for (int i = 0; i < uriStrings.Length; i++)
                    console.WriteLine($"{uriNames[i]}: {uriStrings[i]}");
                    
                console.WriteLine();

                console.WriteLine($"{appName} online.");
                console.WriteLine();
                console.WriteLine("Enjoy!");
                console.WriteLine();

                console.Write("Press any key to exit...");
                console.ReadKey(true);
                console.WriteLine();
                console.WriteLine();
                console.WriteLine("Are you sure you wish to exit? (Y/N)");
                while (response == string.Empty)
                {
                    response = console.ReadKey(true).KeyChar.ToString();
                    if (response.ToUpper() != "Y" && response.ToUpper() != "N")
                    {
                        response = string.Empty;
                        console.Beep();
                    }
                }
            }
        }
    }
}
