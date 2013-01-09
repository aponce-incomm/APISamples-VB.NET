Imports System.Collections.Generic

Imports System.Linq
Imports System.Text
Imports System.Xml.Serialization
Imports System.IO
Imports System.Xml
Imports System.Net
Imports TSG_API_Client.TSG_API_Client
Imports System.Web.Script.Serialization

Module Program
    Sub Main(ByVal args As String())
        Try
            'Settings
            Dim apiUrl As String = "https://sandbox.thesecuregateway.com/rest/v1/transactions"
            Dim apiKey As String = "a20effd6dc1d4512888e6b06d870248a"
            Dim timeout As Integer = 15000 'Milliseconds
            Dim lang_type As String = "xml" '"xml" or "json"

            'Populate Transaction Request Info
            Dim transaction_req As New transaction_request()
            transaction_req.api_key = apiKey
            transaction_req.type = "SALE"
            transaction_req.card = "4111111111111111"
            transaction_req.csc = "123"
            transaction_req.exp_date = "1121"
            transaction_req.amount = "10.00"
            transaction_req.avs_address = "112 N. Orion Court"
            transaction_req.avs_zip = "20210"
            transaction_req.purchase_order = "10"
            transaction_req.invoice = "100"
            transaction_req.email = "email@tsg.com"
            transaction_req.customer_id = "25"
            transaction_req.order_number = "1000"
            transaction_req.client_ip = ""
            transaction_req.description = "Cel Phone"
            transaction_req.comments = "Electronic Product"

            Dim bl As New billing()
            bl.first_name = "Joe"
            bl.last_name = "Smith"
            bl.company = "Company Inc."
            bl.street = "Street 1"
            bl.street2 = "Street 2"
            bl.city = "Jersey City"
            bl.state = "NJ"
            bl.zip = "07097"
            bl.country = "USA"
            bl.phone = "123456789"

            transaction_req.billing = bl

            Dim sh As New shipping()
            sh.first_name = "Joe"
            sh.last_name = "Smith"
            sh.company = "Company 2 Inc."
            sh.street = "Street 1 2"
            sh.street2 = "Street 2 2"
            sh.city = "Colorado City"
            sh.state = "TX"
            sh.zip = "79512"
            sh.country = "USA"
            sh.phone = "123456789"

            transaction_req.shipping = sh

            Dim request As String = ""
            If ("xml".Equals(lang_type)) Then
                'Serialize transaction object to XML representation
                Dim ns As New XmlSerializerNamespaces()
                ns.Add("", "")
                Dim serializer As New System.Xml.Serialization.XmlSerializer(GetType(transaction_request))
                Dim ms As New MemoryStream()
                Dim Utf8 As Encoding = New UTF8Encoding(False)
                Dim xmlTextWriter As New XmlTextWriter(ms, Utf8)
                xmlTextWriter.Formatting = Formatting.Indented
                serializer.Serialize(xmlTextWriter, transaction_req, ns)
                request = Utf8.GetString(ms.ToArray())
            Else
                'Serialize transaction object to JSON representation
                transaction_req.avs_address = Nothing
                transaction_req.avs_zip = Nothing
                Dim serializer As New JavaScriptSerializer()
                Dim tmp As String = serializer.Serialize(transaction_req)
                request = tmp.Replace("""avs_address"":null,""avs_zip"":null,", "") 'avoid avs_address and avs_zip fields
            End If


            'Execute request to gateway
            Console.WriteLine("-----------------------------------------------------")
            Console.WriteLine("REQUEST TO URL: " & apiUrl)
            Console.WriteLine("POST DATA: " & Environment.NewLine & request)
            Dim req As HttpWebRequest = TryCast(WebRequest.Create(New Uri(apiUrl)), HttpWebRequest)
            req.Method = "POST"
            req.ContentType = "application/" & lang_type
            req.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None
            Dim dataByteLen As Byte() = UTF8Encoding.UTF8.GetBytes(request)
            req.ContentLength = dataByteLen.Length
            req.Timeout = timeout
            Dim post As Stream = req.GetRequestStream()
            post.Write(dataByteLen, 0, dataByteLen.Length)
            post.Close()

            Dim resp As HttpWebResponse = TryCast(req.GetResponse(), HttpWebResponse)
            Dim reader As New StreamReader(resp.GetResponseStream())
            Dim response As String = reader.ReadToEnd()

            Console.WriteLine("-----------------------------------------------------")
            Console.WriteLine("RESPONSE DATA: " & Environment.NewLine & response)

            If response.Contains("<transaction>") Or response.Contains("""transaction""") Then
                Dim transaction_res As transaction_response
                If ("xml".Equals(lang_type)) Then
                    Dim serializer As New System.Xml.Serialization.XmlSerializer(GetType(transaction_response))
                    transaction_res = DirectCast(serializer.Deserialize(New StringReader(response)), transaction_response)
                Else
                    Dim serializer As New JavaScriptSerializer()
                    'remove 'transaction' element from the string response
                    response = response.Replace("{""transaction"":", "")
                    response = response.Remove(response.Length - 1)
                    transaction_res = serializer.Deserialize(Of transaction_response)(response)
                End If

                If transaction_res.result_code IsNot Nothing AndAlso transaction_res.result_code = "0000" Then
                    Console.WriteLine("-----------------------------------------------------")
                    Console.WriteLine("TRANSACTION APPROVED: " + transaction_res.authorization_code)
                Else
                    Dim code As [String] = ""
                    If transaction_res.error_code IsNot Nothing Then
                        code = transaction_res.error_code
                    End If
                    If transaction_res.result_code IsNot Nothing Then
                        code = transaction_res.result_code
                    End If
                    Console.WriteLine("-----------------------------------------------------")
                    Console.WriteLine(("TRANSACTION ERROR: Code=" & code & " Message=") + transaction_res.display_message)
                End If
            End If
        Catch e As Exception
            Console.WriteLine("-----------------------------------------------------")
            Console.WriteLine("EXCEPTION: " & e.Message)
        Finally
            Console.ReadLine()
        End Try
    End Sub
End Module
