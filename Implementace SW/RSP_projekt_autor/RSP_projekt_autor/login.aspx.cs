﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void BT_login_Click(object sender, EventArgs e)
    {
        if(TB_email.Text == "")
        {
            LB_email.Text = "Vyplňte email";
        }
        if(TB_heslo.Text == "")
        {
            LB_heslo.Text = "Zadejte heslo";
        }

        Autor autor = new Autor();
        autor.Email = TB_email.Text;
        autor.Heslo = TB_heslo.Text;
        if (autor.Login())
        {
            Response.Write("<script>alert('login probehl uspesne')</script>");
        }
        else
        {
            Response.Write("<font color ='red'>Nesedí heslo nebo email</font>");
        }
    }
}