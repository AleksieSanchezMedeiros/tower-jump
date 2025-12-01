using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] string sceneName;

    void Start()
    {
        if (SceneManager.GetSceneByName(sceneName) == null)
        {
            Debug.LogError($"{sceneName} does not exist!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
