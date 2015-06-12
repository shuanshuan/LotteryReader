using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Data.SqlClient;
using System.Data;
using LotteryDb;
using System.Configuration;
using LotteryReader.Utils;

namespace LotteryReader.Utils
{
    public class DltUtils
    {

        DataTable balls;
        HtmlAgilityPack.HtmlDocument doc;

        public DltUtils(HtmlAgilityPack.HtmlDocument doc)
        {
            Type tinyint = typeof(byte);
            balls = new DataTable();
            balls.Columns.Add("R1", tinyint);
            balls.Columns.Add("R2", tinyint);
            balls.Columns.Add("R3", tinyint);
            balls.Columns.Add("R4", tinyint);
            balls.Columns.Add("R5", tinyint);
            balls.Columns.Add("B1", tinyint);
            balls.Columns.Add("B2", tinyint);
            balls.Columns.Add("Phase");
            this.doc = doc;

        }


        public DataTable getBallsTable()
        {
            getRedBalls();

            getBlueBall();

            getPhase();

            return balls;
        }


        private void getBlueBall()
        {
            int count = 0;
            int m = 2;

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes(
                "//*[contains(@class,\"chart_table_td omission_entry omission_hit blue_ball\")]"))
            {
                if (count % m == 0)
                { 
                    balls.Rows[count / m]["B1"] = Int32.Parse(link.InnerText);
                }
                else
                { 
                    balls.Rows[count / m]["B2"] = Int32.Parse(link.InnerText);
                }
                count++;
            }
        }


        private void getPhase()
        {
            int count = 0;
           
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(
                "//*[@id=\"chartTable\"]/tbody/tr[not(@class)]/td[@class='chart_table_td']");
           
            foreach (HtmlNode link in nodes)
            {
                 balls.Rows[count]["Phase"] = Int32.Parse(link.InnerText) + 2000000;
                count++;
            }
        }



        public void getRedBalls()
        {
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(
            "//*[contains(@class,\"chart_table_td omission_entry omission_hit red_ball\")]");
            int length = nodes.Count;

            DataRow row = balls.NewRow();

            int count = 0;
            int m = 5;
            for (int i = 0; i < length; i++)
            {
                row[i % m] = Int32.Parse(nodes[i].InnerText);
                count++;
                if (count == m)
                {
                    count = 0;
                    balls.Rows.Add(row);
                    row = balls.NewRow();
                }
            }
        }


        public void load()
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

            for (int i = min; i < max; i = (i + 1000) / 1000 * 1000 + 1)
            {

                Type tinyint = typeof(byte);
                DataTable balls = new DataTable();
                balls.Columns.Add("R1", tinyint);
                balls.Columns.Add("R2", tinyint);
                balls.Columns.Add("R3", tinyint);
                balls.Columns.Add("R4", tinyint);
                balls.Columns.Add("R5", tinyint);
                balls.Columns.Add("R6", tinyint);

                balls.Columns.Add("B1", tinyint);
                balls.Columns.Add("Phase");

                string uri = string.Format(urlBase, i, i + 1000);
                string content = HtmlHelper.HtmlRequest(uri);
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                doc.LoadHtml(content);



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
    }
}
