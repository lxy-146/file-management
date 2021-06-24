using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;

namespace filemanager
{
    public partial class Form1 : Form
    {
        public alldisk disk = new alldisk(500, 50);
        public filecatelog filecate = new filecatelog();
        public linknode currentnode;
        public List<string> s = new List<string>();

        public static Form1 form1;
        public Form1()
        {
            InitializeComponent();
            form1 = this;
            
            CheckForIllegalCrossThreadCalls = false;
            filecate.cateloginit(Application.StartupPath + "\\filecatelog.txt");
            disk.readbitmap(Application.StartupPath + "\\bitmap.txt");
            disk.readspace(Application.StartupPath + "\\space.txt");
            updateui(filecate.root);
            currentnode = filecate.root;
            fileplace.btnClick += new EventHandler(fileplacebuttonclick);
        }
        //新建文件夹
        private void 新建文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string foldername = Interaction.InputBox("请输入文件的名称", "字符串", "", 100, 100);
            if(foldername!=null)
            filecate.addfolder(foldername, currentnode);//需要进行判断，是否有重名情况，通过返回值判断
            updateui(currentnode);
        }
        //更新ui界面中的文档和文件夹
        public void updateui(linknode R)
        {
            int n=new int();
            int i = new int();
            i = 0;
            n = s.Count()-1;
            showroad.Text = "";
            while(i<=n)
            {
                showroad.Text = showroad.Text+"/" + s[i];
                i++;
            }
            this.fileplace.Init();
            linknode shownode;
            if(R==filecate.root||R.fcb.isfile==1)
            {
                if (R.firstchild == null)
                    return;
                shownode = R.firstchild;
                while(shownode!=null)
                {
                    fileplace.showFiles(shownode.fcb.filename, shownode.fcb.changetime, shownode.fcb.isfile, shownode.fcb.filesize);
                    shownode = shownode.nextbrother;
                }
                return;
            }
            else
            {
                label1.Text = label1.Text + "is file";
            }
        }
        //点击文件或者文件夹时触发
        public void fileplacebuttonclick(object sender,EventArgs e)
        {
            linknode tempnode;
            if(sender.GetType().ToString()== "System.Windows.Forms.Button")
            {
                Button b = (Button)sender;
                if(b.Tag.ToString()=="0")//file
                {
                    openfile(b.Text);
                    updateui(currentnode);
                }
                else if(b.Tag.ToString()=="1")//folder
                {
                    tempnode = searchthismenu(b.Text, currentnode, 1);
                    if (tempnode == null) ;//给出错误提示
                    else
                    {
                        currentnode = tempnode;
                        s.Add(currentnode.fcb.filename);
                        updateui(currentnode);
                    }
                }
            }
            else
            {
                string fname = fileplace.contextMenuStrip_FileChoose.SourceControl.Text;
                string ifile = fileplace.contextMenuStrip_FileChoose.SourceControl.Tag.ToString();
                if (((ToolStripMenuItem)sender).Name == "打开ToolStripMenuItem")
                {
                    if(ifile=="0")//file
                    {
                        openfile(fname);
                        //updateui(currentnode);
                    }
                    else if(ifile=="1")//folder
                    {
                        tempnode = searchthismenu(fname, currentnode,1);
                        if (tempnode == null) ;//给出错误提示
                        else
                        {
                            currentnode=tempnode;
                            s.Add(currentnode.fcb.filename);
                            updateui(currentnode);
                        }
                        
                    }
                }
                else if (((ToolStripMenuItem)sender).Name == "删除ToolStripMenuItem")
                {
                    if (ifile == "0")//file
                    {
                        delete(fname,0);
                        //updateui(currentnode);
                    }
                    else if(ifile=="1")//folder
                    {
                        delete(fname, 1);
                        updateui(currentnode);
                    }
                }
            }
        }
        public void delete(string fname,int isfile)
        {
            linknode dnode=null;
            if (isfile == 0)
                dnode = searchthismenu(fname, currentnode, 0);
            else if (isfile == 1)
                dnode = searchthismenu(fname, currentnode, 1);
            if(dnode!=null)
            filecate.delete(dnode,disk);
        }
        //在本目录下寻找fname对应的文件结点
        public linknode searchthismenu(string fname,linknode l,int isfile)
        {
            linknode cnode;
            cnode = l.firstchild;
            while(cnode!=null)
            {
                if (cnode.fcb.filename == fname&&cnode.fcb.isfile==isfile)
                    return cnode;
                cnode = cnode.nextbrother;
            }
            return null;
        }
        public void openfile(string filename)
        {
            linknode p = currentnode.firstchild;
            while(p!=null)
            {
                if(p.fcb.filename==filename&&p.fcb.isfile==0)
                {
                    content form = new content(p,disk);
                    form.Show();
                }
                p = p.nextbrother;
            }
            //updateui(currentnode);
        }
        public void updatenodetime(linknode l)
        {
            string latesttime="0";
            linknode cnode = l.firstchild;
            if (l.firstchild == null) return;
            while(cnode!=null)
            {
                if (cnode.fcb.changetime.CompareTo(latesttime)==1) latesttime = cnode.fcb.changetime;
                cnode = cnode.nextbrother;
            }
            l.fcb.changetime = latesttime;
        }
        //返回上级目录
        private void backbutton_Click(object sender, EventArgs e)
        {
            if (currentnode== filecate.root) return;
            else
            {
                updatenodetime(currentnode);
                currentnode = currentnode.parent;
                s.RemoveAt(s.Count() - 1);
                updateui(currentnode);
                
            }
        }
        //新建文件
        private void 新建文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string foldername = Interaction.InputBox("请输入文件的名称", "字符串", "", 100, 100);
            bool j = new bool();
            if (foldername != null)
            { 
                j = filecate.addfile(foldername, currentnode, disk);//需要进行判断，是否有重名情况，通过返回值判断
                if(j==false)
                {
                    MessageBox.Show("磁盘空间不足！");
                }
            }
                updateui(currentnode);

        }
        //关闭表时触发
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (File.Exists(Application.StartupPath + "\\filecatelog.txt"))
                File.Delete(Application.StartupPath + "\\filecatelog.txt");
            filecate.writecatelog(Application.StartupPath + "\\filecatelog.txt");
            if (File.Exists(Application.StartupPath + "\\bitmap.txt"))
                File.Delete(Application.StartupPath + "\\bitmap.txt");
            disk.writebitmap(Application.StartupPath + "\\bitmap.txt");
            if (File.Exists(Application.StartupPath + "\\space.txt"))
                File.Delete(Application.StartupPath + "\\space.txt");
            disk.writespace(Application.StartupPath + "\\space.txt");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //格式化
        private void formatbutton_Click(object sender, EventArgs e)
        {
            currentnode = filecate.root;
            linknode cnode,nextnode;
            cnode = currentnode.firstchild;
            while(cnode!=null)
            {
                nextnode = cnode.nextbrother;
                delete(cnode.fcb.filename, cnode.fcb.isfile);
                cnode = nextnode;
            }
            updateui(currentnode);
        }
    }
}
