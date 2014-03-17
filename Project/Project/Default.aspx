﻿<%@ Page Title="BlissKom" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Project._Default" ViewStateMode="Disabled" %>

<%-- attributions
    New Moon http://findicons.com/icon/229953/emblem_ok    ok.png  (GNU/GPL)
    New Moon http://findicons.com/icon/229887/gnome_status   info.png (GNU/GPL)
    Gnome Icon Artists http://findicons.com/icon/67135/gnome_go_home   info.png (GNU/GPL)
    Asher http://findicons.com/icon/64629/stop  close.png   (Creative Commons ShareAlike)
    capital18 (Jugal Paryani) http://findicons.com/icon/13745/forward  right.png (freeware)
    capital18 (Jugal Paryani) http://findicons.com/icon/13715/back  left.png (freeware) --%>


<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="Content">

    <asp:Panel ID="pnlTablet" runat="server" BackImageUrl="~/Images/tablet-PD.svg" Height="600px" HorizontalAlign="Center" Width="900px">

        <asp:Panel ID="pnlInnerTablet" runat="server">
            <%-- Navigeringsknappar --%>
            <asp:ImageButton ID="imbOK" runat="server" ImageUrl="~/Images/ok.png" CssClass="navImbs" CausesValidation="False" />
            <asp:ImageButton ID="imbCancel" runat="server" ImageUrl="~/Images/close.png" CssClass="navImbs" CausesValidation="False" OnClientClick="return false;" />
            <asp:ImageButton ID="imbLeft" runat="server" ImageUrl="~/Images/left.png" CssClass="navImbs" CausesValidation="False" OnClientClick="return false;" />
            <asp:ImageButton ID="imbRight" runat="server" ImageUrl="~/Images/right.png" CssClass="navImbs" CausesValidation="False" OnClientClick="return false;" />
            <asp:ImageButton ID="imbHome" runat="server" ImageUrl="~/Images/home.png" CssClass="navImbs" CausesValidation="False" />
            <asp:ImageButton ID="imbInfo" runat="server" ImageUrl="~/Images/info.png" CssClass="navImbs" CausesValidation="False" OnClientClick="return false;" />
            <%-- Platshållare för items, alltså bilderna på "kartan".--%>
            <asp:PlaceHolder ID="phItems" runat="server" />
            <%-- Formulär för kommunikation med databasen --%>
            <asp:Panel ID="pnlForm" runat="server">
                <%-- DropDownList för ordtyper. AutoPostBack för att SelectedIndexChanged ska köras.
                Renderar färgerna vid varje PostBack eftersom ListItems inte har någon ViewStateMode att aktivera.
                DropDownList toppfärg samma som vald ListItem. Färgerna laddas vid DataBound och vid PostBack.
                DropDownListans värden binds dock bara en gång, ViewStateMode behöver inte vara aktiverad. --%>
                <asp:Label ID="lblMeaning" runat="server" Text="Betydelse"></asp:Label>
                <asp:DropDownList ID="ddlPageWordType" runat="server"      
                    DataValueField="WTypeId" ItemType="Project.PageModel.PageWordType"
                    DataTextField="WType" OnDataBound="ddlPageWordType_DataBound"
                    SelectMethod="GetPageWordTypeData">
                </asp:DropDownList>
                <asp:Label ID="lblWordType" runat="server" Text="Ordtyp"></asp:Label>                                 
                <asp:ListBox ID="lstMeaning" runat="server"
                    DataValueField="MeaningId" ItemType="Project.Model.Meaning"
                    DataTextField="Word" SelectMethod="GetMeaningData" AutoPostBack="True" OnSelectedIndexChanged="lstMeaning_SelectedIndexChanged" >
                </asp:ListBox>
                        <asp:Label ID="lblWord" runat="server" Text="Ord"></asp:Label>
                        <asp:TextBox ID="txtWord" runat="server"></asp:TextBox>
                        <asp:Label ID="lblWordComment" runat="server" Text="Kommentar"></asp:Label>
                        <asp:TextBox ID="txtWordComment" runat="server"></asp:TextBox>
                <asp:Label ID="lblItem" runat="server" Text="Bildfil"></asp:Label>                                 
                <asp:ListBox ID="lstItem" runat="server"
                    DataValueField="MeaningId" ItemType="Project.PageModel.PageItem"
                    DataTextField="ImageFileName" AutoPostBack="True" OnSelectedIndexChanged="lstItem_SelectedIndexChanged" >
                </asp:ListBox>
                <asp:Button ID="btnUpdateMeaning" runat="server" Text="Uppdatera" OnClick="btnUpdateMeaning_Click" />
                <asp:Button ID="btnAddNewMeaning" runat="server" Text="Skapa ny" OnClick="btnAddNewMeaning_Click" />
                <asp:Button ID="btnDeleteMeaning" runat="server" Text="Radera" OnClick="btnDeleteMeaning_Click" />
                <asp:Button ID="btnResetMeaning" runat="server" Text="Återställ" OnClick="btnResetMeaning_Click" />

            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
