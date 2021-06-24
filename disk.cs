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
    public class Space
    {
        public const int empty = -1;

        public string content;
        public int nextone=new int();
        public Space() {
            content = "";
            nextone = empty;
        }
        public Space(string cont,int nxone)
        {
            content = cont;
            nextone = nxone;
        }
    }
    public class alldisk
    {
        public const int empty = -1;
        public const int used = 0;

        public int spacesize = new int();//磁盘大小：500
        public int blocksize = new int();//块大小：50

        public Space[] space;//磁盘空间
        public int[] bitmap = new int[] { };//位示图

       
        public alldisk(int ssize, int bsize)
        {
            spacesize = ssize;
            blocksize = bsize;
            space = new Space[spacesize];
            bitmap = new int[spacesize];
            for (int i = 0; i < spacesize; i++)
            {
                space[i] = new Space();
                bitmap[i] = empty;
            }
        }
        public bool isfull()
        {
            for(int i=0;i<spacesize; i++)
            {
                if (bitmap[i] == empty) return false;
            }
            return true;
        }
        public int getstartplace()
        {
            int i = new int();
            i = 0;
            if (isfull()) return -1;
            while (i < spacesize)
            {
                if (bitmap[i] == empty)
                {
                    bitmap[i] = used;
                    return i;
                }
                i++;
            }
            return -1;
        }
        public int getnext(int lastspace)
        {
            if (isfull()) return -1;
            for(int i=0;i<spacesize;i++)
                if(bitmap[i]==empty)
                {
                    space[lastspace].nextone = i;
                    bitmap[i] = used;
                    return i;
                }
            return -1;
        }
        //删除对应文件
        public void deletefile(linknode l)
        {
            if (l.fcb.start == -1) return;
            int cspace = l.fcb.start;
            l.fcb.start = -1;
            l.fcb.filesize = 0;
            l.fcb.changetime = DateTime.Now.ToLocalTime().ToString();
            int oldspace=new int();
            while(cspace!=-1)
            {
                space[cspace].content = "";
                bitmap[cspace] = empty;
                oldspace = cspace;
                cspace = space[cspace].nextone;
                space[oldspace].nextone = Space.empty;
            }            
        }
        public bool updatefile(linknode l,string s)
        {
            deletefile(l);
            if(l.fcb.start==-1)
            {
                l.fcb.start = getstartplace();
            }
            int currentspace = l.fcb.start;
            int lengths = new int();
            lengths=s.Length;
            int nowstart = new int();
            nowstart = 0;
            int stacknum = new int();
            stacknum = 0;
            l.fcb.changetime = DateTime.Now.ToLocalTime().ToString();
            while (lengths>0)
            {
                if (lengths >= 50)
                    space[currentspace].content = s.Substring(nowstart, nowstart + blocksize);
                else space[currentspace].content = s.Substring(nowstart, s.Length);
                stacknum++;
                lengths = lengths - blocksize;
                if (lengths > 0)
                { 
                    currentspace = getnext(currentspace);
                    if (currentspace == -1) return false;
                }
            }
            l.fcb.filesize = stacknum;
            return true ;
        }
        public void readbitmap(string path)
        {
            StreamReader reader = new StreamReader(path);
            int i = new int();
            for (i=0; i < spacesize; i++)
                bitmap[i] = int.Parse(reader.ReadLine());
            reader.Close();
        }
        public void writebitmap(string path)
        {
            StreamWriter writer = File.AppendText(path);
            int i = new int();
            for (i = 0; i < spacesize; i++)
                writer.WriteLine(bitmap[i].ToString());
            writer.Close();
        }
        public void readspace(string path)
        {
            StreamReader reader = new StreamReader(path);
            int i = new int();
            string oldline;
            for (i = 0; i < spacesize; i++)
            {
                oldline = reader.ReadLine();
                if (oldline.IndexOf("<>") >= 0)
                    space[i].content = oldline.Replace("<>", "\r\n");
                else
                {
                    if (oldline.IndexOf('<') >= 0)
                        space[i].content = oldline.Replace('<', '\r');
                    else if (oldline.IndexOf('>') >= 0)
                        space[i].content = oldline.Replace('>', '\n');
                    else if (oldline != "#*&%")
                        space[i].content =oldline;
                    else if (oldline == "#*&%")
                       space[i].content = "";
                }
                space[i].nextone = int.Parse(reader.ReadLine());
            }
            reader.Close();
        }
        public void writespace(string path)
        {
            StreamWriter writer = File.AppendText(path);
            int i = new int();
            for (i = 0; i < spacesize; i++)
            {
                if(space[i].content.IndexOf("\r\n")>0)
                {
                    writer.WriteLine(space[i].content.Replace("\r\n", "<>"));
                }
                else if(space[i].content.IndexOf("\r")>0) writer.WriteLine(space[i].content.Replace("\r", "<"));
                else if (space[i].content.IndexOf("\n") > 0) writer.WriteLine(space[i].content.Replace("\n", ">"));
                else if(space[i].content=="") writer.WriteLine("#*&%");
                else writer.WriteLine(space[i].content);    
                writer.WriteLine(space[i].nextone.ToString());
            }
            writer.Close();
        }

    }
}
