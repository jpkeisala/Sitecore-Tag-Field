TAG MODULE 

INSTALLATION GUIDE:

1.Install the package and publish template "Tag" and all tags under sitecore/content/Home/Global/Tags

2.At the beginning of /sitecore/shell/themes/standard/default/Default.css add the next line:
@import url('/sitecore modules/Shell/addition/tags/styles.css'); 

3.At the end of /sitecore/shell/controls/Sitecore.js add the next line:
document.write("<script type=\"text/javascript\" src=\"/sitecore modules/Shell/addition/tags/scripts.js\"></script>"); 

4.In web.config at <controlSources> section add this line:
<source mode="on" namespace="Addition.Context.Types.Tag" assembly="Addition.Context.Types.Tag" prefix="customControls"/>

5.In web.config, in the /configuration/sitecore/xslExtensions/extension element with namespace http://www.sitecore.net/sc, 
replace the value of the type attribute with the signature of your class. 
The sc namespace then exposes the methods in your class as well as the methods in the Sitecore.Xml.Xsl.XslHelper base class.
<extension mode="on" type="Addition.Context.Types.Tag.Xml.Xsl.XsltHelper, Addition.Context.Types.Tag" namespace="http://www.sitecore.net/sc" singleInstance="true"/>

USER GUIDE:
1.Create a new template. Add a new field type "Tag". 
  The source represents the folder where the new tags will be created.



