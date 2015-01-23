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
                nids.Add(node.id);
                model.nodes.Add(node.id, node);
            }
            if (cmd.parameters.ContainsKey("NSET"))
            {
                var name = cmd.parameters["NSET"];
                var nset = new NSet(name);
                nset.UnionWith(nids.Select(i => model.nodes[i]));
                model.nsets.Add(nset);
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
                model.elements.Add(e.id, e);
                eids.Add(e.id);
            }
            // 指定があれば要素集合を作成
            if (cmd.parameters.ContainsKey("ELSET"))
            {
                var name = cmd.parameters["ELSET"];
                var elset = new ELSet(name);
                elset.UnionWith(eids.Select(i => model.elements[i]));
                model.elsets.Add(name, elset);
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
                elset.Add(model.elements[i]);
            }
        }

        private void parse_elset(Command cmd)
        {
            if (cmd.keyword != "ELSET") throw new ArgumentException(cmd.keyword + " is not ELSET");
            var name = cmd.parameters["ELSET"];
            var elset = model.elsets[name];
            if (elset == null) elset = new ELSet(name);
            if (cmd.parameters.ContainsKey("INSTANCE"))
            {
                
            }
            else
            {
                if (cmd.parameters.ContainsKey("GENERATE"))
                    cmd.datablock.ForEach(line => ELSetGenerate(elset, line));
                else
                    cmd.datablock.ForEach(line => elset.UnionWith(line.Split(',').Select(s => model.elements[uint.Parse(s)])));
            }
        }

        private void parse_nset(Command cmd)
        {
            

 
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

