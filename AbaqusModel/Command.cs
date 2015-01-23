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
        public Command()
        {
            parameters = new Dictionary<string, string>();
            datablock = new List<string>();
        }

        //Syntax sugers

        /// <summary>
        ///   keyで指定したオプションの値を返す．オプションが存在しない場合はnull.
        /// </summary>
        /// <param name="key">オプションの名前（自動的に大文字に変換される）．</param>
        /// <returns>オプションの値もしくはnull．'='のないオプションの場合は空文字列．</returns>
        public string this[string key] { get { return Has(key) ? parameters[key.ToUpper()] : null; } }
        /// <summary>
        ///   keyで指定されたオプションがあるかどうかを判定する．
        ///   keyの大文字小文字は無視．
        ///   this.parameters.ContainsKey(key.ToUpper())と同義．（シンタックスシュガー）
        /// </summary>
        /// <param name="key">オプションの名前</param>
        /// <returns>オプションが存在すればtrue.無ければfalse</returns>
        public bool Has(string key) { return parameters.ContainsKey(key.ToUpper()); }
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
    }
}
