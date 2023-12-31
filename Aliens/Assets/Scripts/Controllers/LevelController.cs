using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private int _numberOfMoves = 15;

    [Space]
    [SerializeField] private RectTransform _gameUI;
    [SerializeField] private RectTransform _losePanel;
    [SerializeField] private RectTransform _winPanel;
    [SerializeField] private RectTransform _pausePanel;

    [Space]
    [SerializeField] private Button[] _restartButton;
    [SerializeField] private Button[] _mainMenuButton;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _unpauseButton;

    [Space]
    [SerializeField] private TextMeshProUGUI _loseTipText;
    [SerializeField] private TextMeshProUGUI _numberOfMovesText;

    private const string _levelSceneBaseName = "Level ";

    private void Start()
    {
        SetNumberOfMovesText();

        MovableObject.SetBlock(false);
        MovableObject.OnObjectMoved += OnMovableObjectMoved;

        _player.OnTriggerEndLevel.AddListener(OnTriggerEndLevel);
        _player.OnTriggerWakingUpObject.AddListener(OnTriggerWakingUpObject);

        foreach (var b in _restartButton)
            b.onClick.AddListener(OnRestartButtonClick);
        foreach (var b in _mainMenuButton)
            b.onClick.AddListener(OnMainMenuButtonClick);
        _nextLevelButton.onClick.AddListener(OnNextLevelButtonClick);
        _pauseButton.onClick.AddListener(OnPauseButtonClick);
        _unpauseButton.onClick.AddListener(OnUnpauseButtonClick);

        SetAllPanelEnable(true);
        SetAllPanelEnable(false);
    }

    private void OnDestroy()
    {
        MovableObject.OnObjectMoved -= OnMovableObjectMoved;

        _player.OnTriggerEndLevel.RemoveListener(OnTriggerEndLevel);
        _player.OnTriggerWakingUpObject.RemoveListener(OnTriggerWakingUpObject);

        foreach (var b in _restartButton)
            b.onClick.RemoveListener(OnRestartButtonClick);
        foreach (var b in _mainMenuButton)
            b.onClick.RemoveListener(OnMainMenuButtonClick);
        _nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClick);
        _pauseButton.onClick.RemoveListener(OnPauseButtonClick);
        _unpauseButton.onClick.RemoveListener(OnUnpauseButtonClick);
    }

    private void SetAllPanelEnable(bool v)
    {
        _losePanel.gameObject.SetActive(v);
        _winPanel.gameObject.SetActive(v);
        _pausePanel.gameObject.SetActive(v);
    }

    private void OnTriggerEndLevel()
    {
        var data = GameDataIO.LoadData();
        if (data == null)
            data = new GameData();
        data.SetLevelStatus(GetCurrentLevelNumber() + 1, false);
        GameDataIO.SaveData(data);

        MovableObject.SetBlock(true);

        if (!IsLevelExists(GetCurrentLevelNumber() + 1))
            _nextLevelButton.gameObject.SetActive(false);

        _gameUI.gameObject.SetActive(false);
        _winPanel.gameObject.SetActive(true);
    }

    private void OnTriggerWakingUpObject()
    {
        LoseLevel("Try not to touch objects that might wake up the player");
    }

    private void OnRestartButtonClick()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
    }

    private void OnNextLevelButtonClick()
    {
        SceneManager.LoadScene(_levelSceneBaseName + (GetCurrentLevelNumber() + 1));
    }

    private void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene(0);
    }

    private void OnPauseButtonClick()
    {
        if (MovableObject.IsAnyMoving)
            return;

        MovableObject.SetBlock(true);
        _pausePanel.gameObject.SetActive(true);
    }

    private void OnUnpauseButtonClick()
    {
        MovableObject.SetBlock(false);
        _pausePanel.gameObject.SetActive(false);
    }

    private void OnMovableObjectMoved(MovableObject @object)
    {
        _numberOfMoves--;
        SetNumberOfMovesText();

        if (_numberOfMoves <= 0)
        {
            LoseLevel("Try to use your moves more competently and sparingly, they can end at the wrong time");
        }
    }

    private int GetCurrentLevelNumber()
    {
        var name = SceneManager.GetActiveScene().name;
        return int.Parse(name.Replace(_levelSceneBaseName, string.Empty));
    }

    private void LoseLevel(string tip)
    {
        MovableObject.SetBlock(true);
        _loseTipText.text = tip;
        _gameUI.gameObject.SetActive(false);
        _losePanel.gameObject.SetActive(true);
    }

    private void SetNumberOfMovesText()
    {
        _numberOfMovesText.text = "Moves left: " + _numberOfMoves;
    }

    private bool IsLevelExists(int number)
    {
        var name = _levelSceneBaseName + number.ToString();
        var sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath(name);
        return sceneBuildIndex != -1;
    }
}
