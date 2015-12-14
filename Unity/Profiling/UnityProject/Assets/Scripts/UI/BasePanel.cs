namespace Assets.Scripts.UI
{
    using UnityEngine;

    public abstract class BasePanel : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            this.gameObject.SetActive(true);
        }
    }
}
