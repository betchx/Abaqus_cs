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


        #region ツール
        // 書きやすくするための細工．
        private string dot_join(string parent, string child) { return parent + "." + child; }
        private Address address(string parent, uint id) { return new Address(parent, id); }
        private Address address(uint id) { return new Address(id); }

        /// <summary>
        ///   集合定義でGenerateオプションがあった場合のデータセット文字列から
        ///   列挙を作成する．
        /// </summary>
        /// <param name="line">データセット文字列．(例： 1, 4, 1)</param>
        /// <returns>列挙</returns>
        internal IEnumerable<uint> generate(string line) { return new IDGenerater(line); }


        /// <summary>
        ///    節点を表す文字列をアドレスの列挙に展開する．
        ///    列挙なのは，集合名が設定されることがある為
        /// </summary>
        /// <param name="key">対象文字列</param>
        /// <returns>節点アドレスの列挙</returns>
        internal IEnumerable<Address> node_addresses(string key) { return to_addresses(key, true); }
        /// <summary>
        ///   要素を表す文字列からアドレスの列挙を作成する．
        ///   列挙なのは集合名が設定されることがある為．
        /// </summary>
        /// <param name="key">対象文字列</param>
        /// <returns>要素アドレスの列挙</returns>
        internal IEnumerable<Address> element_addresses(string key) { return to_addresses(key, false); }

        /// <summary>
        ///   文字列をアドレスに変換する下請け関数
        /// </summary>
        /// <param name="key">変換対象文字列</param>
        /// <param name="is_n">節点ならtrue, 要素ならfalse. 集合名を展開する必要があるので区分の為に必要</param>
        /// <returns>Address</returns>
        private IEnumerable<Address> to_addresses(string key, bool is_n)
        {
            uint id;
            if (key.Contains(".")) {
                var arr = key.Split('.').ToArray();
                if (uint.TryParse(arr[1], out id)) {
                    return new uint[] { id }.Select(i => new Address(arr[0], i));
                }
                // other set
                var ins = model.instances[arr[0]];
                var part = model.parts[ins.part];
                if (is_n) {
                    return part.nsets[arr[1]].Select(a => new Address(arr[0], a.id));
                }
                return part.elsets[arr[1]].Select(a => new Address(arr[0], a.id));
            } else {
                if (uint.TryParse(key, out id)) {
                    return new uint[] { id }.Select(i => new Address(i));
                }
                if (is_n) {
                    return model.nsets[key].Select(a => new Address(a.id));
                }
                return model.elsets[key].Select(a => new Address(a.id));
            }
        }
        /// <summary>
        ///  カンマ区切りの文字列からuintの列挙を作成する．
        /// </summary>
        /// <param name="str">非負整数のカンマ区切りの文字列</param>
        /// <returns>uintの列挙</returns>
        private static IEnumerable<uint> ids(string str) { return str.Split(',').Select(s => uint.Parse(s)); }

        #endregion

        public Parser()
        {
            model = new Model();
            lexer = new Lexer();
            current_part = model;
            system = new Transform3DGroup();
            dict = new SortedDictionary<string, command_parser>();
            dict.Add(Keyword.NODE, parse_node);
            dict.Add(Keyword.ELEMENT, parse_element);
            dict.Add(Keyword.NSET, parse_nset);
            dict.Add(Keyword.ELSET, parse_elset);
            dict.Add(Keyword.SYSTEM, parse_system);
            dict.Add(Keyword.PART, parse_part);
            dict.Add(Keyword.END_PART, parse_end_part);
            dict.Add(Keyword.INSTANCE, parse_instance);
            dict.Add(Keyword.ASSEMBLY, parse_assembly);
        }

        #region パースメソッド
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
        #endregion


        #region コマンドパーザ
        /// <summary>
        ///  ノードコマンドをパースし，モデルに節点を追加する．
        ///  NSETパラメータがあればNsetも追加する．
        /// </summary>
        /// <param name="cmd">ノードコマンド</param>
        private void parse_node(Command cmd)
        {
            cmd.must_be(Keyword.NODE);

            var nids = new List<uint>();
            foreach (var line in cmd)
            {
                var node = new Node(line);
                if (system.Children.Count > 0) {
                    node.Transform(system);
                }
                nids.Add(node.id);
                current_part.nodes.Add(node);
            }
            if (cmd.Has("NSET"))
            {
                var name = cmd["NSET"];
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
            cmd.must_be(Keyword.ELEMENT);

            if (!cmd.Has("TYPE"))
                throw new InvalidFormatException("Element");

            var type = cmd.parameters["TYPE"];

            // 継続行（最後がカンマ）をまとめる
            bool cont = false;
            var lines = new List<string>();
            string buf = "";
            foreach (var item in cmd)
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
            if (cmd.Has("ELSET"))
            {
                var name = cmd["ELSET"].ToUpper();
                var elset = new ELSet(name);
                elset.UnionWith(eids.Select(i => new Address(i)));
                try {
                    current_part.elsets.Add(name, elset);
                }
                catch (ArgumentException e) {
                    throw new ArgumentException("elsetsのキーに'" + name + "'が既に存在", e);
                }
            }
        }

        /// <summary>
        /// 要素集合のパース
        /// 登録先はcurrent_part
        /// </summary>
        /// <param name="cmd"></param>
        private void parse_elset(Command cmd)
        {
            cmd.must_be(Keyword.ELSET);

            var name = cmd["ELSET"].ToUpper();

            // セットは追加が可能なので，存在確認が必要．
            ELSet elset;
            if (current_part.elsets.ContainsKey(name)) {
                elset = current_part.elsets[name];
            } else {
                elset = new ELSet(name);
                current_part.elsets.Add(elset);
            }

            if (cmd.Has("INSTANCE")) {
                //throw new NotImplementedException("Instance option support in parse_elset");
                var ins = cmd["INSTANCE"];
                if (cmd.Has("GENERATE")) {
                    foreach (var line in cmd) {
                        generate(line).ForEach(id => elset.Add(address(ins, id)));
                    }
                } else {
                    foreach (var line in cmd){
                        ids(line).ForEach(id => elset.Add(address(ins, id)));
                    }
                }
            } else {
                if (cmd.Has("GENERATE")){
                    //cmd.ForEach(line => ELSetGenerate(elset, line));
                    foreach (var line in cmd) {
                        generate(line).ForEach(id => elset.Add(address(id)));
                    }
                }else{
                    foreach (var line in cmd) {
                        ids(line).ForEach(id => elset.Add(address(id)));
                    }
                }
            }
        }


        /// <summary>
        /// 節点集合のパース
        /// </summary>
        /// <param name="cmd">コマンド</param>
        private void parse_nset(Command cmd)
        {
            cmd.must_be(Keyword.NSET);

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

        /// <summary>
        ///  パートのパース
        /// </summary>
        /// <param name="cmd"></param>
        private void parse_part(Command cmd)
        {
            cmd.must_be(Keyword.PART);

            var name = cmd["NAME"].ToUpper();
            current_part = new Part(name, model);
            model.parts.Add(current_part);
        }

        /// <summary>
        ///  パート定義終了．
        ///  current_partをモデルに戻す為に必要．
        /// </summary>
        /// <param name="cmd">コマンド</param>
        private void parse_end_part(Command cmd)
        {
            cmd.must_be(Keyword.END_PART);
            current_part = model;
        }

        /// <summary>
        ///   インスタンスのパース.
        ///   ALLへの登録も同時に行う．
        /// </summary>
        /// <param name="cmd"></param>
        private void parse_instance(Command cmd)
        {
            cmd.must_be(Keyword.INSTANCE);

            var name = cmd.parameters["NAME"].ToUpper();
            var part_name = cmd.parameters["PART"].ToUpper();
            var trans = ""; if (cmd.datablock.Count > 1) trans = cmd.datablock[0].Trim();
            var rot = ""; if (cmd.datablock.Count > 2) rot = cmd.datablock[1].Trim();
            var ins = new Instance(name, part_name, model, trans, rot);
            model.instances.Add(ins);

            // パート
            var part = model.parts[part_name];

            // 移動させながらノードをコピー
            foreach (var n in part.nodes.Values) {
                var pos = ins.system.Transform(n.pos);
                ins.nodes.Add(new Node(n.id, pos.X, pos.Y, pos.Z));
            }
            //ins.elements = part.elements;
            foreach (var original in part.elements.Values) {
                ins.elements.Add(new Element(original));
            }
            part.elsets.ForEach(kv => ins.elsets.Add(kv.Value));
            part.nsets.ForEach(kv => ins.nsets.Add(kv.Value));

            // グローバルコレクションへの追加
            // こちらはAddressへの変換があるのでひと手間かかる．
            foreach (var nsets_of_part in part.nsets.Values) {
                var absolute_name = dot_join(instance_name, nsets_of_part.name);
                var global_nset = new NSet(absolute_name);
                nsets_of_part.ForEach(nset_of_part => global_nset.Add(address(instance_name, nset_of_part.id)));
                model.global_nsets.Add(global_nset);
            }
            foreach (var val in part.elsets.Values) {
                var absolute_naem = dot_join(instance_name, val.name);
                var global_elset = new ELSet(absolute_naem);
                val.ForEach(elset_in_part => global_elset.Add(address(instance_name, elset_in_part.id)));
                model.global_elsets.Add(global_elset);
            }

        }

        private void parse_assembly(Command cmd)
        {
            cmd.must_be(Keyword.ASSEMBLY);
        }


        /// <summary>
        ///  システムの登録．
        ///  節点定義で位置を変化させるもの．
        /// </summary>
        /// <param name="cmd">コマンド</param>
        private void parse_system(Command cmd)
        {
            cmd.must_be(Keyword.SYSTEM);

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

        #endregion
    }
}

