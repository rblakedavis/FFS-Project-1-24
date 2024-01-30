using UnityEngine;
using UnityEngine.SceneManagement;
public class MagicAttack : MonoBehaviour
{
    public Enemy enemy;

    private void Awake()
    {
    }

    private void Update()
    {
        CheckIfOutOfCameraView();
    }

    private void CheckIfOutOfCameraView()
    {
        Vector3 newPosition = transform.position;
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(newPosition);

        // Check if the magic attack is below the bottom of the screen
        if (viewportPoint.y < 0)
        {
            Destroy(gameObject);
            ResetMagicAttack();
        }
    }

    private bool IsInCameraView(Vector3 position)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);

        return (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1);
    }

    public void OnMouseOver()
    {
        enemy = FindObjectOfType<Enemy>();
        if (enemy != null)
        {
            if (enemy.damage != 0)
            {

                Destroy(gameObject);
                //play a sound
                enemy.curHealth -= Player.Instance.magicPower;
                Debug.Log($"magic attack dealt {Player.Instance.magicPower} damage!");
                Player.Instance.magic=0;
                ResetMagicAttack();
                SFXManager sFXManager = GameObject.Find("SFXManager").GetComponent<SFXManager>();
                AudioSource audioSource = GameObject.Find("SFXManager").GetComponent<AudioSource>();
                AudioClip clip = sFXManager.magicAttackSounds[0];
                audioSource.PlayOneShot(clip);
            }
        }
    }

    private void ResetMagicAttack()
    {
        if (SceneManager.GetActiveScene().name == "Grind")
        {
            NewBattleManager battleManager = FindObjectOfType<NewBattleManager>();
            GameObject.Find("BattleManager").GetComponent<NewBattleManager>().DespawnMagicAttack();
        }
        if (SceneManager.GetActiveScene().name == "Boss")
        {
            BossBattleManager battleManager = FindObjectOfType<BossBattleManager>();
            GameObject.Find("BossBattleManager").GetComponent<BossBattleManager>().DespawnMagicAttack();
        }
    }
}
