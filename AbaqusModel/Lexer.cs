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
            list.Where(x => x.Contains('=')).Select(x => x.Split('='))
                .ForEach(x => dic.Add(x[0].Trim().ToUpper(), x[1].Trim()));
            list.Skip(1).Where(x => !x.Contains('=')).ForEach(x => dic.Add(x.ToUpper(), ""));
            return new KeyValuePair<string, Dictionary<string, string>>(keyword, dic);
        }

        public bool read_file(string filename)
        {
            var q = lex_file(filename);
            read_includes(q);
            return commands.Count > 0;
        }

        private void read_includes(Queue<Command> q)
        {
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
        }

        public bool read_string(string target)
        {
            return read_stream(new System.IO.StringReader(target));
        }

        public bool read_stream(System.IO.TextReader stream)
        {
            var q = lex_stream(stream);
            read_includes(q);
            return commands.Count > 0;
        }


        private Queue<Command> lex_file(string filename)
        {
            Queue<Command> res = null;
            using (var sr = new System.IO.StreamReader(filename))
            {
                res = lex_stream(sr);
            }
            return res;
        }


        private Queue<Command> lex_stream(System.IO.TextReader sr)
        {
            var res = new Queue<Command>();
            Command c = new Command(); // Dummy
            var line = sr.ReadLine();
            while (line != null)
            {
                if (line.StartsWith("**"))
                {
                    //comment
                }
                else
                {
                    if (line.StartsWith("*"))
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
            return res;
        }



    }
}
