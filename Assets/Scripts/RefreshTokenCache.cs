using UnityEngine;

[System.Serializable]
public class RefreshTokenCache : ISaveable
{
   public string _idToken;
   public string _accessToken;
   public string _refreshToken;

   public RefreshTokenCache() {}

   public RefreshTokenCache(string idToken, string accessToken, string refreshToken)
   {
      _idToken = idToken;
      _accessToken = accessToken;
      _refreshToken = refreshToken;
   }

   public string getIdToken()
   {
      return _idToken;
   }

   public string getAccessToken()
   {
      return _accessToken;
   }

   public string getRefreshToken()
   {
      return _refreshToken;
   }

   public string ToJson()
   {
      return JsonUtility.ToJson(this);
   }

   public void LoadFromJson(string jsonToLoadFrom)
   {
      JsonUtility.FromJsonOverwrite(jsonToLoadFrom, this);
   }

   public string FileNameToUseForData()
   {
      return "bad_data_01.dat";
   }
}
