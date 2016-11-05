using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnController : MonoBehaviour
{
    public KingController KingController;

    public CriminalController CriminalController;

    public Button EndTurnButton;

    public Text CurrentTurnDisplayText;

    public GameObject BlackOverlay;

    public Button StartTurnButton;

    public Turn CurrenTurn;

    public Text WonText;

    public Button WonButton;

    public Image KingUi;

    public Image CriminalUi;

    public enum Turn
    {
        King,

        Criminal
    }

    // Use this for initialization
    public void Start()
    {
        this.CurrenTurn = Turn.Criminal;

        this.EndTurnButton.onClick.RemoveAllListeners();
        this.EndTurnButton.onClick.AddListener(this.EndCurrentTurn);

        this.StartTurnButton.onClick.RemoveAllListeners();
        this.StartTurnButton.onClick.AddListener(this.StartTurn);

        this.WonButton.onClick.RemoveAllListeners();
        this.WonButton.onClick.AddListener(() =>
        {
            GameObject.Destroy(GameObject.Find("Bootstrap"));
            SceneManager.LoadScene(0);
        });

        this.BlackOverlay.SetActive(true);
        this.CriminalController.StartTurn();
    }

    public void StartTurn()
    {
        this.BlackOverlay.SetActive(false);
        this.StartTurnButton.gameObject.SetActive(false);
        this.KingController.DisplayStartOfRoundInformation();
    }

    public void EndCurrentTurn()
    {
        this.CurrenTurn = this.CurrenTurn == Turn.King ? Turn.Criminal : Turn.King;
        this.BlackOverlay.SetActive(true);
        this.StartTurnButton.gameObject.SetActive(true);

        switch (this.CurrenTurn)
        {
            case Turn.King:
                this.CriminalController.EndTurn();
                this.KingUi.gameObject.SetActive(true);
                this.CriminalUi.gameObject.SetActive(false);
                this.KingController.StartTurn();
                break;
            case Turn.Criminal:
                this.KingController.EndTurn();
                this.KingUi.gameObject.SetActive(false);
                this.CriminalUi.gameObject.SetActive(true);
                this.CriminalController.StartTurn();
                break;
        }
    }

    public void Update()
    {
        if (CriminalController.HasWon)
        {
            this.StartTurnButton.gameObject.SetActive(false);
            this.BlackOverlay.gameObject.SetActive(true);
            this.WonText.text = "Thief Wins!";
            this.WonButton.gameObject.SetActive(true);
        }
        else if (this.KingController.HasWon)
        {
            this.StartTurnButton.gameObject.SetActive(false);
            this.BlackOverlay.gameObject.SetActive(true);
            this.WonText.text = "King Wins!";
            this.WonButton.gameObject.SetActive(true);
        }
    }
}
