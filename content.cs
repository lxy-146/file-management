using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace filemanager
{
    public partial class content : Form
    {
        public string content_text="";
        public linknode nownode;
        public alldisk cdisk;
        public bool ischange = new bool();
        public content()
        {
            InitializeComponent();
        }
        public content(linknode l,alldisk d)
        {
            InitializeComponent();
            nownode = l;
            cdisk = d;
            ischange = false;
            inittext();
        }
        public void inittext()
        {
            int nowspace = nownode.fcb.start;
            content_text = cdisk.space[nowspace].content;
            while(cdisk.space[nowspace].nextone!=Space.empty)
            {
                nowspace = cdisk.space[nowspace].nextone;
                content_text = content_text + cdisk.space[nowspace].content;
            }
            textBox1.Text = content_text;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (content_text != textBox1.Text)
            {  
                content_text = textBox1.Text;
                ischange = true;
            }
        }

        private void content_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ischange)
            {
                DialogResult result = MessageBox.Show("是否进行保存？", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    string s = content_text.Trim();
                    bool b = new bool();
                    b = false;
                    b=cdisk.updatefile(nownode, s);
                    Form1.form1.updateui(Form1.form1.currentnode);
                    if (!b) MessageBox.Show("存储失败！");
                }
                else return;
            }
            else return;
        }
    }
}
