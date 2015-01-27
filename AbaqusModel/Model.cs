using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    /// <summary>
    /// 解析モデルを表す．
    /// 構成要素のコンテナを保持する．
    /// </summary>
    public class Model : Part
    {

        public Parts parts { get; set; }
        public Instances instances { get; set; }


        // 絶対名称で保存したもの
        public SortedDictionary<Address, Node> all_nodes { get; set; }
        public SortedDictionary<Address, Element> all_elements { get; set; }
        public SortedDictionary<string, NSet> all_nsets { get; set; }
        public SortedDictionary<string, ELSet> all_elsets { get; set; }

        public Model():base("", null)
        {
            parts = new Parts();
            instances = new Instances();
            all_nodes = new SortedDictionary<Address, Node>();
            all_elements = new SortedDictionary<Address, Element>();
            all_elsets = new ELSets();
            all_nsets = new NSets();

            // overwrite
            this.model = this;

            nsets.model = this;
            elsets.model = this;
            parts.model = this;
            instances.model = this;
            nodes.model = this;
            elements.model = this;
        }

        public override bool isModel { get { return true; } }
    }
}
