using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsLoader : MonoBehaviour
{
    [SerializeField] private GameObject _levelButtonPrefab;

    private const string _levelSceneBaseName = "Level ";

    private void Start()
    {
        LoadLevelStates();
    }

    private void LoadLevelStates()
    {
        var data = GameDataIO.LoadData();
        var levelNumber = 1;

        while(IsLevelExists(levelNumber))
        {
            var obj = Instantiate(_levelButtonPrefab, transform);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = levelNumber.ToString();
            
            var button = obj.GetComponent<Button>();
            button.name = levelNumber.ToString();
            button.onClick.AddListener(() => LoadLevel(button.name));

            if (levelNumber != 1)
            {
                var isBlocked = true;
            
                if (data != null)
                    isBlocked = data.IsLevelBlocked(levelNumber);
            
                button.interactable = !isBlocked;
            }

            levelNumber++;
        }
    }

    private void LoadLevel(string levelNumber)
    {
        SceneManager.LoadScene(_levelSceneBaseName + levelNumber);
    }

    private bool IsLevelExists(int number)
    {
        var name = _levelSceneBaseName + number.ToString();
        var sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath(name);
        return sceneBuildIndex != -1;
    }
}
