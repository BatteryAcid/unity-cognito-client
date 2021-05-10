using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Manages all the text and button inputs
// Also acts like the main manager script for the game.
public class UIInputManager : MonoBehaviour
{
   public static string CachePath;

   public Button signupButton;
   public Button loginButton;
   public Button startButton;
   public Button logoutButton;
   public InputField emailFieldLogin;
   public InputField passwordFieldLogin;
   public InputField usernameField;
   public InputField emailField;
   public InputField passwordField;

   private AuthenticationManager _authenticationManager;
   private GameObject _unauthInterface;
   private GameObject _authInterface;
   private LambdaManager _lambdaManager;
   private GameObject _loading;
   private GameObject _welcome;
   private GameObject _confirmEmail;
   private GameObject _signupContainer;

   private void processSceneFromAuthStatus(bool authStatus)
   {
      if (authStatus)
      {
         Debug.Log("User authenticated, show welcome screen with options");
         _loading.SetActive(false);
         _unauthInterface.SetActive(false);
         _authInterface.SetActive(true);
         _welcome.SetActive(true);
      }
      else
      {
         Debug.Log("User not authenticated, activate/stay on login scene");
         _loading.SetActive(false);
         _unauthInterface.SetActive(true);
         _authInterface.SetActive(false);
      }
   }

   private async void onLoginClicked()
   {
      _unauthInterface.SetActive(false);
      _loading.SetActive(true);
      Debug.Log("onLoginClicked: " + emailFieldLogin.text + ", " + passwordFieldLogin.text);
      bool successfulLogin = await _authenticationManager.Login(emailFieldLogin.text, passwordFieldLogin.text);
      processSceneFromAuthStatus(successfulLogin);
   }

   private async void onSignupClicked()
   {
      _unauthInterface.SetActive(false);
      _loading.SetActive(true);

      Debug.Log("onSignupClicked: " + usernameField.text + ", " + emailField.text + ", " + passwordField.text);
      bool successfulSignup = await _authenticationManager.Signup(usernameField.text, emailField.text, passwordField.text);

      if (successfulSignup)
      {
         // here we re-enable the whole auth container but hide the sign up panel
         _signupContainer.SetActive(false);

         _confirmEmail.SetActive(true);

         // copy over the new credentials to make the process smoother
         emailFieldLogin.text = emailField.text;
         passwordFieldLogin.text = passwordField.text;
      }
      else
      {
         _confirmEmail.SetActive(false);
      }

      _loading.SetActive(false);
      _unauthInterface.SetActive(true);
   }

   private void onLogoutClick()
   {
      _authenticationManager.SignOut();
      processSceneFromAuthStatus(false);
   }

   private void onStartClick()
   {
      SceneManager.LoadScene("GameScene");
      Debug.Log("Changed to GameScene");

      // call to lambda to demonstrate use of credentials
      _lambdaManager.ExecuteLambda();
   }

   private async void RefreshToken()
   {
      bool successfulRefresh = await _authenticationManager.RefreshSession();
      processSceneFromAuthStatus(successfulRefresh);
   }

   void Start()
   {
      Debug.Log("UIInputManager: Start");
      // check if user is already authenticated 
      // I perform the refresh here to keep our user's session alive so they don't have to keep logging in.
      RefreshToken();

      signupButton.onClick.AddListener(onSignupClicked);
      loginButton.onClick.AddListener(onLoginClicked);
      startButton.onClick.AddListener(onStartClick);
      logoutButton.onClick.AddListener(onLogoutClick);
   }

   void Awake()
   {
      _unauthInterface = GameObject.Find("UnauthInterface");
      _authInterface = GameObject.Find("AuthInterface");
      _loading = GameObject.Find("Loading");
      _welcome = GameObject.Find("Welcome");
      _confirmEmail = GameObject.Find("ConfirmEmail");
      _signupContainer = GameObject.Find("SignupContainer");

      _unauthInterface.SetActive(false); // start as false so we don't just show the login screen during attempted token refresh
      _authInterface.SetActive(false);
      _welcome.SetActive(false);
      _confirmEmail.SetActive(false);
      _signupContainer.SetActive(true);

      _authenticationManager = FindObjectOfType<AuthenticationManager>();
      _lambdaManager = FindObjectOfType<LambdaManager>();

      CachePath = Application.persistentDataPath;
   }
}
