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
        public GlobalNodes all_nodes { get; set; }
        public GlobalElements all_elements { get; set; }
        public GlobalNSets all_nsets { get; set; }
        public GlobalELSets all_elsets { get; set; }


        public Model():base("", null)
        {
            parts = new Parts(this);
            instances = new Instances(this);
            all_nodes = new GlobalNodes(this);
            all_elements = new GlobalElements(this);
            all_elsets = new GlobalELSets(this);
            all_nsets = new GlobalNSets(this);

            // overwrite
            this.model = this;

            nsets.model = this;
            elsets.model = this;
            nodes.model = this;
            elements.model = this;
        }

        public override bool isModel { get { return true; } }
    }
}
