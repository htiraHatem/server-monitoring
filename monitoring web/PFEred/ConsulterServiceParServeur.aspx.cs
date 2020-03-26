﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PFEred
{
    public partial class ConsulterServiceParServeur : System.Web.UI.Page
    {

        String id;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {


                Bind();
            }
        }
        public int PageNumber
        {
            get
            {
                if (ViewState["PageNumber"] != null)
                    return Convert.ToInt32(ViewState["PageNumber"]);
                else
                    return 0;
            }
            set
            {
                ViewState["PageNumber"] = value;
            }
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            rptPages.ItemCommand +=
               new RepeaterCommandEventHandler(rptPages_ItemCommand);
        }
        public int Bind()
        {
            id = Request.QueryString["id"];
            
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["test"].ToString());
            cn.Open();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM service where codeService in (select codeService from monitoring where codeServeur="+id+")", cn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cn.Close();
            PagedDataSource pgitems = new PagedDataSource();
            DataView dv = new DataView(dt);
            pgitems.DataSource = dv;
            pgitems.AllowPaging = true;
            pgitems.PageSize = 5;
            pgitems.CurrentPageIndex = PageNumber;
            int total = pgitems.PageCount;


            if (pgitems.PageCount > 1)
            {
                rptPages.Visible = true;
                ArrayList pages = new ArrayList();
                for (int i = 0; i < pgitems.PageCount; i++)
                    pages.Add((i + 1).ToString());
                rptPages.DataSource = pages;
                rptPages.DataBind();
            }
            else
                rptPages.Visible = false;




            ProductList.DataSource = pgitems;
            ProductList.DataBind();
            return total;
        }
        void rptPages_ItemCommand(object source,
                                 RepeaterCommandEventArgs e)
        {
            PageNumber = Convert.ToInt32(e.CommandArgument) - 1;
            Bind();
        }

        protected void delete_Command1(object sender, CommandEventArgs e)
        {
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["test"].ToString());
            connection.Open();
            SqlCommand command = new SqlCommand("DELETE FROM service WHERE codeService = " + e.CommandArgument + "", connection);

            command.ExecuteNonQuery();

            connection.Close();
            Bind();

        }
        protected void back_Click(object sender, EventArgs e)
        {
            if (PageNumber > 0)
            {
                PageNumber = PageNumber - 1;
                Bind();
            }

        }

        protected void next_Click(object sender, EventArgs e)
        {
            if (Bind() > PageNumber + 1)
            {
                PageNumber = PageNumber + 1;
                Bind();
            }

        }
    }
}