﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class mojeClanky : System.Web.UI.Page
{
    User user;
    DBHandler dbHandler = new DBHandler(@"Data Source = SQL5044.site4now.net; Initial Catalog = DB_A50E52_rsp019; User Id = DB_A50E52_rsp019_admin; Password=Voracek2019;");

    List<KeyValuePair<int, string>> vydaniList;

    protected void Page_Load(object sender, EventArgs e)
    {
        // pokud neni uzivatel prihlasen, dojde k presmerovani na prihlaseni
        user = (User)Session["userObject"];
        if (user == null) { Response.Redirect("login.aspx"); }
        // pokud role "Redaktor" pak se zobrazí NoveClanky.aspx
        if (user.Role == "redaktor") { Response.Redirect("NoveClanky.aspx"); }
        //nacteni vsech vydani pro naplneni dropdown listu
        vydaniList = dbHandler.getAllVydani();
        NaplnitDDL();
    }

    /**
     * nahrani souboru na server, zkontroluje typ souboru a do databaze ulozi relativni cesta k souboru, dane vydani a id autora a datum nahrani
     */
    protected void Upload(object sender, EventArgs a)
    {
        if ((FileUpload.PostedFile != null) && (FileUpload.PostedFile.ContentLength > 0))
        {
            string fileType = System.IO.Path.GetExtension(FileUpload.FileName);
            string fileName = System.IO.Path.GetFileName(FileUpload.PostedFile.FileName);
            string saveLocation = "~/Clanky/" + fileName;

            if (!IsValidType(fileType))
            {
                Response.Write("<script>alert('Zvolte soubor typu pdf nebo doc/docx')</script>");
                return;
            }
            if (DDL_vyberVydani.SelectedIndex == 0)
            {
                Response.Write("<script>alert('Zvolte vydani')</script>");
                return;
            }
            try
            {
                FileUpload.PostedFile.SaveAs(Server.MapPath(saveLocation));
                dbHandler.uploadFile(user.id, Convert.ToInt32(DDL_vyberVydani.SelectedValue), fileName, saveLocation);
                Response.Write("Článek byl nahrán");
                GV_clanky.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("error: " + ex.Message);
            }
        }
        else if (FileUpload.PostedFile.ContentLength == 0)
        {
            Response.Write("Zvolený soubor má nulovou velikost");
        }
        else
        {
            Response.Write("Prosím zvolte soubor pro nahrání");
        }
    }
    /**
     * naplneni dropdown listu aktivnimi vydani casopisu na 0 index se vlozi prazdny vyber 
     */
    protected void NaplnitDDL()
    {
        if (DDL_vyberVydani.Items.Count != 0)
            return;
        DDL_vyberVydani.Items.Insert(0, "-"); //prazdny vyber
        for (int i = 0; i < vydaniList.Count; ++i)
        {
            DDL_vyberVydani.Items.Insert(i + 1, new ListItem(vydaniList[i].Value, vydaniList[i].Key.ToString()));
        }
    }

    protected void DDL_vyberVydani_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateCounter(DDL_vyberVydani.SelectedItem.Value);
    }
    /*
     * update labelu se zobrazenim kolik clanku je k danemu vydani v DB
     */
    protected void UpdateCounter(string vydani)
    {
        if(DDL_vyberVydani.SelectedIndex == 0)
        {
            LB_counterClanku.Text = "";
            return;
        }
        LB_counterClanku.Text = Convert.ToString(dbHandler.pocetClankuVydani( Convert.ToInt32(DDL_vyberVydani.SelectedValue)));
    }

    //kontrola zda soubor je formatu pdf doc nebo docx
    protected Boolean IsValidType(string extension)
    {
        Boolean isValid = false;
        if(extension.ToLower().Equals(".pdf") || extension.ToLower().Equals(".doc") || extension.ToLower().Equals(".docx"))
        {
            isValid = true;
        }
        return isValid;
    }
}