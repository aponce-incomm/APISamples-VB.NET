Imports System.Xml.Serialization

Namespace TSG_API_Client
    <XmlRootAttribute(ElementName:="transaction", IsNullable:=False)> _
    Public Class transaction_request
        Public api_key As [String]
        Public type As [String]
        Public card As [String]
        Public csc As [String]
        Public exp_date As [String]
        Public amount As [String]
        Public avs_address As [String]
        Public avs_zip As [String]
        Public purchase_order As [String]
        Public invoice As [String]
        Public email As [String]
        Public customer_id As [String]
        Public order_number As [String]
        Public client_ip As [String]
        Public description As [String]
        Public comments As [String]
        Public shipping As shipping
        Public billing As billing
    End Class

    <XmlRootAttribute(ElementName:="transaction", IsNullable:=False)> _
    Public Class transaction_response
        Public id As [String]
        Public result As [String]
        Public display_message As [String]
        Public result_code As [String]
        Public avs_result_code As [String]
        Public csc_result_code As [String]
        Public error_code As [String]
        Public authorization_code As [String]
    End Class

    Public Class shipping
        Public first_name As [String]
        Public last_name As [String]
        Public company As [String]
        Public street As [String]
        Public street2 As [String]
        Public city As [String]
        Public state As [String]
        Public zip As [String]
        Public country As [String]
        Public phone As [String]
    End Class

    Public Class billing
        Public first_name As [String]
        Public last_name As [String]
        Public company As [String]
        Public street As [String]
        Public street2 As [String]
        Public city As [String]
        Public state As [String]
        Public zip As [String]
        Public country As [String]
        Public phone As [String]
    End Class
End Namespace