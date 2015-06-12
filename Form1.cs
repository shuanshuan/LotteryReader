using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using LotteryDb;
using LotteryReader.Utils;

namespace LotteryReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btSSQ_Click(object sender, EventArgs e)
        {
            string urlBase = "http://trend.lecai.com/ssq/redBaseTrend.action?startPhase={0}&endPhase={1}&phaseOrder=up&coldHotOrder=number&onlyBody=true";
            // string url = "http://trend.lecai.com/ssq/redBaseTrend.action?startPhase=2014000&endPhase=2015000&phaseOrder=up&coldHotOrder=number&onlyBody=true";
            
            int min = 2003000;
            int max = (DateTime.Now.Year + 1) * 1000;

            using (LotteryEntities db = new LotteryEntities())
            {
                string sMax = db.ShuangSeQius.Max(q => q.Phase);
                if (!string.IsNullOrEmpty(sMax))
                    min = int.Parse(sMax) + 1;
            }

            for (int i = min; i < max; i = (i + 1000) / 1000 * 1000)
            {
                string uri = string.Format(urlBase, i, i + 1000);
                string content = HtmlHelper.HtmlRequest(uri);
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                doc.LoadHtml(content);

                SSQ  ssq = new SSQ(doc);
                DataTable balls = ssq.getBallsTable();


                string connection = ConfigurationManager.ConnectionStrings["LotteryConnectionString"].ConnectionString;
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "dbo.ShuangSeQiu";

                    bulkCopy.ColumnMappings.Add("R1", "R1");
                    bulkCopy.ColumnMappings.Add("R2", "R2");
                    bulkCopy.ColumnMappings.Add("R3", "R3");
                    bulkCopy.ColumnMappings.Add("R4", "R4");
                    bulkCopy.ColumnMappings.Add("R5", "R5");
                    bulkCopy.ColumnMappings.Add("R6", "R6");
                    bulkCopy.ColumnMappings.Add("B1", "B1");
                    bulkCopy.ColumnMappings.Add("Phase", "Phase");
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.WriteToServer(balls);
                }


            }


        }


        private void Form1_Load(object sender, EventArgs e)
        {


        }



        private void btDLT_Click(object sender, EventArgs e)
        {

            string urlBase = "http://trend.lecai.com/dlt/redBaseTrend.action?startPhase={0}&endPhase={1}&phaseOrder=up&coldHotOrder=number&onlyBody=yes";
            int min = 7000;
            int max = (DateTime.Now.Year % 2000 + 1) * 1000;

            using (LotteryEntities db = new LotteryEntities())
            {
                string sMax = db.DLTs.Max(q => q.Phase);
                if (!string.IsNullOrEmpty(sMax)) 
                {
                    min = int.Parse(sMax) - 2000000 +1;
                }
            }

            for (int i = min; i < max; i = (i + 1000) / 1000 * 1000)
            {
                string start = i < 10000 ? "0" + i : ""+i;
                int iEnd = i + 1000;
                string end = iEnd < 10000 ? "0" + iEnd : "" + iEnd;
                
                string uri = string.Format(urlBase, start, end);
                string content = HtmlHelper.HtmlRequest(uri);
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                doc.LoadHtml(content);

                DltUtils dlt = new DltUtils(doc);
                DataTable balls = dlt.getBallsTable();

                string connection = ConfigurationManager.ConnectionStrings["LotteryConnectionString"].ConnectionString;
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "dbo.DLT";

                    bulkCopy.ColumnMappings.Add("R1", "R1");
                    bulkCopy.ColumnMappings.Add("R2", "R2");
                    bulkCopy.ColumnMappings.Add("R3", "R3");
                    bulkCopy.ColumnMappings.Add("R4", "R4");
                    bulkCopy.ColumnMappings.Add("R5", "R5");
                    bulkCopy.ColumnMappings.Add("B1", "B1");
                    bulkCopy.ColumnMappings.Add("B2", "B2");
                    bulkCopy.ColumnMappings.Add("Phase", "Phase");
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.WriteToServer(balls);
                }
            }
        }
    }
}
