using System.Linq;
using UnityEngine;

namespace Main
{
    public class Main : MonoBehaviour
    {
        private CameraControls cameraControls;
        private MapGenerator mapGenerator;
        public static event System.Action Updating;
        public static Main instance;

        private void Start()
        {
            instance = this;
            GameObject[] gameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            GameObject map = gameObjects.Where(g => g.name == "Map Generator").ToArray()[0];
            GameObject canvas = gameObjects.Where(g => g.name == "Canvas").ToArray()[0];
            new UI.Menu(map, canvas);

            // TODO Implement a save and load functionality. Links below.
            // https://answers.unity.com/answers/1240797/view.html
        }

        private void Update()
        {
            if (Updating != null)
            {
                Updating();
            }
        }

        public MeshGenerator.Square CellFromWorldPoint(Vector3 worldPosition)
        {
            return mapGenerator.CellFromWorldPoint(worldPosition);
        }
    }
}