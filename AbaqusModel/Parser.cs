using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

using ELSet = System.Collections.Generic.SortedSet<Abaqus.Element>;
using NSet = System.Collections.Generic.SortedSet<Abaqus.Node>;

namespace Abaqus
{
    public class Parser
    {
        Transform3DGroup system;
        public Model model { get; set; }
        public Lexer lexer { get; set; }

        private Part current_part;

        delegate void command_parser(Command cmd);

        private SortedDictionary<string, command_parser> dict;


        public Parser()
        {
            model = new Model();
            lexer = new Lexer();
            current_part = model;
            system = new Transform3DGroup();
            dict = new SortedDictionary<string, command_parser>();
            dict.Add("NODE", parse_node);
            dict.Add("ELEMENT", parse_element);
            dict.Add("NSET", parse_nset);
            dict.Add("ELSET", parse_elset);
            dict.Add("SYSTEM", parse_system);
            dict.Add("PART", parse_part);
            dict.Add("END PART", parse_end_part);
            dict.Add("INSTANCE", parse_instance);
            dict.Add("ASSEMBLY", parse_assembly);
        }

        public Model parse_file(string filename)
        {
            lexer.read_file(filename);
            return parse_lex();
        }

        public Model parse_string(string target)
        {
            lexer.read_string(target);
            return parse_lex();
        }

        public Model parse_stream(System.IO.TextReader stream)
        {
            lexer.read_stream(stream);
            return parse_lex();
        }

        private Model parse_lex()
        {
            foreach (var item in lexer.commands)
            {
                if (dict.ContainsKey(item.keyword))
                {
                    dict[item.keyword](item);
                }
            }
            return model;
        }

        /// <summary>
        ///  ノードコマンドをパースし，モデルに節点を追加する．
        ///  NSETパラメータがあればNsetも追加する．
        /// </summary>
        /// <param name="cmd">ノードコマンド</param>
        private void parse_node(Command cmd)
        {
            if (cmd.keyword != "NODE") throw new ArgumentException( cmd.keyword + " is not NODE");
            var nids = new List<uint>();
            foreach (var line in cmd.datablock)
            {
                var node = new Node(line);
                if (system.Children.Count > 0) {
                    node.Transform(system);
                }
                nids.Add(node.id);
                current_part.nodes.Add(node);
            }
            if (cmd.parameters.ContainsKey("NSET"))
            {
                var name = cmd.parameters["NSET"];
                var nset = new NSet(name);
                nset.UnionWith(nids.Select(i => new Address(current_part.name,i)));
                current_part.nsets.Add(nset);
            }
        }

        /// <summary>
        ///   エレメントコマンドをパースし，モデルに要素を追加する．
        ///   ELSETパラメータがあればELSetも追加する．
        /// </summary>
        /// <param name="cmd">エレメントコマンド</param>
        private void parse_element(Command cmd)
        {
            if (cmd.keyword != "ELEMENT") throw new ArgumentException(cmd.keyword + " is not ELEMENT", "cmd");
            var type = cmd.parameters["TYPE"];

            // 継続行（最後がカンマ）をまとめる
            bool cont = false;
            var lines = new List<string>();
            string buf = "";
            foreach (var item in cmd.datablock)
            {
                if (cont) 
                    buf += item.Trim();
                else 
                    buf = item.Trim();
                cont = buf.Last() == ',';
                if (!cont) lines.Add(buf);
            }

            // 要素作成
            var eids = new List<uint>();
            foreach (var line in lines)
            {
                var e = new Element(type, line);
                current_part.elements.Add(e.id, e);
                eids.Add(e.id);
            }

            // 指定があれば要素集合を作成
            if (cmd.parameters.ContainsKey("ELSET"))
            {
                var name = cmd.parameters["ELSET"].ToUpper();
                var elset = new ELSet(name);
                elset.UnionWith(eids.Select(i => new Address(i)));
                current_part.elsets.Add(name, elset);
            }
        }

        private void ELSetGenerate(ELSet elset, string line)
        {
            var arr = line.Split(',').Select(s => uint.Parse(s)).ToArray();
            var start = arr[0];
            var last = arr[1];
            var step = arr[2];
            for (uint i = start; i <= last; i += step)
            {
                elset.Add(i);
            }
        }

        private void parse_elset(Command cmd)
        {
            if (cmd.keyword != "ELSET") throw new ArgumentException(cmd.keyword + " is not ELSET");
            var name = cmd["ELSET"].ToUpper();
            var elset = model.elsets[name];
            if (elset == null) elset = new ELSet(name);
            if (cmd.parameters.ContainsKey("INSTANCE"))
            {
                throw new NotImplementedException("Instance option support in parse_elset");
            }
            else
            {
                if (cmd.parameters.ContainsKey("GENERATE"))
                    cmd.ForEach(line => ELSetGenerate(elset, line));
                else
                    cmd.ForEach(line => elset.UnionWith(line.Split(',').Select(s => new Address( uint.Parse(s)))));
            }
        }

        internal IEnumerable<uint> generate(string line) { return new IDGenerater(line); }

        internal IEnumerable<Address> node_addresses(string key) { return to_addresses(key, true); }
        internal IEnumerable<Address> element_addresses(string key) { return to_addresses(key, false); }
 
        private IEnumerable<Address> to_addresses(string key, bool is_n)
        {
            uint id;
            if (key.Contains("."))
            {
                var arr = key.Split('.').ToArray();
                if(uint.TryParse(arr[1],out id) )
                {
                    return new  uint[] {id}.Select(i => new Address(arr[0], i));
                }
                // other set
                var ins = model.instances[arr[0]];
                var part = model.parts[ins.part];
                if (is_n)
                {
                    return part.nsets[arr[1]].Select(a => new Address(arr[0], a.id));
                }
                return part.elsets[arr[1]].Select(a => new Address(arr[0], a.id));
            }
            else
            {
                if (uint.TryParse(key, out id))
                {
                    return new uint[] { id }.Select(i => new Address(i));
                }
                if (is_n)
                {
                    return model.nsets[key].Select(a => new Address(a.id));
                }
                return model.elsets[key].Select(a => new Address(a.id));
            }
        }


        private void parse_nset(Command cmd)
        {
            if( ! cmd.Is("NSET")) throw new ArgumentException(cmd.keyword + " is not NSET");
            if( ! cmd.Has("NSET") )  throw new ArgumentException("No NSET option is found");
            var name = cmd["NSET"].ToUpper();

            // 後回し
            if (cmd.Has("INSTANCE")) throw new NotImplementedException();

            var nset = new NSet(name);
            current_part.nsets.Add(nset);
            if (cmd.Has("GENERATE"))
            {
                cmd.ForEach(line => generate(line).ForEach(i => nset.Add(i)));
            }
            else
            {
                cmd.ForEach(line => line.Split(',').ForEach(s => nset.UnionWith(node_addresses(s))));
            }
       }

        private void parse_part(Command cmd)
        {
            var name = cmd.parameters["NAME"].ToUpper();
            current_part = new Part(name);
            model.parts.Add(current_part);
        }
        private void parse_end_part(Command cmd)
        {
            current_part = model;
        }

        private void parse_instance(Command cmd)
        {
            var name = cmd.parameters["NAME"].ToUpper();
            var part = cmd.parameters["PART"].ToUpper();
            var trans = ""; if (cmd.datablock.Count > 1) trans = cmd.datablock[0].Trim();
            var rot = ""; if (cmd.datablock.Count > 2) rot = cmd.datablock[1].Trim();
            var ins = new Instance(name, part, trans, rot);
            model.instances.Add(ins);
        }

        private void parse_assembly(Command cmd)
        {
        }

        private void parse_system(Command cmd)
        {
            system.Children.Clear();
            var line = cmd.datablock[0].Trim();
            if (cmd.datablock.Count > 1)
            {
                if (line.Last() != ',') line += ",";
                line += cmd.datablock[1].Trim();
            }
            var arr = line.Split(',').Select(s => double.Parse(s)).ToList();
            while (arr.Count % 3 != 0) { arr.Add(0.0); }
            var a = new Point3D(arr[0], arr[1], arr[2]);
            var b = a; b.X += 1;
            var c = a; c.Y += 1;
            if (arr.Count > 3) { b = new Point3D(arr[3], arr[4], arr[5]); }
            if (arr.Count > 6) { c = new Point3D(arr[6], arr[7], arr[8]); }
            var origin = new Point3D(0.0, 0.0, 0.0);
            var e1 = b - a;
            var e2 = c - a;
            var e3 = Vector3D.CrossProduct(e1, e2);

        }

    }
}

