using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;

namespace PscdPack
{
    /// <summary>
    /// Class with program entry point.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = Path.GetFileName(Environment.GetCommandLineArgs()[0]),
                FullName = "PscdPack"
            };

            app.Command("unpack", (config) =>
            {
                config.FullName = "Unpacks/extracts .PAK file";
                config.Description = "Unpacks/extracts SEGA Genesis Classics ROM";
                var pathArg = config.Argument("path", "The path of the .PAK to unpack").IsRequired();

                config.OnExecute(() =>
                {
                    PscdFormat pak;
                    // FIXME check keybytes format to use
                    pak = new PscdFormat(pathArg.Value, PscdFormat.ClassicsGoldKeyBytes);
                    pak.Load();
                    string newFileName = System.IO.Path.ChangeExtension(pathArg.Value, ".bin");
                    using (var file = File.Open(newFileName, FileMode.Open))
                    using (var rom = pak.GetImage())
                    {
                        rom.WriteTo(file);
                        file.Flush();
                    }
                });

                config.HelpOption();
            });

            app.VersionOptionFromAssemblyAttributes(System.Reflection.Assembly.GetExecutingAssembly());
            app.HelpOption();

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });

            try
            {
                return app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error while processing: {0}", ex);
                return -1;
            }

        }

    }
}
