using UnityEngine;
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

        this.BlackOverlay.SetActive(true);
        this.SetText();
        this.CriminalController.StartTurn();
    }

    public void StartTurn()
    {
        this.BlackOverlay.SetActive(false);
        this.StartTurnButton.gameObject.SetActive(false);
    }

    public void EndCurrentTurn()
    {
        this.CurrenTurn = this.CurrenTurn == Turn.King ? Turn.Criminal : Turn.King;
        this.SetText();
        this.BlackOverlay.SetActive(true);
        this.StartTurnButton.gameObject.SetActive(true);

        switch (this.CurrenTurn)
        {
            case Turn.King:
                this.CriminalController.EndTurn();
                this.KingController.StartTurn();
                break;
            case Turn.Criminal:
                this.KingController.EndTurn();
                this.CriminalController.StartTurn();
                break;
        }
    }

    private void SetText()
    {
        this.CurrentTurnDisplayText.text = this.CurrenTurn.ToString();
    }
}
