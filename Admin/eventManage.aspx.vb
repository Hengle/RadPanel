﻿Imports System.Data.SqlClient
Imports System.Data

Partial Class Admin_eventManage
    Inherits System.Web.UI.Page

    Public g_username As String = ""
    Public g_email As String = ""
    Public g_password As String = ""
    Public g_auth As String = ""
    Public g_ip As String = ""

    Public owner As String = "S"
    Public admin As String = "D"
    Public moderator As String = "O"
    Public everyone As String = "F"


    Public webFunc As New webFunctions

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        usergroupDropdown.Items.Add("Everyone")
        usergroupDropdown.Items.Add("Owners")
        usergroupDropdown.Items.Add("Administrators")
        usergroupDropdown.Items.Add("Moderators/GMs")


        If Not Request.Cookies("accountInfo") Is Nothing Then

            Dim accountCookie As HttpCookie = Request.Cookies("accountInfo")

            g_username = Server.HtmlEncode(accountCookie.Values("g_username"))
            g_email = Server.HtmlEncode(accountCookie.Values("g_email"))
            g_password = Server.HtmlEncode(accountCookie.Values("g_password"))
            g_auth = Server.HtmlEncode(accountCookie.Values("g_auth"))
            g_ip = Request.ServerVariables("REMOTE_ADDR").ToString

        End If

        If webFunc.getAccountAuth(g_email) = "S" Then
            Dim newsAction As String = Request.QueryString("actn")

            If newsAction = "trun" Then
                Dim id As String = Request.QueryString("id")
                webFunc.deleteEvent(id)
            End If

            If newsAction = "result" Then
                resultLabel.Text = "Posted Event"
            End If

        End If

        newsStoriesPlaceHolder.Text = loadEvents(webFunc.getAccountAuth(g_email), 0, False, False)

    End Sub

    Public Function loadEvents(ByVal auth As String, ByVal id As String, ByVal loadByID As Boolean, ByVal isEdit As Boolean, Optional ByVal isMini As Boolean = False, Optional ByVal isLimited As Boolean = False) As String
        Dim finalString As String = ""

        Dim queryString As String =
            "SELECT * FROM WEB_EVENTS_TBL WHERE n_Authority = 'F' ORDER BY n_DateTime DESC"
        Dim adminString As String =
            "SELECT * FROM WEB_EVENTS_TBL ORDER BY n_DateTime DESC"
        Dim gmString As String =
            "SELECT * FROM WEB_EVENTS_TBL WHERE n_Authority = 'O' OR n_Authority = 'F' ORDER BY n_DateTime DESC"

        Dim idString As String =
            "SELECT * FROM WEB_EVENTS_TBL WHERE ID = " + id

        Using connection As New SqlConnection(webFunc.getConn())
            Dim command As New SqlCommand
            command.Connection = connection

            Dim da As New SqlDataAdapter
            Dim ds As New DataSet
            Dim dt As New DataTable

            If loadByID = False Then
                If auth = "S" Then
                    command.CommandText = adminString
                ElseIf auth = "O" Then
                    command.CommandText = gmString
                Else
                    command.CommandText = queryString
                End If
            Else
                command.CommandText = idString
            End If

            connection.Open()

            da.SelectCommand = command
            da.Fill(ds, "temp")
            dt = ds.Tables("temp")

            If Not dt.Rows.Count <= 0 Then
                Dim reader As SqlDataReader = command.ExecuteReader

                If isLimited Then
                    For counter As Integer = 0 To 4 Step 1
                        reader.Read()

                        Dim temp As String
                        Dim temp2(2) As String
                        Dim curDate As String
                        Dim curTime As String

                        If counter >= dt.Rows.Count Then
                            Exit For
                        End If

                        temp = reader("n_DateTime")
                        temp2 = temp.Split(" ")
                        curDate = temp2(0)
                        Dim currentDate As Date = temp2(1)
                        curTime = Format(currentDate, "h:mm tt")

                        finalString &= _
                            "<div class='box'>" +
                            "<div style='float: right;'>" +
                            "<div class='fb-like' data-href='http://main.eternalgames.net/events.aspx?ID=" +
                            reader("ID").ToString +
                            "' data-send='false' data-layout='button_count' data-width='450' data-show-faces='false'></div></div>"

                        finalString &= _
                            "<h2 style='color: #59B200;'>" +
                            reader("n_Title") +
                            "</h2><h3>Posted By: " +
                            reader("n_PostedBy") +
                            " on " + curDate + " at " + curTime + " EST" +
                            "</h3><hr/>" +
                            "<p>" +
                            HttpContext.Current.Server.HtmlDecode(reader("n_News")) +
                            "</p>"

                        finalString &= _
                            "<hr/><p style='text-align:right !important;margin-bottom: -8px;'><a href='events.aspx?ID=" + reader("ID").ToString + "'>Permalink</a>"
                        If auth = "S" Then
                            finalString &= " | <a href='./Admin/editEvent.aspx?ID=" + reader("ID").ToString + "'>Edit Event</a> | <a href='./Admin/eventManage.aspx?actn=trun&id=" +
                        reader("ID").ToString + "'>Delete Post</a></p></div>"
                        Else
                            finalString &= "</p></div>"
                        End If
                    Next
                Else
                    While (reader.Read())

                        Dim temp As String
                        Dim temp2(2) As String
                        Dim curDate As String
                        Dim curTime As String

                        temp = reader("n_DateTime")
                        temp2 = temp.Split(" ")
                        curDate = temp2(0)
                        Dim currentDate As Date = temp2(1)
                        curTime = Format(currentDate, "h:mm tt")

                        finalString &= _
                            "<div class='box'>" +
                            "<div style='float: right;'>" +
                            "<div class='fb-like' data-href='http://main.eternalgames.net/events.aspx?ID=" +
                            reader("ID").ToString +
                            "' data-send='false' data-layout='button_count' data-width='450' data-show-faces='false'></div></div>"

                        finalString &= _
                            "<h2>" +
                            reader("n_Title") +
                            "</h2><h3>Posted By: " +
                            reader("n_PostedBy") +
                            " on " + curDate + " at " + curTime + " EST" +
                            "</h3><hr/><p>" +
                            HttpContext.Current.Server.HtmlDecode(reader("n_News")) +
                            "</p>"

                        finalString &= _
                            "<hr/><p style='text-align:right !important;margin-bottom: -8px;'><a href='events.aspx?ID=" + reader("ID").ToString + "'>Permalink</a>"
                        If auth = "S" Then
                            finalString &= " | <a href='./Admin/editEvent.aspx?ID=" + reader("ID").ToString + "'>Edit Event</a> | <a href='./Admin/eventManage.aspx?actn=trun&id=" +
                        reader("ID").ToString + "'>Delete Post</a></p></div>"
                        Else
                            finalString &= "</p></div>"
                        End If
                    End While
                End If

                reader.Close()
            Else
                finalString = "<div class='box'><h2>Oops!</h2><h3>Problem Loading Events!</h3><hr/><p>It looks like the event you tried to view doesn't exist! If you feel like you reached this page in error, please contact an Administrator.</p><p><a href='default.aspx'>Back To Home</a></p></div>"
            End If

            connection.Close()

            Return finalString

        End Using
    End Function

    Public Sub refreshCookie()
        If Not Request.Cookies("accountInfo") Is Nothing Then

            Dim accountCookie As HttpCookie = Request.Cookies("accountInfo")

            g_username = Server.HtmlEncode(accountCookie.Values("g_username"))
            g_email = Server.HtmlEncode(accountCookie.Values("g_email"))
            g_password = Server.HtmlEncode(accountCookie.Values("g_password"))
            g_auth = Server.HtmlEncode(accountCookie.Values("g_auth"))
            g_ip = Request.ServerVariables("REMOTE_ADDR").ToString

        End If
    End Sub

    Protected Sub newsPostButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles newsPostButton.Click

        refreshCookie()

        Dim authSelected As String = usergroupDropdown.SelectedItem.ToString

        Dim auth As String = owner

        Select Case usergroupDropdown.SelectedItem.ToString
            Case Is = "Administrators"
                auth = admin
            Case Is = "Moderators/GMs"
                auth = moderator
            Case Is = "Everyone"
                auth = everyone
            Case Else
                auth = owner
        End Select

        webFunc.writeEvent(g_username, Server.HtmlEncode(newsTitleTextBox.Text), Server.HtmlEncode(NewsPosting.Text), auth)

        Response.Redirect("eventManage.aspx?actn=result")
    End Sub

End Class
