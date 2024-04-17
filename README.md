# Spirits demo Client
Spirits demo Client is a Client part of the Spirits project created within the framework of the Spirits (Niačyściki) application, which was created for the presentation “*Characters of Belarusian mythology as an object of geographical study*“ to participate in BSU`s scientific-practical conference.
## Build requirements
####Appsettings
To run this app you\`ll need to create **`appsettings.Development.json`** file to the project. With such a structure:
```json
{
  "ConnectionStrings": {
        "Api": {
      "BaseUrl": "[URL of an API server]"
    },
  "ArcGis": {
    "ApiKey": "[ArcGIS API key]"
  },
    "auth": {
      "client_id": "[google client id for auth]"
    }
  }
}
```
*[URL of an API server]* - address of running a [Demo Spirits API](https://github.com/Discipuls/DemoSpiritsAPI "Demo Spirits API").
*[ArcGis API key]* - key to connect ArcGIS API. [Official docs](https://developers.arcgis.com/documentation/mapping-apis-and-services/security/api-keys/ "Official docs")
*[google client id for auth]*  - required by GoogleSignInOptions.Builder to request id token.  Obtained in google cloud console -> apis&services -> credentials
## Download
[Spirits.apk](https://drive.google.com/file/d/11jTjoyiZWQdZzLfWg_waIXe7WGyvWZq5/view?usp=sharing "Spirits.apk")
