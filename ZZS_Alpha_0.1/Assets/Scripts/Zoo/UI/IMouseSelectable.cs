namespace Zoo.UI
{
    public interface IMouseSelectable
    {
        bool CanBeSelected();

        void OnClicked();
    }
}
