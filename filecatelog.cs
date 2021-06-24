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

namespace filemanager
{
    public class linknode
    {
        public FCB fcb = new FCB();
        public linknode firstchild = null;
        public linknode nextbrother = null;
        public linknode parent = null;

        public linknode() { }
        public linknode(FCB f)
        {
            fcb.filename = f.filename;
            fcb.filesize = f.filesize;
            fcb.changetime = f.changetime;
            fcb.isfile = f.isfile;
            fcb.start = f.start;
        }
    }
    /*
             * 
             * 
             *1.rootname父节点
             *2.filename文件名
             *3.changetime修改日期
             *4.filesize文件的大小
             *5.isfile是否为文件夹
             *6.start
             *7.#
             *
             *
             */
    public class filecatelog
    {
        public linknode root = new linknode();
        public filecatelog()
        {

        }
        //寻找节点
        public linknode search(string name, linknode start,int isfile)
        {
            linknode cnode = start,renode;
            if (name == "") return root;
            while (cnode != null)
            {
                if (cnode.fcb.filename == name&&cnode.fcb.isfile==isfile) return cnode;
                if (cnode.firstchild != null)
                {
                    renode = search(name, cnode.firstchild,isfile);
                    if (renode != null)
                        return renode;
                }
                cnode = cnode.nextbrother;
            }
            return null;
        }
        //添加新结点
        public void addtree(string pname, FCB newfcb)
        {
            linknode newnode = new linknode(newfcb);
            linknode pnode = search(pname, root,1);
            if (pnode == null) Form1.form1.label1.Text = Form1.form1.label1.Text + " without parent";
            else if (pnode.firstchild == null) { pnode.firstchild = newnode; newnode.parent = pnode; }
            else
            {
                newnode.parent = pnode;
                pnode = pnode.firstchild;
                while (pnode.nextbrother != null)
                    pnode = pnode.nextbrother;
                pnode.nextbrother = newnode;
                
            }

        }
        //从txt中读取目录
        public void cateloginit(string path)
        {
            StreamReader reader = new StreamReader(path);
            string s = reader.ReadLine();
            int t = new int();
            int start = new int();
            string fname = "", rname = "", changetime = "", fsize = "", isfile = "";
            t = 0;
            while (s != null)
            {
                switch (t)
                {
                    case 0:
                        {
                            rname = s; t++; break;
                        }
                    case 1:
                        {
                            fname = s; t++; break;
                        }
                    case 2:
                        {
                            changetime = s; t++; break;
                        }
                    case 3:
                        {
                            fsize = s; t++; break;
                        }
                    case 4:
                        {
                            isfile = s; t++; break;
                        }
                    case 5:
                        {
                            start = int.Parse(s); t++; break;
                        }
                }
                if (t == 6)
                {
                    FCB newone = new FCB(fname, changetime, int.Parse(fsize), int.Parse(isfile), start);
                    addtree(rname, newone);
                    t = 0;
                    s = reader.ReadLine();
                    //Form1.form1.textBox1.Text = fname + changetime + fsize + isfile + start.ToString();
                }
                s = reader.ReadLine();
            }

            reader.Close();
        }
        //将目录写回
        public void writecatelog(string p)
        {
            string path = p;
            StreamWriter writer = File.AppendText(path);
            linknode cnode = root;
            Queue<linknode> q = new Queue<linknode>();
            q.Enqueue(cnode.firstchild);
            while (q.Count != 0)
            {
                cnode = q.Dequeue();
                while (cnode != null)
                {
                    if (cnode.parent == root) writer.WriteLine("");
                    else writer.WriteLine(cnode.parent.fcb.filename);
                    writer.WriteLine(cnode.fcb.filename);
                    writer.WriteLine(cnode.fcb.changetime);
                    writer.WriteLine(cnode.fcb.filesize.ToString());
                    writer.WriteLine(cnode.fcb.isfile.ToString());
                    writer.WriteLine(cnode.fcb.start.ToString());
                    writer.WriteLine("#");
                    if (cnode.firstchild != null) q.Enqueue(cnode.firstchild);
                    cnode = cnode.nextbrother;
                }
            }
            writer.Close();
        }
        public bool checkrepeat(string name,linknode l,int isfolder)
        {
            l = l.firstchild;
            while(l!=null)
            {
                if (l.fcb.filename == name&&l.fcb.isfile==isfolder) return false;
                l = l.nextbrother;
            }
            return true;
        }
        public bool addfolder(string foldername, linknode l)
        {
            linknode newl = new linknode(new FCB(foldername, DateTime.Now.ToLocalTime().ToString(), 0, 1, -1));
            if (!checkrepeat(foldername, l,1)) return false;
            newl.parent = l;
            if (l.firstchild == null) { l.firstchild = newl;return true; }
            l = l.firstchild;
            while(l.nextbrother!=null)
            {
                l = l.nextbrother;
            }
            l.nextbrother = newl;
            return true;
        }
        public bool addfile(string fname,linknode l,alldisk mydisk)
        {
            int q = new int();
            q = mydisk.getstartplace();
            linknode newl = new linknode(new FCB(fname, DateTime.Now.ToLocalTime().ToString(), 0, 0,q));//分配空间
            if (!checkrepeat(fname, l, 0)) return false;
            newl.parent = l;
            if (l.firstchild == null) { l.firstchild = newl; return true; }
            l = l.firstchild;
            while (l.nextbrother != null)
            {
                l = l.nextbrother;
            }
            l.nextbrother = newl;
            return true;
        }
        public void delete(linknode l,alldisk mdisk)
        {
            linknode cnode;
            cnode = l.parent;//将当前结点从树中取出来，接下来只需要释放存储空间
            if (cnode.firstchild == l) cnode.firstchild = l.nextbrother;
            else
            {
                cnode = cnode.firstchild;
                while (cnode.nextbrother != null)
                    if (cnode.nextbrother == l) { cnode.nextbrother = l.nextbrother; break; }
                    else cnode = cnode.nextbrother;
            }
            if (l.fcb.isfile == 0)
                mdisk.deletefile(l);
            else if (l.fcb.isfile == 1)
            {
                Queue<linknode> q = new Queue<linknode>();
                cnode = l.firstchild;
                q.Enqueue(cnode);
                while(q.Count()!=0)
                {
                    cnode = q.Dequeue();
                    while(cnode!=null)
                    {
                        if (cnode.firstchild != null)
                            q.Enqueue(cnode.firstchild);
                        else if (cnode.fcb.isfile == 0) mdisk.deletefile(cnode);
                        cnode = cnode.nextbrother;
                    }
                }
            }
        }
       
    }
}
