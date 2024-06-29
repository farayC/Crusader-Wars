using Crusader_Wars.data.save_file;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Crusader_Wars.armies.commander_traits
{

    internal class Trait
    {
        string Key { get; set; }
        int Index { get; set; }

        public Trait(string key, int index) { 
            Key = key;
            Index = index;
        }
    }
    public class CommanderTraits
    {
        List<Trait> Traits { get; set; }


        static string traits_folder_path = @".\data\traits\";
        public CommanderTraits() 
        {
            foreach(var file_path in  Directory.GetFiles(traits_folder_path))
            {
                if(Path.GetExtension(file_path) == ".xml")
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file_path);


                }
            }
        }
    }
}
