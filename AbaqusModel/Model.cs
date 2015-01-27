using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    /// <summary>
    /// 解析モデルを表す．
    /// 構成要素のコンテナを保持する．
    /// Assemblyでもある．
    /// </summary>
    public class Model : Part
    {
        // サブアイテム
        public Parts parts { get; set; }
        public Instances instances { get; set; }
        // Assemblyは無しとする．（Assembly == Model の設計）


        // グローバルコレクション
        // 絶対名称で保存したもの
        public GlobalNodes global_nodes { get; set; }
        public GlobalElements global_elements { get; set; }
        public GlobalNSets global_nsets { get; set; }
        public GlobalELSets global_elsets { get; set; }


        public Model():base("", null)
        {
            parts = new Parts(this);
            instances = new Instances(this);
            global_nodes = new GlobalNodes(this);
            global_elements = new GlobalElements(this);
            global_elsets = new GlobalELSets(this);
            global_nsets = new GlobalNSets(this);

            // overwrite
            this.model = this;

            nsets.model = this;
            elsets.model = this;
            nodes.model = this;
            elements.model = this;
        }

        public override bool isModel { get { return true; } }

        // Assemblyを有するかどうかを返す
        public bool isAssembly { get { return instances.Count > 0; } }
    }
}
