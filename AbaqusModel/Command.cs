using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Command : IEnumerable<string>
    {
        public string keyword { get; set; }
        public Dictionary<string, string> parameters { get; set; }
        public List<string> datablock { get; set; }

        public Command(string keyword)
        {
            this.keyword = keyword.ToUpper();
            parameters = new Dictionary<string, string>();
            datablock = new List<string>();
        }

        //Syntax sugers

        /// <summary>
        ///   keyで指定したオプションの値を返す．
        ///   オプションが存在しない場合は情報を追加した上で例外をスロー.
        /// </summary>
        /// <param name="key">オプションの名前（自動的に大文字に変換される）．</param>
        /// <returns>オプションの値もしくはnull．'='のないオプションの場合は空文字列．</returns>
        public string this[string key]
        {
            get
            {
                if (Has(key)) return parameters[key.ToUpper()];
                var msg =        parameters.Keys
                    .Select(s => "'" + s + "'")
                    .Aggregate(string.Format("パラメータ'{0}'は存在しません．\n"
                    + "候補: ", key), (a, b) => a + ", " + b);
                throw new ArgumentOutOfRangeException(msg);
            }
        }

        /// <summary>
        ///   keyで指定されたオプションがあるかどうかを判定する．
        ///   keyの大文字小文字は無視．
        ///   this.parameters.ContainsKey(key.ToUpper())と同義．（シンタックスシュガー）
        /// </summary>
        /// <param name="key">オプションの名前</param>
        /// <returns>オプションが存在すればtrue.無ければfalse</returns>
        public bool Has(string key) { return parameters.ContainsKey(key.ToUpper()); }

        public bool Missing(string key) { return !Has(key); }

        /// <summary>
        ///   キーワードがkeyであるかどうかを判定する．大文字小文字は無視される．
        /// </summary>
        /// <param name="key">比較対象のキーワード</param>
        /// <returns>キーワードが指定したもに一致するかどうか</returns>
        public bool Is(string key) { return key.ToUpper() == keyword; }


        // Enumerable
        public IEnumerator<string> GetEnumerator()
        {
            return datablock.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return datablock.GetEnumerator();
        }

        // Check method
        public void must_be(string expected)
        {
            if (keyword != expected.ToUpper()) throw new ArgumentException(keyword + " is not " + expected);
        }


        public override string ToString()
        {
            return  "*"+keyword+"(" + parameters.Count.ToString() + ")[" + datablock.Count.ToString() + "]" ;
        }

        public  void must_have(string parameter)
        {
            if (Missing(parameter)) {
                throw new InvalidFormatException(keyword + "に必須パラメータの" + parameter + "がありません");
            }
        }
    }
}
