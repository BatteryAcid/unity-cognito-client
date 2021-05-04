using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIInputManager : MonoBehaviour
{
   public Button signupButton;
   public Button loginButton;
   public Button startButton;
   public InputField emailFieldLogin;
   public InputField passwordFieldLogin;
   public InputField usernameField;
   public InputField emailField;
   public InputField passwordField;

   // TODO: move to a more appropriate file once established
   public static string CachePath;

   private AuthenticationManager _authenticationManager;
   private GameObject _authInterface;
   private LambdaManager _lambdaManager;
   private GameObject _loading;
   private GameObject _welcome;
   private GameObject _confirmEmail;
   private GameObject _signupContainer;

   void Awake()
   {
      _authInterface = GameObject.Find("AuthInterface");
      _loading = GameObject.Find("Loading");
      _welcome = GameObject.Find("Welcome");
      _confirmEmail = GameObject.Find("ConfirmEmail");
      _signupContainer = GameObject.Find("SignupContainer");

      startButton.gameObject.SetActive(false);
      _authInterface.SetActive(false); // start as false so we don't just show the login screen during attempted token refresh
      _welcome.SetActive(false);
      _confirmEmail.SetActive(false);
      _signupContainer.SetActive(true);

      _authenticationManager = FindObjectOfType<AuthenticationManager>();
      _lambdaManager = FindObjectOfType<LambdaManager>();

      CachePath = Application.persistentDataPath;
   }

   private async void RefreshToken()
   {
      bool successfulRefresh = await _authenticationManager.RefreshSession();

      processSceneFromAuthStatus(successfulRefresh);
   }

   private void processSceneFromAuthStatus(bool authStatus)
   {
      if (authStatus)
      {
         _loading.SetActive(false);
         _authInterface.SetActive(false);
         _welcome.SetActive(true);
         startButton.gameObject.SetActive(true);
      }
      else
      {
         Debug.Log("Not authenticated, activate/stay on login scene");
         _loading.SetActive(false);
         _authInterface.SetActive(true);
      }
   }

   private void onStartClick()
   {
      SceneManager.LoadScene("GameScene");
      Debug.Log("Changed to GameScene");

      _lambdaManager.ExecuteLambda();
   }

   private async void onLoginClicked()
   {
      _authInterface.SetActive(false);
      _loading.SetActive(true);
      Debug.Log("onLoginClicked: " + emailFieldLogin.text + ", " + passwordFieldLogin.text);
      bool successfulLogin = await _authenticationManager.Login(emailFieldLogin.text, passwordFieldLogin.text);
      processSceneFromAuthStatus(successfulLogin);
   }

   private async void onSignupClicked()
   {
      _authInterface.SetActive(false);
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
      _authInterface.SetActive(true);

   }

   void Start()
   {
      // check if user is already authenticated 
      RefreshToken();

      signupButton.onClick.AddListener(onSignupClicked);
      loginButton.onClick.AddListener(onLoginClicked);
      startButton.onClick.AddListener(onStartClick);
   }
}
