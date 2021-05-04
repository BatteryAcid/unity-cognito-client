using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System;
using System.Threading.Tasks;
using System.Net;

// TODO:
// - include some docs on how/why these tokens work and refresh stuff
public class AuthenticationManager : MonoBehaviour
{
   public static Amazon.RegionEndpoint Region = Amazon.RegionEndpoint.USEast1; //insert region user pool was created in, if it is different than US EAST 1

   // TODO: move these to config file
   const string IdentityPool = "us-east-1:f7a0ab43-0346-4960-9922-ca94bbbffda0"; //insert your Cognito User Pool ID, found under General Settings
   const string AppClientID = "75225apkfdj0pbsnr9k2uj8i17"; //insert App client ID, found under App Client Settings

   public static string userPoolId = "us-east-1_1hbXtfSYh";
   public static string userid = ""; // TODO: is this needed here?  probably, we may use this for gamelift requests, as it needs the user id

   private AmazonCognitoIdentityProviderClient _provider;
   private CognitoAWSCredentials cognitoAWSCredentials;
   private string _jwt;

   public async Task<bool> RefreshSession()
   {
      Debug.Log("RefreshSession");

      DateTime issued = DateTime.Now;
      RefreshTokenCache refreshTokenCache = new RefreshTokenCache();
      SaveDataManager.LoadJsonData(refreshTokenCache);

      try
      {
         CognitoUserPool userPool = new CognitoUserPool(userPoolId, AppClientID, _provider);

         //TODO do we need to put something here in the email field??
         CognitoUser user = new CognitoUser("", AppClientID, userPool, _provider);

         if (user == null)
         {
            Debug.Log("User is null");
         }

         if (_provider == null)
         {
            Debug.Log("provider is null");
         }

         // The  "Refresh token expiration (days)" (Cognito->UserPool->General Settings->App clients->Show Details) is the
         // amount of time since the last login that you can use the refresh token to get new tokens.
         // After that period the refresh will fail
         // Using DateTime.Now.AddHours(1) is a workaround for https://github.com/aws/aws-sdk-net-extensions-cognito/issues/24
         user.SessionTokens = new CognitoUserSession(
            refreshTokenCache.getIdToken(),
            refreshTokenCache.getAccessToken(),
            refreshTokenCache.getRefreshToken(),
            issued, DateTime.Now.AddHours(1));

         AuthFlowResponse authFlowResponse = await user.StartWithRefreshTokenAuthAsync(new InitiateRefreshTokenAuthRequest
         {
            AuthFlowType = AuthFlowType.REFRESH_TOKEN_AUTH
         })
         .ConfigureAwait(false);

         string token = authFlowResponse.AuthenticationResult.AccessToken;

         // Debug.Log("User Access Token after refresh: " + token);
         Debug.Log("User refreshed successfully!");

         cognitoAWSCredentials = user.GetCognitoAWSCredentials(IdentityPool, Region);

         return true;
      }
      catch (NotAuthorizedException ne)
      {
         // https://docs.aws.amazon.com/cognito/latest/developerguide/amazon-cognito-user-pools-using-tokens-with-identity-providers.html
         // refresh tokens will expire - user must login manually every x days (see user pool -> app clients -> details)
         Debug.Log("NotAuthorizedException: " + ne);
      }
      catch (WebException webEx)
      {
         // we get a web exception when we cant connect to aws - means we are offline
         Debug.Log("WebException: " + webEx);
      }
      catch (Exception ex)
      {
         Debug.Log("Exception: " + ex);
      }
      return false;
   }

   public async Task<bool> Signup(string username, string email, string password)
   {
      Debug.Log("SignUpRequest: " + username + ", " + email + ", " + password);

      SignUpRequest signUpRequest = new SignUpRequest()
      {
         ClientId = AppClientID,
         Username = email,
         Password = password
      };

      // must provide all attributes required by the User Pool that you configured
      List<AttributeType> attributes = new List<AttributeType>()
      {
         new AttributeType(){
            Name = "email", Value = email
         },
         new AttributeType(){
            Name = "preferred_username", Value = username
         }
      };

      signUpRequest.UserAttributes = attributes;

      try
      {
         SignUpResponse sighupResponse = await _provider.SignUpAsync(signUpRequest);
         Debug.Log("Sign up worked");
         return true;
      }
      catch (Exception e)
      {
         Debug.Log("Sign up failed, exception: " + e);
         return false;
      }
   }

   public async Task<bool> Login(string email, string password)
   {
      Debug.Log("Login: " + email + ", " + password);

      CognitoUserPool userPool = new CognitoUserPool(userPoolId, AppClientID, _provider);
      CognitoUser user = new CognitoUser(email, AppClientID, userPool, _provider);

      if (user == null)
      {
         Debug.Log("User is null");
      }

      InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
      {
         Password = password
      };

      try
      {
         AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

         _jwt = authResponse.AuthenticationResult.AccessToken;

         GetUserRequest getUserRequest = new GetUserRequest();

         RefreshTokenCache refreshTokenCache = new RefreshTokenCache(
            authResponse.AuthenticationResult.IdToken,
            authResponse.AuthenticationResult.AccessToken,
            authResponse.AuthenticationResult.RefreshToken);

         SaveDataManager.SaveJsonData(refreshTokenCache);

         string subId = await GetUserId();
         Debug.Log("Users unique ID from cognito: " + subId);

         // This how you get credentials to use for accessing other services.
         // This IdentityPool is your Authorization, so if you tried to access using an
         // IdentityPool that didn't have GameLift permissions, it would fail.
         cognitoAWSCredentials = user.GetCognitoAWSCredentials(IdentityPool, Region);

         return true;

         // Debug.Log(authResponse.AuthenticationResult.IdToken + ", "
         //    + authResponse.AuthenticationResult.AccessToken + ", "
         //    + authResponse.AuthenticationResult.RefreshToken + ", "
         //    + authResponse.AuthenticationResult.ExpiresIn);
      }
      catch (Exception e)
      {
         Debug.Log("Login failed, exception: " + e);
         return false;
      }
   }

   private async Task<string> GetUserId()
   {
      Debug.Log("Getting user's id...");

      string subId = "";

      Task<GetUserResponse> responseTask =
         _provider.GetUserAsync(new GetUserRequest
         {
            AccessToken = _jwt
         });

      GetUserResponse responseObject = await responseTask;

      // set the user id
      foreach (var attribute in responseObject.UserAttributes)
      {
         if (attribute.Name == "sub")
         {
            subId = attribute.Value;
            break;
         }
      }

      return subId;
   }

   public CognitoAWSCredentials GetCredentials()
   {
      return cognitoAWSCredentials;
   }

   void Start()
   {
   }

   void Awake()
   {
      Debug.Log("Awake");
      _provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Region);
   }
}
