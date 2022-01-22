using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    [RequireComponent(typeof(LineRenderer))]
    public class Electro : MonoBehaviour
    {

        public int m_ShakeInternal = 5;   //震动的时间间隔
        private int m_shakeframeCount = 1;

        [SerializeField]
        private float m_noiseRange = 2;  //噪波缩放，越小就越平缓
        public int m_MaxLineCount = 30;  //最大分段，分段越多，闪电就越碎
        public int m_MaxnoiseRange = 1;  //一般为1就可以了，控制振幅
        /// <summary>
        /// 闪电节点数量
        /// </summary>
        private int m_linePointcount = 10;  
        /// <summary>
        /// 闪电节点距离
        /// </summary>
        [SerializeField]
        private float m_pointDis = 0.5f;  //点间距
        [SerializeField]
        private Transform m_targetts;   //闪电的端点，你可以改public然后设置端点
        private LineRenderer m_linerender;  //线渲染器
        [SerializeField]
        private bool m_isLighting = false;  //定义是否发生闪电，在运行时关闭m_isLighting的话只是暂停，不会消失，消失还是靠Gameobj的Trigger来实现




    // Start is called before the first frame update
    void Start()
        {
            m_linerender = this.GetComponent<LineRenderer>();
        }

        public void StartLight(Transform targetts)
        {
            m_targetts = targetts;
            this.m_isLighting = true;
            this.m_shakeframeCount = m_ShakeInternal;
        }



        // Update is called once per frame
        void Update()
        {
            if (this.m_shakeframeCount > 0)
            {
                m_shakeframeCount--;
                return;
            }
            if (!this.m_isLighting) return;
            this.m_shakeframeCount = m_ShakeInternal;//间隔越大越慢
            float distance = Vector3.Distance(transform.position, m_targetts.position);  //这个GameObj的点到目标点的距离
            int pointcount = Mathf.CeilToInt(distance / this.m_pointDis);  //距离/点间距=实际点数量
            this.m_linePointcount = pointcount > this.m_MaxLineCount ? this.m_MaxLineCount : pointcount;  //判断有无超过最大点
            if (this.m_linePointcount >= this.m_MaxLineCount)
                m_pointDis = distance / this.m_MaxLineCount;  //若超出重新计算点间距
            this.m_linerender.positionCount = this.m_linePointcount + 1; //传入线渲染器默认有11个顶点
            Vector3 dir = (this.m_targetts.position - transform.position).normalized;  //这个GameObj的点到目标点的方向向量
            for (int i = 0; i < this.m_linePointcount; i++)  //遍历所有点
            {    
                Vector3 pos = this.transform.position + dir * m_pointDis * i; //取得原始点的位置
                float newnoiseRange = this.m_noiseRange * distance;  //将偏移范围和距离绑定
                if (newnoiseRange > this.m_MaxnoiseRange) newnoiseRange = this.m_MaxnoiseRange; //判断有无超出最大范围
                pos.x += Random.Range(-newnoiseRange, newnoiseRange); //偏移x
                pos.y += Random.Range(-newnoiseRange, newnoiseRange); //偏移y
                this.m_linerender.SetPosition(i, pos);  //重新设值
            }
            this.m_linerender.SetPosition(this.m_linerender.positionCount - 1, this.m_targetts.position);  //将最后一个点（默认第十个点）还原成目标点
        }
    }
