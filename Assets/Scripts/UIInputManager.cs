using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInputManager : MonoBehaviour
{

   public Button signupButton;
   public Button loginButton;
   public TextMeshProUGUI emailField;
   public TextMeshProUGUI passwordField;

   // TODO: public Text statusText;

   // Start is called before the first frame update
   void Start()
   {
      signupButton.onClick.AddListener(onSignupClicked);
      loginButton.onClick.AddListener(onLoginClicked);
   }

   private void onSignupClicked()
   {
      Debug.Log("onSignupClicked: " + emailField.text + ", " + passwordField.text);
   }

   private void onLoginClicked()
   {
      Debug.Log("onLoginClicked: " + emailField.text + ", " + passwordField.text);
   }

   // Update is called once per frame
   void Update()
   {

   }
}
