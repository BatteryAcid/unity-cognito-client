using UnityEngine;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This script just demonstrates we can use credentials from an authenticated user to call AWS APIs
public class LambdaManager : MonoBehaviour
{
   public Button mainMenuButton;

   private AuthenticationManager _authenticationManager;
   private string _lambdaFunctionName = "YOUR_LAMBDA_FUNCTION_NAME";

   public async void ExecuteLambda()
   {
      Debug.Log("ExecuteLambda");

      AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(_authenticationManager.GetCredentials(), AuthenticationManager.Region);

      InvokeRequest invokeRequest = new InvokeRequest
      {
         FunctionName = _lambdaFunctionName,
         InvocationType = InvocationType.RequestResponse
      };

      InvokeResponse response = await amazonLambdaClient.InvokeAsync(invokeRequest);
      Debug.Log("Response statusCode: " + response.StatusCode);

      if (response.StatusCode == 200)
      {
         Debug.Log("Successful lambda call");

         // demonstrate we can get the users ID for use in our game
         string userId = _authenticationManager.GetUsersId();
         Debug.Log("UserId in LambdaManager: " + userId);
      }
   }

   private void onMainMenuClick()
   {
      SceneManager.LoadScene("LoginScene");
      Debug.Log("Changed to Login scene");
   }

   void Awake()
   {
      _authenticationManager = FindObjectOfType<AuthenticationManager>();
   }

   void Start()
   {
      // ignore setup during switch back to login scene
      if (mainMenuButton != null)
      {
         mainMenuButton.onClick.AddListener(onMainMenuClick);
      }
   }
}
