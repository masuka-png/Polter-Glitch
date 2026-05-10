using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorTransition : MonoBehaviour
{
    [SerializeField] private string sceneName  = "Boss Level";


    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
