using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LotteryReader.Utils
{
    public class SSQ
    {
        DataTable balls;
        HtmlAgilityPack.HtmlDocument doc;

        public SSQ( HtmlAgilityPack.HtmlDocument  doc ) 
        {
            Type tinyint = typeof(byte);
            balls = new DataTable();
            balls.Columns.Add("R1", tinyint);
            balls.Columns.Add("R2", tinyint);
            balls.Columns.Add("R3", tinyint);
            balls.Columns.Add("R4", tinyint);
            balls.Columns.Add("R5", tinyint);
            balls.Columns.Add("R6", tinyint);
            balls.Columns.Add("B1", tinyint);
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
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes(
                "//*[contains(@class,\"chart_table_td omission_entry omission_hit blue_ball\")]"))
            {
                balls.Rows[count][6] = Int32.Parse(link.InnerText);
                //  balls.Rows[count][7] = n++;
                count++;
            }
        }


        private void getPhase()
        {

            //"//td[@width='100' and @class='chart_table_td']")
            int count = 0;
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes(
                "//*[contains(@class,\"chart_table_td chart_table_td_phase\")]"))
            {
                balls.Rows[count][7] = Int32.Parse(link.InnerText);
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

            for (int i = 0; i < length; i++)
            {
                row[i % 6] = Int32.Parse(nodes[i].InnerText);
                count++;
                if (count == 6)
                {
                    count = 0;
                    balls.Rows.Add(row);
                    row = balls.NewRow();
                }
            }
        }

    }
}
