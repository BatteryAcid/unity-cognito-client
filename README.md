## Add sign-up and login to Unity game with AWS Cognito

This project demonstrates how to use the AWS SDK and Cognito to create sign-up and login functionality in your Unity project. 

### ** UPDATE 10/2021 ** 
While this video demonstrates how to make calls from the game client through the AWS SDK, it is NOT best practice.  Calls should go through some form of authentication like through an API Gateway Authorizer, then on to a Lambda function, for example, that performs the API calls after authorization.  See my other video that demonstrates this: https://youtu.be/lzQ2rLqlqyk

Note: this is not a production ready implementation, but more of a jumping-off point to help you get started on your project.

## Tutorial Video  

* https://youtu.be/qzr57U2gWeE

## Required Plugins for AWS SDK  

* [How to download and install AWS SDK] https://youtu.be/rUAhtlD-wXg?t=54 
* [AWS SDK Download link] https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-install-assemblies.html#download-zip-files
* [CognitoAuthentication SDK Extension Library] https://github.com/aws/aws-sdk-net-extensions-cognito
    * download project and build in Visual Studio to get DLL for project
    * example code: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/cognito-authentication-extension.html
* [Additional Packages] https://youtu.be/rUAhtlD-wXg?t=108
    * Follow instructions in GitHub issue: https://github.com/aws/aws-sdk-net/issues/1684#issuecomment-692999656
    
## 👋 Let's talk shop 👇  
[Discord] https://discord.gg/psjbBDvNBK  
[Twitter] https://twitter.com/BatteryAcidDev
    
## References
* https://serverless-game-analytics.workshop.aws/en/cognito/code.html
* https://aws.amazon.com/blogs/developer/cognitoauthentication-extension-library-developer-preview/
* https://aws.amazon.com/blogs/mobile/use-csharp-to-register-and-authenticate-with-amazon-cognito-user-pools/
