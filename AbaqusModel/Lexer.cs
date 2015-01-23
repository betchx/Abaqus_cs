using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{

    public class Lexer
    {
        public Queue<Command> commands {get; private set;}
        public Lexer()
        {
            commands = new Queue<Command>();
        }
        public Command get() {
            return commands.Dequeue();
        }

        private KeyValuePair<string,Dictionary<string,string> > parse_keyword(string line)
        {
            var list = line.Substring(1).Split(',');
            var keyword = list[0].Trim();
            var dic = new Dictionary<string, string>();
            list.Where(x => x.Contains('=')).Select(x => x.Split('=')).AsParallel().ForAll(x => dic.Add(x[0].Trim().ToUpper(), x[1].Trim()));
            list.Skip(1).Where(x => !x.Contains('=')).AsParallel().ForAll(x => dic.Add(x.ToUpper(), ""));
            return new KeyValuePair<string, Dictionary<string, string>>(keyword, dic);
        }

        public bool read_file(string filename)
        {
            var q = lex_file(filename);

            foreach (var item in q)
            {
                if (item.keyword.ToUpper() == "INCLUDE")
                {
                    read_file(item.parameters["INPUT"]);
                }
                else
                {
                    commands.Enqueue(item);
                }
            }

            return commands.Count > 0;

        }

        private Queue<Command> lex_file(string filename)
        {
            var res = new Queue<Command>();
            Command c = new Command(); // Dummy
            using (var sr = new System.IO.StreamReader(filename))
            {
                var line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Substring(0, 2) == "**")
                    {
                        //comment
                    }
                    else
                    {
                        if (line[0] == '*')
                        {
                            c = new Command();
                            var kv = parse_keyword(line);
                            c.keyword = kv.Key;
                            c.parameters = kv.Value;
                            res.Enqueue(c);
                        }
                        else
                        {
                            c.datablock.Add(line);
                        }
                    }
                    line = sr.ReadLine();
                }
            }
            return res;
        }
    
    }
}
