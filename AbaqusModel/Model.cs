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
        public NSets all_nsets { get; set; }
        public ELSets all_elsets { get; set; }

        public Model():base("")
        {
            parts = new Parts();
            instances = new Instances();
            all_nodes = new SortedDictionary<Address, Node>();
            all_elements = new SortedDictionary<Address, Element>();
            all_elsets = new ELSets();
            all_nsets = new NSets();


            nsets.model = this;
            elsets.model = this;
            parts.model = this;
            instances.model = this;
            nodes.model = this;
            elements.model = this;
           
        }
        public virtual bool isModel { get { return true; } }
    }
}
