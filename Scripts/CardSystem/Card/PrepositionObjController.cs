using UnityEngine;

namespace Game.CardSystem
{
    public class PrepositionObjController : MonoBehaviour
    {
        [SerializeField] private GameObject toy;
        public Vector3 offSetPrePosition;
        public Vector3 offSetPreRotation;
        private Vector3 defaultPosition;
        private Quaternion defaultRotation;
        private bool isPreposition = false;
        private GameObject oldChank;

        void Start()
        {
            defaultPosition = toy.transform.localPosition;
            defaultRotation = toy.transform.localRotation;
        }

        private void Update()
        {
            if (isPreposition)
            {
                toy.transform.rotation = Quaternion.Lerp(toy.transform.rotation, Quaternion.Euler(offSetPreRotation), Time.deltaTime * 10f);
                toy.transform.position = Vector3.Lerp(toy.transform.position, oldChank.transform.position + offSetPrePosition, Time.deltaTime * 10f);
            }
            else
            {
                if (toy.transform.rotation != defaultRotation)
                    toy.transform.localRotation = Quaternion.Lerp(toy.transform.localRotation, defaultRotation, Time.deltaTime * 10f);

                if (toy.transform.position != defaultPosition)
                    toy.transform.localPosition = Vector3.Lerp(toy.transform.localPosition, defaultPosition, Time.deltaTime * 10f);
            }
            isPreposition = false;
        }

        public void PreUseCard(GameObject chank)
        {
            isPreposition = true;
            oldChank = chank;
        }
    }
}
