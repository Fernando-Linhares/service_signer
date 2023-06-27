using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Service;

public class Apparence
{
    public string Reason = "I Agree";

    public string Location =  "Brasil";

    public bool HasLayers = true;

    public bool Visible = true;

    public string Content = "Signed By @cert_name@ Date @signature_date@";

    public int[] Dimensions = new int[] { 100, 100, 300, 200 };
}
