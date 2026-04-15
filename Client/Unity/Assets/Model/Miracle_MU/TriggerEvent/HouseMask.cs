using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ETModel
{
    public class HouseMask : MonoBehaviour
    {
        public GameObject houseTop;
        public List<GameObject> otherObj;

        public GameObject DoorLeft=null;
        public Vector3 LeftAngle_Y;
        public GameObject DoorRight=null;
        public Vector3 RightAngle_Y;
        Vector3 vector_Left;
        Vector3 vector_Right;

        public GameObject Sound;

        private void Start()
        {
            if(DoorLeft!=null)
            vector_Left= DoorLeft.transform.rotation.eulerAngles;
            if(DoorRight!=null)
            vector_Right = DoorRight.transform.rotation.eulerAngles;
        }

        private void OnTriggerEnter(Collider other)
        {
         
            if (other.gameObject.CompareTag("LocaRole"))
            {
                houseTop.SetActive(false);
                foreach (GameObject obj in otherObj)
                {
                    obj.SetActive(false);
                }

                if (DoorLeft != null)
                {
                    DoorLeft.transform.localEulerAngles = LeftAngle_Y;
                }
                if (DoorRight != null)
                {
                    DoorRight.transform.localEulerAngles = RightAngle_Y;
                }

                if (Sound != null)
                {
                    Sound.GetComponent<SoundData>().PlayAudio();
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("LocaRole"))
            {
                houseTop.SetActive(true);
                foreach (GameObject obj in otherObj)
                {
                    obj.SetActive(true);
                }

                if (DoorLeft != null)
                {
                   
                    DoorLeft.transform.localEulerAngles = vector_Left;
                }
                if (DoorRight != null)
                {
                   
                    DoorRight.transform.localEulerAngles = vector_Right;
                }
                if (Sound != null)
                {
                    Sound.GetComponent<SoundData>().AudioSource.Stop();
                }
            }
        }
    }
}