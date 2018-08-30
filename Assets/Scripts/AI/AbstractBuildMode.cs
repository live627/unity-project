using UnityEngine;
using UnityEngine.UI;

namespace Main.AI
{
    public abstract class AbstractBuildMode
    {
        protected GameObject Ground;
        protected GameObject gameObject;
        protected Text topLabel;
        protected bool isBuilding = false;
        protected KeyCode keyCode;

        public static event System.Action<AbstractBuildMode> CancelBuildMode;

        public AbstractBuildMode(GameObject gameObject, GameObject canvas)
        {
            this.gameObject = gameObject;
            Ground = gameObject.transform.GetChild(0).gameObject;
            topLabel = canvas.transform.GetChild(0).GetComponent<Text>();

            MouseManager.RaycastHitEvent += OnRaycastHit;

            // This event will be propogated to all BuildMode objects.
            CancelBuildMode += OnCancelBuildMode;

            // Add callback for our custom MonoBehavior.Update() event.
            Main.Updating += OnUpdate;
        }

        private void OnCancelBuildMode(AbstractBuildMode abstractBuild)
        {
            if (abstractBuild != this)
            {
                isBuilding = false;
            }
            Reset();
        }

        private void OnUpdate()
        {
            if (Input.GetKeyDown(keyCode))
            {
                CancelBuildMode(this);
                isBuilding = !isBuilding;
                topLabel.text = isBuilding ? Localization.GetString("build_mode_" + GetType().Name.ToLower()) : Application.productName;
            }

            // Check if the mouse is not over a UI element.
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && isBuilding)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                {
                    PreviewBuild(hitInfo.point);
                }
            }
        }

        private void OnRaycastHit(RaycastHit hitInfo)
        {
            if (isBuilding)
            {
                Build(hitInfo.point);
            }
        }

        protected abstract void Reset();
        protected abstract void Build(Vector3 worldPosition);
        protected abstract void PreviewBuild(Vector3 worldPosition);
    }
}