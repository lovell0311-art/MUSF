
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// Íæ¼Ò¿¿Ç½
    /// </summary>
    public class AgainstTheWall : MonoBehaviour
    {

        public float Euler_Y = 90;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("LocaRole") || other.CompareTag("Player"))
            {
                if (other.gameObject.GetComponent<Animator>() is Animator animator)
                {
                    animator.SetBool("Stand", true);
                    other.transform.rotation = Quaternion.Euler(0, Euler_Y,0);
                    for (int i = other.transform.childCount - 1; i >= 0; i--)
                    {
                        if (other.transform.GetChild(i).name.Contains("Suit"))
                        {
                            other.transform.GetChild(i).GetComponent<Animator>().SetBool("Stand", true);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("LocaRole") || other.CompareTag("Player"))
            {
                if (other.gameObject.GetComponent<Animator>() is Animator animator)
                {
                   
                    if (!animator.GetBool("IsMove"))
                    {
                        animator.SetBool("Stand", true);
                        other.transform.rotation = Quaternion.Euler(0, Euler_Y, 0);
                        for (int i = other.transform.childCount-1; i>=0; i--)
                        {
                            if (other.transform.GetChild(i).name.Contains("Suit"))
                            {
                                other.transform.GetChild(i).GetComponent<Animator>().SetBool("Stand", true);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("LocaRole") || other.CompareTag("Player"))
            {
               
                if (other.gameObject.GetComponent<Animator>() is Animator animator)
                {
                    animator.SetBool("Stand", false);
                    for (int i = other.transform.childCount - 1; i >= 0; i--)
                    {
                        if (other.transform.GetChild(i).name.Contains("Suit"))
                        {
                            other.transform.GetChild(i).GetComponent<Animator>().SetBool("Stand", false);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}