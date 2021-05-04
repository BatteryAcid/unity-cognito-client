using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.Lambda;
using Amazon.Lambda.Model;

public class LambdaManager : MonoBehaviour
{
   private AuthenticationManager _authenticationManager;
   private string _lambdaFunctionName = "cognito-auth-demo-1";

   public async void ExecuteLambda()
   {
      Debug.Log("ExecuteLambda...");

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
      }
   }

   void Awake()
   {
      _authenticationManager = FindObjectOfType<AuthenticationManager>();
   }

   // Start is called before the first frame update
   void Start()
   {

   }

   // Update is called once per frame
   void Update()
   {

   }
}
