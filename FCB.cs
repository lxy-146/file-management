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
    public class FCB
    {
        public string filename;
        public string changetime;
        public int filesize=new int();
        public int isfile = new int();//0文件 1文件夹 -2根目录
        public int start = new int();

        public FCB()
        {
            filename = "";
            filesize = 0;
            start = -1;
        }
        public FCB(string fname,string ctime,int fsize,int ifile,int s)
        {
            filename = fname;
            changetime = ctime;
            filesize = fsize;
            isfile = ifile;
            start = s;
        }
    }
}
